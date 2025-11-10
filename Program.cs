using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;



class Program 
{
    static double taxRate = 0.095;
    static string cafeName = "Monty's Coffee Shop";

    static List<string> itemNames = new List<string>();

    static List<double> itemPrices = new List<double>();

    static List<int> itemQuantities = new List<int>();



    public static void ShowBanner(string cafeName, double taxRate)
    {
        Console.WriteLine("----------------------------");
        Console.WriteLine($"Welcome to {cafeName} - Tax Rate: {taxRate}");
    }


    public static void MainMenu()

    {
        while (true)
        {
            Console.WriteLine("\nPlease Select an Option");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. View Cart");
            Console.WriteLine("3. Remove Item By Number");
            Console.WriteLine("4. Checkout");
            Console.WriteLine("5. Save Cart");
            Console.WriteLine("6. Load Cart");
            Console.WriteLine("7. Quit");

            Console.WriteLine("\nPlease Select an Option (1-7): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Clear();

                string name = "";
                while (true)
                {

                    Console.WriteLine("\nEnter the name of the item: ");
                    name = Console.ReadLine();
                    bool isValidName = ValidateString(name);

                    if (isValidName == true)
                    {
                        break;
                    }
                }

                Console.Clear();
                Console.WriteLine("\nPlease enter the price of the item: ");
                double price = double.Parse(Console.ReadLine());
                NoNegatives(price);

                Console.Clear();
                Console.WriteLine("\nPlease enter the quantity of the item: ");
                int quantity = int.Parse(Console.ReadLine());
                NoNegatives(quantity);

                Console.Clear();
                AddItem(name, price, quantity);
                Console.WriteLine("\nItem Added To Cart Successfully.");

            }
            else if (choice == "2")
            {
                Console.Clear();

                ViewCart();
            }

            else if (choice == "3")
            {
                Console.Clear();

                Console.WriteLine("\nEnter the number of the item you would like to remove.");
                int index = int.Parse(Console.ReadLine());
                string itemName = RemoveItem(index);
                Console.WriteLine($"\n{itemName} was sucessfully removed.");

            }

            else if (choice == "4")
            {
                Console.Clear();

                CheckOut();

            }

            else if (choice == "5")
            {
                Console.Clear();
                SaveCart();
            }

            else if(choice == "6")
            {
                Console.Clear();
                LoadCart();
            }


            else if (choice == "7")
            {
                Console.Clear();

                Console.WriteLine("\nThank you for using my Program!- Stoat");

                System.Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("\nThis is not a valid option, try again.");
            }

            
        }
    }

            
    public static void SaveCart()
    {
        string filePath = "cart.txt";
        using(StreamWriter sw = new StreamWriter(filePath))
        {
            for(int i = 0; i <itemNames.Count; i++)
            {
                sw.WriteLine($"{itemNames[i]}, {itemPrices[i]}, {itemQuantities[i]}");

            }

        }

        Console.WriteLine("Cart saved successfully to cart.txt!");
    }


    public static void LoadCart()
    {
        string filePath = "cart.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("No Saved Cart Found.");
            return;
        }

        foreach (string line in File.ReadAllLines(filePath))
        {
            string[] parts = line.Split(",");
            if (parts.Length == 3)
            {
                itemNames.Add(parts[0]);
                itemPrices.Add(double.Parse(parts[1]));
                itemQuantities.Add(int.Parse(parts[2]));

            }
        }
        Console.WriteLine("Cart Loaded from cart.txt");
        

    }   

    public static void PrintList<T>(List<T> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
    }


    public static void ViewCart()
    {   
        Console.Clear();
        int cartLength = itemNames.Count;
        if (cartLength >= 1)
        {
            Console.WriteLine("---------Current Cart---------");

            Console.WriteLine($"\nYour cart has {cartLength} item(s). ");

            double maxPrice = itemPrices.Max();
            int maxIndex = itemPrices.IndexOf(maxPrice);

            for (int i = 0; i < cartLength; i++)
            {
                double lineItemTotal = itemPrices[i] * itemQuantities[i];
                Console.WriteLine($"{i + 1}). {itemNames[i]} ${itemPrices[i]} x{itemQuantities[i]} = ${lineItemTotal}");


            }


            Console.WriteLine($"\nHighest priced item in your cart is {itemNames[maxIndex]}");
            double total = ComputeSubtotal(itemPrices, itemQuantities);
            Console.WriteLine($"\nYour Total is ${total} ");
        }





    
    }   


    public static double ComputeSubtotal(List<double> itemPrices, List<int> itemQuantities)
    {
        double subTotal = 0;
        for (int i = 0; i < itemPrices.Count; i++)
        {
            subTotal += itemPrices[i] * itemQuantities[i];
        }
        return subTotal;

    }


    public static double ComputeTax(double subTotal, double taxRate)
    {
        double taxAmount = Math.Round(subTotal *  taxRate, 2);
        Console.WriteLine($"You Pay ${taxAmount} In Tax. ");
        return taxAmount;

    }
    

    public static void AddItem(string name, double price, int quantity)
    {
        itemNames.Add(name);
        itemPrices.Add(price);
        itemQuantities.Add(quantity);
        
    }


    public static string RemoveItem(int index)

    {
        string itemName = itemNames[index - 1];    

        if (index > itemNames.Count)
        {
            Console.WriteLine("Invalid Entry");

        }

        else
        {
            itemNames.RemoveAt(index - 1);
            itemPrices.RemoveAt(index - 1);
            itemQuantities.RemoveAt(index - 1);

        }

        return itemName;

    }


    public static void CheckOut()
    {
        Console.Clear();

        Console.WriteLine("\nEnter Discount Code:");
        string typedCode = Console.ReadLine().ToUpper();
        if (typedCode != "STUDENT10")
        {
            Console.WriteLine("\nInvalid Code");
        }

        double subTotal = ComputeSubtotal(itemPrices, itemQuantities);
        double tax = ComputeTax(subTotal, taxRate);
        double discount = ApplyDiscount(subTotal, typedCode);
        double finalTotal = Math.Round((subTotal + tax) - discount, 2);

        Console.WriteLine("---------Receipt---------");
        Console.WriteLine($"Subtotal: ${subTotal}");
        Console.WriteLine($"Discount: ${discount}");
        Console.WriteLine($"Tax: ${tax}");
        Console.WriteLine($"Final Total: ${finalTotal}");
        Console.WriteLine($"Thank you for visiting {cafeName}! ");

    }
    


    public static double ApplyDiscount(double subTotal, string code)
    {
        if(code == "STUDENT10")
        {
            return Math.Round(subTotal * 0.10, 2);
        }
        else
        {
            return 0;
        }
    }


    public static void NoNegatives(double value) 
    { 
        if(value <= 0)
        {
            Console.WriteLine("This is not a valid number. Please enter a positive number");

            System.Environment.Exit(1);

        }   

    }


    public static bool ValidateString(string text)
    {
        if (text == "")
        {
            Console.WriteLine("Blank");
            return false;
        
        }
        else
        {
            return true;
        }



    }   
  
   



    

    

    static void Main(string[] args)
    {
            
        Console.WriteLine("Application Started.");
        ShowBanner(cafeName, taxRate);

        MainMenu();

         
    }


   


}
