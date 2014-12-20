using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SendToKodi
{
    public static class WakeOnLan
    {
		private const string destinationAddress = "255.255.255.255";
		private const string destinationPort = "9";
		private const int payloadSize = 102;
		private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger("WakeOnLan");

		/// <summary>
		/// Wake the device with the given MAC address.
		/// </summary>
		/// <param name="mac">The MAC address of the device to wake. Must be an array of 6 bytes.</param>
		public static async Task Wake(byte[] mac)
		{
			var payload = ConstructMagicPacket(mac);
			var destHostname = new HostName(destinationAddress);

			// Try sending on all ip interfaces.
			foreach (var localHostname in NetworkInformation.GetHostNames().Where(hn => hn.IPInformation != null))
			{
				try
				{
					// Create socket to send the data.
					using (var socket = new DatagramSocket())
					{
						// Connect to the broadcast address with the current local address.
						var endpointPair = new EndpointPair(localHostname, "", destHostname, destinationPort);
						await socket.ConnectAsync(endpointPair);

						// Write magic packet to the socket.
						using (var writer = new DataWriter(socket.OutputStream))
						{
							writer.WriteBytes(payload);
							await writer.StoreAsync();
						}
					}
				}
				catch (Exception)
				{
					// This can fail if the network is down.
					// Just silently fail and try the next adapter.
				}
			}
        }

		/// <summary>
		/// Construct a magic packet for waking a device with the given MAC address.
		/// </summary>
		/// <param name="mac">The MAC address of the device to wake. Must be an array of 6 bytes.</param>
		/// <returns>The magic packet payload as an array of bytes.</returns>
		public static byte[] ConstructMagicPacket(byte[] mac)
		{
			Debug.Assert(mac.Length == 6);

			var payload = new byte[payloadSize];
			int destIndex = 0;

			// First 6 bytes are 0xFF.
			for (; destIndex < 6; destIndex++)
			{
				payload[destIndex] = 0xFF;
			}

			// Next, repeat the mac 16 times.
			for (int i = 0; i < 16; i++)
			{
				System.Buffer.BlockCopy(mac, 0, payload, destIndex, 6);
				destIndex += 6;
			}

			return payload;
		}
	}
}
