using System;

namespace Sumeru.Flex.RedisClient
{
	/// <summary>
	/// If a Tcp connection could not be established to the server ip and port combination, this exception is thrown. This happens prior to attempting a PING-PONG check with Redis. 
	/// </summary>
	public class RedisConnectionException : Exception
	{
		public RedisConnectionException()
		{
		}

		public RedisConnectionException(string message)
			: base(message)
		{
		}

		public RedisConnectionException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}