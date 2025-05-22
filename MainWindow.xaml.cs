using DiscordRPC;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RichConnect
{
    public class AppConfig
    {
        public string ApplicationId { get; set; }
    }

    public partial class MainWindow : Window
    {
        private DiscordRpcClient _client;

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
                    Application.Current.Shutdown();
                    return;
                }

                string jsonString = File.ReadAllText("./config.json");
                var configData = JsonSerializer.Deserialize<AppConfig>(jsonString);

                if (configData == null || string.IsNullOrWhiteSpace(configData.ApplicationId))
                {
                    MessageBox.Show("Invalid config file or missing ApplicationId.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                    return;
                }

                _client = new DiscordRpcClient(configData.ApplicationId);
                _client.Initialize();

                _selectedImageKey = _imageKeys.Count > 0 ? _imageKeys[0] : null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize application: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
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
            if (sender is RadioButton rb && rb.Tag is string key)
            {
                _selectedImageKey = key;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Prevent the window from closing, just minimize it
            e.Cancel = true;
            this.WindowState = WindowState.Minimized;
        }

        // Remove or comment out your OnClosed override, as it will not be called unless the app is actually closed
        // protected override void OnClosed(EventArgs e)
        // {
        //     _client?.Dispose();
        //     base.OnClosed(e);
        // }
    }
}