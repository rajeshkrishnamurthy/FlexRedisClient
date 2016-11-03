using System;

namespace Sumeru.Flex.RedisClient                
{
	/// <summary>
	/// It is possible due to network failures that connection with Redis may have been successfully established, but a particular command could not be sent to Redis. This is in all probability a network issue. Another possibility is Redis Server which was running has gone down due to some reason.  
	/// </summary>
	public class RedisCommunicationException : Exception
	{
		public RedisCommunicationException()
		{
		}

		public RedisCommunicationException(string message)
			: base(message)
		{
		}

		public RedisCommunicationException(string message, Exception inner)
			: base(message, inner)
		{
		}
}
}