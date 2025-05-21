using DiscordRPC;
using System.Windows;


namespace RichConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DiscordRpcClient _client;
        public MainWindow()
        {
            InitializeComponent();

            _client = new DiscordRpcClient("YOUR_DISCORD_APP_CLIENT_ID");
            _client.Initialize();

            _client.SetPresence(new RichPresence()
            {
                Details = "Using RichConnect!",
                State = "Setting up Discord RPC",
                Assets = new Assets()
                {
                    LargeImageKey = "large_image",
                    LargeImageText = "RichConnect"
                }
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            _client.Dispose();
            base.OnClosed(e);
        }

        private void UpdatePresenceButton_Click(object sender, RoutedEventArgs e)
        {
            _client.SetPresence(new RichPresence()
            {
                Details = "User clicked the button!",
                State = "Active in RichConnect",
                Assets = new Assets()
                {
                    LargeImageKey = "large_image",
                    LargeImageText = "RichConnect"
                }
            });
        }
    }
}