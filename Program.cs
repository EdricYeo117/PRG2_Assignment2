using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Xml.Schema;
using T03_Group02_PRG2Assignment;
//==========================================================
// Student Number : S10258457
// Student Name : Yeo Jin Rong
// Partner Name : Jason Ng
//=========================================================

// Done By : Yeo Jin Rong
// Method of main loop to execute programme
void Main()
{
    Queue<Order> regularQueue = new Queue<Order>();
    Queue<Order> goldQueue = new Queue<Order>();
    Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
    InitCustomers(customers);
    // While loop to programme run till exit
    while (true)
    {
        DisplayMenu();
        Console.Write("Enter the option: ");
        string option = Console.ReadLine();
        if (option == "1")
        {
            DisplayAllCustomers(customers);
        }
        else if (option == "2")
        {
            DisplayOrders(regularQueue, goldQueue);
        }
        else if (option == "0")
        {
            Console.WriteLine("Program exited.");
            break;
        }
        else
        {
            Console.WriteLine("Key in a valid option.");
        }
    }

}
// Done By : Yeo Jin Rong
// Method to pull information from csv file to read customer information
// Method to initialise customer information from customers.csv
// Done By: Yeo Jin Rong
void InitCustomers(Dictionary<int, Customer> customers)
{
    try
    {
        // Uses stream reader to parse data from CSV file
        using (StreamReader sr = new StreamReader("customers.csv"))
        {
            // Skip the header line
            sr.ReadLine();

            string str;
            while ((str = sr.ReadLine()) != null)
            {
                string[] data = str.Split(',');
                // Defining exactly what each column in CSV represents
                string name = data[0].Trim();
                int memberId = int.Parse(data[1].Trim());
                DateTime dob = DateTime.ParseExact(data[2].Trim(), "dd/MM/yyyy", null);
                string membershipstatus = data[3].Trim();
                int memberpoints = int.Parse(data[4].Trim());
                int punchcard = int.Parse(data[5].Trim());
                // Create customer object to store customer information
                Customer customer = new Customer(name, memberId, dob);
                customer.Rewards.Tier = membershipstatus;
                customer.Rewards.Points = memberpoints;
                customer.Rewards.PunchCard = punchcard;
                customers.Add(memberId, customer);
            }
        }
        Console.Write("Customers successfully added");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading CSV file: {ex.Message}");
    }
}

// Method to display customer details, parameters of Dictionary<int, Customer> used. MemberId is the key, Customer is the value
// Done By: Yeo Jin Rong
void DisplayAllCustomers(Dictionary<int, Customer> customers)
{
    Console.WriteLine("List of all customers details:");
    Console.WriteLine($"{nameof(Customer.Name),-15}\t{nameof(Customer.MemberId),-15}\t{nameof(Customer.DOB),-15}\t{"Points",-15}\t{"Punchcard",-15}\t{"Tier",-15}");
    // For loop to use customer ToString() method to print out customer details
    foreach (Customer customer in customers.Values)
    {
        Console.WriteLine(customer);
    }
}

// Method to display customer orders in queue, calling DisplayOrdersInQueue() method, parameters of Queue<Order> used. 
// Done By: Ng Kai Huat Jason
void DisplayOrders(Queue<Order> regularQueue, Queue<Order> goldQueue)
{
    Console.WriteLine("Current Orders for Gold Members:");
    DisplayOrdersInQueue(goldQueue);

    Console.WriteLine("\nCurrent Orders for Regular Customers:");
    DisplayOrdersInQueue(regularQueue);
}

// Method to display order in Queue
// Done By: Ng Kai Huat Jason
void DisplayOrdersInQueue(Queue<Order> orderQueue)
{
    // Validation to check that orderQueue is not empty
    if (orderQueue.Count > 0)
    {
        // For loop to print out order details using ToString() method
        foreach (Order order in orderQueue)
        {
            Console.WriteLine(order);
        }
    }
    else
    {
        Console.WriteLine("No orders in the queue.");
    }
}


// Method to display menu for Main()
// Done By: Ng Kai Huat Jason
void DisplayMenu()
{
    List<string> menuOptions = new List<string>
                {
                    "List all customers",
                    "List all current orders",
                    "Register a new customer",
                    "Create a customer’s order",
                    "Display order details of customer",
                    "Modify Order Details",
                    "Process Order and Checkout",
                    "Display monthly charged amounts breakdown & total charged amounts for the year",
                    "Post Customer & Orders to REST Database",
                    "Exit"
                };
    Console.WriteLine("\nMenu");
    for (int i = 0; i < menuOptions.Count - 1; i++)
    {
        Console.WriteLine($"[{i + 1}]. {menuOptions[i]}");
    }
    Console.WriteLine($"[0]. {menuOptions[9]}");
}