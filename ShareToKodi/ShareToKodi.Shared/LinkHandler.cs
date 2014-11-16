using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ShareToKodi
{
    class LinkHandler
    {
		// Process a URI.
		public async Task ProcessUri(Uri uri)
		{
			if (uri.Host == "channel9.msdn.com")
			{
				await ProcessChannel9(uri);
			}
			else
			{
				Debug.WriteLine("Unhandled URI: " + uri);
			}
		}

		// Process a Channel 9 URI.
		private async Task ProcessChannel9(Uri uri)
		{
			// Get HTML from link.
			var htmlDoc = await GetHtml(uri);

			// Get the link of the high-res mp4 video.
			var node = htmlDoc.DocumentNode.Descendants()
				.Where(n => n.GetAttributeValue("rel", null) == "Mp4High")
				.FirstOrDefault();
			if (node == null || node.GetAttributeValue("href", "") == "")
			{
				throw new Exception("Could not find video link in Channel 9 page");
			}

			var videoUrl = node.GetAttributeValue("href", "");
			Debug.WriteLine(videoUrl);
		}

		// Load HTML document from the internet.
		private async Task<HtmlDocument> GetHtml(Uri uri)
		{
			using (var httpClient = new HttpClient())
			{
				// Get response from http server.
				var response = await httpClient.GetAsync(uri);

				// Throw if the request failed.
				response.EnsureSuccessStatusCode();

				// Get the actual page contents.
				var pageContents = await response.Content.ReadAsStringAsync();

				// Parse HTML content.
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(pageContents);
				return htmlDoc;
			}
		}
    }
}
