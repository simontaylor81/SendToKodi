using MetroLog;
using SendToKodi.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Share Target Contract item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace SendToKodi
{
	/// <summary>
	/// This page allows other applications to share content through this application.
	/// </summary>
	public sealed partial class ShareTargetPage : Page
	{
		/// <summary>
		/// Provides a channel to communicate with Windows about the sharing operation.
		/// </summary>
		private ShareOperation shareOperation;
		private ObservableDictionary defaultViewModel = new ObservableDictionary();
		private Uri webLink;

		private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ShareTargetPage>();

		public ShareTargetPage()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Gets the view model for this <see cref="Page"/>.
		/// This can be changed to a strongly typed view model.
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get { return this.defaultViewModel; }
		}

		/// <summary>
		/// Invoked when another application wants to share content through this application.
		/// </summary>
		/// <param name="e">Activation data used to coordinate the process with Windows.</param>
		public async void Activate(ShareTargetActivatedEventArgs e)
		{
			this.shareOperation = e.ShareOperation;

			// Communicate metadata about the shared content through the view model
			var shareProperties = this.shareOperation.Data.Properties;
			var thumbnailImage = new BitmapImage();
			this.DefaultViewModel["Title"] = shareProperties.Title;
			this.DefaultViewModel["Description"] = shareProperties.Description;
			this.DefaultViewModel["Image"] = thumbnailImage;
			this.DefaultViewModel["Sharing"] = false;
			this.DefaultViewModel["ShowImage"] = false;
			this.DefaultViewModel["Comment"] = string.Empty;
			this.DefaultViewModel["Placeholder"] = "Add a comment";
			this.DefaultViewModel["SupportsComment"] = true;
			Window.Current.Content = this;
			Window.Current.Activate();

			// Update the shared content's thumbnail image in the background
			if (shareProperties.Thumbnail != null)
			{
				var stream = await shareProperties.Thumbnail.OpenReadAsync();
				thumbnailImage.SetSource(stream);
				this.DefaultViewModel["ShowImage"] = true;
			}

			// Retrieve the link from the share operation.
			if (shareOperation.Data.Contains(StandardDataFormats.WebLink))
			{
				webLink = await shareOperation.Data.GetWebLinkAsync();

				// Kick off sharing immediately.
				await DoShare();
			}
		}

		/// <summary>
		/// Invoked when the user clicks the Share button.
		/// </summary>
		/// <param name="sender">Instance of Button used to initiate sharing.</param>
		/// <param name="e">Event data describing how the button was clicked.</param>
		//private async void ShareButton_Click(object sender, RoutedEventArgs e)
		//{
		//	// Kick off sharing.
		//	if (webLink != null)
		//	{
		//		await DoShare();
		//	}
		//}

		// Perform the actual share.
		private async Task DoShare()
		{
			this.DefaultViewModel["Sharing"] = true;

			Debug.Assert(webLink != null);

			try
			{
				logger.Trace("Initiating share: " + webLink.ToString());
				await ((App)Application.Current).SendUri(webLink);

				logger.Trace("Share operation completed");
				shareOperation.ReportCompleted();
			}
			catch (Exception ex)
			{
				logger.Info("Share operation failed", ex);
				shareOperation.ReportError(ex.Message);
			}
		}
	}
}
