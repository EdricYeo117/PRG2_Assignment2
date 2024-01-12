using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==========================================================
// Student Number : S10258457
// Student Name : Yeo Jin Rong
// Partner Name : Ng Kai Huat Jason
//==========================================================
// Done By: Ng Kai Huat Jason
namespace T03_Group02_PRG2Assignment
{
    class Topping
    {
        public string Type { get; set; }

        public Topping() { }

        public Topping(string type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"{Type,-15}";
        }
    }
}
