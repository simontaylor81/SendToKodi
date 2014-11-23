using System;
using System.Collections.Generic;
using System.Text;

namespace SendToKodi
{
	// Available payload types.
	enum SharePayloadType
	{
		Uri,
		YouTube,
	}

	// A media link of some description to be sent to the media centre.
    class SharePayload
    {
		public string contents;
		public SharePayloadType type;

		// Create a payload object with a direct URI to the media.
		public static SharePayload CreateUri(string uri)
		{
			return new SharePayload()
			{
				contents = uri,
				type = SharePayloadType.Uri
			};
		}

		// Create a payload object for a YouTube video.
		public static SharePayload CreateYouTube(string videoId)
		{
			return new SharePayload()
			{
				contents = videoId,
				type = SharePayloadType.YouTube
			};
		}

		// Use Create* methods to create.
		private SharePayload()
		{
		}
    }
}
