using MetroLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SendToKodi
{
    public sealed partial class MainPage
	{
		public MainPage()
		{
			this.InitializeComponent();
			this.NavigationCacheMode = NavigationCacheMode.Required;
		}

		private async void sendUrlButton_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Use commands
			if (isSending)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(sendUrlBox.Text))
			{
				return;
			}

			var url = sendUrlBox.Text;

			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				// Try adding http://
				url = "http://" + url;
			}

			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				// Invalid URL
				await Util.ShowError("Invalid URL");
				return;
			}

			await SendGuarded(url);
		}

		private async void testYouTubeButton_Click(object sender, RoutedEventArgs e)
		{
			if (isSending)
			{
				return;
			}
			await SendGuarded("https://www.youtube.com/watch?v=RR3ORb5gVqk");
		}

		private async Task SendGuarded(string url)
		{
			// TODO: Use commands
			isSending = true;

			try
			{
				// TODO: put this somewhere more appropriate?
				await ((App)Application.Current).SendUri(new Uri(url, UriKind.Absolute));
			}
			catch (Exception ex)
			{
				logger.Info("Failed to send to Kodi", ex);
				await Util.ShowError("Failed to send to Kodi. See debug log.");
			}
			finally
			{
				isSending = false;
			}
		}
		private async void wakeButton_Click(object sender, RoutedEventArgs e)
		{
			await WakeOnLan.Wake(new byte[] { 0x00, 0x23, 0xae, 0x03, 0xee, 0x55 });
		}

		private bool isSending = false;
		private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
	}
}
