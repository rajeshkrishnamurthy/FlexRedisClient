using System;
using System.Collections.Generic;

namespace RedisNetClientSim
{
	public class Person
	{
		public string name { get; set;} 
		public int age { get; set;}
		public List<string> favouriteBooks { get; set; }
	}
}
