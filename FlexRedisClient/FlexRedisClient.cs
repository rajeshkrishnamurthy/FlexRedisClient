using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Newtonsoft.Json;

namespace Sumeru.Flex.RedisClient
{
	public class FlexRedisClient : IRedisClient 
	{
		RedisCommunicationManager manager;
		string keyPrefix;
		/// <summary>
		/// Initializes a new instance of the FlexRedisClient when a successful connection to the Redis Server is established.  
		/// </summary>
		/// <example>
		/// <code>
		/// IRedisClient client = new FlexRedisClient(SERVER_IP, PORT);
		/// </code>
		/// </example>
		/// <exception cref="Sumeru.Flex.RedisClient.NoRedisRunningException">Thrown when TCP connection is established, but Redis PING-PONG response is not obtained</exception>
		/// <exception cref="Sumeru.Flex.RedisClient.RedisConnectionException">Thrown when no TCP connection can be established</exception>
		/// <param name="SERVER_IP">Server ip.</param>
		/// <param name="PORT_NO">Port no.</param>
		/// <param name="key_prefix">A key prefix can optionally be provided. This is useful in dev and test when multiple devs may be trying out stuff against the same Redis server instance. All keys are automatically prefixed with this key param name="key_prefix".</param>
		public FlexRedisClient(string SERVER_IP, int PORT_NO, string key_prefix = "")
		{
			manager = new RedisCommunicationManager(SERVER_IP, PORT_NO);
			keyPrefix = key_prefix;
		}

		/// <summary>
		/// Wrapper around the Redis 'set' command. Should be used for setting simple key values. For setting objects into keys, use <see cref="SetEntity{T}"/> 
		/// </summary>
		/// <example>
		/// <code>

		/// 	string k = "z";
		/// 	string v = "26";
		/// 	CommandResult result = client.set(k, v);
		/// </code>
		/// </example>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public CommandResult set(string key, string value)
		{
			key = PrefixKey(key);
			List<string> elements = new List<string>();
			elements.Add("set");
			elements.Add(key);
			elements.Add(value);
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// Wrapper around the Redis 'sadd' command. Add the specified members to the set stored at key. Specified members that are already a member of this set are ignored. If key does not exist, a new set is created before adding the specified members.
		/// </summary>
		/// <example>
		/// <code>
		/// string key = "index:lead:status:documents-approved";
		///	List<string> members = SeedData.saddSeed();
		///	CommandResult result = client.sadd(key, members);
		/// </code>
		/// </example>
		/// <returns>A CommandResult with Success flag set and RecordsAffected property updated</returns>
		/// <param name="key">Key that identifies the set. Use proper : separated names for easy identification</param>
		/// <param name="members">Typically a set will contain references to primary key id's which are then retrieved using a <see cref="get(string)"/>  or <see cref="GetEntity{T}"/>  or <see cref="GetEntities{T}"/></param>
		public CommandResult sadd(string key, List<string> members)
		{
			List<string> elements = new List<string>();
			elements.Add("sadd");
			key = PrefixKey(key);
			elements.Add(key);
			elements.AddRange(members);
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// Wrapper around the Redis 'zadd' command. Add the specified member to the set stored at key providing a score. Specified members that are already a member of this set are ignored. While zadd in Redis permits multiple members to be added at the same time, each with a different score, this current implementation of zadd permits only one member at a time to be added. If key does not exist, a new set is created before adding the specified members.
		/// </summary>
		/// <example>
		/// <code>
		/// string key = "z1";
		///	int score = 10;
		/// string members = "m1";
		///	CommandResult result = client.zadd(key, score, member);
		/// </code>
		/// </example>
		/// <returns>A CommandResult with Success flag set and RecordsAffected property updated</returns>
		/// <param name="key">Key that identifies the set. Use proper : separated names for easy identification</param>
		/// <param name="score">The score associated with the member </param>
		/// <param name="member">Typically a set will contain references to primary key id's which are then retrieved using a <see cref="get(string)"/>  or <see cref="GetEntity{T}"/>  or <see cref="GetEntities{T}"/></param>
		public CommandResult zadd(string key, int score, string member)
		{
			List<string> elements = new List<string>();
			elements.Add("zadd");
			key = PrefixKey(key);
			elements.Add(key);
			elements.Add(score.ToString());
			elements.Add(member);
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// The list of values that needs to be searched on through autocomplete should be added through AutocompleteAdd. The provided AutocompleteItem is serialized to the format `lowercase:original:id` in order to enable a case-insensitive auto-complete search experience. 
		/// </summary>
		/// <example>
		/// <code>
		/// string key = "collector:names";
		/// AutocompleteItem item1 = new AutocompleteItem { value = "Mohan", id = "1" };
		/// AutocompleteItem item2 = new AutocompleteItem { value = "Mohit", id = "2" };
		/// AutocompleteItem item3 = new AutocompleteItem { value = "Manmohan", id = "3" };
		///
		/// List<AutocompleteItem> items = new List<AutocompleteItem>();
		/// items.Add(item1);
		///	items.Add(item2);
		///	items.Add(item3);
		/// 
		///	CommandResult result = client.AutocompleteAdd(key, items);
		/// </code>
		/// </example>
		/// <returns>A CommandResult with Success flag set and RecordsAffected property updated</returns>
		/// <param name="index">The name of the auto-complete list. Best stored in typical : separated format.</param>
		/// <param name="members">The list itself that is to be searched on. However for sake of object simplicity, it is returned as an AutoComplete object list with ID included. </param>
		public CommandResult AutocompleteAdd(string index, List<AutocompleteItem> members)
		{
			List<string> elements = new List<string>();
			elements.Add("zadd");
			index = PrefixKey(index);
			elements.Add(index);

			foreach (AutocompleteItem member in members)
			{
				StringBuilder serializedAutocompleteItem = new StringBuilder();
				serializedAutocompleteItem.Append(member.value.ToLower()); // this takes care that if some values are added capitalized and some small, the comparison happens on all lower case basis. The value is stored as `normalized:original:id` and will be handled transparently during retrieval so AutocompleteItem is returned 
				serializedAutocompleteItem.Append(":");
				serializedAutocompleteItem.Append(member.value);
				serializedAutocompleteItem.Append(":");
				serializedAutocompleteItem.Append(member.id);
				elements.Add("0");
				elements.Add(serializedAutocompleteItem.ToString());
			}
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// Given an index and a search string, it returns a list of matching auto-completes with the id attached. 
		/// </summary>
		/// <example>
		/// <code>
		/// 
		/// string index = "collector:names";
		/// string searchString = "Mo";
		/// 
		/// List<AutocompleteItem> items = client.AutocompleteSearch(index, searchString);
		///	
		/// foreach (var item in items)
		///	{
		///		Console.WriteLine(item.value + '|' + item.id);
		///	}
		/// 
		/// </code>
		/// </example>
		/// <returns>List of AutocompleteItem</returns></returns>
		/// <param name="index">Name of index to retrieve from</param>
		/// <param name="searchString">Search string.</param>
		public List<AutocompleteItem> AutocompleteSearch(string index, string searchString)
		{
			searchString = searchString.ToLower();
			List<string> elements = new List<string>();
			elements.Add("zrangebylex");
			index = PrefixKey(index);
			elements.Add(index);
			string start = "[" + searchString;
			elements.Add(start);
			string end = start + ' '; //the final bytestream needs a hex 255. that cannot be added as a string, it can only be added as a byte. However, Redistranslation requires the number following the $ to contain the number of bytes. So this space is added, only to be replaced a few lines later with hex 255. This is to suit Redis syntax.
			elements.Add(end); 
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			int len = msg.Length;
			msg[len-3] = (byte)0xff; //the hex 255 replaces the space that was earlier added. This is to suit Redis syntax, this is just some byte manipulation that has to be done.
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);

			List<AutocompleteItem> items = new List<AutocompleteItem>();
			foreach (string s in response.listResponse)
			{
				string[] t = s.Split(':');
				AutocompleteItem item = new AutocompleteItem();
				item.value = t[1]; // since the second field holds the original value (first field had a ToLower conversion)
				item.id = t[2]; // the third field was stored with the id.
				items.Add(item);
			}
			return items;
		}

		/// <summary>
		/// Serializes object as a JSON and stores the JSON as the value against the given key.
		/// </summary>
		/// <example>
		/// <code>
		/// string key = "person:prateek";
		/// 
		/// Person p = new Person();
		/// p.name = "Somename";
		/// p.age = 50;
		/// 
		/// CommandResult result = client.SetEntity<Person>(key, p);
		/// </code>
		/// </example>
		/// <returns>A CommandResult with Success flag set and RecordsAffected property updated</returns>
		/// <param name="key">This is the key that is stored in Redis.</param>
		/// <param name="entity">The object that is desired to be stored.</param>
		public CommandResult SetEntity<T>(string key, T entity) where T:class
		{
			Contract.Requires((key != null && key != ""), "Key cannot be empty or null");
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.NullValueHandling = NullValueHandling.Ignore;
			string json = JsonConvert.SerializeObject(entity, settings);
			List<string> elements = new List<string>();
			elements.Add("set");
			key = PrefixKey(key);
			elements.Add(key);
			elements.Add(json);
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// Mappings between entities can be maintained by Redis and is an innovative use of Redis' sorted set. Given a key k1 and two strings z1 and z2, this method produces zadd k1 0 z1:z3 0 z2:z1. Any number of such string pairs can be provided as a list of KeyValuePairs.
		/// </summary>
		/// <returns>The way map add.</returns>
		/// <param name="key">The key of the sorted set.</param>
		/// <param name="mapData">IDs between whom mappings are to be established are provided as a list of keyvaluepairs.  </param>
		public CommandResult TwoWayMapAdd(string key, List<KeyValuePair<string, string>> mapData)
		{
			List<string> elements = new List<string>();
			elements.Add("zadd");
			key = PrefixKey(key);
			elements.Add(key);

			foreach (KeyValuePair<string, string> kv in mapData)
			{
				elements.Add("0");
				elements.Add(kv.Key + ':' + kv.Value);
				elements.Add("0");
				elements.Add(kv.Value + ':' + kv.Key);
			}
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// Reverses the effect of <see cref="TwoWayMapAdd"/> for the given key and members. Both way mappings will be removed. Given a key k1 and strings z1 and z2, this method generates 'zrem k1 z1:z2 z2:z1'
		/// </summary>
		/// <returns>The way map remove.</returns>
		/// <param name="key">Key of the sorted set.</param>
		/// <param name="mapData">Members to remove from the mapping. </param>
		public CommandResult TwoWayMapRemove(string key, List<KeyValuePair<string, string>> mapData)
		{
			List<string> elements = new List<string>();
			elements.Add("zrem");
			key = PrefixKey(key);
			elements.Add(key);

			foreach (KeyValuePair<string, string> kv in mapData)
			{
				elements.Add(kv.Key + ':' + kv.Value);
				elements.Add(kv.Value + ':' + kv.Key);
			}
			return ExecuteCommand(elements);
		}

		/// <summary>
		/// Indexes are FlexRedisClient wrappers which mimic the creation of an index (these are then used by 'sinter' for simulating SQL joins). Maintaining these indexes using native Redis commands involves manipulating sets and hashes. This method masks that complexity and maintains indexes with the required level of data integrity. IMPORTANT: This assumes that within a given indexname, a key occurs only once. eg. if indexing on firstname, then a key can have only one firstname. This is not the same as unique index though, as multiple keys can have the same first name!  
		/// </summary>
		/// <example>
		/// <code>
		/// string indexName = "index:lead:firstname";
		/// string indexLeaf = "dhanya";
		/// string keyReference = "k50";
		/// client.AddIndex(indexName, indexLeaf, keyReference);
		/// </code>
		/// </example>
		/// <param name="indexName">In a key of index:lead:firstname:name1, index:lead:firstname would be the index name</param>
		/// <param name="indexLeaf">In a key of index:lead:firstname:name1, name1 would be the leaf</param>
		/// <param name="keyReference">The key reference that is to be stored as the value.</param>
		public void AddIndex(string indexName, string indexLeaf, string keyReference)
		{
			char nameLeafDelimiter = ':'; //Left un-editable on purpose.

			// Get the old index that had this key reference
			List<string> hashOldElement = new List<string>(); 
			hashOldElement.Add("hget");
			string hashRedisKey = "ih:" + indexName;
			hashOldElement.Add(hashRedisKey);
			hashOldElement.Add(keyReference);
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(hashOldElement);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			string oldIndexLeaf = translator.TranslateFromRedis(ReSPFromRedis).strResponse;

			List<string> indexOldElement = new List<string>();
			// Remove the keyReference from the old index
			if (oldIndexLeaf != null)
			{
				indexOldElement.Add("srem");
				indexOldElement.Add(indexName + nameLeafDelimiter + oldIndexLeaf);
				indexOldElement.Add(keyReference);

				hashOldElement.Clear();
				hashOldElement.Add("hdel");
				hashOldElement.Add(hashRedisKey);
				hashOldElement.Add(keyReference);
			}
	
			// Add the key reference to the new index
			List<string> indexNewElement = new List<string>();
			string key = indexName + nameLeafDelimiter + indexLeaf;
			indexNewElement.Add("sadd");
			indexNewElement.Add(key);
			indexNewElement.Add(keyReference);

			// Create an internal hash to connect up the key reference and the indexLeaf. The hash key is based on indexName
			List<string> hashNewElement = new List<string>();
			hashNewElement.Add("hset");
			hashNewElement.Add(hashRedisKey);
			hashNewElement.Add(keyReference);
			hashNewElement.Add(indexLeaf); //The keyReference is a key in the hash, and its value is the indexLeaf. For eg. this tells us that a key k1 is mapped to a specific leaf say 'Amitabh' in the hint:lead:firstname index.

			StartTransaction();

			if (indexOldElement.Count > 0)
			{
				ExecuteCommand(hashOldElement); // Remove the hash reference that was maintained for the index
				ExecuteCommand(indexOldElement); // Remove past references of key reference against given index Name
			}
			ExecuteCommand(hashNewElement); // Create internal hash for the provided index
			ExecuteCommand(indexNewElement); // Add index for provided index

			RunTransaction();
		}

		/// <summary>
		/// Removes the index for given index name, leaf and key reference
		/// </summary>
		/// <example>
		/// <code>
		/// string indexName = "index:lead:firstname";
		/// string indexLeaf = "dhanya3";
		/// string keyReference = "k5";
		/// client.RemoveIndex(indexName, indexLeaf, keyReference);
		/// </code>
		/// </example>
		/// <param name="indexName">Index name.</param>
		/// <param name="indexLeaf">Index leaf.</param>
		/// <param name="keyReference">Key reference.</param>
		public void RemoveIndex(string indexName, string indexLeaf, string keyReference)
		{
			Contract.Requires(indexName != null, "Index name cannot be null");
			Contract.Requires(indexLeaf != null, "Index leaf cannot be null");
			Contract.Requires(keyReference != null, "Key Reference cannot be null");

			char nameLeafDelimiter = ':'; //Left un-editable on purpose.

			List<string> hashElement = new List<string>();
			List<string> indexElement = new List<string>();
			// Remove the keyReference from the index
			indexElement.Add("srem");
			indexElement.Add(indexName + nameLeafDelimiter + indexLeaf);
			indexElement.Add(keyReference);

			// To remove the key reference from the hash
			hashElement.Clear();
			hashElement.Add("hdel");
			string hashRedisKey = "ih:" + indexName;
			hashElement.Add(hashRedisKey);
			hashElement.Add(keyReference);

			StartTransaction();
			ExecuteCommand(indexElement);
			ExecuteCommand(hashElement);
			RunTransaction();
		}

		/// <summary>
		/// Wrapper around the 'multi' command of Redis. Marks the start of a transaction block. Subsequent commands will be queued until <see cref="RunTransaction"/> or <see cref="CancelTransaction"/> is executed.
		/// </summary>
		/// <example><code>client.StartTransaction()</code></example>
		public void StartTransaction()
		{
			List<string> elements = new List<string>() { "multi" };
			ExecuteCommand(elements);
		}

		/// <summary>
		/// Wrapper around 'exec' command of Redis. Executes all previously queued commands.
		/// </summary>
		/// <example><code>client.RunTransaction()</code></example>
		/// <returns>The return value of this must not be processed at this point. Since multiple transactions are executed, the return value needs more thought than just a CommandResult.</returns>
		public CommandResult RunTransaction()
		{
			List<string> elements = new List<string>() { "exec" };
			return ExecuteCommand(elements); //TODO: Examine whether this result shoUld be processed
		}

		/// <summary>
		/// Wrapper around the 'discard' command of Redis. Flushes all previously queued commands. 
		/// </summary>
		/// <example><code>client.CancelTransaction()</code></example>
		public void CancelTransaction()
		{
			List<string> elements = new List<string>() { "discard" };
			ExecuteCommand(elements);
		}

		/// <summary>
		/// Get the specified key. Wrapper around the Redis get method. Takes only a single key. Ideally this should be used only for keys that dont store objects. For object retrieval, use <see cref="GetEntity{T} instead"/> 
		/// </summary>
		/// <example>
		/// <code>
		/// string value = client.get("somekey");
		/// </code>
		/// </example>
		/// <param name="key">Key.</param>

		public string get(string key)
		{
			key = PrefixKey(key);
			List<string> elements = new List<string>() { "get", key };
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);
			return response.strResponse;
		}



		/// <summary>
		/// Given a key, deserializes and returns the object stored against the key. The supplied class T must match the object stored.
		/// </summary>
		/// <example>
		/// <code>
		/// string key = "Lead:G1";
		/// Person p = client.GetEntity<Person>(key);
		/// </code>
		/// </example>
		/// <exception cref="System.Exception.SystemException.InvalidCastException">If supplied class (as return type) doesnt match with the data stored against the key</exception>
		/// <returns>Value stored in Redis as an object of type T.</returns>
		/// <param name="key">Name of the key</param>
		/// <param name="T">The type to which data stored in Redis must be cast to</typeparam>
		public T GetEntity<T>(string key) where T : class
		{
			key = PrefixKey(key);
			List<string> elements = new List<string>() { "get", key };
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);
			T entity;
			try
			{
				entity = JsonConvert.DeserializeObject<T>(response.strResponse);
 			}
			catch (InvalidCastException)
			{
				throw new InvalidCastException("Unable to translate data to type " + typeof(T) + ". The value stored in this key is" + response.strResponse);
			}
			return entity;
		}

		/// <summary>
		/// Similar to <see cref="GetEntity{T}"/>. This returns a list of objects. 
		/// </summary>
		/// <example>
		/// <code>
		/// List<string> keys = new List<string> { "person:p1", "person:p2" }; 
		/// Dictionary<string, Person> p = client.GetEntities<Person>(keys);
		/// </code>
		/// </example>
		/// <exception cref="System.Exception.SystemException.InvalidCastException">If supplied class (as return type) doesnt match with the data stored against the key</exception>
		/// <returns>Dictionary with objects as values and Redis keys as the keys of the dictioary.</returns>
		/// <param name="keys">Keys to retrieve.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public Dictionary<string, T> GetEntities<T>(List<string> keys) where T : class
		{
			List<string> elements = new List<String>() { "mget" };

			for (int i = 0; i < keys.Count; i++)
			{
				keys[i] = PrefixKey(keys[i]);
			}
			elements.AddRange(keys);
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);

			List<T> entities = new List<T>();
			foreach (string value in response.listResponse)
			{
				try
				{
					entities.Add(JsonConvert.DeserializeObject<T>(value));
				}
				catch (InvalidCastException)
				{
					throw new InvalidCastException("Unable to translate data to type " + typeof(T) + ". The value that caused this exception is" + value);
				}
			}

			// Now mapping the keys (input) and entities (output) into a dictionary before returning.
			Dictionary<string, T> dictResponse = new Dictionary<string, T>();

			for (int i = 0; i < keys.Count; i++)
			{
				dictResponse[keys[i]] = entities[i];
			}
			return dictResponse;
		}

		/// <summary>
		/// Wrapper around the sinter command of Redis. Returns the members of the set resulting from the intersection of all the given sets. Typically used on indexes to simulate a relational database style join.
		/// </summary>
		/// <example>
		/// <code>
		/// List<string> sets = new List<string> { "set1", "set2" };
		/// List<string> intersect = client.sinter(sets);
		/// </code>
		/// </example>
		/// 
		/// <param name="sets">The keys of the sets that need to be intersected</param>
		public List<string> sinter(List<string> sets)
		{
			List<String> elements = new List<String>() { "sinter" };

			for (int i = 0; i < sets.Count; i++)
			{
				sets[i] = PrefixKey(sets[i]);
			}

			elements.AddRange(sets);
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);
			return response.listResponse;
		}

		/// <summary>
		/// Wrapper around the sunion command of Redis. Returns the members of the set resulting from the union of all the given sets.
		/// </summary>
		/// <example>
		/// <code>
		/// List<string> sets = new List<string> { "set1", "set2" };
		/// List<string> union = client.sunion(sets);
		/// </code>
		/// </example>
		/// <param name="sets">Sets that must be union'ed</param>
		/// 
		public List<string> sunion(List<string> sets)
		{
			List<String> elements = new List<String>() { "sunion" };

			for (int i = 0; i < sets.Count; i++)
			{
				sets[i] = PrefixKey(sets[i]);
			}
			elements.AddRange(sets);
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);
			return response.listResponse;
		}

		private Dictionary<string, string> mget(List<string> keys)
		{
			List<string> elements = new List<String>() { "mget" };

			for (int i = 0; i < keys.Count; i++)
			{
				keys[i] = PrefixKey(keys[i]);
			}
			elements.AddRange(keys);
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] ReSPFromRedis = manager.SendToRedis(msg);
			RedisResponse response = translator.TranslateFromRedis(ReSPFromRedis);

			Dictionary<string, string> dictResponse = new Dictionary<string, string>();

			for (int i = 0; i < keys.Count; i++)
			{
				dictResponse[keys[i]] = response.listResponse[i];
			}
			return dictResponse;
		}

		// All commands that write into Redis will return a CommandResult
		private CommandResult ParseResponse(byte[] response)
		{
 			string result = Encoding.ASCII.GetString(response);
			CommandResult Cr = new CommandResult();

			// All errors return - as the first char in the response
			if (result.Substring(0, 1) == "-")
			{
				Cr.Success = false;
				Cr.ErrorMessage = result.Substring(1, result.Length - 1);
			}

			else
			{
				Cr.Success = true;
			}
			Cr.RecordsAffected = SetRecordsAffectedBy(result);
			return Cr;
		}

		private int SetRecordsAffectedBy(string result)
		{
			int count;
			//Whenever Redis returns a records affected count (an int response), the first character in the response is a colon (:)
			if (result.Substring(0, 1) == ":")
			{
				int endIndex = result.IndexOf("\r"); // All numbers between the starting colon and the first \r (carriage return) refer to number of records affected. 
				count = Convert.ToInt32(result.Substring(1, endIndex));
			}
			else
			{
				count = -1; //If no record is affected, setting to -1. This is just an arbitrary convention I am adopting. 
			}
			return count;
		}

		//public CommandResult SetEntities<T>(string key, List<T> objs)
		//{
		//	throw new NotImplementedException();
		//}

		string PrefixKey(string key)
		{
			return keyPrefix + key;
		}

		/// <summary>
		/// All commands (as opposed to queries) should go through this method.  
		/// </summary>
		/// <returns>CommandResult.Success will indicate whether the command succeeded or failed. CommandResult.RecordsAffected will give more info.  
		/// </returns>
		/// <param name="elements">For > set key value, this would contain 3 elements with "set", "key" and "value"</param></param>
		private CommandResult ExecuteCommand(List<string> elements)
		{
			ReSPTranslator translator = new ReSPTranslator();
			byte[] msg = translator.TranslateToRedis(elements);
			byte[] response = manager.SendToRedis(msg);
			CommandResult Cr = ParseResponse(response);
			return Cr;
		}
	}
}