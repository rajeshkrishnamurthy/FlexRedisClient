using System;
namespace Sumeru.Flex.RedisClient
{
	/// <summary>
	/// When TCP connection succeeds. FlexRedisClient sends a PING message. This expects a PONG response if Redis server is up. This exception is thrown when the PONG is not received. Solution is to check if Redis server is up and running on the SERVER-IP and PORT combination. 
	/// </summary>
	public class NoRedisRunningException : Exception
	{
		public NoRedisRunningException()
		{
		}

		public NoRedisRunningException(string message)
			: base(message)
		{
		}

		public NoRedisRunningException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
