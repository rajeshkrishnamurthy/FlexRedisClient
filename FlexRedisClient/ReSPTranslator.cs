using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sumeru.Flex.RedisClient
{
	// TODO: Change access modifiers to internal
	internal class ReSPTranslator
	{
		internal byte[] TranslateToRedis(List<string> elements)
		{
			string crlf = "\r\n";
			int elementCount = elements.Count;
			string bulkArray = "*" + elementCount + crlf;
			foreach (string element in elements)
			{
				int length = element.Length;
				bulkArray = bulkArray + "$" + length + crlf + element + crlf;
			}

			byte[] msg = Encoding.ASCII.GetBytes(bulkArray);
			return msg;
		} 

		internal RedisResponse TranslateFromRedis(byte[] ReSPFromRedis)
		{
			string strReSPFromRedis = Encoding.ASCII.GetString(ReSPFromRedis);
			IRedisReader reader = RedisReaderFactory.GetReader(strReSPFromRedis[0]);
			RedisResponse response = reader.Translate(strReSPFromRedis);
			return response;
		}
	}
}