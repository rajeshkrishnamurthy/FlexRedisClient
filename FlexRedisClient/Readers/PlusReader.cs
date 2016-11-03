using System;

namespace Sumeru.Flex.RedisClient
{
	internal class PlusReader : IRedisReader
	{
		public RedisResponse Translate(string strReSPFromRedis)
		{
			return new RedisResponse();
		}
	}
}