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
        //private static String path = "C:/Users/micha/Downloads/ffa_customrankings2020-10.csv";
		private static String path = "C:/Users/micha/Downloads/projections (1).csv";
		private static String path2 = "C:/Users/micha/Downloads/FanDuel-NFL-2020-11-15-51632-players-list.csv";
        private static string outputcsv = "C:/Users/micha/Documents/Code/FanduelLineups.csv";

        private static List<Thread> threads = new List<Thread>();
        private static List<List<List<player>>> threadLineups = new List<List<List<player>>>();

        static void Main(string[] args)
		{ 
            //var scraper = new WebScraper.

            readCSV(path, "\"IND\"", "\"TEN\"");
            //QB  RB  WR TE DST 
            setAllPositions(18, 12,11, 8, 6);
            //setAllPositionsQb("Eli Manning", 12, 10, 10, 10);
            //setAllPositionsDst(18, 12, 10, 10, "Patriots");
            findBest(rb, bestRB, 40);
            findBest(wr, bestWR, 40);
            //findBest2(rb, bestRB, 30);
            //findBest4(wr, bestWR, 56);
            Console.WriteLine("RB " +bestRB.Count);
            Console.WriteLine("WR " + bestWR.Count);
            Console.WriteLine("QB " + qb.Count);
            Console.WriteLine("TE " + te.Count);
            Console.WriteLine("DST " + dst.Count);
            findLineups(130, 42, 100);
            //findBestLineup(130, 42, 100);
            consolidateThreadRosters(threadLineups);
            Console.WriteLine(bestLineups.Count);
            printLineupsThreads();
            //printLineups();
            //outputCSV();
            Console.WriteLine("done");
            Console.ReadKey();
        }
        public static void readCSV(String path, string team1, string team2)
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
                        if (!values[2].Equals(team1) && !values[2].Equals(team2))
                        {
                            float points = 0;
                            float lower = 0;
                            float upper = 0;
                            if (!values[7].Equals("NA")) points = float.Parse(values[7]);
                            if (!values[8].Equals("NA")) lower = float.Parse(values[8]);
                            if (!values[9].Equals("NA")) upper = float.Parse(values[9]);
                            float left = (points - lower) / (upper - points);
                            //Console.WriteLine(points + "  " + lower + "  " + upper + "  " + left);
                            if ((lower < points && upper > points) || values[3].Equals("\"DST\""))
                            {
                                //Console.WriteLine(left);
                                values[1] = values[1].Replace(".", "");
                                values[1] = values[1].Replace("'", "");

								//"playerId","player","team","position","age","exp","bye","points","lower","upper","sdPts","positionRank","dropoff","tier","ptSpread","overallECR","positionECR","sdRank","risk","sleeper","salary"
								//0          1        2       3         4      5     6    7        8       9       10      11				12		13		14			15			16				17		18		19		20
								//data.Add(new player(values[1], values[2], values[3], values[7], values[8], values[9], values[10],
								//    values[11], values[12], values[13], values[14], values[16], values[17], values[18]));
								data.Add(new player(values[1], values[2], values[3], values[7], values[8], values[9], values[10],
									values[11], values[12], "0", "0", "0", "0", "0"));
							}

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
                        foreach (player p in data)
                        {
                            string str = p.name.Replace("\"", "");
                            string pos = p.position.Replace("\"", "");
                            string str2 = values[1].Replace("\"", "");
                            string str3 = values[3].Replace("\"", "");
                            string str4 = values[4].Replace("\"", "");
                            //Console.WriteLine(pos + "   " + str2);
                            if ((str3.Contains(str) && str2.Contains(pos)) || str4.Contains(str))
                            {
                                int salary = int.Parse(values[7].Replace("\"", ""));
                                //Console.WriteLine(str + "   " + str3 + "   " + str4 + "    " + salary);
                                p.setSalary(salary);
                            }
                        }
                    }
                    else first = true;
                }
            }
        }
        public static void readCSV(String path, string team1, string team2, string team3, string team4)
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
                        if (!values[2].Equals(team1) && !values[2].Equals(team2) && !values[2].Equals(team3) && !values[2].Equals(team4))
                        {
                            float points = 0;
                            float lower = 0;
                            float upper = 0;
                            if (!values[7].Equals("NA")) points = float.Parse(values[7]);
                            if (!values[8].Equals("NA")) lower = float.Parse(values[8]);
                            if (!values[9].Equals("NA")) upper = float.Parse(values[9]);
                            float left = (points - lower) / (upper - points);
                            //Console.WriteLine(points + "  " + lower + "  " + upper + "  " + left);
                            if ((left < 2 && lower < points && upper > points) || values[3].Equals("\"DST\""))
                            {
                                //Console.WriteLine(left);
                                values[1] = values[1].Replace(".", "");
                                values[1] = values[1].Replace("'", "");
                                data.Add(new player(values[1], values[2], values[3], values[7], values[8], values[9], values[10],
                                    values[11], values[12], values[13], values[14], values[16], values[17], values[18]));
                            }
                            
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
                    values[3] = values[3].Replace("'","");
                    values[3] = values[3].Replace(".", "");
                    //values[3] = values[3].Replace(" Jr", "");
                    //values[3] = values[3].Replace(" IV", "");
                    //values[3] = values[3].Replace(" III", "");
                    //values[3] = values[3].Replace(" II", "");
                    if (first)
                    {
                        foreach(player p in data)
                        {
                            string str = p.name.Replace("\"", "");
                            string pos = p.position.Replace("\"", "");
                            string str2 = values[1].Replace("\"", "");
                            string str3 = values[3].Replace("\"", "");
                            string str4 = values[4].Replace("\"", "");
                            //Console.WriteLine(pos + "   " + str2);
                            if ((str3.Contains(str) && str2.Contains(pos)) || str4.Contains(str))
                            {
                                int salary = int.Parse(values[7].Replace("\"", ""));
                                //Console.WriteLine(str + "   " + str3 + "   " + str4 + "    " + salary);
                                p.setSalary(salary);
                            }
                        }
                    }
                    else first = true;
                }
            }
        }
        public static void readCSV(String path, string team1, string team2, string team3, string team4, string team5, string team6)
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
                        if (!values[2].Equals(team1) && !values[2].Equals(team2) && !values[2].Equals(team3) && !values[2].Equals(team4) && !values[2].Equals(team5) && !values[2].Equals(team6))
                        {
                            float points = 0;
                            float lower = 0;
                            float upper = 0;
                            //Console.WriteLine(values[1]);
                            if (!values[7].Equals("NA")) points = float.Parse(values[7]);
                            if (!values[8].Equals("NA")) lower = float.Parse(values[8]);
                            if (!values[9].Equals("NA")) upper = float.Parse(values[9]);
                            float left = (points - lower) / (upper - points);
                            //Console.WriteLine(points + "  " + lower + "  " + upper + "  " + left);
                            if ((lower < points && upper > points) || values[3].Equals("\"DST\""))
                            {
                                //Console.WriteLine(left);
                                //Console.WriteLine(values[1]);
                                values[1] = values[1].Replace(".", "");
                                values[1] = values[1].Replace("'", "");
                                data.Add(new player(values[1], values[2], values[3], values[7], values[8], values[9], values[10],
                                    values[11], values[12], values[13], values[14], values[16], values[17], values[18]));
                            }

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
                        foreach (player p in data)
                        {
                            string str = p.name.Replace("\"", "");
                            string pos = p.position.Replace("\"", "");
                            string str2 = values[1].Replace("\"", "");
                            string str3 = values[3].Replace("\"", "");
                            string str4 = values[4].Replace("\"", "");
                            //Console.WriteLine(pos + "   " + str2);
                            if ((str3.Contains(str) && str2.Contains(pos)) || str4.Contains(str))
                            {
                                int salary = int.Parse(values[7].Replace("\"", ""));
                                //Console.WriteLine(str + "   " + str3 + "   " + str4 + "    " + salary);
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
            setPosition("\"QB\"", qb, qbmin);
            setPosition("\"RB\"", rb, rbmin);
            setPosition("\"WR\"", wr, wrmin);
            setPosition("\"TE\"", te, temin);
            setPosition("\"DST\"", dst, dstmin);
        }

        public static void setPosition(string str, List<player> l, double min)
        {
            foreach (player p in data)
            {
                //float value = (p.points / p.salary) * 1000;
                //Console.WriteLine(value);
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
                        if(sumPoints(best) > min)
                        {
                            //Console.WriteLine(list.ElementAt(i).name + "  " + list.ElementAt(j).name + "  " + list.ElementAt(k).name + "  " + sumPoints(best) + "   " + sumSalary(best));
                            temp.Add(best);
                        }
                    }
                }
            }
        }
        public static void findBest2(List<player> list, List<List<player>> temp, float min)
        {

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    List<player> best = new List<player>();
                    best.Add(list.ElementAt(i));
                    best.Add(list.ElementAt(j));
                    if (sumPoints(best) > min)
                    {
                        //Console.WriteLine(list.ElementAt(i).name + "  " + list.ElementAt(j).name + "  " + list.ElementAt(k).name + "  " + sumPoints(best) + "   " + sumSalary(best));
                        temp.Add(best);
                    }
                }
            }
        }
        public static void findBest4(List<player> list, List<List<player>> temp, float min)
        {

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    for (int k = j + 1; k < list.Count; k++)
                    {
                        for (int a = k + 1; a < list.Count; a++)
                        {
                            List<player> best = new List<player>();
                            best.Add(list.ElementAt(i));
                            best.Add(list.ElementAt(j));
                            best.Add(list.ElementAt(k));
                            best.Add(list.ElementAt(a)) ;
                            if (sumPoints(best) > min)
                            {
                                //Console.WriteLine(list.ElementAt(i).name + "  " + list.ElementAt(j).name + "  " + list.ElementAt(k).name + "  " + sumPoints(best) + "   " + sumSalary(best));
                                temp.Add(best);
                            }
                        }
                    }
                }
            }
        }
        public static void findBestLineup(int points, int risk, int floor)
        {

            for (int i = 0; i < qb.Count; i++)
            {
                Console.WriteLine(i);
                for (int j = 0; j < bestRB.Count; j++)
                {
                    for (int k = 0; k < bestWR.Count; k++)
                    {
                        for(int a = 0; a < te.Count; a++)
                        {
                            for(int b = 0; b < dst.Count; b++)
                            {
                                addTeamToLineup(i, j , k, a, b, points, risk, floor);
                            }
                        }
                    }
                }
            }
            removeLineups();
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
                                if (roster.Count < 10)
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
                    checkRoster(bestLineups, tRoster.ElementAt(i).ElementAt(j));
                }
            }
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
            //bool twoandfour = (bestRB.ElementAt(j).Count != 3 && bestWR.ElementAt(k).Count != 4) || (bestRB.ElementAt(j).Count != 2 && bestWR.ElementAt(k).Count != 3);
            if (sumSalary(temp) <= 60000 && sumSalary(temp) > 59000)
            {
                //Console.WriteLine(sumPoints(temp) + "  " + sumFloor(temp) + "  " + sumRisk(temp) + "  " + sumSalary(temp));
                if (sumPoints(temp) > points && sumSdPts(temp) < risk && sumFloor(temp) > floor && temp.Count == 9)
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
                    Console.WriteLine("name= " + p.name);
                    Console.WriteLine("salary= " + p.salary);
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
            foreach (List<player> list in bestLineups)
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("points= " + sumPoints(list));
                Console.WriteLine("salary= " + sumSalary(list));
                Console.WriteLine("floor= " + sumFloor(list));
                Console.WriteLine("Bell= " + sumBell(list));
                Console.WriteLine("Same= " + findDifPlayers(best, list));
                foreach (player p in list)
                {
                    Console.WriteLine("name= " + p.name);
                    Console.WriteLine("salary= " + p.salary);
                }
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

