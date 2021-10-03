namespace ffaCalcualtor
{
    public class playerSingleGame
    {
        public playerSingleGame(string name, string team, string position, string points, string lower, string upper, string sdPts, string positionRank, string dropoff, string tier, string ptSpread, string positionECR, string sdRank, string risk)
        {
            this.name = name;
            this.team = team;
            this.position = position;
            if (!points.Equals("NA")) this.points = float.Parse(points);
            if (!lower.Equals("NA")) this.lower = float.Parse(lower);
            if (!upper.Equals("NA")) this.upper = float.Parse(upper);
            if (!sdPts.Equals("NA") && !sdPts.Equals("Inf")) this.sdPts = float.Parse(sdPts);
            //if (!positionRank.Equals("NA")) this.positionRank = int.Parse(positionRank);
            if (!dropoff.Equals("NA")) this.dropoff = float.Parse(dropoff);
            if (!tier.Equals("NA")) this.tier = int.Parse(tier);
            if (!ptSpread.Equals("NA")) this.ptSpread = float.Parse(ptSpread);
            if (!positionECR.Equals("NA")) this.positionECR = float.Parse(positionECR);
            if (!sdRank.Equals("NA")) this.sdRank = float.Parse(sdRank);
            if (!risk.Equals("NA")) this.risk = float.Parse(risk);
            this.salary = 20000;
			this.risk = 10;
			this.playerID = "";
        }

		public playerSingleGame(playerSingleGame p)
		{
			this.name = p.name;
			this.team = p.team;
			this.position = p.position;
			this.points = p.points;
			this.lower = p.lower;
			this.upper = p.upper;
			this.sdPts = p.sdPts;
			//if (!positionRank.Equals("NA")) this.positionRank = int.Parse(positionRank);
			this.dropoff = p.dropoff;
			this.tier = p.tier;
			this.ptSpread = p.ptSpread;
			this.positionECR = p.positionECR;
			this.sdRank = p.sdRank;
			this.risk = p.risk;
			this.salary = p.salary;
			this.risk = p.risk;
			this.playerID = p.playerID;
		}

        public bool Equals(playerSingleGame p)
        {
            if (this.name.Equals(p.name)) return true;
            return false;
        }
        public string toString()
        {
            return (position + ", " + name + ", " + points + ", " + lower + ", " + salary + ", " + risk);
        }

        public string name { get; set; }
        public string team { get; set; }
        public string position { get; set; }
        public float points { get; set; }
        public float lower { get; set; }
        public float upper { get; set; }
        public float sdPts { get; set; }
        public int positionRank { get; set; }
        public float dropoff { get; set; }
        public int tier { get; set; }
        public float ptSpread { get; set; }
        public float positionECR { get; set; }
        public float sdRank { get; set; }
        public float risk { get; set; }
		public string playerID { get; set; }
        public int salary=20000;
        public void setSalary(int num) { this.salary = num; }
    }

}

