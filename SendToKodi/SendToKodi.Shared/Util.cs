using MetroLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SendToKodi
{
    static class Util
    {
		private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger("Util");

		public static async Task ShowError(string message)
		{
			try
			{
				var dialog = new MessageDialog(message, "Error");
				await dialog.ShowAsync();
			}
			catch (UnauthorizedAccessException ex)
			{
				// This happens when there's already a message box shown.
				// Log and continue for now. Obviously this shouldn't happen though.
				// TODO: Make this fatal.
				logger.Error("Failed to show message box, probably because one was already shown.", ex);
			}
		}
	}
}
