using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==========================================================
// Student Number : S10258457
// Student Name : Yeo Jin Rong
// Partner Name : Ng Kai Huat Jason
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

        // Method to calculate price, overrides abstract method in IceCream base class
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

            // Add $2 for each premium flavour
            foreach (Flavour flavour in Flavours)
            {
                if (flavour.Premium)
                {
                    basePrice += flavour.Quantity * 2.00;
                }
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
            return $"{base.ToString()}Dipped: {Dipped.ToString()}\n-----------------------------------\nPrice: ${CalculatePrice():F2}";
        }
    }
}
