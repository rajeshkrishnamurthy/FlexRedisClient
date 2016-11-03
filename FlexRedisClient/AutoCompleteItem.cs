using System;
namespace Sumeru.Flex.RedisClient
{
	/// <summary>
	/// Usually auto-completes require an ID to accompany each of the matching search results. This class packages these two essential items in an autocomplete search.
	/// </summary>
	public class AutocompleteItem
	{
		/// <summary>
		/// The value that is to be searched on.
		/// </summary>
		public string value { get; set; }

		/// <summary>
		/// The id associated with the value.
		/// </summary>
		/// <value>The identifier.</value>
		public string id { get; set; }
	}
}
