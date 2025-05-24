using DiscordRPC;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace RichConnect
{
    public class AppConfig
    {
        public string ApplicationId { get; set; }
    }

    public partial class MainWindow : Window
    {
        private DiscordRpcClient _client;
        private NotifyIcon _notifyIcon;
        private bool _isExit;

        private readonly List<string> _imageKeys = new()
        {
            "jinwoo",
            "femboy",
        };

        private string _selectedImageKey;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                if (!File.Exists("./config.json"))
                {
                    MessageBox.Show("Config file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Windows.Application.Current.Shutdown();
                    return;
                }

                string jsonString = File.ReadAllText("./config.json");
                var configData = JsonSerializer.Deserialize<AppConfig>(jsonString);

                if (configData == null || string.IsNullOrWhiteSpace(configData.ApplicationId))
                {
                    MessageBox.Show("Invalid config file or missing ApplicationId.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Windows.Application.Current.Shutdown();
                    return;
                }

                _client = new DiscordRpcClient(configData.ApplicationId);
                _client.Initialize();

                _selectedImageKey = _imageKeys.Count > 0 ? _imageKeys[0] : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
            }

            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("appicon.ico"),
                Visible = true,
                Text = "RichConnect"
            };
            _notifyIcon.DoubleClick += (s, e) => ShowMainWindow();

            AddNotifyIconMenu();
        }

        private void ShowMainWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Select an image (must be uploaded to Discord Developer Portal)"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImagePreview.Source = bitmap;
                    ImagePreview.Visibility = Visibility.Visible;
                }
                catch
                {
                    MessageBox.Show("Failed to load image.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePresenceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client == null) return;

            _client.SetPresence(new RichPresence()
            {
                Details = DetailsBox.Text,
                State = StateBox.Text,
                Assets = new Assets()
                {
                    LargeImageKey = _selectedImageKey,
                    LargeImageText = _selectedImageKey
                }
            });
        }

        private void ImageRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.RadioButton rb && rb.Tag is string key)
            {
                _selectedImageKey = key;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;
                _notifyIcon.BalloonTipTitle = "RichConnect";
                _notifyIcon.BalloonTipText = "Application minimized to tray.";
                _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                _notifyIcon.ShowBalloonTip(1000);
            }
            else
            {
                _notifyIcon.Dispose();
            }
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon.Dispose();
            _client?.Dispose();
            base.OnClosed(e);
        }

        private void AddNotifyIconMenu()
        {
            var contextMenu = new ContextMenuStrip();

            var discordBackground = System.Drawing.Color.FromArgb(54, 57, 63);
            var discordForeground = System.Drawing.Color.FromArgb(220, 221, 222);
            var discordHighlight = System.Drawing.Color.FromArgb(79, 84, 92);

            contextMenu.BackColor = discordBackground;
            contextMenu.ForeColor = discordForeground;
            contextMenu.Renderer = new DiscordRenderer(discordBackground, discordHighlight, discordForeground);

            var exitMenuItem = new ToolStripMenuItem("Exit")
            {
                BackColor = discordBackground,
                ForeColor = discordForeground,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular)
            };
            exitMenuItem.Click += (s, e) =>
            {
                _isExit = true;
                Close();
            };
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private class DiscordRenderer : ToolStripProfessionalRenderer
        {
            private readonly System.Drawing.Color _backColor;
            private readonly System.Drawing.Color _highlightColor;
            private readonly System.Drawing.Color _foreColor;

            public DiscordRenderer(System.Drawing.Color backColor, System.Drawing.Color highlightColor, System.Drawing.Color foreColor)
                : base(new DiscordColorTable(backColor, highlightColor))
            {
                _backColor = backColor;
                _highlightColor = highlightColor;
                _foreColor = foreColor;
            }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                var bounds = e.AffectedBounds;
                using (var brush = new System.Drawing.SolidBrush(_backColor))
                using (var path = RoundedRect(bounds, 8))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                var rect = new System.Drawing.Rectangle(System.Drawing.Point.Empty, e.Item.Size);
                if (e.Item.Selected)
                {
                    using (var brush = new System.Drawing.SolidBrush(_highlightColor))
                    using (var path = RoundedRect(rect, 6))
                    {
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        e.Graphics.FillPath(brush, path);
                    }
                    e.Item.ForeColor = System.Drawing.Color.White;
                }
                else
                {
                    using (var brush = new System.Drawing.SolidBrush(_backColor))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                    e.Item.ForeColor = _foreColor;
                }
            }
            private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(System.Drawing.Rectangle bounds, int radius)
            {
                int diameter = radius * 2;
                var path = new System.Drawing.Drawing2D.GraphicsPath();
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                return path;
            }
        }

        private class DiscordColorTable : ProfessionalColorTable
        {
            private readonly System.Drawing.Color _backColor;
            private readonly System.Drawing.Color _highlightColor;

            public DiscordColorTable(System.Drawing.Color backColor, System.Drawing.Color highlightColor)
            {
                _backColor = backColor;
                _highlightColor = highlightColor;
            }

            public override System.Drawing.Color MenuItemSelected => _highlightColor;
            public override System.Drawing.Color MenuItemBorder => _highlightColor;
            public override System.Drawing.Color ToolStripDropDownBackground => _backColor;
            public override System.Drawing.Color ImageMarginGradientBegin => _backColor;
            public override System.Drawing.Color ImageMarginGradientMiddle => _backColor;
            public override System.Drawing.Color ImageMarginGradientEnd => _backColor;
        }
    }
}