using System;
namespace Sumeru.Flex.RedisClient
{
	/// <summary>
	/// A pure data class that holds the result of executed commands in Redis. This is relevant only if the command is a pure write command - such as set, sadd, zadd etc. It is irrelevant for fetch queries such as get, GetEntity, sinter etc. 
	/// </summary>
	public class CommandResult
	{
		/// <summary>
		/// True if last executed command was a Success. False if failure. If failed, the error provided by Redis (which is not usually very helpful!!) is stored in <see cref="ErrorMessage"/> 
		/// </summary>
		/// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
		public bool Success { get; set;} 

		/// <summary>
		/// Contains the error message returned by Redis. Usually this message doesnt help too much... but that is Redis style, and usually the things that go wrong have to go with wrong value being provided against a particular data type.
		/// </summary>
		/// <value>The error message.</value>
		public string ErrorMessage { get; set; }

		/// <summary>
		/// If a command is successful Redis returns the number of records affected. If a value already exists in a set and a sadd is performed, it affects 0 records. But if a new value is added through 'sadd' then 1 record is affected. 
		/// </summary>
		/// <value>The records affected.</value>
		public int RecordsAffected { get; set; }
	}
}