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
    abstract class IceCream
    {
        public string Option { get; set; }
        public int Scoops { get; set; }
        //Use private and public, because flavours and toppings have constraints
        private List<Flavour> _flavours;
        public List<Flavour> Flavours
        {
            get { return _flavours; }
            set
            {
                if (value.Count > 3)
                {
                    throw new ArgumentException("Number of flavours should not exceed 3.");
                }
                _flavours = value;
            }
        }

        private List<Topping> _toppings;
        public List<Topping> Toppings
        {
            get { return _toppings; }
            set
            {
                if (value.Count > 4)
                {
                    throw new ArgumentException("Number of toppings should not exceed 4.");
                }
                _toppings = value;
            }
        }

        public IceCream() { }

        public IceCream(string option, int scoops, List<Flavour> flavours, List<Topping> toppings)
        {
            Option = option;
            Scoops = scoops;
            Flavours = flavours;
            Toppings = toppings;
        }

        // Abstract method to calculate price, override in sub-class
        public abstract double CalculatePrice();

        // Override ToString() method to display ice cream details in a table format
        public override string ToString()
        {
            StringBuilder table = new StringBuilder();

            table.AppendLine("----------------------------------");
            table.AppendLine($"Option : {Option,-15}");
            table.AppendLine("----------------------------------");
            table.AppendLine($"Scoops: {Scoops,-15}");
            table.AppendLine("----------------------------------");
            table.AppendLine("Flavours: ");
            if (Flavours.Count > 0)
            {
                table.AppendLine(string.Format("{0,-15}\t{1,-15}\t{2,-15}", "Type", "Premium", "Quantity"));
                foreach (var flavour in Flavours)
                {
                    table.AppendLine($"{flavour,-35}");
                }
                table.AppendLine("-----------------------------------");
            }

            if (Toppings.Count > 0)
            {
                table.AppendLine("Toppings:");
                table.AppendLine("-----------------------------------");

                foreach (var topping in Toppings)
                {
                    table.AppendLine($"{topping,-35}");
                }

                table.AppendLine("-----------------------------------");
            }

            return table.ToString();
        }
    }
}
