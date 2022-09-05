using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using IronWebScraper;

namespace ffaCalcualtor
{
	class Program
	{
		public static List<playerSingleGame> data = new List<playerSingleGame>();
		public static List<playerSingleGame> mvp = new List<playerSingleGame>();
		public static List<int> mvpUsage = new List<int>();
		public static List<List<playerSingleGame>> bestLineups = new List<List<playerSingleGame>>();
		public static IEnumerable<List<playerSingleGame>> bestLineups2 = new List<List<playerSingleGame>>();
		public static List<List<playerSingleGame>> Lineup = new List<List<playerSingleGame>>();
		public static float points = 0;
		public static int numTeams = 150;
		public static float mvpUsageRate = .33333f;
		public static float kickerUsageRate = .2f;
		//private static String path = "C:/Users/micha/Downloads/ffa_customrankings2020-10.csv";
		private static String fdName = "Week1 Thursday - ffa (1)";
		private static String path = "C:/Users/micha/Downloads/" + fdName + ".csv";
		private static String path2 = "C:/Users/micha/Downloads/FanDuel-NFL-2022 ET-09 ET-08 ET-79000-players-list.csv";
		private static string outputcsv = "C://Users/Micha/OneDrive/Documents/Fantasy/Football/" + fdName + ".csv";
		private static List<Thread> threads = new List<Thread>();
		private static List<List<List<playerSingleGame>>> mvpTeams = new List<List<List<playerSingleGame>>>();

		static void Main(string[] args)
		{
			readCSV("BUF", "LAR");
			findBestLineup(50, 30, 40);
			bubbleSortMvpTeams();
			getMvpTeamsUsage();
			bubbleSort(bestLineups);
			Console.WriteLine(bestLineups.Count);
			printLineupsThreads();
			Console.WriteLine("done");
			Console.ReadKey();
		}

		public static void readCSV(String team1, String team2)
		{
			using (var reader = new StreamReader(path))
			{
				bool first = false;
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');
					if (first)
					{
						float points = 0;
						float lower = 0;
						float upper = 0;
						if (!values[4].Equals("NA")) points = float.Parse(values[4]);//7
						if (!values[7].Equals("NA") || values[4] != null) lower = float.Parse(values[7]);//8
						if (!values[8].Equals("NA") || !values[6].Equals("")) upper = float.Parse(values[8]);//9
						//float left = (points - lower) / (upper - points);
						//Console.WriteLine(values[1] + " " + values[2] + " " + points + "  " + lower + "  " + upper );
						if ((lower < points && upper > points) && (values[3].Equals(team1) || values[3].Equals(team2)))
						{
							Console.WriteLine(values[1]);
							values[1] = values[1].Replace(".", "");
							values[1] = values[1].Replace("'", "");
							//old
							//"playerId","player","team","position","age","exp","bye","points","lower","upper","sdPts","positionRank","dropoff","tier","ptSpread","overallECR","positionECR","sdRank","risk","sleeper","salary"
							//0          1        2       3         4      5     6    7        8       9       10      11				12		13		14			15			16				17		18		19		20
							
							//new
							//"playerId","player","position","team","points","sdPts","dropoff","lower","upper","vor","vor_lower","vor_upper","rank","lower_rank","upper_rank","position_rank","tier","adp","aav","risk"
							//0          1          2          3         4      5        6        7        8       9       10      11		   12		 13		      14			15			16	   17	18		19	
							
							//data.Add(new player(values[1], values[2], values[3], values[7], values[8], values[9], values[10],
							//    values[11], values[12], values[13], values[14], values[16], values[17], values[18]));
							//string name, string team, string position, string points, string lower, string upper, string sdPts, string positionRank, string dropoff, string tier, string ptSpread, string positionECR, string sdRank, string risk

							//data.Add(new playerSingleGame(values[1], values[2], values[3], values[5], values[4], values[6], values[7],
							//values[10], "0", "0", "0", "0", "0", "0"));
							data.Add(new playerSingleGame(values[1], values[3], values[2], values[4], values[7], values[8], values[5],
							values[15], values[6], values[16], "0", "0", "0", values[19]));
						}
					}
					else first = true;
				}
			}
			using (var reader = new StreamReader(path2))
			{
				bool first = false;
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');
					values[3] = values[3].Replace("'", "");
					values[3] = values[3].Replace(".", "");
					//values[3] = values[3].Replace(" Jr", "");
					//values[3] = values[3].Replace(" IV", "");
					//values[3] = values[3].Replace(" III", "");
					//values[3] = values[3].Replace(" II", "");
					if (first)
					{
						foreach (playerSingleGame p in data)
						{
							string str = p.name.Replace("\"", "");
							string pos = p.position.Replace("\"", "");
							string str1 = values[0].Replace("\"", "");
							string str2 = values[1].Replace("\"", "");
							if (str2.Equals("D")) str2 = "k";
							string str3 = values[3].Replace("\"", "");
							string str4 = values[4].Replace("\"", "");
							//Console.WriteLine(pos + "   " + str2);
							if ((str3.Contains(str) && str2.Contains(pos)) || str4.Contains(str))
							{
								int salary = int.Parse(values[7].Replace("\"", ""));
								Console.WriteLine(str + "   " + str3 + "   " + str4 + "    " + salary + "   " + p.points);
								p.playerID = str1;
								p.setSalary(salary);
							}
						}
					}
					else first = true;
				}
			}
		}

		public static void setPosition(string str, List<playerSingleGame> l, double min)
		{
			foreach (playerSingleGame p in data)
			{
				float value = (p.points / p.salary) * 1000;
				//Console.WriteLine(p.name + " : " + p.points + " : " + p.salary + " : " +min);
				//Console.WriteLine(p.name);
				if (p.position.Equals(str) && p.points > min)
				{
					//Console.WriteLine(p.points + "  " + p.name + "  " + p.salary);
					l.Add(p);
				}
			}
		}

		public static void findBestLineup(int points, int risk, int floor)
		{

			for (int i = 0; i < data.Count; i++)
			{
				//Console.WriteLine(data.ElementAt(i).points);
				if (data.ElementAt(i).points > 10)
				{
					//Console.WriteLine("bigger than 10");
					playerSingleGame mvpPlayer = new playerSingleGame(data.ElementAt(i));
					mvpPlayer.points = mvpPlayer.points * 1.5f;
					mvp.Add(mvpPlayer);
					mvpUsage.Add(0);
				}
			}

			for (int i = 0; i < mvp.Count; i++)
			{
				mvpTeams.Add(new List<List<playerSingleGame>>());
				//Console.WriteLine(i);
				for (int j = 0; j < data.Count; j++)
				{
					for (int c = j+1; c < data.Count; c++)
					{
						for (int a = c+1; a < data.Count; a++)
						{
							for (int b = a+1; b < data.Count; b++)
							{
								Console.WriteLine(mvp.ElementAt(i).name + " " + data.ElementAt(j).name + " " + data.ElementAt(c).name + " " + data.ElementAt(a).name + " " + data.ElementAt(b).name);
								addTeamToLineup(i, j, c, a, b, points, risk, floor);
							}
						}
					}
				}
			}
			removeLineups();
		}

		public static void checkRoster(List<List<playerSingleGame>> roster, List<playerSingleGame> newRoster)
		{
			int pos = 0;
			for (int t = 1; t < roster.Count; t++)
			{
				if (sumPoints(roster.ElementAt(t)) < sumPoints(roster.ElementAt(pos)))
				{
					pos = t;
				}
			}
			if (sumPoints(roster.ElementAt(pos)) < sumPoints(newRoster))
			{
				//Console.WriteLine("remove and add a roster");
				roster.RemoveAt(pos);
				roster.Add(newRoster);
			}
		}

		public static void WaitAll(IEnumerable<Thread> threads)
		{
			if (threads != null)
			{
				foreach (Thread thread in threads)
				{ thread.Join(); }
			}
		}

		public static void bubbleSortMvpTeams()
		{
			for (int i = 0; i < mvpTeams.Count; i++)
			{
				bubbleSort(mvpTeams.ElementAt(i));
			}
		}

		static void bubbleSort(List<List<playerSingleGame>> arr)
		{
			int n = arr.Count;
			for (int i = 0; i < n - 1; i++)
				for (int j = 0; j < n - i - 1; j++)
					if (sumPoints(arr.ElementAt(j)) > sumPoints(arr.ElementAt(j+1)))
					{
						// swap temp and arr[i]
						List<playerSingleGame> temp = arr.ElementAt(j);
						arr[j] = arr.ElementAt(j + 1);
						arr[j + 1] = temp;
					}
		}

		public static void getMvpTeamsUsage()
		{
			int kickers = 0;
			int numLin = 0;
			for(int i = 0; i < mvpTeams.Count; i++)
			{
				kickers = 0;
				numLin = 0;
				float size = numTeams * mvpUsageRate;
				if (mvpTeams.ElementAt(i).Count < size) size = mvpTeams.ElementAt(i).Count;
				//Console.WriteLine(size);
				for (int j = mvpTeams.ElementAt(i).Count - 1; j > 0; j--)
				{
					//Console.WriteLine(i + " " + j);
					if (checkPosition("K", mvpTeams.ElementAt(i).ElementAt(j)) && numLin <= size)
					{
						//Console.WriteLine("i=" + i + " j=" + j + " kickers=" + kickers);
						if (kickers < size * kickerUsageRate)
						{
							//Console.WriteLine("i=" + i + " j=" + j + " added lineup");
							bestLineups.Add(mvpTeams.ElementAt(i).ElementAt(j));
							numLin++;
							kickers++;
						}
					}
					else if (numLin <= size)
					{
						bestLineups.Add(mvpTeams.ElementAt(i).ElementAt(j));
						numLin++;
					}
				}
			}
		}

		public static bool checkPosition(string pos, List<playerSingleGame> team)
		{
			foreach(playerSingleGame p in team) 
			{
				if (p.position.Equals(pos)) return true;
			}
			return false;
		}

		/* Prints the array */
		static void printArray(int[] arr)
		{
			int n = arr.Length;
			for (int i = 0; i < n; ++i)
				Console.Write(arr[i] + " ");
			Console.WriteLine();
		}

		private static void addTeamToLineup(int i, int j, int d, int a, int b, int points, int risk, int floor)
		{
			List<playerSingleGame> temp = new List<playerSingleGame>();

			temp.Add(mvp.ElementAt(i));
			temp.Add(data.ElementAt(j));
			temp.Add(data.ElementAt(d));
			temp.Add(data.ElementAt(a));
			temp.Add(data.ElementAt(b));
			if (sumSalary(temp) <= 60000 && sumSalary(temp) > 55000 && !checkMvpDup(i,j,d,a,b))
			{
				//Console.WriteLine(sumPoints(temp) + "  " + sumFloor(temp) + "  " + sumRisk(temp) + "  " + sumSalary(temp));
				if (sumPoints(temp) > points && sumSdPts(temp) < risk && sumFloor(temp) > floor && temp.Count == 5 && dupPlayer(temp) && !checkSameTeam(temp))
				{
					//Console.WriteLine(sumPoints(temp));
					mvpUsage[i] = mvpUsage.ElementAt(i) + 1;
					//Console.WriteLine(mvp.ElementAt(i).name + " " + mvpUsage.ElementAt(i) + " " + (mvpUsageRate * numTeams));
					mvpTeams.ElementAt(i).Add(temp);
				}
			}
		}
		private static bool dupPlayer(List<playerSingleGame> team)
		{
			for (int i = 0; i < team.Count; i++)
			{
				for (int j = i+1; j < team.Count; j++)
				{
					if (team.ElementAt(i).name.Equals(team.ElementAt(j).name)) return false;
				}
			}
			return true;
		}
		private static bool checkSameTeam(List<playerSingleGame> team)
		{
			bool sameTeam = true;
			String str = team.ElementAt(0).team;
			for (int i = 0; i < team.Count; i++)
			{
				if (!str.Equals(team.ElementAt(i).team)) return false;
			}
			return sameTeam;
		}

		private static bool checkMvpDup(int m, int a, int b, int c, int d)
		{
			if (mvp.ElementAt(m).name.Equals(data.ElementAt(a).name))
				return true;
			if (mvp.ElementAt(m).name.Equals(data.ElementAt(b).name))
				return true;
			if (mvp.ElementAt(m).name.Equals(data.ElementAt(c).name))
				return true;
			if (mvp.ElementAt(m).name.Equals(data.ElementAt(d).name))
				return true;
			//Console.WriteLine("checkMvpDup false");
			return false;
		}

		private static void removeLineups()
		{
			//Console.WriteLine(bestLineups.Count);
			foreach (List<playerSingleGame> list in bestLineups)
			{
				//bool temp = false;
				if (Lineup.Count > numTeams)
				{
					//Console.WriteLine("remove team");
					List<playerSingleGame> lowestList = null;
					foreach (List<playerSingleGame> p in Lineup)
					{
						if (sumPoints(p) < sumPoints(list) && compareLists(list, p))
						{

							if (sumPoints(p) < sumPoints(lowestList) || sumPoints(lowestList) == 0)
							{
								lowestList = p;
							}
							//temp = true;
							break;
						}
					}
					if (lowestList != null)
					{
						//Console.WriteLine("add");
						//Console.WriteLine(sumPoints(list));
						//Console.WriteLine(sumPoints(lowestList));
						Lineup.Remove(lowestList);
						Lineup.Add(list);
					}
				}
				else
				{
					Lineup.Add(list);
				}
			}
		}
		public static void printLineups()
		{
			List<playerSingleGame> best = getBestLineups();
			foreach (List<playerSingleGame> list in bestLineups)
			{
				Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
				Console.WriteLine("points= " + sumPoints(list));
				Console.WriteLine("salary= " + sumSalary(list));
				Console.WriteLine("floor= " + sumFloor(list));
				Console.WriteLine("Bell= " + sumBell(list));
				Console.WriteLine("Same= " + findDifPlayers(best, list));
				foreach (playerSingleGame p in list)
				{
					Console.WriteLine(p.name + ":" + p.salary + ":" + p.points);
				}
			}
		}
		public static List<playerSingleGame> getBestLineups()
		{
			List<playerSingleGame> best = Lineup[0];
			foreach (List<playerSingleGame> list in Lineup)
			{
				if (sumPoints(list) > sumPoints(best))
				{
					best = list;
				}
			}
			return best;
		}

		public static void printLineupsThreads()
		{
			List<playerSingleGame> best = getBestLineupsThreads();
			string csv = "MVP - 1.5X Points,AnyFLEX,AnyFLEX,AnyFLEX,AnyFLEX" + System.Environment.NewLine;
			if (bestLineups.Count < numTeams) numTeams = bestLineups.Count;
			foreach (List<playerSingleGame> list in bestLineups.GetRange(bestLineups.Count-numTeams, numTeams))
			{
				Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
				Console.WriteLine("points= " + sumPoints(list));
				Console.WriteLine("salary= " + sumSalary(list));
				Console.WriteLine("floor= " + sumFloor(list));
				Console.WriteLine("Same= " + findDifPlayers(best, list));
				string csvLineup = "";
				foreach (playerSingleGame p in list)
				{
					csvLineup += p.playerID + ":" + p.name + ",";
					Console.WriteLine(p.name + ":" + p.salary + ":" + p.points);
				}
				csv += csvLineup + System.Environment.NewLine;
			}
			Console.WriteLine(csv);
			using (StreamWriter writer = new StreamWriter(outputcsv))
			{
				writer.WriteLine(csv);
			}
		}
		public static List<playerSingleGame> getBestLineupsThreads()
		{
			List<playerSingleGame> best = bestLineups[0];
			foreach (List<playerSingleGame> list in bestLineups)
			{
				if (sumPoints(list) > sumPoints(best))
				{
					best = list;
				}
			}
			return best;
		}

		public static int findDifPlayers(List<playerSingleGame> best, List<playerSingleGame> list)
		{
			int same = 0;
			foreach (playerSingleGame pb in best)
			{
				foreach (playerSingleGame pl in list)
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
		public static bool compareLists(List<playerSingleGame> list1, List<playerSingleGame> list2)
		{
			int count = 0;
			foreach (playerSingleGame p in list1)
			{
				foreach (playerSingleGame p2 in list2)
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

		public static float sumPoints(List<playerSingleGame> list)
		{
			if (list != null)
			{
				float sum = 0;
				foreach (playerSingleGame p in list)
				{
					sum += p.points;
				}
				return sum;
			}
			return 0;

		}
		public static int sumSalary(List<playerSingleGame> list)
		{
			int sum = 0;
			foreach (playerSingleGame p in list)
			{
				sum += p.salary;
			}
			return sum;
		}
		public static float sumSdPts(List<playerSingleGame> list)
		{
			float sum = 0;
			foreach (playerSingleGame p in list)
			{
				sum += p.sdPts;
			}
			return sum;
		}
		public static float sumBell(List<playerSingleGame> list)
		{
			float sum = 0;
			foreach (playerSingleGame p in list)
			{
				sum += (p.upper - p.points) / (p.points - p.lower);
			}
			return sum;
		}

		public static float sumRisk(List<playerSingleGame> list)
		{
			float sum = 0;
			foreach (playerSingleGame p in list)
			{
				sum += p.risk;
			}
			return sum;
		}
		public static float sumFloor(List<playerSingleGame> list)
		{
			float sum = 0;
			foreach (playerSingleGame p in list)
			{
				sum += p.lower;
			}
			return sum;
		}
		public static void outputCSV()
		{
			string header = "position" + ", " + "name" + ", " + "points" + ", " + "lower" + ", " + "salary" + ", " + "risk\n";
			File.WriteAllText(outputcsv, header);
			foreach (List<playerSingleGame> list in Lineup)
			{
				foreach (playerSingleGame p in list)
				{
					File.WriteAllText(outputcsv, p.toString());
				}
				string sums = "" + ", " + "" + ", " + sumPoints(list) + ", " + sumFloor(list) + ", " + sumSalary(list) + ", " + sumRisk(list) + "\n";
				File.WriteAllText(outputcsv, sums + "\n");
				File.WriteAllText(outputcsv, "\n");
			}
		}
	}

}

