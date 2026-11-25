using Microsoft.Data.Sqlite;
using System.Globalization;
// See https://aka.ms/new-console-template for more information

using (var cont = new SqliteConnection("Data Source=inventory.db"))
{
    cont.Open();
    string table = "CREATE TABLE IF NOT EXISTS inventory (brand TEXT, item TEXT UNIQUE, price INTEGER);";
    using (var comd = new SqliteCommand(table, cont))
    {
        comd.ExecuteNonQuery();
        
    }
}

bool theloop = true;
while (theloop)
{
    Console.WriteLine("The Menu: ");
    Console.WriteLine("1: Add Item");
    Console.WriteLine("2: View ");
    Console.WriteLine("3: Update ");
    Console.WriteLine("4: Delete ");
    Console.WriteLine("5: Exit ");

    string input = Console.ReadLine() ?? "";
    if (int.TryParse(input, out int ch))
    {
        switch (ch)
        {
            case 1:
                Console.WriteLine("Enter the brand: ");
                string brand = Console.ReadLine() ?? "";

                Console.WriteLine("Enter the item: ");
                string useritem = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(useritem) && !string.IsNullOrWhiteSpace(brand))
                {
                    Console.WriteLine("Enter the price: ");
                    string userprice = Console.ReadLine() ?? "";
                    if (int.TryParse(userprice, out int pricez) && pricez >= 0)
                    {
                        AddInventory(brand, useritem, pricez);
                        Console.WriteLine("Item added successfully!");
                        ViewInventory();
                    }
                    else
                    {
                        Console.WriteLine("Invalid price. Please enter a valid whole number or a number that is not negative.");
                    }
                    
                }
                else
                {
                    Console.WriteLine("Invalid item name or brand. Please enter a valid item name or brand.");
                    
                }
             break;



            case 2:
                ViewInventory();
                break;

            case 3:

                


                Console.WriteLine("Enter the item you would like to update: ");
                string itemToUpdate = Console.ReadLine() ?? "";

                if (!string.IsNullOrWhiteSpace(itemToUpdate))
                {
                    Console.WriteLine("Enter the price: ");
                    string userprice = Console.ReadLine() ?? "";
                    if (int.TryParse(userprice, out int pricez) && pricez >= 0)
                    {
                        UpdateInventory(itemToUpdate, pricez);
                        
                    }
                    else
                    {
                     Console.WriteLine("Invalid price. Please enter a valid whole number or a number that is not negative.");
                    }
                }
                else
                {
                Console.WriteLine("Invalid item name, please enter a valid item name.");   
                }
                break;


            case 4:
                Console.WriteLine("Enter the item you would like to delete: ");
                string itemToDelete = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(itemToDelete))
                {
                    DeleteInventory(itemToDelete);
                }
                else
                {
                Console.WriteLine("Invalid item name, please enter a valid item name.");
                }
                break;





            case 5:
                        Console.WriteLine("Are you sure? (Y/N)");
                        string confirm = Console.ReadLine() ?? "";
                        if (confirm.ToUpper() == "Y")
                        {
                            Console.WriteLine("Exiting the program...");
                            theloop = false;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Returning to the menu...");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                    }
    }
    else
    {
        Console.WriteLine("Invalid, try add a whole number");
       
    }

       
    


}


string filePath = "inventory.csv";

string[] lines = File.ReadAllLines(filePath); // get contents in inventory file and put in a array 
// brand item price row is 0 index
// lines[2] holds nike, hoodie, 120
string[] parts = [];
parts = lines[0].Split(',');

foreach (string currentrow in lines) 
{
    parts = currentrow.Split(',');
    if (parts[2] == "Price")
    {
       
        continue;
    }

    // check if the price is a valid number if it is insert to database
    if (int.TryParse(parts[2], out int numeralNumber))
    {
       
       AddInventory(parts[0], parts[1], numeralNumber);
    }
    else
    {
        // 2. If the price isn't a valid number, it drops here instead of crashing
        Console.WriteLine($"Skipping invalid row: {parts[0]}");
    }


}
void AddInventory(string brands, string items, int prices)
{
    using (var connection = new SqliteConnection("Data Source=inventory.db"))
    {
        connection.Open();

        string query = "INSERT OR IGNORE INTO inventory (brand, item, price) VALUES (@brand, @item, @price)";

        using (var con = new SqliteCommand(query, connection))
        {
            con.Parameters.AddWithValue("@brand", brands);
            con.Parameters.AddWithValue("@item", items);
            con.Parameters.AddWithValue("@price", prices);
            con.ExecuteNonQuery();
            Console.WriteLine($"Inserted item: {items} with price {prices}");
        }
    }
}
ViewInventory();
void ViewInventory()
{
    using (var con = new SqliteConnection("Data Source=inventory.db"))
    {
        con.Open();
        string squery = "SELECT brand, item, price FROM inventory;";

        using (var command = new SqliteCommand(squery, con))
        {

            using (var reader = command.ExecuteReader())
            {
                bool hasRows = false;
                while (reader.Read())
                {
                    hasRows = true;
                    string brand = reader.GetString(0);
                    string item = reader.GetString(1);
                    long price = reader.GetInt64(2);
                    Console.WriteLine($"Brand: {brand}, Item: {item}, Price: {price}");
                }
                if (!hasRows)
                {
                 Console.WriteLine("No items found in inventory.");
                }
            }
        }
    }
}



void UpdateInventory(string itemz, int newPrice)
{

    // update block then show users what has been updated
    using (var con = new SqliteConnection("Data Source=inventory.db"))
    {
        con.Open();
        string updateQuery = "UPDATE inventory SET price = @price WHERE item = @item;";

        using (var com = new SqliteCommand(updateQuery, con))
        {

            com.Parameters.AddWithValue("@item", itemz);
            com.Parameters.AddWithValue("@price", newPrice);
            int rowsAffected = com.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                Console.WriteLine("Updated item!");
                string squery = "SELECT brand, item, price FROM inventory;";

                using (var command = new SqliteCommand(squery, con))
                {

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string brand = reader.GetString(0);
                            string item = reader.GetString(1);
                            long price = reader.GetInt64(2);
                            Console.WriteLine($"Brand: {brand}, Item: {item}, Price: {price}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Item {itemz} not found in inventory.");
            }

           

        }
    }

}

void DeleteInventory(string youritem)
{
    using (var con = new SqliteConnection("Data Source=inventory.db"))
    {
        con.Open();
        string deleteQuery = "DELETE FROM inventory WHERE item = @item;";

        using (var com = new SqliteCommand(deleteQuery, con))
        {
            com.Parameters.AddWithValue("@item", youritem);
            int rowsaffected = com.ExecuteNonQuery();
            if (rowsaffected > 0)
            {
                Console.WriteLine($"Deleted item: {youritem}");
                Console.WriteLine("Updated Inventory: ");
                ViewInventory();
            }
            else
            {
                Console.WriteLine($"Item {youritem} not found in inventory.");


            }
        }
    }
}
