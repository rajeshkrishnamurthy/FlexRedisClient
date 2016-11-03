using System;
using System.Collections.Generic;
using System.IO;
using Sumeru.Flex.RedisClient;

namespace RedisNetClientSim
{
	class MainClass
	{
		public static void Main(String[] args)
		{
			//string SERVER_IP = "199.43.199.17";
			//int PORT = 9379;
			string SERVER_IP = "127.0.0.1";
			int PORT = 6379;
			IRedisClient client = new FlexRedisClient(SERVER_IP, PORT);

			// These methods are my stand-in for test methods. 
			// I have no idea how to write tests on Xamarin Studio and 
			// run them, hence this poor man's approach to unit testing!
			// Any of these can be commented without any adverse effect. 
			// However comment EndTransaction and StartTransaction together always. 

			//StartTransaction(client);
			//set(client);
			//sadd(client);
			//SetEntity(client);
			//CancelTransaction(client);
			//get(client);
			//GetEntity(client);
			//sinter(client);
			//mget(client);
			//sunion(client);
			//EndTransaction(client);
			//GetEntities(client);
			//PrepareForBulkImport();
			RedisDataImportClient();
			//zadd(client);
			// AutocompleteAdd(client);
			//AutocompleteSearch(client);
		}

		private static void set(IRedisClient client)
		{
			string k = "z";
			string v = "26";
			CommandResult result = client.set(k,v);
			ProcessResult(result);
		}

		private static void sadd(IRedisClient client)
		{
			string key = "index:lead:status:documents-approved";
			List<string> members = SeedData.saddSeed();
			CommandResult result = client.sadd(key, members);
			ProcessResult(result);
		}

		private static void zadd(IRedisClient client)
		{
			string key = "z1";
			int score = 0;
			string member = "m10";
			CommandResult result = client.zadd(key, score, member);
			ProcessResult(result);
		}

		private static void AutocompleteAdd(IRedisClient client)
		{
			string key = "collector:names";
			AutocompleteItem item1 = new AutocompleteItem { value = "Mohan", id = "1" };
			AutocompleteItem item2 = new AutocompleteItem { value = "Mohit", id = "2" };
			AutocompleteItem item3 = new AutocompleteItem { value = "Manmohan", id = "3" };

			List<AutocompleteItem> items = new List<AutocompleteItem>();
			items.Add(item1);
			items.Add(item2);
			items.Add(item3);
			CommandResult result = client.AutocompleteAdd(key, items);
			ProcessResult(result);
		}

		private static void AutocompleteSearch(IRedisClient client)
		{
			string index = "collector:names";
			string searchString = "Mo";
			List<AutocompleteItem> items = client.AutocompleteSearch(index, searchString);
			foreach (var item in items)
			{
				Console.WriteLine(item.value + '|' + item.id);
			}
		}

		private static void SetEntity(IRedisClient client)
		{
			string key = "person:prateek";
			Person p = SeedData.SeedForSetEntity();
 			CommandResult result = client.SetEntity<Person>(key, p);
			ProcessResult(result);
		}

		static void get(IRedisClient client)
		{
			string k = "p3";
			string value = client.get(k);
			Console.WriteLine(value);
		}

		private static void GetEntity(IRedisClient client)
		{
			string key = "Lead:G1";
			Person p = client.GetEntity<Person>(key);
			Console.WriteLine(p.ToString());
		}

		static void GetEntities(IRedisClient client)
		{
			List<string> keys = SeedData.SeedForGetEntities(); 
			Dictionary<string, Person> p = client.GetEntities<Person>(keys);
			Console.WriteLine(p.Keys);
		}

		static void sinter(IRedisClient client)
		{
			List<string> sets = SeedData.SeedForSinter();
			List<string> intersect = client.sinter(sets);
			foreach (string s in intersect)
			{
				Console.WriteLine(s);
			}
		}

		static void sunion(IRedisClient client)
		{
			List<string> sets = SeedData.SeedForSunion();
			List<string> union = client.sunion(sets);
			foreach (string s in union)
			{
				Console.WriteLine(s);
			}
		}

		private static void StartTransaction(IRedisClient client)
		{
			client.StartTransaction();
		}

		static void EndTransaction(IRedisClient client)
		{
			client.RunTransaction();
		}

		static void CancelTransaction(IRedisClient client)
		{
			client.CancelTransaction();
		}

		private static void ProcessResult(CommandResult result)
		{
			if (result.Success)
			{
				int count = result.RecordsAffected;
				Console.WriteLine(count);
			}
			else
			{
				Console.WriteLine("Command failed with error {0}", result.ErrorMessage);
			}
		}

		private static void RedisDataImportClient()
		{
			RedisDataImportHelper helper = new RedisDataImportHelper("/Users/Rajesh/Temp/redis-import.txt");
			//helper.setToFile(helper, "key1", "value1");
			//helper.setToFile(helper, "key2", "value2");
			//helper.setToFile(helper, "key3", "value3");
			//string skey = "setkey1";
			//List<string> smembers = new List<string> { "m1", "m2", "m3" };
			//helper.saddToFile(skey, smembers);
			//string entitykey = "entitykey1";
			//Person p = new Person();
			//p.name = "Ram";
			//p.age = 40;
			//p.favouriteBooks = new List<string> { "Book1", "Book2" };
			//helper.SetEntityToFile<Person>(entitykey, p);
			string index = "collector:names";
			AutocompleteItem item1 = new AutocompleteItem { value = "Mohan", id = "1" };
			AutocompleteItem item2 = new AutocompleteItem { value = "Mohit", id = "2" };
			AutocompleteItem item3 = new AutocompleteItem { value = "Manmohan", id = "3" };

			List<AutocompleteItem> items = new List<AutocompleteItem>();
			items.Add(item1);
			items.Add(item2);
			items.Add(item3);

			helper.AutocompleteAddToFile(index, items);
		}
	}
}