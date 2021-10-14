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
        public static List<player> data = new List<player>();
        public static List<player> qb = new List<player>();
        public static List<player> rb = new List<player>();
        public static List<player> wr = new List<player>();
        public static List<player> te = new List<player>();
        public static List<player> dst = new List<player>();
        public static List<List<player>> bestRB = new List<List<player>>();
        public static List<List<player>> bestWR = new List<List<player>>();
        public static List<List<player>> bestLineups = new List<List<player>>();
        public static IEnumerable<List<player>> bestLineups2 = new List<List<player>>();
        public static List<List<player>> Lineup = new List<List<player>>();
        public static float points = 0;
		public static int numTeams = 150;
		public static float usageRate = .33f;
		//private static String path = "C:/Users/micha/Downloads/ffa_customrankings2020-10.csv";
		private static String fdName = "Week5 - ffa";
		private static String path = "C:/Users/micha/Downloads/" + fdName + ".csv";
		private static String path2 = "C:/Users/micha/Downloads/FanDuel-NFL-2021 ET-10 ET-10 ET-64897-players-list.csv";
		private static string outputcsv = "C://Users/Micha/OneDrive/Documents/FantasyFootball/" + fdName + ".csv";

		private static List<Thread> threads = new List<Thread>();
        private static List<List<List<player>>> threadLineups = new List<List<List<player>>>();

        static void Main(string[] args)
		{ 
            //var scraper = new WebScraper.

            readCSV(path, "SEA", "LAR", "NYJ", "ATL", "BUF", "KC", "IND", "BAL");
			//readCSV(path);
			//				QB  RB WR TE  DST 
			setAllPositions(18.5, 11,10, 9, 6.5);
			//setAllPositionsQb("Eli Manning", 12, 10, 10, 10);
			//setAllPositionsDst(18, 12, 10, 10, "Patriots");
			//Console.WriteLine(rb.Count);
            findBest(rb, bestRB, 40);
            findBest(wr, bestWR, 40);
            //findBest2(rb, bestRB, 30);
            //findBest4(wr, bestWR, 56);
            Console.WriteLine("RB " + bestRB.Count);
            Console.WriteLine("WR " + bestWR.Count);
            Console.WriteLine("QB " + qb.Count);
            Console.WriteLine("TE " + te.Count);
            Console.WriteLine("DST " + dst.Count);
            findLineups(120, 50, 90);
            consolidateThreadRosters(threadLineups);
            Console.WriteLine(bestLineups.Count);
            printLineupsThreads();
            //printLineups();
            //outputCSV();
            Console.WriteLine("done");
            Console.ReadKey();
        }

		public static void readCSV(String path)
		{
			readCSV(path, null, null, null, null, null, null, null, null);
		}
		public static void readCSV(String path, string team1, string team2)
        {
			readCSV(path, team1, team2, null, null, null, null, null, null);
		}
        public static void readCSV(String path, string team1, string team2, string team3, string team4)
        {
			readCSV(path, team1, team2, team3, team4, null, null, null, null);
		}
        public static void readCSV(String path, string team1, string team2, string team3, string team4, string team5, string team6)
        {
			readCSV(path, team1, team2, team3, team4, team5, team6, null, null);
		}

		public static void readCSV(String path, string team1, string team2, string team3, string team4, string team5, string team6, string team7, string team8)
		{
			readFFA(path, team1, team2, team3, team4, team5, team6, team7, team8);
			readFD();
		}

		private static void readFFA(String path, string team1, string team2, string team3, string team4, string team5, string team6, string team7, string team8)
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
						if (!values[2].Equals(team1) && !values[2].Equals(team2) && !values[2].Equals(team3) && !values[2].Equals(team4) && !values[2].Equals(team5) && !values[2].Equals(team6) && !values[2].Equals(team7) && !values[2].Equals(team8))
						{
							float points = 0;
							float lower = 0;
							float upper = 0;
							//Console.WriteLine(values[1]);
							if (!values[5].Equals("NA")) points = float.Parse(values[5]);//7
							if (!values[4].Equals("NA") || values[4] != null) lower = float.Parse(values[4]);//8
							if (!values[6].Equals("NA") || !values[6].Equals("")) upper = float.Parse(values[6]);//9
							float left = (points - lower) / (upper - points);
							//Console.WriteLine(values[1] + " " + points + "  " + lower + "  " + upper + "  " + left);
							if ((lower < points && upper > points) || values[3].Equals("\"DST\""))
							{
								//Console.WriteLine(left);
								//Console.WriteLine(values[1]);
								values[1] = values[1].Replace(".", "");
								values[1] = values[1].Replace("'", "");
								data.Add(new player(values[1], values[2], values[3], values[5], values[4], values[6], values[7],
								values[10], "0", "0", "0", "0", "0", "0"));
							}

						}
					}
					else first = true;
				}
			}
		}

		private static void readFD()
		{
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
						foreach (player p in data)
						{
							string str = p.name.Replace("\"", "");
							string pos = p.position.Replace("\"", "");
							string str1 = values[0].Replace("\"", "");
							string str2 = values[1].Replace("\"", "");
							if (str2.Equals("D")) str2 = "DST";
							string str3 = values[3].Replace("\"", "");
							string str4 = values[4].Replace("\"", "");
							// Console.WriteLine(pos + "   " + str2);
							if ((str3.Contains(str) && str2.Contains(pos)) || str4.Contains(str))
							{
								int salary = int.Parse(values[7].Replace("\"", ""));
								//Console.WriteLine(str3 + "    " + salary);
								p.playerID = str1;
								p.setSalary(salary);
							}
						}
					}
					else first = true;
				}
			}
		}

		public static void setAllPositions(double qbmin, double rbmin, double wrmin, double temin, double dstmin)
        {
            setPosition("QB", qb, qbmin);
            setPosition("RB", rb, rbmin);//"\"RB\""
			setPosition("WR", wr, wrmin);
            setPosition("TE", te, temin);
            setPosition("DST", dst, dstmin);
        }

        public static void setPosition(string str, List<player> l, double min)
        {
            foreach (player p in data)
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

        public static void findBest(List<player> list, List<List<player>> temp, float min)
        {
            
            for(int i = 0; i < list.Count; i++)
            {
                for(int j = i + 1; j < list.Count; j++)
                {
                    for(int k = j + 1; k < list.Count; k++)
                    {
                        List<player> best = new List<player>();
                        best.Add(list.ElementAt(i));
                        best.Add(list.ElementAt(j));
                        best.Add(list.ElementAt(k));
						//Console.WriteLine(list.ElementAt(i).name + "  " + list.ElementAt(j).name + "  " + list.ElementAt(k).name + "  " + sumPoints(best) + "   " + sumSalary(best));
						if (sumPoints(best) > min)
                        {
                            //Console.WriteLine(list.ElementAt(i).name + "  " + list.ElementAt(j).name + "  " + list.ElementAt(k).name + "  " + sumPoints(best) + "   " + sumSalary(best));
                            temp.Add(best);
                        }
                    }
                }
            }
        }

        public static void findLineups(int points, int risk, int floor)
        {
            for (int i = 0; i < qb.Count; i++)
            {
                Console.WriteLine("thread = " + i);
                int[] temp = new int[] { points, risk, floor, i };
                lock (threads)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(findLineupsWithQb));
                    threads.Add(t);
                    t.Start(temp);
                }
            }
            Console.WriteLine("qbs = " + qb.Count);
            Console.WriteLine("threads = " + threads.Count);
            for ( int i = 0; i < threads.Count; i++)
            {
                threads.ElementAt(i);
            }
        }

        public static void findLineupsWithQb(object args)
        {
            Array nums = new object[3];
            nums = (Array)args;
            int i = (int)nums.GetValue(3);
            List<List<player>> roster = new List<List<player>>();
            //rb
            for (int j = 0; j < bestRB.Count; j++)
            {
                //Console.WriteLine("thread = " + i + " rb = " + j);
                //wr
                for (int k = 0; k < bestWR.Count; k++)
                {
                    //te
                    for (int a = 0; a < te.Count; a++)
                    {
                        //dst
                        for (int b = 0; b < dst.Count; b++)
                        {
                            List<player> temp = createLineup(i, j, k, a, b, (int)nums.GetValue(0), (int)nums.GetValue(1), (int)nums.GetValue(2));
                            //Console.WriteLine(sumPoints(temp));
                            //Console.WriteLine(" j = " + j + " k = " + k + " a = " + a + " b = " + b);
                            if(temp != null)
                            {
								//Console.WriteLine("temp not null" + sumPoints(temp));
								if (roster.Count < numTeams * usageRate)
                                {
                                    //Console.WriteLine("add roster");
                                    roster.Add(temp);
                                }
                                else
                                {
                                    checkRoster(roster, temp);
                                }
                            }
                        }
                    }
                }
            }
            lock (threadLineups)
            {
                Console.WriteLine("add roster to thread pool");
                threadLineups.Add(roster);
                Console.WriteLine("i = " + i);
                Thread t = threads.ElementAt(i);
                //threads.RemoveAt(i);
                //t.Join();
            }
        }
	//	private static bool checkOtherUsage(List<player> team, List<List<player>> roster)
	//	{
	//		if (roster.Count < numTeams * usageRate * .5) return true;
	//		int num = 0;
	//		for (int i = 0; i < roster.Count; i++)
	//		{
	//			for (int j = 0; j < )
	//		}
	//	}
	//
	//	private static bool checkPlayers(List<player> a, List<player> b)
	//	{
	//		string[] pName = new string[] { };
	//		for (int i = 0; i < a.Count; i++)
	//		{
	//			for (int j = 0; j < b.Count; j++)
	//			{
	//				if
	//			}
	//		}
	//	}

        public static void checkRoster(List<List<player>> roster, List<player> newRoster)
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

        public static void consolidateThreadRosters(List<List<List<player>>> tRoster)
        {
            //list of 10 best rosters
            //Thread.Sleep(100000);
            /*bool waiting = true;
            while (waiting )
            {
                if (threads.Count == 0) waiting = false;
                else Thread.Sleep(1000);
            }*/
            WaitAll(threads);
            bestLineups = tRoster.ElementAt(0);
            for (int i = 1; i < tRoster.Count; i++)
            {
                for(int j = 0; j < tRoster.ElementAt(i).Count; j++)
                {
					//Console.WriteLine("consolidate");
					if (bestLineups.Count < numTeams) bestLineups.Add(tRoster.ElementAt(i).ElementAt(j));
                    else checkRoster(bestLineups, tRoster.ElementAt(i).ElementAt(j));
                }
            }
			bubbleSort(bestLineups);
        }

		static void bubbleSort(List<List<player>> arr)
		{
			int n = arr.Count;
			for (int i = 0; i < n - 1; i++)
				for (int j = 0; j < n - i - 1; j++)
					if (sumPoints(arr.ElementAt(j)) > sumPoints(arr.ElementAt(j + 1)))
					{
						// swap temp and arr[i]
						List<player> temp = arr.ElementAt(j);
						arr[j] = arr.ElementAt(j+1);
						arr[j + 1] = temp;
					}
		}

		/* Prints the array */
		static void printArray(int[] arr)
		{
			int n = arr.Length;
			for (int i = 0; i < n; ++i)
				Console.Write(arr[i] + " ");
			Console.WriteLine();
		}

		private static List<player> createLineup(int i, int j, int k, int a, int b, int points, int risk, int floor)
        {
            List<player> temp = new List<player>();
            temp.Add(qb.ElementAt(i));
            //Console.WriteLine(qb.ElementAt(i).name);
            for (int c = 0; c < bestRB.ElementAt(j).Count; c++)
            {
                //Console.WriteLine(bestRB.ElementAt(j).ElementAt(c).name);
                temp.Add(bestRB.ElementAt(j).ElementAt(c));
            }
            for (int c = 0; c < bestWR.ElementAt(k).Count; c++)
            {
                temp.Add(bestWR.ElementAt(k).ElementAt(c));
            }
            temp.Add(te.ElementAt(a));
            temp.Add(dst.ElementAt(b));
			//Console.WriteLine(sumPoints(temp) + "  " + sumFloor(temp) + "  " + sumRisk(temp) + "  " + sumSalary(temp));
			//bool twoandfour = (bestRB.ElementAt(j).Count != 3 && bestWR.ElementAt(k).Count != 4) || (bestRB.ElementAt(j).Count != 2 && bestWR.ElementAt(k).Count != 3);
			if (sumSalary(temp) <= 60000 && sumSalary(temp) > 59000)
            {
                //Console.WriteLine(sumPoints(temp) + "  " + sumFloor(temp) + "  " + sumRisk(temp) + "  " + sumSalary(temp));
                if (sumPoints(temp) > points && sumFloor(temp) > floor && temp.Count == 9)
                {
                    return (temp);
                }
            }
            return null;
        }

        private static void addTeamToLineup(int i, int j, int k, int a, int b, int points, int risk, int floor)
        {
            List<player> temp = new List<player>();
            temp.Add(qb.ElementAt(i));
            for (int c = 0; c < bestRB.ElementAt(j).Count; c++)
            {
                temp.Add(bestRB.ElementAt(j).ElementAt(c));
            }
            for (int c = 0; c < bestWR.ElementAt(k).Count; c++)
            {
                temp.Add(bestWR.ElementAt(k).ElementAt(c));
            }
            temp.Add(te.ElementAt(a));
            temp.Add(dst.ElementAt(b));
            bool twoandfour = (bestRB.ElementAt(j).Count != 3 && bestWR.ElementAt(k).Count != 4) || (bestRB.ElementAt(j).Count != 2 && bestWR.ElementAt(k).Count != 3);
            if (sumSalary(temp) <= 60000 && sumSalary(temp) > 59000)
            {
                //Console.WriteLine(sumPoints(best) + "  " + sumFloor(best) + "  " + sumRisk(best) + "  " + sumSalary(best));
                if (sumPoints(temp) > points && sumSdPts(temp) < risk && sumFloor(temp) > floor && temp.Count == 9)
                {
                    bestLineups.Add(temp);
                }
            }
        }

        private static void removeLineups()
        {
            Console.WriteLine(bestLineups.Count);
            foreach (List<player> list in bestLineups)
            {
                //bool temp = false;
                if (Lineup.Count > 20)
                {
                    List<player> lowestList = null;
                    foreach (List<player> p in Lineup)
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
            List<player> best = getBestLineups();
            foreach(List<player> list in bestLineups)
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("points= " + sumPoints(list));
                Console.WriteLine("salary= " + sumSalary(list));
                Console.WriteLine("floor= " + sumFloor(list));
                Console.WriteLine("Bell= " + sumBell(list));
                Console.WriteLine("Same= " + findDifPlayers(best, list));
                foreach (player p in list)
                {
                    Console.WriteLine(p.name + ":" + p.salary + ":" +p.points);
                }
            }
        }
        public static List<player> getBestLineups()
        {
            List<player> best = Lineup[0];
            foreach(List<player> list in Lineup)
            {
                if(sumPoints(list) > sumPoints(best))
                {
                    best = list;
                }
            }
            return best;
        }

        public static void printLineupsThreads()
        {
            List<player> best = getBestLineupsThreads();
			string csv = "QB,RB,RB,WR,WR,WR,TE,FLEX,DEF" + System.Environment.NewLine;
			foreach (List<player> list in bestLineups)
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("points= " + sumPoints(list));
                Console.WriteLine("salary= " + sumSalary(list));
                Console.WriteLine("floor= " + sumFloor(list));
                Console.WriteLine("Bell= " + sumBell(list));
                Console.WriteLine("Same= " + findDifPlayers(best, list));
				string csvLineup = "";
				int c = 0;
				foreach (player p in list)
				{
					if (c != 3) 
					{
						csvLineup += p.playerID + ":" + p.name + ",";
						Console.WriteLine(p.name + ":" + p.salary + ":" + p.points);
					} 
					if (c == 7)
					{
						player pl1 = list.ElementAt(3);
						csvLineup += pl1.playerID + ":" + pl1.name + ",";
						Console.WriteLine(pl1.name + ":" + pl1.salary + ":" + pl1.points);
					}
					c++;
				}
				csv += csvLineup + System.Environment.NewLine;
			}
			Console.WriteLine(csv);
			using (StreamWriter writer = new StreamWriter(outputcsv))
			{
				writer.WriteLine(csv);
			}
		}
        public static List<player> getBestLineupsThreads()
        {
            List<player> best = bestLineups[0];
            foreach (List<player> list in bestLineups)
            {
                if (sumPoints(list) > sumPoints(best))
                {
                    best = list;
                }
            }
            return best;
        }

        public static int findDifPlayers(List<player> best, List<player> list)
        {
            int same = 0;
            foreach(player pb in best)
            {
                foreach(player pl in list)
                {
                    //Console.WriteLine(pb.name + " " + pl.name);
                    if(pb.name.Equals(pl.name))
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
            foreach(player p in list1)
            {
                foreach(player p2 in list2)
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

        public static float sumPoints(List<player> list)
        {
            if(list != null)
            {
                float sum = 0;
                foreach (player p in list)
                {
                    sum += p.points;
                }
                return sum;
            }
            return 0;
            
        }
        public static int sumSalary(List<player> list)
        {
            int sum = 0;
            foreach (player p in list)
            {
                sum += p.salary;
            }
            return sum;
        }
        public static float sumSdPts(List<player> list)
        {
            float sum = 0;
            foreach (player p in list)
            {
                sum += p.sdPts;
            }
            return sum;
        }
        public static float sumBell(List<player> list)
        {
            float sum = 0;
            foreach (player p in list)
            {
                sum += (p.upper - p.points)/(p.points - p.lower);
            }
            return sum;
        }

        public static float sumRisk(List<player> list)
        {
            float sum = 0;
            foreach (player p in list)
            {
                sum += p.risk;
            }
            return sum;
        }
        public static float sumFloor(List<player> list)
        {
            float sum = 0;
            foreach (player p in list)
            {
                sum += p.lower;
            }
            return sum;
        }
        public static void outputCSV()
        {
            string header = "position" + ", " + "name" + ", " + "points" + ", " + "lower" + ", " + "salary" + ", " + "risk\n";
            File.WriteAllText(outputcsv, header);
            foreach (List<player> list in Lineup)
            {
                foreach(player p in list)
                {
                    File.WriteAllText(outputcsv, p.toString());
                }
                string sums = "" + ", " + "" + ", " + sumPoints(list) + ", " + sumFloor(list) + ", " + sumSalary(list) + ", " + sumRisk(list)+"\n";
                File.WriteAllText(outputcsv, sums + "\n");
                File.WriteAllText(outputcsv, "\n");
            }
        }
    }

}

