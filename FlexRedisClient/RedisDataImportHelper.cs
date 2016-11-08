using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Sumeru.Flex.RedisClient
{
	public class RedisDataImportHelper
	{
		string outfile;

		/// <summary>
		/// Initialize RedisDataImportHelper by providing a filename that will be the destination for the ReSP output. File will be opened in Append mode. 
		/// </summary>
		/// <example>
		/// <code>
		/// RedisDataImportHelper helper = new RedisDataImportHelper("filename.txt");
		/// </code></example>
		/// <param name="filename">Output filename</param>
		public RedisDataImportHelper(string filename)
		{
			outfile = filename;
		}

		/// <summary>
		/// Identical to the set command in RedisClient. Only this writes to the filestream, and doesn't execute. 
		/// </summary>
		/// <example>
		/// <code>
		/// helper.setToFile("key1","value1");
		/// </code>
		/// </example>
		/// <param name="key">Key to set</param>
		/// <param name="value">Value to set</param>
		public void setToFile(RedisDataImportHelper instance, string key,string value)
		{
			List<string> elements = new List<string>();
			elements.Add("set");
			elements.Add(key);
			elements.Add(value);
			byte[] command = Convert(elements);
			Write(command);
		}

		/// <summary>
		/// Identical to the sadd command in RedisClient. Only this writes to the filestream, and doesn't execute.
		/// </summary>
		/// <example>
		/// <code>
		/// List<string> members = new List<string> { "member1", "member2" };
		/// helper.saddToFile("setkey", members);
		/// </code>
		/// </example>
		/// <param name="key">Key.</param>
		/// <param name="members">Members.</param>
		public void saddToFile(string key, List<string> members)
		{
			List<string> elements = new List<string>();
			elements.Add("sadd");
			elements.Add(key);
			elements.AddRange(members);
			byte[] command = Convert(elements);
			Write(command);
		}

		/// <summary>
		/// Identical to the SetEntity command in RedisClient. Only this writes to the filestream, and doesn't execute.
		/// </summary>
		/// <example>
		/// <code>
		/// Person p = new Person();
		/// p.name = "Somename";
		/// p.age = 40;
		/// helper.SetEntityToFile<Person>("key1", p);
		/// </code>
		/// </example>
		/// <param name="key">The Redis key to write to</param>
		/// <param name="entity">Object to serialize</param>
		/// <typeparam name="T">The type of the object</typeparam>
		public void SetEntityToFile<T>(string key, T entity)
		{
			Contract.Requires((key != null && key != ""), "Key cannot be empty or null");
			string json = JsonConvert.SerializeObject(entity);
			List<string> elements = new List<string>();
			elements.Add("set");
			elements.Add(key);
			elements.Add(json);
			byte[] command = Convert(elements);
			Write(command);
		}

		/// <summary>
		/// Exactly same input API spec as AutocompleteAdd. Instead of executing, writes out the ReSP to the named file. 
		/// </summary>
		/// <example>
		/// <code>
		/// RedisDataImportHelper helper = new RedisDataImportHelper("redis-import.txt");
		/// 
		/// string index = "collector:names";
		/// AutocompleteItem item1 = new AutocompleteItem { value = "Mohan", id = "1" };
		/// AutocompleteItem item2 = new AutocompleteItem { value = "Mohit", id = "2" };
		/// 
		/// List<AutocompleteItem> items = new List<AutocompleteItem>();
		/// items.Add(item1);
		///	items.Add(item2);
		///
		///	helper.AutocompleteAddToFile(index, items);
		/// </code>
		/// </example>
		/// <param name="index">Index.</param>
		/// <param name="members">Members.</param>

		public void AutocompleteAddToFile(string index, List<AutocompleteItem> members)
		{
			List<string> elements = new List<string>();
			elements.Add("zadd");
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
			byte[] command = Convert(elements);
			Write(command);
		}

		public void TwoWayMapAdd(string key, List<KeyValuePair<string, string>> mapData)
		{
			List<string> elements = new List<string>();
			elements.Add("zadd");
			elements.Add(key);

			foreach (KeyValuePair<string, string> kv in mapData)
			{
				elements.Add("0");
				elements.Add(kv.Key + ':' + kv.Value);
				elements.Add("0");
				elements.Add(kv.Value + ':' + kv.Key);
			}

			byte[] command = Convert(elements);
			Write(command);
		}
		                                  
		byte[] Convert(List<string> elements)
		{
			ReSPTranslator translator = new ReSPTranslator();
			return translator.TranslateToRedis(elements);
		}

		void Write(byte[] msg)
		{
			using (FileStream fs = new FileStream(outfile, FileMode.Append))
			{
				fs.Write(msg, 0, msg.Length);
			}
		}
	}
}