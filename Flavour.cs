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
// Done By: Ng Kai Huat Jason
namespace T03_Group02_PRG2Assignment
{
    class Flavour
    {
        public string Type { get; set; }
        public bool Premium { get; set; }
        //Set constraint for quantity, because cannot be 0
        private int quantity;
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative.");
                }
                quantity = value;
            }
        }

        public Flavour() { }

        public Flavour(string type, bool premium, int quantity)
        {
            Type = type;
            Premium = premium;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Type,-15}\t{Premium,-15}\t{Quantity,-15}";
        }
    }
}
