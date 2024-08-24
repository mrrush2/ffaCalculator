using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffaCalcualtor
{
	class Helper
	{

	public static void bubbleSort(List<List<player>> arr)
		{
			int n = arr.Count;
			for (int i = 0; i < n - 1; i++)
				for (int j = 0; j < n - i - 1; j++)
					if (SumHelper.sumPoints(arr.ElementAt(j)) > SumHelper.sumPoints(arr.ElementAt(j + 1)))
					{
						// swap temp and arr[i]
						List<player> temp = arr.ElementAt(j);
						arr[j] = arr.ElementAt(j + 1);
						arr[j + 1] = temp;
					}
		}

		/* Prints the array */
		public static void printArray(int[] arr)
		{
			int n = arr.Length;
			for (int i = 0; i < n; ++i)
				Console.Write(arr[i] + " ");
			Console.WriteLine();
		}

		public static int findDifPlayers(List<player> best, List<player> list)
		{
			int same = 0;
			foreach (player pb in best)
			{
				foreach (player pl in list)
				{
					//Console.WriteLine(pb.name + " " + pl.name);
					if (pb.name.Equals(pl.name))
					{
						same++;
					}
				}
			}
			return same;
		}
		public static bool compareLists(List<player> list1, List<player> list2)
		{
			int count = 0;
			foreach (player p in list1)
			{
				foreach (player p2 in list2)
				{
					if (p2.Equals(p))
					{
						count++;
						break;
					}
				}
			}
			if (count < 9) return true;
			else return false;
		}

		public static void outputCSV(String outputcsv, List<List<player>> Lineup)
		{
			string header = "position" + ", " + "name" + ", " + "points" + ", " + "lower" + ", " + "salary" + ", " + "risk\n";
			File.WriteAllText(outputcsv, header);
			foreach (List<player> list in Lineup)
			{
				foreach (player p in list)
				{
					File.WriteAllText(outputcsv, p.toString());
				}
				string sums = "" + ", " + "" + ", " + SumHelper.sumPoints(list) + ", " + SumHelper.sumFloor(list) + ", " + SumHelper.sumSalary(list) + ", " + SumHelper.sumRisk(list) + "\n";
				File.WriteAllText(outputcsv, sums + "\n");
				File.WriteAllText(outputcsv, "\n");
			}
		}
	}
}
