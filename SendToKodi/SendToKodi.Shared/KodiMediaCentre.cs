using MetroLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SendToKodi
{
	// Class for sending media to a Kodi media centre (i.e. XBMC)
	class KodiMediaCentre
	{
		private string url;
		private string username;
		private string password;
		private HttpClient httpClient;

		private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<KodiMediaCentre>();

		public KodiMediaCentre()
		{
			// TODO
			url = "http://192.168.0.150:8080/jsonrpc";
			username = "xbmc";
			password = "xbmc";

			// Create http client with the given username & password.
			var credentials = new NetworkCredential(username, password);
			var handler = new HttpClientHandler() { Credentials = credentials };
			httpClient = new HttpClient(handler);
		}

		public Task Send(SharePayload media)
		{
			switch (media.type)
			{
				case SharePayloadType.Uri:
					return SendRpc("Player.Open", new { file = media.contents });

				case SharePayloadType.YouTube:
					var uri = "plugin://plugin.video.youtube/?action=play_video&videoid=" + media.contents;
					return SendRpc("Player.Open", new { file = uri });
            }

			throw new Exception("Unknown media type: " + media.type.ToString());
		}

		private async Task SendRpc(string method, params object[] args)
		{
			var rpcid = "SendToKodi";

			// Create JSON payload.
			// Can't use JsonConvert.SerializeObject because 'params' is a reserved word in C#.
			var json = new JObject(
				new JProperty("jsonrpc", "2.0"),
				new JProperty("method", method),
				new JProperty("id", rpcid)
			);

			if (args != null)
			{
				json.Add(new JProperty("params",
					new JArray(args.Select(arg => JObject.FromObject(arg)))));
			}

			logger.Debug("Sending request to {0}", url);
			logger.Trace(json.ToString());

			// POST it to Kodi
			try
			{
				var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
				logger.Trace("Posting request...");
                var responseTask = httpClient.PostAsync(url, content);
				logger.Trace("Awaiting request...");
				var response = await responseTask.ConfigureAwait(false);

				logger.Trace("Response status code = {0}", response.StatusCode);
				logger.Trace("Response.Content.ToString() = {0}", response.Content.ToString());

				response.EnsureSuccessStatusCode();

				if (logger.IsTraceEnabled)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					logger.Trace("Successfully sent request. Response from Kodi:");
					logger.Trace(responseContent);
				}
			}
			catch (Exception ex)
			{
				logger.Info("Error sending to Kodi", ex);
				throw;
			}
		}
	}
}
