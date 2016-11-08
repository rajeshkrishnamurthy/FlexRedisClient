using System;
using System.Collections.Generic;

namespace Sumeru.Flex.RedisClient
{
	public interface IRedisClient
	{
		void StartTransaction();
		CommandResult RunTransaction();
		void CancelTransaction();

		CommandResult set(string key, string value);
		string get(string key);
		// Dictionary<string, string> mget(List<string> keys);

		CommandResult SetEntity<T>(string key, T obj) where T:class;
		T GetEntity<T>(string key) where T:class;
		Dictionary<string, T> GetEntities<T>(List<string> keys) where T : class;

		CommandResult sadd(string key, List<string> members);
		List<string> sinter(List<string> sets);
		List<string> sunion(List<string> sets);
		CommandResult zadd(string key, int score, string member);
		CommandResult AutocompleteAdd(string index, List<AutocompleteItem> members);
		List<AutocompleteItem> AutocompleteSearch(string index, string searchString);
		CommandResult TwoWayMapAdd(string key, List<KeyValuePair<string, string>> mapData);
		CommandResult TwoWayMapRemove(string key, List<KeyValuePair<string, string>> mapData);
	}
}