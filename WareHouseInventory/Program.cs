using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventory
{
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Quantity { get; set; }
        public string Brand { get; set; }
        public int WarrantyMonths { get; set; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }
    }

    public class GroceryItem : IInventoryItem
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }
    }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        public Dictionary<int, T> _items = new Dictionary<int, T>();
        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            }
            _items[item.Id] = item;
        }
        public List<T> GetAllItems()
        {
            return _items.Values.ToList();
        }

        public T GetItemById(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            return _items[id];
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            _items.Remove(id);
        }

        public void UpdateItemQuantity(int id, int newQuantity)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Quantity cannot be negative.");
            }
            _items[id].Quantity = newQuantity;
        }
    }

    public class WarehouseManager
    {
        public InventoryRepository<ElectronicItem> _electronics { get; set; } = new InventoryRepository<ElectronicItem>();
        public InventoryRepository<GroceryItem> _groceries { get; set; } = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "BrandA", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 20, "BrandB", 12));
            _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(3)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var items = repo.GetAllItems();
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                repo.UpdateItemQuantity(id, quantity);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void RemoveItem<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            WarehouseManager manager = new WarehouseManager();

            manager.SeedData();

            Console.WriteLine("Grocery Items:");
            manager.PrintAllItems(manager._groceries);

            Console.WriteLine("\nElectronic Items:");
            manager.PrintAllItems(manager._electronics);

            Console.WriteLine("\nAdding duplicate item");
            try
            {
                manager._electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "BrandA", 24));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\nRemoving non-existing item");
            try
            {
                manager._groceries.RemoveItem(999);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\nUpdating with invalid quantity");
            try
            {
                manager._electronics.UpdateItemQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}