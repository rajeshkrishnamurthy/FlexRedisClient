using System;
using System.Net.Sockets;
using System.Text;
using Sumeru.Flex.RedisClient;

namespace Sumeru.Flex.RedisClient
{
	internal class RedisCommunicationManager
	{
		TcpClient client;
		public Boolean connected { get; private set; }

		public RedisCommunicationManager(string SERVER_IP, int PORT_NO)
		{
			client = new TcpClient();
			TryTcpConnection(SERVER_IP, PORT_NO);
			TryPing();
			connected = true;
		}

		private void TryTcpConnection(string SERVER_IP, int PORT_NO)
		{
			try
			{
				client.Connect(SERVER_IP, PORT_NO);
			}
			catch (Exception)
			{
				throw new RedisConnectionException("Unable to make a Tcp connection to the IP address " + SERVER_IP + "at port " + PORT_NO + ". Check if Redis is running on port 6379 on the server IP mentioned. If it is not, start up redis. If Redis is already running, then this is some network issue, pure and simple. Someone with networking skills needs to be brought in.");
			}
		}

		private void TryPing()
		{
			string command = "$4\r\nping\r\n";
			byte[] pong = SendToRedis(Encoding.ASCII.GetBytes(command));
			if (!Encoding.ASCII.GetString(pong).Contains("PONG"))
			{
				throw new NoRedisRunningException("Tcp connection was established, but not getting a PING-PONG response from Redis server.");
			}
		}

		internal byte[] SendToRedis(byte[] msg)
		{
			NetworkStream nwStream = client.GetStream();
			try
			{
				nwStream.Write(msg, 0, msg.Length);
			}
			catch (Exception)
			{
				throw new RedisCommunicationException("Could not send the command to Redis. Cannot auto-fix. Some problems with the Tcp connectivity perhaps.");
			}

			byte[] bytes = new byte[0];
			try
			{
				bytes = new byte[client.ReceiveBufferSize];
				nwStream.Read(bytes, 0, (int)client.ReceiveBufferSize);
			}
			catch (Exception)
			{
				throw new RedisCommunicationException("Unable to read response from Redis. Cannot auto-correct this. Some Tcp connection issue perhaps.");
			}
			return bytes;
		}
	}
}
