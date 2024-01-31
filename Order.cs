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
// Done By: Yeo Jin Rong
namespace T03_Group02_PRG2Assignment
{
    class Order
    {
        public int Id { get; set; }
        public DateTime TimeReceived { get; set; }
        public DateTime? TimeFulfilled { get; set; }
        public List<IceCream> IceCreamList { get; set; }

        public Order()
        {
        }

        // Constructor class sets TimeFulfilled as null, and initializes IceCreamList
        public Order(int id, DateTime timeReceived)
        {
            Id = id;
            TimeReceived = timeReceived;
            TimeFulfilled = null;
            IceCreamList = new List<IceCream>();
        }

        // Method to modify an ice cream in the order, uses same logic as Option 4 but uses strings as inputs for smooth validation (unable to implement menus)
        public void ModifyIceCream(int iceCreamIndex)
        {
            if (iceCreamIndex < 0 || iceCreamIndex >= IceCreamList.Count)
            {
                Console.WriteLine("Invalid ice cream index. Please enter a valid number.");
                return;
            }

            IceCream existingIceCream = IceCreamList[iceCreamIndex];
            Console.WriteLine($"Current details of Ice Cream {iceCreamIndex + 1}:");
            Console.WriteLine(existingIceCream);
            Console.WriteLine("Enter new details for the Ice Cream:");

            // Create a new ice cream object based on user input
            string option;
            do
            {
                Console.Write("Choose ice cream option (waffle/cone/cup): ");
                option = Console.ReadLine().ToLower().Trim();

                if (option != "waffle" && option != "cone" && option != "cup")
                {
                    Console.WriteLine("Invalid option. Please enter waffle, cone, or cup.");
                }
            } while (option != "waffle" && option != "cone" && option != "cup");

            int scoops;
            do
            {
                Console.Write("Enter number of scoops (up to 3): ");
                if (!int.TryParse(Console.ReadLine(), out scoops) || scoops < 1 || scoops > 3)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number of scoops between 1 and 3.");
                }
            } while (scoops < 1 || scoops > 3);

            //Get Flavours
            List<Flavour> flavours = new List<Flavour>();
            int numberOfFlavours;
            int totalQuantity = 0;
            // Validate number of flavours 1 <= x <= 3, and is int
            do
            {
                Console.Write($"Enter the number of flavours (up to {scoops}) (vanilla/chocolate/strawberry/durian/sea salt/ube): ");
                numberOfFlavours = Convert.ToInt32(Console.ReadLine());

                if (numberOfFlavours > scoops)
                {
                    Console.WriteLine("Invalid input. You can only enter up to 3 flavours. Please try again.");
                }
            } while (numberOfFlavours > scoops);

            for (int i = 0; i < numberOfFlavours; i++)
            {
                if (totalQuantity == scoops)
                {
                    Console.WriteLine("Total quantity already equals the number of scoops. Skipping remaining flavours.");
                    break;
                }
                // do while 
                Console.Write($"Enter flavour {i + 1} (vanilla/chocolate/strawberry/durian/sea salt/ube): ");
                string flavourType = Console.ReadLine().ToLower().Trim().Replace(" ", "");
                string[] validFlavours = { "vanilla", "chocolate", "strawberry", "seasalt", "durian", "ube" };
                string[] premiumFlavours = { "durian", "ube", "seasalt" };

                // Validation to check if a valid flavour and not duplicate
                if (validFlavours.Contains(flavourType) && !flavours.Any(f => f.Type.Equals(flavourType)))
                {
                    //Validate if premium flavour
                    bool isPremium = premiumFlavours.Contains(flavourType);
                    int remainingScoops = scoops - totalQuantity;
                    //Validate if quantity > 0
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
                    if (totalQuantity + quantity <= scoops)
                    {
                        flavours.Add(new Flavour(flavourType, isPremium, quantity));
                        totalQuantity += quantity;  // Update the total quantity
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
            }
            //Get Toppings
            List<Topping> toppings = new List<Topping>();
            string[] validToppings = { "sprinkles", "mochi", "sago", "oreos" };
            int numberOfToppings;
            do
            {
                Console.Write($"Enter the number of toppings (up to {validToppings.Length}): ");
                // Validate that toppings is not out of range
                if (!int.TryParse(Console.ReadLine(), out numberOfToppings) || numberOfToppings < 0 || numberOfToppings > validToppings.Length)
                {
                    Console.WriteLine($"Invalid input for the number of toppings. Please enter a number between 0 and {validToppings.Length}.");
                }
                else
                {
                    // User input is valid, break out of the loop
                    break;
                }
            } while (true); // Continue prompting until a valid input is provided
            for (int i = 0; i < numberOfToppings; i++)
            {
                Console.Write($"Enter topping {i + 1} (sprinkles/mochi/sago/oreos): ");
                string toppingType = Console.ReadLine().ToLower();

                if (validToppings.Contains(toppingType) && !toppings.Any(t => t.Type.Equals(toppingType)))
                {
                    toppings.Add(new Topping(toppingType));
                }
                else
                {
                    Console.WriteLine($"Invalid topping type: {toppingType}. Skipping this topping.");
                    i--; // Decrement due to invalid option
                }
            }

            //Check for subclass type
            switch (option)
            {
                case "waffle":
                    string waffleFlavour;
                    string[] validWaffleFlavours = { "plain", "redvelvet", "pandan", "charcoal" };
                    do
                    {
                        Console.Write("Enter waffle flavour (plain, red velvet, charcoal, pandan): ");
                        waffleFlavour = Console.ReadLine().ToLower().Trim().Replace(" ", "");
                        if (string.IsNullOrWhiteSpace(waffleFlavour) || validWaffleFlavours.Contains(waffleFlavour))
                        {
                            Console.WriteLine("Invalid waffle flavour. Please enter a valid waffle flavour.");
                        }
                    } while (string.IsNullOrWhiteSpace(waffleFlavour) || !validWaffleFlavours.Contains(waffleFlavour));

                    IceCream modifiedIceCreamWaffle = new Waffle(option, scoops, flavours, toppings, waffleFlavour);
                    IceCreamList[iceCreamIndex] = modifiedIceCreamWaffle;
                    Console.WriteLine("Ice cream modification successful.");
                    break;

                case "cone":
                    string dippedInput;
                    do
                    {
                        Console.Write("Is the cone dipped? (Y/N): ");
                        dippedInput = Console.ReadLine().ToLower().Trim();

                        if (dippedInput != "y" && dippedInput != "n")
                        {
                            Console.WriteLine("Invalid input. Please enter y for 'yes' or n for 'no'.");
                        }
                    } while (dippedInput != "y" && dippedInput != "n");
                    bool dipped = dippedInput == "y";

                    IceCream modifiedIceCreamCone = new Cone(option, scoops, flavours, toppings, dipped);
                    IceCreamList[iceCreamIndex] = modifiedIceCreamCone;
                    Console.WriteLine("Ice cream modification successful.");
                    break;

                case "cup":
                    IceCream modifiedIceCreamCup = new Cup(option, scoops, flavours, toppings);
                    IceCreamList[iceCreamIndex] = modifiedIceCreamCup;
                    Console.WriteLine("Ice cream modification successful.");
                    break;

                default:
                    Console.WriteLine("Invalid ice cream option. Ice cream modification aborted.");
                    break;
            }
        }

        // Method to add an ice cream to the order, downcasts the ice cream object to the appropriate subclass
        public void AddIceCream(IceCream iceCream)
        {
            switch (iceCream.Option.ToLower())
            {
                case "waffle":
                    IceCreamList.Add((Waffle)iceCream);
                    break;
                case "cone":
                    IceCreamList.Add((Cone)iceCream);
                    break;
                case "cup":
                    IceCreamList.Add((Cup)iceCream);
                    break;
                default:
                    throw new ArgumentException("Invalid ice cream option.");
            }
        }

        // Method to delete an icecream from IceCreamList by index
        public void DeleteIceCream(int icecreamno)
        {
            if (IceCreamList.Count == 0)
            {
                Console.WriteLine("No ice creams in the order to delete.");
                return;
            }

            if (icecreamno < 1 || icecreamno > IceCreamList.Count)
            {
                Console.WriteLine("Invalid selection. Please enter a valid number.");
                return;
            }

            IceCream removedIceCream = IceCreamList[icecreamno - 1];
            IceCreamList.RemoveAt(icecreamno - 1);
            Console.WriteLine($"Ice cream removed: {removedIceCream}");
        }

        // Method that takes advantage of dynamic binding to calculate the total cost of the order
        public double CalculateTotal()
        {
            double totalCost = 0.0;
            foreach (IceCream iceCream in IceCreamList)
            {
                // Dynamic method from waffle, cup and cone
                totalCost += iceCream.CalculatePrice();
            }
            return totalCost;
        }

        public override string ToString()
        {
            StringBuilder orderDetails = new StringBuilder();
            orderDetails.AppendLine("---------------------------------------------------");
            orderDetails.AppendLine("Ice Creams:");
            orderDetails.AppendLine("---------------------------------------------------");
            int i = 0;

            foreach (IceCream iceCream in IceCreamList)
            {
                orderDetails.AppendLine("---------------------------------------------------");
                orderDetails.AppendLine($"Ice Cream {i + 1}:");
                orderDetails.AppendLine("---------------------------------------------------");
                orderDetails.AppendLine(iceCream.ToString());
                i++;
            }

            string timeFulfilledString = TimeFulfilled.HasValue ? TimeFulfilled.Value.ToShortDateString() : "Not fulfilled yet";
            string header = string.Format("{0,-15} {1,-15} {2,-15}", "Order ID", "Time Received", "Time Fulfilled");
            string orderDetailsFormatted = string.Format("{0,-15} {1,-15} {2,-15}", Id, TimeReceived.ToShortDateString(), timeFulfilledString);

            return $"{header}\n{orderDetailsFormatted}\n{orderDetails}";
        }
    }
}
