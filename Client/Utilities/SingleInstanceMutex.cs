namespace Client.Utilities
{
    public class SingleInstanceMutex : IDisposable
    {
        private readonly Mutex _appMutex;

        public bool CreatedNew { get; }
        public bool IsDisposed { get; private set; }

        public SingleInstanceMutex(string name)
        {
            _appMutex = new Mutex(false, $"Local\\{name}", out var createdNew);
            CreatedNew = createdNew;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            _appMutex?.Dispose();

            IsDisposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
