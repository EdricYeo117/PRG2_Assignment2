using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==========================================================
// Student Number : S10258457
// Student Name : Yeo Jin Rong
// Partner Name : Jason Ng
//=========================================================
// Done By: Yeo Jin Rong
namespace T03_Group02_PRG2Assignment
{
    class PointCard
    {
        public int Points { get; set; }

        public int PunchCard { get; set; }

        public string Tier { get; set; }

        public PointCard() { }

        public PointCard(int points, int punchCard)
        {
            Points = points;
            PunchCard = punchCard;
            Tier = "Ordinary";

        }

        public void AddPoints(int addpoints)
        {
            Points += addpoints;
        }

        public void RedeemPoints(int redeempoints)
        {
            Points -= redeempoints;
        }

        public void Punch()
        {
            PunchCard++;
            // Validates so that punchcard never exceeds 10
            if (PunchCard >= 10)
            {
                PunchCard = 10;
            }
        }

        public override string ToString()
        {
            return $"{Points,-15}\t{PunchCard,-15}\t{Tier,-15}";
        }
    }
}
