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
            // Waiting for Jason's Code
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
void InitCustomers(Dictionary<int, Customer> customers)
{
    try
    {
        using (StreamReader sr = new StreamReader("customers.csv"))
        {
            // Skip the header line
            sr.ReadLine();

            string str;
            while ((str = sr.ReadLine()) != null)
            {
                string[] data = str.Split(',');

                string name = data[0].Trim();
                int memberId = int.Parse(data[1].Trim());
                DateTime dob = DateTime.ParseExact(data[2].Trim(), "dd/MM/yyyy", null); // Parses dates properly
                Customer customer = new Customer(name, memberId, dob);
                // Add to dictionary
                customers.Add(memberId, customer);
            }
        }
        Console.Write("Customer successfully added");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading CSV file: {ex.Message}");
    }
}

// Done By: Yeo Jin Rong
//Method to display all customers info, as per customer.cs
void DisplayAllCustomers(Dictionary<int, Customer> customers)
{
    Console.WriteLine("List of all customers details:");
    // Print Header
    Console.WriteLine($"{nameof(Customer.Name),-15}\t{nameof(Customer.MemberId),-15}\t{nameof(Customer.DOB),-15}\t{"Points",-15}\t{"Punchcard",-15}\t{"Tier",-15}");
    // For loop to print each customer
    foreach (Customer customer in customers.Values)
    {
        Console.WriteLine(customer);
    }
}


// Done By: Yeo Jin Rong
// Method to display menu
void DisplayMenu()
{
    // List to store menu options
    List<string> menuOptions = new List<string>
                {
                    "List all customers",
                    "List all current orders",
                    "Exit"
                };
    Console.WriteLine("\nMenu");
    // For loop to print options
    for (int i = 0; i < menuOptions.Count - 1; i++)
    {
        Console.WriteLine($"[{i + 1}]. {menuOptions[i]}");
    }
    Console.WriteLine($"[0]. {menuOptions[9]}");
}

Main();
Console.Read();