using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace SendToKodi
{
    static class Util
    {
		public static async Task ShowError(string message)
		{
			var dialog = new MessageDialog(message, "Error");
			await dialog.ShowAsync();
		}
	}
}
