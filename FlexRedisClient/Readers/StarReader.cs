using System;
using System.Collections.Generic;

namespace Sumeru.Flex.RedisClient
{
	internal class StarReader : IRedisReader
	{
		int currentPosition = 1; // Since known that first char is a *, setting currentPosition to 1.

		public RedisResponse Translate(string strReSPFromRedis)
		{
			RedisResponse response = new RedisResponse();
			int arrayLen = GetNumber(strReSPFromRedis);
			List<string> elements = new List<string>();

			this.currentPosition = this.currentPosition + arrayLen.ToString().Length + 2; // now points after the /r/n.
			for (int i = 0; i < arrayLen; i++) //this loops the destination array size
			{
				switch (strReSPFromRedis[currentPosition])
				{
					case '$':
						this.currentPosition++; //For the dollar, and this will be used by GetCount.
						int dataLength = GetNumber(strReSPFromRedis);
						this.currentPosition = this.currentPosition + dataLength.ToString().Length + 2;
						elements.Add(GetString(strReSPFromRedis, dataLength));
						this.currentPosition = this.currentPosition + dataLength + 2; // Points to next element. +2 takes care of /r/n
						break;
					case ':': // Should not arise since all ints are pushed into redis as strings anyway... but just in case
						this.currentPosition++; // For the :
						elements.Add(GetNumber(strReSPFromRedis).ToString());
						this.currentPosition = this.currentPosition + elements[i].ToString().Length + 2; 
						break;
				}
			}
			response.listResponse = elements;
			return response;
		}


		/// <summary>
		/// From current position till the next \r whatever number is there is returned. There are Regex ways of doing this, but this is C style and fastest. The number returned from here is used by the caller appropriate to the context in which this is called.
		/// </summary>
		/// <param name="strReSPFromRedis">String of bytestream received from redis.</param>
		private int GetNumber(string strReSPFromRedis)
		{
			int count = 0;
			for (int i = currentPosition; i < strReSPFromRedis.Length; i++)
			{
				char curr = strReSPFromRedis[i];
				if (strReSPFromRedis[i] != '\r')
				{
					count = count * 10 + (int)(curr - '0'); // (int) (curr = '0') is one way of convering a char to int. 
				}
				else
				{
					break;
				}
			}
			return count;
		}

		/// <summary>
		/// Gets the string from current position for specified length. Blindly picks substring, and has no idea of caller's context
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="strReSPFromRedis">String re SPF rom redis.</param>
		/// <param name="len">Length.</param>
		private string GetString(string strReSPFromRedis, int len)
		{
			string value = len > 0 ? strReSPFromRedis.Substring(currentPosition, len) : ""; //This takes care of an empty string result from Redis. Substring crashes if len=0
			return value;
		}
	}
}