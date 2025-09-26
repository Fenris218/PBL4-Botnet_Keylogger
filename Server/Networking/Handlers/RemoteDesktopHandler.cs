using Common.Enums;
using Common.Video.Codecs;
using Server.Networking.Packets.RemoteDesktop;

namespace Server.Networking.Handlers
{
    public class RemoteDesktopHandler : IDisposable
    {
        #region Event
        protected readonly SynchronizationContext SynchronizationContext;
        public delegate void UpdateImageChangedEventHandler(object sender, Bitmap value);
        public event UpdateImageChangedEventHandler UpdateImageChanged;
        private void OnUpdateImageChanged(Bitmap value)
        {
            SynchronizationContext.Post(d =>
            {
                UpdateImageChanged?.Invoke(this, (Bitmap)d);
            }, value);
        }

        public delegate void DisplaysChangedEventHandler(object sender, int value);
        public event DisplaysChangedEventHandler DisplaysChanged;
        private void OnDisplaysChanged(int value)
        {
            SynchronizationContext.Post(d =>
            {
                DisplaysChanged?.Invoke(this, (int)d);
            }, value);
        }
        #endregion

        private readonly Client _client;

        public bool IsStarted { get; set; }
        public Size LocalResolution
        {
            get
            {
                lock (_sizeLock)
                {
                    return _localResolution;
                }
            }
            set
            {
                lock (_sizeLock)
                {
                    _localResolution = value;
                }
            }
        }

        private readonly object _syncLock = new object();
        private readonly object _sizeLock = new object();
        private Size _localResolution;
        private UnsafeStreamCodec _codec;

        public RemoteDesktopHandler(Client client)
        {
            SynchronizationContext = new SynchronizationContext();
            _client = client;
        }

        public void BeginReceiveFrames(int quality, int display)
        {
            lock (_syncLock)
            {
                IsStarted = true;
                _codec?.Dispose();
                _codec = null;
                _ = _client.QueuePacketAsync(new GetDesktopPacket { CreateNew = true, Quality = quality, DisplayIndex = display });
            }
        }

        public void EndReceiveFrames()
        {
            lock (_syncLock)
            {
                IsStarted = false;
            }
        }

        public void RefreshDisplays()
        {
            _ = _client.QueuePacketAsync(new GetMonitorsPacket());
        }

        public void SendMouseEvent(MouseAction mouseAction, bool isMouseDown, int x, int y, int displayIndex)
        {
            lock (_syncLock)
            {
                _ = _client.QueuePacketAsync(new MouseEventPacket
                {
                    Action = mouseAction,
                    IsMouseDown = isMouseDown,
                    // calculate remote width & height
                    X = x * _codec.Resolution.Width / LocalResolution.Width,
                    Y = y * _codec.Resolution.Height / LocalResolution.Height,
                    MonitorIndex = displayIndex
                });
            }
        }

        public void SendKeyboardEvent(byte keyCode, bool keyDown)
        {
            _ = _client.QueuePacketAsync(new KeyboardEventPacket { Key = keyCode, KeyDown = keyDown });
        }

        public void Handler(GetDesktopPacket packet)
        {
            lock (_syncLock)
            {
                try
                {
                    if (!IsStarted)
                        return;
                    if (_codec == null || _codec.ImageQuality != packet.Quality || _codec.Monitor != packet.Monitor || _codec.Resolution != packet.Resolution)
                    {
                        _codec?.Dispose();
                        _codec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);
                    }

                    using (MemoryStream ms = new MemoryStream(packet.Image))
                    {
                        try
                        {
                            // create deep copy & resize bitmap to local resolution
                            OnUpdateImageChanged(new Bitmap(_codec.DecodeData(ms), LocalResolution));
                        }
                        catch (Exception)
                        {

                        }
                    }
                    packet.Image = null;

                    _ = _client.QueuePacketAsync(new GetDesktopPacket { Quality = packet.Quality, DisplayIndex = packet.Monitor });
                }
                catch (Exception)
                {
                    _ = _client.QueuePacketAsync(new GetDesktopPacket { Quality = packet.Quality, DisplayIndex = packet.Monitor });
                }
            }
        }

        public void Handler(GetMonitorsPacket packet)
        {
            OnDisplaysChanged(packet.Number);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
