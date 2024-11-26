using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffaCalcualtor
{
	class SumHelper
	{
		public static double sumPoints(List<player> list)
		{
			if (list != null)
			{
				double sum = 0;
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
				sum += (p.upper - p.points) / (p.points - p.lower);
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
	}
}
