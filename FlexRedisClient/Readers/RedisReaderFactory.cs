using System;
using System.Diagnostics.Contracts;

namespace Sumeru.Flex.RedisClient
{
	internal class RedisReaderFactory
	{
		internal static IRedisReader GetReader(char v)
		{
			Contract.Ensures(Contract.Result<IRedisReader>() != null);
			switch (v)
			{
				case '*':
					return new StarReader();
				case '$':
					return new DollarReader();
				case '-':
					return new MinusReader();
				case '+':
					return new PlusReader();
			}
			return new MinusReader();
		}
	}
}