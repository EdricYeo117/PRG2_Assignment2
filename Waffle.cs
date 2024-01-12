using Microsoft.VisualBasic.FileIO;
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
// Partner Name : Ng Kai Huat Jason
//==========================================================
// Done By: Ng Kai Huat Jason

namespace T03_Group02_PRG2Assignment
{
    class Waffle : IceCream
    {
        public string WaffleFlavour { get; set; }
        public Waffle() : base() { }

        public Waffle(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, string waffleflavour) : base(option, scoops, flavours, toppings)
        {
            WaffleFlavour = waffleflavour;
        }

        public override double CalculatePrice()
        {
            double basePrice;

            // Switch to check number of scoops
            switch (Scoops)
            {
                case 1:
                    basePrice = 7.00;
                    break;
                case 2:
                    basePrice = 8.50;
                    break;
                case 3:
                    basePrice = 9.50;
                    break;
                default:
                    throw new InvalidOperationException("Invalid number of scoops");
            }

            // Add $1 for each topping
            double toppingPrice = Toppings.Count * 1.00;

            double waffleFlavourPrice = 0.00;

            // Check for specific waffle flavors and add the corresponding cost
            if (WaffleFlavour == "redvelvet" || WaffleFlavour == "charcoal" || WaffleFlavour == "pandan")
            {
                waffleFlavourPrice = 3.00;
            }

            double totalPrice = basePrice + toppingPrice + waffleFlavourPrice;

            return totalPrice;
        }
        public override string ToString()
        {
            return $"{base.ToString()}Waffle Flavour: {WaffleFlavour}\n-----------------------------------\nPrice: ${CalculatePrice():F2}";
        }
    }
}
