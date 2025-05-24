# 🎮 RichConnect

**RichConnect** is an open-source Windows desktop application built with WPF that lets you select and manage your current activity — and sync it seamlessly with your **Discord Rich Presence**.

---

## ✨ Features

- 🎛️ Beautiful WPF interface for activity selection  
- 🔄 Real-time updates to Discord Rich Presence  
- 📋 Customizable activity presets  
- 🕒 Optional session timers and status auto-clear  
- 🖥️ Minimizes to system tray for background usage  
- 💬 Built with C# and .NET  

---

## 🚀 Getting Started

### Prerequisites

- [.NET Desktop Runtime](https://dotnet.microsoft.com/en-us/download) (Core or Framework depending on build)  
- [Discord App](https://discord.com/) (installed and running)  
- Windows 10 or higher  

### Installation

1. Clone the repo:
   ```bash
   git clone https://github.com/yourusername/RichConnect.git
   cd RichConnect
   ```

2. Open in **Visual Studio** and restore NuGet packages.

3. Build and run the app.

---

# ⚙️ Configuration

Before launching the app, you must create a configuration file to link RichConnect with your Discord application:

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications) and create a new application.
2. Copy your **Client ID**.
3. In the root directory of the project (next to the executable), create a file named `config.json` with the following content:

```json
{
    "ApplicationId": "YOUR_DISCORD_CLIENT_ID"
}

- 💡 Note: The app will not work unless a valid ApplicationId is present in the config.json file.

---

## 🧠 Example Use Case

Choose:
- 📖 “Studying”  
- 🎮 “Playing Elden Ring”  
- 📺 “Watching YouTube”  

…and RichConnect will update your Discord profile instantly, showing friends what you’re up to.

---

## 📦 Built With

- [WPF (.NET)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/) — for the desktop interface  
- [Lachee’s DiscordRPC](https://github.com/Lachee/discord-rpc-csharp) — to interface with Discord  
- [XAML](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/xaml/) — for rich UI design  

---

## 🤝 Contributing

Pull requests are welcome! Please open an issue first to discuss what you’d like to change or improve.

1. Fork the project  
2. Create your feature branch (`git checkout -b feature/my-feature`)  
3. Commit your changes (`git commit -m 'Add my feature'`)  
4. Push to the branch (`git push origin feature/my-feature`)  
5. Open a Pull Request  

---

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## 🌐 Connect

Created with 💙 by Deebz  
Discord: [ByteMinds](https://discord.gg/BcWyYgWw6a)
GitHub: [@iamdeebz](https://github.com/iamdeebz)
