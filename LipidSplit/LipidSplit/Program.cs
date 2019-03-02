using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LipidSplit
{
	class Program
	{
		static void Main(string[] args)
		{
			//Only parameter is path to file to be processed
			string path = args[0];
			
			//Reads all lipid entries in
			List<string> entries = new List<string>();
			using (StreamReader reader = new StreamReader(path))
			{
				//Assumes file has a header row at the top as is standard in LIQUID
				reader.ReadLine();
				while (reader.Peek() > -1)
				{
					entries.Add(reader.ReadLine());
				}
			}

			//Removes old file for rewrite
			File.Delete(path);
			using (StreamWriter writer = new StreamWriter(path))
			{
				//Header line
				writer.WriteLine("Common Name\tFA1\tFA2\tFA3\tSumC\t#DB");

				//Processes each lipid and writes it out
				foreach (string line in entries)
				{
					StringBuilder output = new StringBuilder();
					output.Append(line);

					int sumCarbons = 0;
					int numDoubleBonds = 0;

					string matched = Regex.Match(line, @"\(.*\)").ToString();
					string[] splits = matched.Split('/');

					if (splits.Any())
					{
						splits[0] = splits[0].Remove(0, 1);
						splits[splits.Count() - 1] = splits[splits.Count() - 1].Remove(splits[splits.Count() - 1].Length - 1);
						for (int i = 0; i < splits.Count(); i++)
						{
							string carbonMatch = Regex.Match(splits[i], @"[0-9]+\:").ToString();
							sumCarbons += Convert.ToInt32(carbonMatch.Remove(carbonMatch.Length - 1, 1));
							string dbMatch = Regex.Match(splits[i], @"\:[0-9]+").ToString();
							numDoubleBonds += Convert.ToInt32(dbMatch.Remove(0 ,1));
							output.Append("\t" + " " + splits[i]);
						}
					}

					switch (splits.Count())
					{
						case 0:
							output.Append("\t\t\t");
							break;
						case 1:
							output.Append("\t\t");
							break;
						case 2:
							output.Append("\t");
							break;
					}
					output.Append("\t" + sumCarbons + "\t" + numDoubleBonds);
					writer.WriteLine(output.ToString());
				}
			}

		}
	}
}
