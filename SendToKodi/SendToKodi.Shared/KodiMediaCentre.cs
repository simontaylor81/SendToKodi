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

		public KodiMediaCentre()
		{
			// TODO
			url = "http://alfred:8080/jsonrpc";
			username = "xbmc";
			password = "xbmc";

			// Create http client with the given username & password.
			var credentials = new NetworkCredential(username, password);
			var handler = new HttpClientHandler() { Credentials = credentials };
			httpClient = new HttpClient(handler);
		}

		public Task Send(SharePayload media)
		{
			return SendRpc("Player.Open", new { file = media.contents });
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

			Debug.WriteLine(json.ToString());

			// POST it to Kodi
			try
			{
				var response = await httpClient.PostAsync(url, new StringContent(json.ToString(), Encoding.UTF8, "application/json"));
				response.EnsureSuccessStatusCode();
				Debug.WriteLine(await response.Content.ReadAsStringAsync());
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
