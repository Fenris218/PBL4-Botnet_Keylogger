using Common.Helper;
using Server.Extensions;
using Server.Helper;
using Server.Networking;
using Server.Networking.Handlers;

namespace Server.Forms
{
    public partial class FrmRemoteShell : Form
    {
        private static readonly Dictionary<Client, FrmRemoteShell> OpenedForms = new Dictionary<Client, FrmRemoteShell>();

        public static FrmRemoteShell CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmRemoteShell f = new FrmRemoteShell(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        private readonly Client _connectClient;
        public readonly RemoteShellHandler _remoteShellHandler;

        public FrmRemoteShell(Client client)
        {
            _connectClient = client;
            _remoteShellHandler = client.ClientHandler.RemoteShellHandler;

            RegisterMessageHandler();// Đăng ký sự kiện xử lý tin nhắn
            InitializeComponent();

            txtConsoleOutput.AppendText(">> Type 'exit' to close this session" + Environment.NewLine, Color.Yellow);
        }

        private void RegisterMessageHandler()
        {
            _connectClient.Disconnected += ClientDisconnected;
            _remoteShellHandler.CommandSuccessEvent += CommandSuccess;
            _remoteShellHandler.CommandErrorEvent += CommandError;
        }

        private void UnregisterMessageHandler()
        {
            _connectClient.Disconnected -= ClientDisconnected;
            _remoteShellHandler.CommandSuccessEvent -= CommandSuccess;
            _remoteShellHandler.CommandErrorEvent -= CommandError;
        }

        private void ClientDisconnected(Client client)
        {
            this.Invoke((MethodInvoker)this.Close);
        }

        private void CommandSuccess(object sender, string output)
        {
            txtConsoleOutput.AppendText(output, Color.WhiteSmoke);
        }

        private void CommandError(object sender, string output)
        {
            txtConsoleOutput.AppendText(output, Color.Red);
        }

        private void FrmRemoteShell_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Shell", _connectClient);
            this.DoubleBuffered = true;
            _remoteShellHandler.SendCommand("");
        }

        private void FrmRemoteShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            _remoteShellHandler.SendCommand("exit");
            UnregisterMessageHandler();
        }

        private void txtConsoleOutput_TextChanged(object sender, EventArgs e)
        {
            NativeMethodsHelper.ScrollToBottom(txtConsoleOutput.Handle);
        }

        private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(txtConsoleInput.Text.Trim()))
            {
                string input = txtConsoleInput.Text.TrimStart(' ', ' ').TrimEnd(' ', ' ');
                txtConsoleInput.Text = string.Empty;

                // Split based on the space key.
                string[] splitSpaceInput = input.Split(' ');
                // Split based on the null key.
                string[] splitNullInput = input.Split(' ');

                // We have an exit command.
                if (input == "exit" ||
                    ((splitSpaceInput.Length > 0) && splitSpaceInput[0] == "exit") ||
                    ((splitNullInput.Length > 0) && splitNullInput[0] == "exit"))
                {
                    this.Close();
                }
                else
                {
                    switch (input)
                    {
                        case "cls":
                            txtConsoleOutput.Text = string.Empty;
                            break;
                        default:
                            _remoteShellHandler.SendCommand(input);
                            break;
                    }
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtConsoleOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)2)
            {
                txtConsoleInput.Text += e.KeyChar.ToString();
                txtConsoleInput.Focus();
                txtConsoleInput.SelectionStart = txtConsoleOutput.TextLength;
                txtConsoleInput.ScrollToCaret();
            }
        }

        private void txtConsoleInput_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
