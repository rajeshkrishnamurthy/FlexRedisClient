﻿using System;
using System.Collections.Generic;

namespace RedisNetClientSim
{
	public class SeedData
	{
		public static List<string> saddSeed()
		{
			List<string> setMembers = new List<string>();
			setMembers.Add("Lead:G4");
			setMembers.Add("Lead:G5");
			return setMembers;
		}

		public static Person SeedForSetEntity()
		{
			Person p = new Person();
			p.name = null;
			p.age = 40;
			p.favouriteBooks = new List<string>();
			p.favouriteBooks.Add("C# Forever");
			p.favouriteBooks.Add("Count of Monte Cristo");
			return p;
		}

		public static List<Person> SeedForSetEntities()
		{
			return new List<Person>();
		}

		internal static List<string> SeedForGetEntities()
		{
			List<string> keys = new List<string>();
			keys.Add("person:prateek2");
			keys.Add("person:prateek3");
			return keys;
		}

		public static List<string> SeedForSinter()
		{
			List<string> sets = new List<string>();
			sets.Add("index:lead:firstname:Ram");
			sets.Add("index:lead:status:documents-approved");
			return sets;
		}

		public static List<string> SeedForSunion()
		{
			List<string> sets = new List<string>();
			sets.Add("tentimes");
			sets.Add("fivetimes");
			sets.Add("twotimes");
			return sets;
		}

		internal static List<string> SeedForMget()
		{
			List<string> keys = new List<string>();
			keys.Add("Srikar");
			keys.Add("Sarveshwar");
			keys.Add("Sriram");
			return keys;
		}

}
}
