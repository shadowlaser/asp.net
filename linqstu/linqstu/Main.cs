using System;
using System.Linq;
namespace linqstu
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int[] num1 = {0,2,4,6,8};
			int[] num2 = {1,3,5,7};
			var pairs = from a in num1
				from b in num2 
				where a > b
        			select new {a, b};
			foreach (var pair in pairs)
				Console.WriteLine ("{0} is less than {1}", pair.a, pair.b);
			Linq1 ();

			/*
			string[] names = {"Jack","Bob","Bill"};
			var rs = from n in names 
				where n.StartsWith ("B") 
					select n.ToLower ();	
			foreach (var r in rs)
				Console.WriteLine (r);
			Console.WriteLine ("Hello World!");*/
		}

		public static void Linq1 ()
		{
			// The Three Parts of a LINQ Query:
			//  1. Data source.
			int[] numbers = new int[7] { 0, 1, 2, 3, 4, 5, 6 };

			// 2. Query creation.
			// numQuery is an IEnumerable<int>
			var numQuery =
            from num in numbers
            where (num % 2) == 0
            select num;

			// 3. Query execution.
			foreach (int num in numQuery) {
				Console.Write ("{0,1} ", num);
			}
		}
	}
}
