using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using T03_Group2_PRG2Assignment;
//==========================================================
// Student Number : S10258457
// Student Name : Yeo Jin Rong
// Partner Name : Jason Ng
//=========================================================
// Done By: Ng Kai Huat Jason

namespace T03_Group02_PRG2Assignment
{
    class Cone : IceCream
    {
        public bool Dipped { get; set; }

        public Cone() : base() { }

        public Cone(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, bool dipped) : base(option, scoops, flavours, toppings)
        {
            Dipped = dipped;
        }

        public override double CalculatePrice()
        {
            double basePrice;

            // Switch to check number of scoops
            switch (Scoops)
            {
                case 1:
                    basePrice = 4.00;
                    break;
                case 2:
                    basePrice = 5.50;
                    break;
                case 3:
                    basePrice = 6.50;
                    break;
                default:
                    throw new InvalidOperationException("Invalid number of scoops");
            }

            // Add $1 for each topping
            double toppingPrice = Toppings.Count * 1.00;

            double dippingPrice = Dipped ? 2.00 : 0.00;

            // Calculate the total price
            double totalPrice = basePrice + toppingPrice + dippingPrice;

            return totalPrice;
        }
        public override string ToString()
        {
            return $"{base.ToString()}\tPrice: ${CalculatePrice():F2}";
        }
    }
}
