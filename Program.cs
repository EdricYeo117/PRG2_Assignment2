using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Schema;
using static System.Formats.Asn1.AsnWriter;
using System.Runtime.CompilerServices;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using T03_Group02_PRG2Assignment;
//==========================================================
// Student Number : S10258457
// Student Name : Yeo Jin Rong
// Partner Name : Ng Kai Huat Jason

//=========================================================

// Done By: Yeo Jin Rong
// Main method of program
void Main()
{
    // Intialise all global variables, lists, queues and dictionaries
    int globalorderid = 0;
    List<Tuple<string, double>> flavourslist = InitFlavours();
    List<Tuple<string, double>> toppingslist = InitToppings();
    Queue<Order> regularQueue = new Queue<Order>();
    Queue<Order> goldQueue = new Queue<Order>();
    Dictionary<Order, Customer> ordertocustomer = new Dictionary<Order, Customer>();
    Dictionary<int, Customer> customers = new Dictionary<int, Customer>();
    Dictionary<int, IceCream> orderid_icecream = new Dictionary<int, IceCream>();
    // Intialise customers.csv and orders.csv
    InitCustomers(customers);
    InitOrders(customers, flavourslist);
    // While loop to run the menu after every option
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
        else if (option == "3")
        {
            RegisterNewCustomer(customers);
        }
        else if (option == "4")
        {
            CreateCustomerOrder(customers, regularQueue, goldQueue, ordertocustomer,
                globalorderid, flavourslist, toppingslist);
        }
        else if (option == "5")
        {
            DisplayOrderDetails(customers);
        }
        else if (option == "6")
        {
            ModifyOrder(customers, flavourslist, toppingslist);
        }
        else if (option == "7")
        {
            if (goldQueue.Count > 0)
            {
                ProcessOrder(goldQueue, customers, ordertocustomer);
            }
            else
            {
                ProcessOrder(regularQueue, customers, ordertocustomer);
            }
        }
        else if (option == "8")
        {
            DisplayMonthlyChargedAmounts(customers);
        }
        else if (option == "9")
        {
            PostDataOrders(customers);
            PostDataCustomers(customers);

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

// Method to initialise order information from orders.csv
// Done By: Yeo Jin Rong
void InitOrders(Dictionary<int, Customer> customers, List<Tuple<string, double>> flavourslist)
{
    try
    {
        // Uses stream reader to parse data from CSV file
        using (StreamReader sr = new StreamReader("orders.csv"))
        {
            // Skip the header line
            sr.ReadLine();

            string str;
            while ((str = sr.ReadLine()) != null)
            {
                string[] data = str.Split(',');
                // Defining exactly what each column in CSV represents
                int orderid = int.Parse(data[0].Trim());
                int memberid = int.Parse(data[1].Trim());
                DateTime timereceived = DateTime.ParseExact(data[2].Trim(), "dd/MM/yyyy HH:mm", null);
                DateTime timefulfilled = DateTime.ParseExact(data[3].Trim(), "dd/MM/yyyy HH:mm", null);
                string option = data[4].Trim().ToLower().Replace(" ", "");
                int scoops = int.Parse(data[5].Trim());
                // Selecting specific customer by memberid key in customers dictionary <memberid, Customer>
                Customer selectedcustomer = customers[memberid];
                // Check if order already exists in customer's order history, so that orders of multiple ice cream objects are parsed correctly
                Order existingOrder = selectedcustomer.OrderHistory.FirstOrDefault(o => o.Id == orderid);
                // Create new order object initorder to be equal to existing order if it exists, else create new order object
                Order initorder = existingOrder ?? new Order(orderid, timereceived);
                initorder.TimeFulfilled = timefulfilled;
                if (option == "waffle")
                {
                    // Waffle option requires waffle flavour string as a parameter
                    string waffleflavour = data[7].Trim().ToLower().Replace(" ", "");
                    // For loop to get flavours, stored in a list of flavour objects, list<flavour>
                    List<Flavour> flavours = new List<Flavour>();
                    // To track added flavours and their quantities in a dictionary
                    Dictionary<string, int> flavourQuantityMap = new Dictionary<string, int>();
                    for (int i = 8; i <= 10; i++)
                    {
                        string flavourType = data[i].Trim().ToLower().Replace(" ", "");
                        if (!string.IsNullOrWhiteSpace(flavourType))
                        {
                            // Use the IsPremiumFlavour method to determine if the flavour is premium
                            bool isPremium = IsPremiumFlavour(flavourType, flavourslist);

                            // Assuming Quantity is set to 1 intially
                            Flavour flavour;

                            if (flavourQuantityMap.ContainsKey(flavourType))
                            {
                                // If flavour is repeated, increment the quantity
                                int quantity = flavourQuantityMap[flavourType];
                                flavourQuantityMap[flavourType]++;
                            }
                            else
                            {
                                // If flavour is not repeated, add it to the dictionary
                                flavourQuantityMap[flavourType] = 1;
                            }
                        }
                    }

                    // For loop to add information from flavourQuantityMap into List "flavours"
                    foreach (var item in flavourQuantityMap)
                    {
                        string flavourType = item.Key;
                        int quantity = item.Value;

                        // Use the IsPremiumFlavour method to determine if the flavour is premium
                        bool isPremium = IsPremiumFlavour(flavourType, flavourslist);

                        // Create the Flavour object with the correct quantity
                        Flavour flavour = new Flavour(flavourType, isPremium, quantity);

                        // Add the flavour to the list
                        flavours.Add(flavour);
                    }

                    // For loop to get toppings, stored in a list of topping Objects, list<Topping>
                    List<Topping> toppings = new List<Topping>();
                    for (int i = 11; i <= 14; i++)
                    {
                        string toppingType = data[i].Trim().ToLower().Replace(" ", "");

                        if (!string.IsNullOrWhiteSpace(toppingType))
                        {
                            // Add the topping to the list
                            Topping topping = new Topping(toppingType);
                            toppings.Add(topping);
                        }
                    }
                    // Creates new icecream object, upcasted from waffle object
                    IceCream newwaffle = new Waffle(option, scoops, flavours, toppings, waffleflavour);
                    initorder.AddIceCream(newwaffle);

                }
                else if (option == "cone")
                {
                    // Cone option requires dipped bool as a parameter
                    bool dipped = string.Equals(data[6].Trim(), "true", StringComparison.OrdinalIgnoreCase);
                    // For loop to get flavours, stored in a list of flavour objects, list<flavour>
                    List<Flavour> flavours = new List<Flavour>();
                    // To track added flavours and their quantities in a dictionary
                    Dictionary<string, int> flavourQuantityMap = new Dictionary<string, int>();
                    for (int i = 8; i <= 10; i++)
                    {
                        string flavourType = data[i].Trim().ToLower().Replace(" ", "");
                        if (!string.IsNullOrWhiteSpace(flavourType))
                        {
                            // Use the IsPremiumFlavour method to determine if the flavour is premium
                            bool isPremium = IsPremiumFlavour(flavourType, flavourslist);

                            // Assuming Quantity is set to 1 intially
                            Flavour flavour;

                            if (flavourQuantityMap.ContainsKey(flavourType))
                            {
                                // If flavour is repeated, increment the quantity
                                flavourQuantityMap[flavourType]++;
                            }
                            else
                            {
                                // If flavour is not repeated, add it to the dictionary
                                flavourQuantityMap[flavourType] = 1;
                                flavour = new Flavour(flavourType, isPremium, 1);
                                flavours.Add(flavour);
                            }

                        }
                    }
                    // For loop to get toppings, stored in a list of topping Objects, list<Topping>
                    List<Topping> toppings = new List<Topping>();
                    for (int i = 11; i <= 14; i++)
                    {
                        string toppingType = data[i].Trim().ToLower().Replace(" ", "");

                        if (!string.IsNullOrWhiteSpace(toppingType))
                        {
                            // Add the topping to the list
                            Topping topping = new Topping(toppingType);
                            toppings.Add(topping);
                        }
                    }
                    IceCream newcone = new Cone(option, scoops, flavours, toppings, dipped);
                    initorder.AddIceCream(newcone);
                }
                else if (option == "cup")
                {
                    // For loop to get flavours, stored in a list of flavour objects, list<flavour>
                    List<Flavour> flavours = new List<Flavour>();
                    // To track added flavours and their quantities in a dictionary
                    Dictionary<string, int> flavourQuantityMap = new Dictionary<string, int>();
                    for (int i = 8; i <= 10; i++)
                    {
                        string flavourType = data[i].Trim().ToLower().Replace(" ", "");
                        if (!string.IsNullOrWhiteSpace(flavourType))
                        {
                            // Use the IsPremiumFlavour method to determine if the flavour is premium
                            bool isPremium = IsPremiumFlavour(flavourType, flavourslist);

                            // Assuming Quantity is set to 1 intially
                            Flavour flavour;

                            if (flavourQuantityMap.ContainsKey(flavourType))
                            {
                                // If flavour is repeated, increment the quantity
                                flavourQuantityMap[flavourType]++;
                            }
                            else
                            {
                                // If flavour is not repeated, add it to the dictionary
                                flavourQuantityMap[flavourType] = 1;
                                flavour = new Flavour(flavourType, isPremium, 1);
                                flavours.Add(flavour);
                            }
                        }
                    }
                    // For loop to get toppings, stored in a list of topping Objects, list<Topping>
                    List<Topping> toppings = new List<Topping>();
                    for (int i = 11; i <= 14; i++)
                    {
                        string toppingType = data[i].Trim().ToLower().Replace(" ", "");

                        if (!string.IsNullOrWhiteSpace(toppingType))
                        {
                            // Add the topping to the list
                            Topping topping = new Topping(toppingType);
                            toppings.Add(topping);
                        }
                    }
                    // Creates new icecream object, upcasted from cup object
                    IceCream newcup = new Cup(option, scoops, flavours, toppings);
                    initorder.AddIceCream(newcup);

                }
                // Check if existing order is null, if null, add initorder object to customer's order history
                if (existingOrder == null)
                {
                    selectedcustomer.OrderHistory.Add(initorder);
                }
            }
        }
        Console.Write("\nOrders successfully added");
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

// Method to register new customer, prompting for information, parameters of Dictionary<int, Customer> used. MemberId is the key, Customer is the value
// Done By: Yeo Jin Rong
void RegisterNewCustomer(Dictionary<int, Customer> customers)
{
    // Logic to obtain customer information
    //Customer Name, check not null or empty
    string name;
    do
    {
        Console.Write("Enter customer name: ");
        name = Console.ReadLine().Trim();

        if (string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Invalid input. Please enter a name.");
            continue; // Skip to the next iteration of the loop
        }

        // Validate that the name contains at least one letter
        if (!name.Any(char.IsLetter))
        {
            Console.WriteLine("The name must contain at least one letter.");
            name = ""; // Clear the name to prompt for input again
        }
    } while (string.IsNullOrEmpty(name));

    //Customer Member ID, check for 6 digit integer
    int memberId;
    do
    {
        Console.Write("Enter customer ID number (exactly 6 digits or non-duplicate): ");
    } while (!int.TryParse(Console.ReadLine(), out memberId) || memberId < 100000 || memberId > 999999 || customers.ContainsKey(memberId));

    // Customer DOB, check for correct format and not in the future
    Console.Write("Enter customer date of birth (DD/MM/YYYY): ");
    DateTime dob;
    string input;
    string[] dateFormats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };

    do
    {
        input = Console.ReadLine();

        if (DateTime.TryParseExact(input, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
        {
            // If statement to check if valid date, if not loops
            if (dob <= DateTime.Now)
            {
                break;
            }
            else
            {
                Console.WriteLine("Date of birth cannot be in the future. Please enter a valid date in DD/MM/YYYY format.");
            }
        }
        else
        {
            Console.WriteLine("Invalid date format. Please enter a valid date in DD/MM/YYYY format.");
        }

    } while (true);

    // Create new customer object, append to csv using AppendCustomerToCsv(), add to dictionary using Add() method
    Customer newCustomer = new Customer(name, memberId, dob);
    AppendCustomerToCSV(newCustomer, "customers.csv");
    customers.Add(newCustomer.MemberId, newCustomer);
    Console.WriteLine("New customer successfully added. Check customer.csv for output");
}

// Method to append information into csv, takes Dictionary<int, Customer> as parameter and file path as parameter
// Done By: Yeo Jin Rong
void AppendCustomerToCSV(Customer customer, string filePath)
{
    try
    {
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            string customerInfo = $"{customer.Name},{customer.MemberId},{customer.DOB.ToString("dd/MM/yyyy")},{customer.Rewards.Tier},{customer.Rewards.Points},{customer.Rewards.PunchCard}";
            sw.WriteLine(customerInfo);
        }
    }

    catch (Exception ex)
    {
        Console.WriteLine($"Error appending to CSV file: {ex.Message}");
    }
}

// Method to calculate order number, takes Dictionary<int, Customer> and int globalorderid as parameters, returns int globalorderid after manipulation
// Done By: Yeo Jin Rong
int OrderNumber(Dictionary<int, Customer> customers, int globalorderid)
{
    foreach (var customer in customers.Values)
    {
        if (customer.CurrentOrder != null)
        {
            globalorderid++;
        }
        if (customer.OrderHistory.Count > 0)
        {
            foreach (var item in customer.OrderHistory)
            {
                globalorderid++;
            }
        }
    }
    return globalorderid;
}

// Method to create a customer order, uses MakeOrder() from customer.cs, OrderNumber() method to calculate order number, CreateIceCream() method to create ice cream object, AddIceCream() in order.cs to add ice cream object to List<IceCream> in Order object
// Parameters of dictionary<int, Customer> [MemberId is the key, Customer is the value], two Queue<Order> used, 
// Dictionary<Order, Customer> used to link order to customer, int globalorderid used to calculate order number, List<Tuple<string, double>> used to store flavours, List<Tuple<string, double>> used to store toppings
// Done By: Yeo Jin Rong
void CreateCustomerOrder(Dictionary<int, Customer> customers,
    Queue<Order> regularOrdersQueue, Queue<Order> goldOrdersQueue, Dictionary<Order, Customer> ordertocustomer,
    int globalorderid, List<Tuple<string, double>> flavourslist, List<Tuple<string, double>> toppingslist)
{
    try
    {
        DisplayCustomer(customers);
        Console.Write("Enter in customer ID: ");
        string userInput = Console.ReadLine();
        // Checks both if input is an integer and if the customer exists in the dictionary
        if (int.TryParse(userInput, out int customerSelect) && customers.ContainsKey(customerSelect))
        {
            Customer selectedCustomer = customers[customerSelect];
            // Checks if CurrentOrder is null, if not null, means customer already has an order
            if (selectedCustomer.CurrentOrder != null)
            {
                Console.WriteLine("Customer already has a current order. Please process checkout or modify the order.");
                return;
            }
            Order newOrder = selectedCustomer.MakeOrder();
            int orderid = OrderNumber(customers, globalorderid);
            newOrder.Id = orderid;
            //  Prompt for first ice cream
            IceCream newIceCream = CreateIceCream(flavourslist, toppingslist);
            newOrder.AddIceCream(newIceCream);
            // Loop for possible additional ice cream
            bool addAnotherIceCream;
            do
            {
                // Prompt user to add an ice cream
                Console.Write("Add an ice cream? (Y/N): ");
                string addIceCreamInput = Console.ReadLine().Trim().ToUpper();

                // Validate Input (Y/N)
                if (addIceCreamInput != "Y" && addIceCreamInput != "N")
                {
                    Console.WriteLine("Invalid input. Please enter either 'Y' or 'N'.");
                    addAnotherIceCream = true;  // Set to true to continue the loop
                }
                else
                {
                    if (addIceCreamInput == "Y")
                    {
                        IceCream additionalIceCream = CreateIceCream(flavourslist, toppingslist);
                        newOrder.AddIceCream(additionalIceCream);
                    }
                    addAnotherIceCream = addIceCreamInput == "Y";
                }
            } while (addAnotherIceCream);
            // Link the new order to the customer's current order
            selectedCustomer.CurrentOrder = newOrder;
            // Enqueue to correct queue, add to Dictionary<Order, Customer>
            if (selectedCustomer.Rewards.Tier == "Gold")
            {
                goldOrdersQueue.Enqueue(newOrder);
                ordertocustomer.Add(newOrder, selectedCustomer);
            }
            else
            {
                regularOrdersQueue.Enqueue(newOrder);
                ordertocustomer.Add(newOrder, selectedCustomer);
            }

            Console.WriteLine("Order has been made successfully!");
        }
        else
        {
            Console.WriteLine("Invalid customer selection. Please enter a valid member id.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

// Method to display all order details, parameters of Dictionary<int, Customer> used. MemberId is the key, Customer is the value
// Done By: Ng Kai Huat Jason
void DisplayOrderDetails(Dictionary<int, Customer> customers)
{
    DisplayCustomer(customers);
    try
    {
        Console.Write("Enter in customer selection (option): ");
        string userInput = Console.ReadLine();
        // Checks both if input is an integer and if the customer exists in the dictionary
        if (int.TryParse(userInput, out int customerSelect) && customers.ContainsKey(customerSelect))
        {
            Customer selectedCustomer = customers[customerSelect];
            Console.WriteLine("Customer Current Order:");
            if (selectedCustomer.CurrentOrder != null)
            {
                Console.WriteLine(selectedCustomer.CurrentOrder);
            }
            else
            {
                Console.WriteLine("No current order.");
            }
            Console.WriteLine("Customer Order History: ");
            if (selectedCustomer.OrderHistory.Count > 0)
            {
                foreach (Order order in selectedCustomer.OrderHistory)
                {
                    Console.WriteLine(order);
                }
            }
            else
            {
                Console.WriteLine("No order history.");
            }
        }
        else
        {
            Console.WriteLine("Invalid customer selection. Please enter a valid member id.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

/// Method to modify order, parameters of Dictionary<int, Customer> used. MemberId is the key, Customer is the value, List<Tuple<string, double>> used to store flavours, List<Tuple<string, double>> used to store toppings
// Uses methods in order.cs to modify order, ModifyIceCream(), AddIceCream(), DeleteIceCream() and CreateIceCream() from program.cs
// Done By: Ng Kai Huat Jason
void ModifyOrder(Dictionary<int, Customer> customers, List<Tuple<string, double>> flavourslist, List<Tuple<string, double>> toppingslist)
{
    DisplayCustomer(customers);
    try
    {
        Console.Write("Enter customer selection (option): ");
        string userInput = Console.ReadLine();
        // Checks both if input is an integer and if the customer exists in the dictionary
        if (int.TryParse(userInput, out int customerSelect) && customers.ContainsKey(customerSelect))
        {
            Customer selectedCustomer = customers[customerSelect];
            // Checks if CurrentOrder is null, if null, that means CurrentOrder does not exist, hence cannot be modified
            if (selectedCustomer.CurrentOrder == null)
            {
                Console.WriteLine("No current order found for the selected customer. Please create an order first.");
                return;  // Exit the method if there is no current order
            }
            Console.WriteLine(selectedCustomer.CurrentOrder);
            DisplayMenu2();
            Console.Write("Enter the option: ");
            string option = Console.ReadLine();
            if (option == "0" || option == "1" || option == "2" || option == "3")
            {
                if (option == "1")
                {
                    Console.Write("Enter ice cream number to modify: ");
                    // Validates if input is an integer and if the ice cream number is within the range of the ice cream list
                    int iceCreamNo;
                    if (int.TryParse(Console.ReadLine(), out iceCreamNo) && iceCreamNo >= 1 && iceCreamNo <= selectedCustomer.CurrentOrder.IceCreamList.Count)
                    {
                        // Utilises ModifyIceCream() method in Order.cs
                        selectedCustomer.CurrentOrder.ModifyIceCream(iceCreamNo - 1);
                    }
                    else
                    {
                        Console.WriteLine("Invalid ice cream number. Please enter a valid number.");
                    }
                }
                else if (option == "2")
                {
                    // Utilises CreateIceCream() and AddIceCream() methods in Order.cs 
                    IceCream newIceCream = CreateIceCream(flavourslist, toppingslist);
                    selectedCustomer.CurrentOrder.AddIceCream(newIceCream);
                }
                else if (option == "3")
                {
                    Console.Write("Enter ice cream number to delete: ");
                    // Validets if input is an integer and if the ice cream number is within the range of the ice cream list
                    if (int.TryParse(Console.ReadLine(), out int iceCreamNo) && iceCreamNo >= 1 && iceCreamNo <= selectedCustomer.CurrentOrder.IceCreamList.Count)
                    {
                        selectedCustomer.CurrentOrder.DeleteIceCream(iceCreamNo);
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number for ice cream deletion.");
                    }
                }
                else if (option == "0")
                {
                    Console.WriteLine("Aborting order modification.");
                }
            }
            else
            {
                Console.WriteLine("Invalid option. Please enter a valid option.");
            }
        }
        else
        {
            Console.WriteLine("Invalid customer selection. Please enter a valid member ID.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

// Method to process and checkout an order, parameters of Queue<Order> used, Dictionary<int, Customer> used with MemberId as key, Dictionary<Order, Customer> used
// Uses methods IsBirthday() from Customer.cs, ApplyBirthdayDiscount(), ApplyPunchCardDiscount(), CanRedeemPoints(), RedeemPointsPrompt(), UpgradeTier() from program.cs
// Done By: Yeo Jin Rong
void ProcessOrder(Queue<Order> Queue, Dictionary<int, Customer> customers, Dictionary<Order, Customer> ordertocustomer)
{
    // Checks if Queue is not empty
    if (Queue.Count > 0)
    {
        Order dequeuedOrder = Queue.Dequeue();
        Console.WriteLine(dequeuedOrder);
        // Gets the correspond customer from the Dictionary<Order, Customer> ordertocustomer, then uses the MemberId from customer in ordertocustomer dictionary, to get the Customer object from the Dictionary<int, Customer> customers
        if (ordertocustomer.TryGetValue(dequeuedOrder, out Customer customer) && customers.ContainsKey(customer.MemberId))
        {
            Customer foundCustomer = customers[customer.MemberId];
            Console.WriteLine($"{"Name",-15}\t{"Member ID",-15}\t{"DOB",-15}\t{"Points",-15}\t{"Punch Card",-15}\t{"Tier",-15}");
            Console.WriteLine(foundCustomer);
            double totalbill = dequeuedOrder.CalculateTotal();
            // Check if customer birthday, if yes, apply birthday discount
            if (foundCustomer.IsBirthday() && IsFirstBirthdayOrder(foundCustomer))
            {
                ApplyBirthdayDiscount(dequeuedOrder, ref totalbill);
            }
            // Check if customer has 10 punches, if yes, apply punch card discount
            if (customer.Rewards.PunchCard == 10)
            {
                ApplyPunchCardDiscount(dequeuedOrder, foundCustomer, ref totalbill);
            }
            // Check if customer can redeem points, if yes, prompt for redemption
            if (CanRedeemPoints(foundCustomer))
            {
                RedeemPointsPrompt(foundCustomer, ref totalbill);
            }
            Console.WriteLine($"\t[Total Price: {totalbill:C}]");
            Console.Write("Press any key to make payment...");
            Console.ReadKey();
            int punch = 0;
            // Punches the punch card for each ice cream in the order
            foreach (IceCream icecream in dequeuedOrder.IceCreamList)
            {
                foundCustomer.Rewards.Punch();
                punch++;
                if (foundCustomer.Rewards.PunchCard >= 10)
                {
                    foundCustomer.Rewards.PunchCard = 10;
                    break;
                }
            }
            Console.WriteLine($"Your punch card has been updated with {punch} time(s)");
            // Calculates points to be added to customer, uses AddPoints() method in PointCard.cs, followed by upgrading tier if applicable
            int points = (int)Math.Floor(totalbill * 0.72);
            foundCustomer.Rewards.AddPoints(points);
            if (points > 0)
            {
                UpgradeTier(customer);
            }
            // Processing the order, removing it from CurrentOrder, adding it to OrderHistory, removing it from ordertocustomer dictionary
            dequeuedOrder.TimeFulfilled = DateTime.Now;
            foundCustomer.OrderHistory.Add(dequeuedOrder);
            foundCustomer.CurrentOrder = null;
            ordertocustomer.Remove(dequeuedOrder);
        }
    }
    else
    {
        Console.WriteLine("No orders in the queue.");
    }
}

// Method to apply birthday discount, OrderByDescending() method used to sort the list of ice cream objects by price from CalculatePrice() overridden methods that use dynamic binding for Waffle/Cone/Cup sub-types, FirstOrDefault() method used to get the most expensive ice cream object
// Done By: Yeo Jin Rong
void ApplyBirthdayDiscount(Order order, ref double cost)
{
    IceCream mostExIceCream = order.IceCreamList.OrderByDescending(ic => ic.CalculatePrice()).FirstOrDefault();
    if (mostExIceCream != null)
    {
        cost -= mostExIceCream.CalculatePrice();
        Console.WriteLine("Happy Birthday! The most expensive ice cream in your order is now free!");
    }
}

// Method to check if its the first birthday order, uses Any() to check if all items in OrderHistory
// Lambda function to check if TimeReceived.Day & TimeReceived.Month = DOB.Day & DOB.Month respectively
// Done By: Yeo Jin Rong
bool IsFirstBirthdayOrder(Customer customer)
{
    // Check if the customer has already made an order on their birthday (ignoring the year)
    bool hasBirthdayOrder = customer.OrderHistory.Any(order =>
        order.TimeReceived.Month == customer.DOB.Month &&
        order.TimeReceived.Day == customer.DOB.Day
    );

    // Return true if it's the first order on their birthday, otherwise false
    return !hasBirthdayOrder;
}

// Method to apply punch card discount (regardless if discount available because need to punch card)
// Done By: Yeo Jin Rong
void ApplyPunchCardDiscount(Order order, Customer customer, ref double totalbill)
{
    if (order.IceCreamList.Count > 0 && customer.Rewards.PunchCard == 10)
    {
        IceCream firstIceCream = order.IceCreamList[0];
        // Check if the most expensive ice cream is the first in the order, similar to birthday
        bool isFirstIceCreamMostExpensive = order.IceCreamList
            .OrderByDescending(ic => ic.CalculatePrice())
            .FirstOrDefault() == firstIceCream;

        if (isFirstIceCreamMostExpensive)
        {
            // Apply punch card discount to the next ice cream, if most expensive icecream is the first in IceCreamList
            if (order.IceCreamList.Count > 1)
            {
                IceCream nextIceCream = order.IceCreamList[1];
                double discountAmount = nextIceCream.CalculatePrice();
                totalbill -= discountAmount;
                customer.Rewards.PunchCard = 0;
                Console.WriteLine($"After discount - Total Bill: {totalbill:C}\tPunch Card: {customer.Rewards.PunchCard}");
                Console.WriteLine($"Congrats! Your next Ice Cream in your order is free!");
            }
            else
            {
                Console.WriteLine("No additional ice cream to apply punch card discount.");
            }
        }
        else
        {
            // Apply punch card discount for the first ice cream
            double discountAmount = firstIceCream.CalculatePrice();
            totalbill -= discountAmount;
            customer.Rewards.PunchCard = 0;
            Console.WriteLine($"After discount - Total Bill: {totalbill:C}\tPunch Card: {customer.Rewards.PunchCard}");
            Console.WriteLine($"Congrats! Your first Ice Cream in your order is free!");
        }
    }
    else
    {
        Console.WriteLine($"{10 - customer.Rewards.PunchCard} punches left to free ice cream!");
    }
}

// Method to check if customer can redeem points
// Done By: Yeo Jin Rong
bool CanRedeemPoints(Customer customer)
{
    return customer.Rewards.Tier == "Silver" || customer.Rewards.Tier == "Gold";
}

// Method to redeem points, uses GetValidRedemptionPoints() method to get input for redemption points, uses RedeemPoints() method in PointCard.cs to redeem points
// Done By: Yeo Jin Rong
void RedeemPointsPrompt(Customer customer, ref double totalBill)
{
    int pointsToRedeem = GetValidRedemptionPoints();
    if (pointsToRedeem == -1)
    {
        return;
    }
    if (pointsToRedeem > customer.Rewards.Points)
    {
        Console.WriteLine("Insufficient points. Points redemption failed.");
        return;
    }
    double redemptionAmount = pointsToRedeem * 0.02;
    if (redemptionAmount > totalBill)
    {
        Console.WriteLine("Redemption amount exceeds the total bill. Points redemption failed.");
        return;
    }
    totalBill -= redemptionAmount;
    customer.Rewards.RedeemPoints(pointsToRedeem);
    Console.WriteLine($"You have redeemed {pointsToRedeem} points. Your new total bill is ${totalBill:F2}");
}

// Method to get input for redemption points
// Done By: Yeo Jin Rong
int GetValidRedemptionPoints()
{
    int pointsToRedeem;
    do
    {
        Console.Write("How many points would you like to use to offset your final bill? ");
        string input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out pointsToRedeem) && pointsToRedeem >= 0)
        {
            return pointsToRedeem;
        }

        Console.WriteLine("Invalid input. Please enter a non-negative integer value.");
    } while (true);
}

// Method to upgrade tier in customer object, uses Rewards property in Customer.cs (which is a PointCard object)
// Done By: Yeo Jin Rong
void UpgradeTier(Customer customer)
{
    if (customer.Rewards.Tier == "Ordinary")
    {
        if (customer.Rewards.Points > 150)
        {
            customer.Rewards.Tier = "Gold";
        }

        else if (customer.Rewards.Points > 50)
        {
            customer.Rewards.Tier = "Silver";
        }
    }

    else if (customer.Rewards.Points > 100 && customer.Rewards.Tier == "Silver")
    {
        customer.Rewards.Tier = "Gold";
    }
}

// Prompt the user for the year and display monthly charged amounts breakdown, parameters of Dictionary<int, Customer> used. MemberId is the key, Customer is the value
// Done By: Ng Kai Huat Jason
void DisplayMonthlyChargedAmounts(Dictionary<int, Customer> customers)
{
    Console.Write("Enter the year: ");
    // Validation to check if input is an integer and if the year is within the range of 1 to 2024
    if (!int.TryParse(Console.ReadLine(), out int inputYear) || inputYear < 1 || inputYear > 2025)
    {
        Console.WriteLine("Invalid input. Please enter a valid year up to 2024.");
        return;
    }
    // Dictionary to store the monthly charges
    Dictionary<int, double> monthlyCharges = new Dictionary<int, double>();
    // Obtain the monthly charges in nested for loop
    foreach (var customer in customers.Values)
    {
        foreach (var order in customer.OrderHistory)
        {
            // Check if the order was fulfilled in the specified year
            if (order.TimeFulfilled.HasValue && order.TimeFulfilled.Value.Year == inputYear)
            {
                int month = order.TimeFulfilled.Value.Month;
                double orderTotal = order.CalculateTotal();
                // Initialize charged amount for the month if not already present
                if (monthlyCharges.ContainsKey(month))
                {
                    monthlyCharges[month] += orderTotal;
                }
                else
                {
                    monthlyCharges.Add(month, orderTotal);
                }
            }
        }
    }
    Console.WriteLine($"Monthly Charges Breakdown for {inputYear}:");
    // For loop to display the monthly charges, using CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName() in-built method to get the month name
    for (int month = 1; month <= 12; month++)
    {
        string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        // Validates if the month is present in the dictionary, if not, set the total charge to 0
        double totalCharge = monthlyCharges.ContainsKey(month) ? monthlyCharges[month] : 0;
        Console.WriteLine($"{monthName} {inputYear}\tTotal Charge: {totalCharge:C}");
    }

    // Calculate and display the total yearly charge, uses Sum() in-built method to sum up the values in the dictionary
    double totalYearlyCharge = monthlyCharges.Values.Sum();
    Console.WriteLine($"Total Yearly Charge for {inputYear}: {totalYearlyCharge:C}");
}

//Method to make ice cream object, used in multiple methods, takes parameters of List<Tuple<string, double>> flavourslist and List<Tuple<string, double>> toppingslist
// Uses GetFlavours(), GetToppings(), IsValidWaffleFlavour() and ReadOptionsCSV() methods.
// Done By: Yeo Jin Rong
IceCream CreateIceCream(List<Tuple<string, double>> flavourslist, List<Tuple<string, double>> toppingslist)
{
    // Display the menu for possible ice cream combinations
    ReadOptionsCSV();
    // Validation for option to be "waffle", "cone" or "cup"
    string option;
    do
    {
        Console.Write("\nChoose ice cream option(waffle/cone/cup): ");
        option = Console.ReadLine().ToLower();

        if (option != "waffle" && option != "cone" && option != "cup")
        {
            Console.WriteLine("Invalid option. Please enter waffle, cone, or cup.");
        }
    } while (option != "waffle" && option != "cone" && option != "cup");

    // Validation for scoops to be 1, 2 or 3
    int scoops;
    do
    {
        Console.Write("Enter number of scoops (up to 3): ");
        if (!int.TryParse(Console.ReadLine(), out scoops) || scoops < 1 || scoops > 3)
        {
            Console.WriteLine("Invalid input. Please enter a valid number of scoops between 1 and 3.");
        }
    } while (scoops < 1 || scoops > 3);
    List<Flavour> flavours = GetFlavours(scoops, flavourslist);
    List<Topping> toppings = GetToppings(toppingslist);
    bool dipped = false;
    // Validation if correct quantity of flavours & scoops
    int totalFlavourQuantity = flavours.Sum(f => f.Quantity);
    if (scoops != totalFlavourQuantity)
    {
        Console.WriteLine($"Error: The number of scoops ({scoops}) does not match the total quantity of flavours ({totalFlavourQuantity}). Ice cream creation aborted.");
        return null; // Return null or handle the error accordingly
    }
    switch (option)
    {
        case "waffle":
            // Waffle has additional parameter of waffleflavour (string)
            string waffleFlavour;
            // Loop to validate waffleflavour option
            do
            {
                Console.Write("Enter waffle flavour (plain, red velvet, charcoal, pandan): ");
                waffleFlavour = Console.ReadLine().ToLower().Trim().Replace(" ", "");

                if (string.IsNullOrWhiteSpace(waffleFlavour) || !IsValidWaffleFlavour(waffleFlavour))
                {
                    Console.WriteLine("Invalid waffle flavour. Please enter a valid waffle flavour.");
                }
            } while (string.IsNullOrWhiteSpace(waffleFlavour) || !IsValidWaffleFlavour(waffleFlavour));

            return new Waffle(option, scoops, flavours, toppings, waffleFlavour);

        case "cone":
            // Cone has additional parameter of dipped (bool)
            string dippedInput;
            // Loop if not Y/N input
            do
            {
                Console.Write("Is the cone dipped? (Y/N): ");
                dippedInput = Console.ReadLine().ToLower().Trim();

                if (dippedInput != "y" && dippedInput != "n")
                {
                    Console.WriteLine("Invalid input. Please enter y for 'yes' or n for 'no'.");
                }
            } while (dippedInput != "y" && dippedInput != "n");
            dipped = dippedInput == "y";
            return new Cone(option, scoops, flavours, toppings, dipped);

        case "cup":
            return new Cup(option, scoops, flavours, toppings);
        default:
            Console.WriteLine("Invalid ice cream option. Please enter waffle, cone, or cup.");
            return null;
    }
}

// Method to intialise flavours from flavours.csv, returns a list of Tuple<string, double> (flavour, cost)
// Done By: Yeo Jin Rong
List<Tuple<string, double>> InitFlavours()
{
    List<Tuple<string, double>> flavourslist = new List<Tuple<string, double>>();
    try
    {
        using (StreamReader sr = new StreamReader("flavours.csv"))
        {
            // Skip the header line
            sr.ReadLine();

            string str;
            while ((str = sr.ReadLine()) != null)
            {
                string[] data = str.Split(',');
                string flavour = data[0].Trim().ToLower().Replace(" ", "");
                double cost = double.Parse(data[1].Trim().Replace(" ", ""));
                Tuple<string, double> flavourTuple = Tuple.Create(flavour, cost);
                flavourslist.Add(flavourTuple);
            }
        }
        Console.WriteLine("All Flavours successfully added");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading CSV file: {ex.Message}");
    }
    return flavourslist;
}

// Method to display flavours, takes List<Tuple<string, double>> as parameter
// Done By: Yeo Jin Rong
static void DisplayFlavourMenu(List<Tuple<string, double>> flavourslist)
{
    Console.WriteLine("Flavour Menu:");
    // Display Header
    Console.WriteLine($"{"Index",-8}{"Flavour Name",-15}{"Cost"}");

    // Display Values
    for (int i = 0; i < flavourslist.Count; i++)
    {
        Console.WriteLine($"{i + 1,-8}{flavourslist[i].Item1,-15}{flavourslist[i].Item2:C}");
    }
}

// Method to get input for flavours in CreateIceCream() method, takes int scoops and List<Tuple<string, double>> as parameter, returns List<Flavour>
// Done By: Yeo Jin Rong
static List<Flavour> GetFlavours(int scoops, List<Tuple<string, double>> flavourslist)
{
    List<Flavour> flavours = new List<Flavour>();
    int totalQuantity = 0;
    // Validate if flavours > 0 and flavours <= scoops
    int numberOfFlavours;
    do
    {
        Console.Write($"Enter the number of flavours (up to {scoops}): ");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out numberOfFlavours) || numberOfFlavours <= 0 || numberOfFlavours > scoops)
        {
            Console.WriteLine($"Invalid input. Please enter a number greater than 0 and up to {scoops}. Please try again.");
        }
    } while (numberOfFlavours <= 0 || numberOfFlavours > scoops);

    for (int i = 0; i < numberOfFlavours; i++)
    {
        // if statement to break loop once totalQuantity equals scoops
        if (totalQuantity == scoops)
        {
            Console.WriteLine("Total quantity already equals the number of scoops. Skipping remaining flavours.");
            break;
        }
        // do while loop to validate input for selectedFlavourNumber and prompt for input repeatedly until 
        int selectedFlavourNumber;
        do
        {
            DisplayFlavourMenu(flavourslist);
            Console.Write($"Enter the number corresponding to flavour {i + 1}: ");
        } while (!int.TryParse(Console.ReadLine(), out selectedFlavourNumber) || selectedFlavourNumber < 1 || selectedFlavourNumber > flavourslist.Count);

        string flavourType = flavourslist[selectedFlavourNumber - 1].Item1.ToLower().Trim().Replace(" ", "");
        // Validation to check if a valid flavour and not duplicate
        if (!flavours.Any(f => f.Type.Equals(flavourType)))
        {
            //Validate if premium flavour
            bool isPremium = IsPremiumFlavour(flavourType, flavourslist);
            int remainingScoops = scoops - totalQuantity;
            //Validate if quantity input is valid for Flavour object
            int quantity;
            if (i == numberOfFlavours - 1)
            {
                quantity = remainingScoops;
            }
            else
            {
                // For subsequent flavors, prompt the user to enter a valid quantity
                do
                {
                    Console.Write($"Enter the quantity (must be greater than 0 and not exceed remaining scoops {remainingScoops}): ");

                    // Read and validate user input for quantity, must be positive, non-zero and <= remainingScoops
                    if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0 || quantity > remainingScoops)
                    {
                        Console.WriteLine("Invalid input. Please enter a valid quantity.");
                    }
                } while (quantity <= 0 || quantity > remainingScoops);
            }
            // Validate no additional scoops
            if (totalQuantity + quantity <= scoops)
            {
                flavours.Add(new Flavour(flavourType, isPremium, quantity));
                totalQuantity += quantity;  // Update the total quantity
            }
            else
            {
                Console.WriteLine($"Total quantity exceeds the number of scoops. Skipping this flavour.");
                i--;
            }
        }
        else
        {
            Console.WriteLine($"Invalid flavour: {flavourType}. Skipping this flavour.");
            i--; // Decrement i to re-enter the current flavour
        }
    }
    // Validate that the total quantity of flavours matches the number of scoops
    if (totalQuantity != scoops)
    {
        Console.WriteLine($"Total quantity of flavours ({totalQuantity}) does not match the number of scoops ({scoops}).");
        flavours.Clear(); // Clear the list if the total quantity is not correct
        return null;
    }
    return flavours;
}

// Validation for premium flavour
// Done By: Yeo Jin Rong
static bool IsPremiumFlavour(string flavourType, List<Tuple<string, double>> flavourslist)
{
    //return flavourslist.Find(tuple => tuple.Item1.Equals(flavourType, StringComparison.OrdinalIgnoreCase) && tuple.Item2 == 2.0) != null;
    // Uses .Any to find a tuple inside the flavours list with Item1 in tuple equal to flavourType string and corresponding Item2 is 2.0
    // Lambda expression to provide parameters for tuple
    return flavourslist.Any(tuple => tuple.Item1.Equals(flavourType, StringComparison.OrdinalIgnoreCase) && tuple.Item2 == 2.0);
}

// Validation if waffle flavour is premium
// Done By: Yeo Jin Rong
static bool IsValidWaffleFlavour(string flavourType)
{
    string[] waffleflavourType = { "plain", "redvelvet", "pandan", "charcoal" };
    return waffleflavourType.Contains(flavourType);
}

// Method to intialise toppings from toppings.csv, returns a list of Tuple<string, double> (flavour, cost)
// Done By: Yeo Jin Rong
List<Tuple<string, double>> InitToppings()
{
    List<Tuple<string, double>> toppingslist = new List<Tuple<string, double>>();
    try
    {
        using (StreamReader sr = new StreamReader("toppings.csv"))
        {
            // Skip the header line
            sr.ReadLine();

            string str;
            while ((str = sr.ReadLine()) != null)
            {
                string[] data = str.Split(',');
                string topping = data[0].Trim().ToLower().Replace(" ", "");
                double cost = double.Parse(data[1].Trim());
                Tuple<string, double> toppingTuple = Tuple.Create(topping, cost);
                toppingslist.Add(toppingTuple);
            }
        }
        Console.WriteLine("Toppings successfully added");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading CSV file: {ex.Message}");
    }

    return toppingslist;
}

// Method to display toppings, takes List<Tuple<string, double>> as parameter
// Done By: Yeo Jin Rong
static void DisplayToppingsMenu(List<Tuple<string, double>> toppingslist)
{
    Console.WriteLine("Toppings Menu:");
    // Display Header
    Console.WriteLine($"{"Index",-8}{"Topping Name",-15}{"Cost"}");

    // Display Values
    for (int i = 0; i < toppingslist.Count; i++)
    {
        Console.WriteLine($"{i + 1,-8}{toppingslist[i].Item1,-15}{toppingslist[i].Item2:C}");
    }
}

// Method to get input for toppings in CreateIceCream() method, takes int scoops and List<Tuple<string, double>> as parameter, returns List<Topping>
// Done By: Yeo Jin Rong
static List<Topping> GetToppings(List<Tuple<string, double>> toppingTypes)
{
    List<Topping> toppings = new List<Topping>();
    int numberOfToppings;
    do
    {
        Console.Write($"Enter the number of toppings (up to {toppingTypes.Count}): ");
        // Validate that toppings is not out of range
        if (!int.TryParse(Console.ReadLine(), out numberOfToppings) || numberOfToppings < 0 || numberOfToppings > toppingTypes.Count)
        {
            Console.WriteLine($"Invalid input for the number of toppings. Please enter a number between 0 and {toppingTypes.Count}.");
        }
        else
        {
            // User input is valid, break out of the loop
            break;
        }
    } while (true); // Continue prompting until a valid input is provided

    for (int i = 0; i < numberOfToppings; i++)
    {
        DisplayToppingsMenu(toppingTypes);
        Console.Write($"Enter topping {i + 1}: ");
        // Validate that toppings choice is not out of range
        if (!int.TryParse(Console.ReadLine(), out int selectedToppingNumber) || selectedToppingNumber < 1 || selectedToppingNumber > toppingTypes.Count)
        {
            Console.WriteLine($"Invalid input. Please enter a valid number between 1 and {toppingTypes.Count}.");
            i--; // Decrement due to invalid option
            continue;
        }
        // Select the correct topping
        string toppingType = toppingTypes[selectedToppingNumber - 1].Item1.ToLower().Trim().Replace(" ", "");
        // Validate that topping is not repeated
        if (!toppings.Any(t => t.Type.Equals(toppingType)))
        {
            toppings.Add(new Topping(toppingType));
        }
        else
        {
            Console.WriteLine($"Invalid topping type: {toppingType}. Skipping this topping.");
            i--; // Decrement due to invalid option
        }
    }
    return toppings;
}

// Method to display menu for Main()
// Done By: Yeo Jin Rong
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

// Method to display menu for option 6
// Done By: Ng Kai Huat Jason 
void DisplayMenu2()
{
    List<string> menuOptions = new List<string>
    {
        "Choose existing ice cream to modify",
        "Add an entirely new ice cream to order",
        "Choose existing ice cream to delete",
    };

    Console.WriteLine("\nMenu");

    for (int i = 0; i < menuOptions.Count; i++)
    {
        Console.WriteLine($"[{i + 1}]. {menuOptions[i]}");
    }

    // Add the option for 0 at the end
    Console.WriteLine("[0]. Exit");
}

// Method to read options.csv and display it as a menu
// Done By : Yeo Jin Rong
void ReadOptionsCSV()
{
    try
    {
        using (StreamReader sr = new StreamReader("options.csv"))
        {
            string str;
            while ((str = sr.ReadLine()) != null)
            {
                string[] data = str.Split(',');

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == "null")
                    {
                        data[i] = string.Empty;
                    }
                }

                string option = data[0].Trim();
                string scoops = data[1].Trim();
                string dipped = data[2].Trim();
                string waffleflavour = data[3].Trim();
                string cost = data[4].Trim();

                Console.WriteLine($"{option,-15} {scoops,-10} {dipped,-10} {waffleflavour,-15} {cost,-10:C}");
            }
        }
        Console.Write("\nIce Cream Menu Printed");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading CSV file: {ex.Message}");
    }
}

// Method to display customers, used in multiple methods (this is seperate from DisplayAllCustomers() for the number at the front. That's it)
// Done By : Yeo Jin Rong
void DisplayCustomer(Dictionary<int, Customer> customers)
{
    Console.WriteLine($"{"No.",-5}\t{"Name",-15}\t{"Member ID",-15}\t{"DOB",-15}\t{"Points",-15}\t{"Punch Card",-15}\t{"Tier",-15}");
    int i = 1;
    foreach (Customer customer in customers.Values)
    {
        Console.WriteLine($"{i,-5}\t{customer}");
        i++;
    }
}

// Advanced Option 3, post data to REST Database
// Original implementation used static async task and task.run, however code encountered bottlenecks in RestDB thus normal static void methods are used
// Done By : Yeo Jin Rong
static void PostDataCustomers(Dictionary<int, Customer> customers)
{
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("x-apikey", "65b88d513d3b7ffef5c26560");

        foreach (Customer customer in customers.Values)
        {
            var customOutput = new
            {
                customer.Name,
                customer.MemberId,
                DOB = customer.DOB.ToString("yyyy-MM-ddTHH:mm:ss"),
                CurrentOrder = customer.CurrentOrder == null ? null : customer.CurrentOrder.ToString(),
                OrderHistory = customer.OrderHistory,
                Rewards = customer.Rewards == null ? null : customer.Rewards.ToString()
            };

            string customOutputJson = JsonSerializer.Serialize(customOutput, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var payload = new StringContent(customOutputJson, Encoding.UTF8, "application/json");
            var endpoint = new Uri("https://prg2assignment-ddec.restdb.io/rest/customer");

            try
            {
                var response = client.PostAsync(endpoint, payload).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Sucessful Customer Database Input");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}

static void PostDataOrders(Dictionary<int, Customer> customers)
{
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("x-api-key", "65b88d513d3b7ffef5c26560");

        foreach (Customer customer in customers.Values)
        {
            foreach (Order order in customer.OrderHistory)
            {
                try
                {
                    var endpoint = new Uri("https://prg2assignment-ddec.restdb.io/rest/order");
                    var newPostJson = JsonSerializer.Serialize(order);
                    var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(endpoint, payload).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine("Sucessful Order Database Input");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            if (customer.CurrentOrder != null)
            {
                try
                {
                    var endpoint = new Uri("https://prgassignment2-99f7.restdb.io/rest/orders");
                    var newPostJson = JsonSerializer.Serialize(customer.CurrentOrder);
                    var payload = new StringContent(newPostJson, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(endpoint, payload).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        Console.WriteLine("Sucessful Order Database Input");
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }
        }
    }
}

Main();
Console.Read();