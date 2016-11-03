using System;

namespace Sumeru.Flex.RedisClient
{
	internal class DollarReader : IRedisReader
	{
		public RedisResponse Translate(string strReSPFromRedis)
		{
			int len = 0;
			int bytePosOfValue = 0;
			RedisResponse response = new RedisResponse();
			for (int i = 1; i < strReSPFromRedis.Length; i++)
			{
				char curr = strReSPFromRedis[i];
				if (strReSPFromRedis[i] != '\r')
				{
					len = len * 10 + (int)(curr - '0'); // (int) (curr = '0') is one way of convering a char to int. 
				}
				else
				{
					bytePosOfValue = i + 2; // i points to \r, then there is \n and then value begins. 
					break;
				}
			}
			string value = len > 0 ? strReSPFromRedis.Substring(bytePosOfValue, len) : ""; //This takes care of an empty string result from Redis. Substring crashes if len=0
			response.strResponse = value;
			return response;
		}
	}
}