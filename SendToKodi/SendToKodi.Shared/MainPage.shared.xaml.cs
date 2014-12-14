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
			if (string.IsNullOrWhiteSpace(sendUrlBox.Text))
			{
				return;
			}

			// TODO: Use commands
			if (isSending)
			{
				return;
			}
			isSending = true;

			try
			{
				string url = sendUrlBox.Text;
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

				// TODO: put this somewhere more appropriate?
				await ((App)Application.Current).SendUri(new Uri(url, UriKind.Absolute));
			}
			finally
			{
				isSending = false;
			}
		}

		private async void testYouTubeButton_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Use commands
			if (isSending)
			{
				return;
			}
			isSending = true;

			try
			{
				await ((App)Application.Current).SendUri(new Uri("https://www.youtube.com/watch?v=RR3ORb5gVqk"));
			}
			finally
			{
				isSending = false;
            }
		}

		private bool isSending = false;
    }
}
