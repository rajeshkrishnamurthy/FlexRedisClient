namespace Sumeru.Flex.RedisClient
{
	internal interface IRedisReader
	{
		RedisResponse Translate(string strReSPFromRedis);
	}
}