using System;
using System.Collections.Generic;
using System.Text;

namespace SendToKodi
{
	// A media link of some description to be sent to the media centre.
    class SharePayload
    {
		public string contents;

		// Create a payload object with a direct URI to the media.
		public static SharePayload CreateUri(string uri)
		{
			var result = new SharePayload();
			result.contents = uri;
			return result;
		}

		// Use Create* methods to create.
		private SharePayload()
		{
		}
    }
}
