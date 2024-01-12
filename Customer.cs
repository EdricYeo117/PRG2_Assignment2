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
    class Customer
    {
        public string Name { get; set; }
        public int MemberId { get; set; }
        public DateTime DOB { get; set; }
        public Order CurrentOrder { get; set; }
        public List<Order> OrderHistory { get; set; }

        public PointCard Rewards { get; set; }

        public Customer() { }

        public Customer(string name, int memberId, DateTime dob)
        {
            Name = name;
            MemberId = memberId;
            DOB = dob;
            CurrentOrder = null;
            OrderHistory = new List<Order>();
            Rewards = new PointCard(0, 0);
        }

        public Order MakeOrder()
        {
            if (CurrentOrder == null)
            {
                int orderId = OrderHistory.Count + 1;
                DateTime TimeRecieved = DateTime.Now;
                Order order = new Order(orderId, TimeRecieved);
                return order;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return $"{Name,-15}\t{MemberId,-15}\t{DOB.ToShortDateString(),-15}\t{Rewards.ToString()}";
        }
    }
}
