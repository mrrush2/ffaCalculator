using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        public static List<List<player>> bestLineup = new List<List<player>>();
        public static IEnumerable<List<player>> bestLineup2 = new List<List<player>>();
        public static List<List<player>> Lineup = new List<List<player>>();
        public static float points = 0;
        private static String path = "C:/Users/micha/Downloads/ffa_customrankings2019-12 (1).csv";
        private static String path2 = "C:/Users/micha/Downloads/FanDuel-NFL-2019-11-24-40674-players-list.csv";
        private static string outputcsv = "C:/Users/micha/Documents/Code/FanduelLineups.csv";
        static void Main(string[] args)
        {
            readCSV(path, "\"HOU\"", "\"HOU\"", "\"IND\"", "\"IND\"");
                          //QB  RB  WR TE DST 
            setAllPositions(20, 12, 10, 7, 8);
            findBest(rb, bestRB, 40);
            findBest(wr, bestWR, 38);
            Console.WriteLine("RB " +bestRB.Count);
            Console.WriteLine("WR " + bestWR.Count);
            Console.WriteLine("QB " + qb.Count);
            Console.WriteLine("TE " + te.Count);
            Console.WriteLine("DST " + dst.Count);
            findBestLineup(130, 42, 110);
            Console.WriteLine(bestLineup.Count);
            printLineups();
            //outputCSV();
            Console.WriteLine("done");
            Console.ReadKey();
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
                if (p.position.Equals(str) && p.points > min)
                {
                    //Console.WriteLine(p.points + "  " + p.name + "  " + p.salary);
                    l.Add(p);
                }
            }
        }
        public static void setQBPosition(string str, List<player> l, double min, String name)
        {
            foreach (player p in data)
            {
                //float value = (p.points / p.salary) * 1000;
                //Console.WriteLine(value);
                if (p.name.Contains(name))
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
                                List<player> best = new List<player>();
                                best.Add(qb.ElementAt(i));
                                for(int c = 0; c < bestRB.ElementAt(j).Count; c++)
                                {
                                    best.Add(bestRB.ElementAt(j).ElementAt(c));
                                }
                                for (int c = 0; c < bestWR.ElementAt(k).Count; c++)
                                {
                                    best.Add(bestWR.ElementAt(k).ElementAt(c));
                                }
                                best.Add(te.ElementAt(a));
                                best.Add(dst.ElementAt(b));
                                
                                if (sumSalary(best) <= 60000 && sumSalary(best) > 59000)
                                {
                                    //Console.WriteLine(sumPoints(best) + "  " + sumFloor(best) + "  " + sumRisk(best) + "  " + sumSalary(best));
                                    if (sumPoints(best) > points && sumSdPts(best) < risk && sumFloor(best) > floor)
                                    { 
                                        bestLineup.Add(best);
                                        //Console.WriteLine(i + " " + j + " " + k + " " + a + " " + b);
                                        //Console.WriteLine(sumPoints(best));
                                        //foreach (player p in best)
                                        //{
                                        //    Console.WriteLine(p.name + "  " + p.points);
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(bestLineup.Count);
            foreach(List<player> list in bestLineup)
            {
                //bool temp = false;
                if (Lineup.Count > 20)
                {
                    List<player> lowestList = null;
                    foreach (List<player> p in Lineup)
                    {
                        if (sumPoints(p) < sumPoints(list) && compareLists(list, p))
                        {
                            
                            if (sumPoints(p) < sumPoints(lowestList) || sumPoints(lowestList)== 0)
                            {
                                lowestList = p;
                            }
                            //temp = true;
                            break;
                        }
                    }
                    if(lowestList != null)
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
            foreach(List<player> list in Lineup)
            {
                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine("points= " + sumPoints(list));
                Console.WriteLine("salary= " + sumSalary(list));
                Console.WriteLine("floor= " + sumFloor(list));
                Console.WriteLine("Bell= " + sumBell(list));
                foreach (player p in list)
                {
                    Console.WriteLine("name= " + p.name);
                    Console.WriteLine("salary= " + p.salary);
                }
            }
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

