﻿using System;

namespace InventoryRecord
{
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded): IInventoryEntity;

    public interface IInventoryEntity
    {
       int Id { get; }
    }

    public class InventoryLogger<T> where T : InventoryItem
    {
        List<T> _log = new List<T>();
        public string _filePath;

        public InventoryLogger(string filePath)
        {
             _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }


        public void SaveToFile()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                throw new InvalidOperationException("File path is not set.");
            }

            try
            {
                using (var writer = new StreamWriter(_filePath, true))
                {
                    foreach (T item in _log)
                    {
                        writer.WriteLine($"{item.Id},{item.Name},{item.Quantity},{item.DateAdded}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                throw new InvalidOperationException("File path is not set.");
            }
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 4 && int.TryParse(parts[0], out int id) && int.TryParse(parts[2], out int quantity) && DateTime.TryParse(parts[3], out DateTime dateAdded))
                        {
                            var item = new InventoryItem(id, parts[1], quantity, dateAdded);
                            if (item != null) _log.Add((T)item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading from file: {ex.Message}");
            }
        }
    }


    public class InventoryManager
    {
        public InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>("C:\\Users\\fente\\Desktop\\dcit318-assignment3-11210750\\InventoryRecords\\inventory.txt");

        public void SeedSampleData()
        {
            
            _logger.Add(new InventoryItem(1, "Sample Item 1", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Sample Item 2", 80, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Sample Item 3", 90, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Sample Item 4", 50, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();

        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
            }
        }

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            InventoryManager manager = new InventoryManager();

            //Console.WriteLine("Seeding sample data...");
            //manager.SeedSampleData();

            //Console.WriteLine("Saving data to file...");
            //manager.SaveData();

            Console.WriteLine("Loading data from file...");
            manager.LoadData();

            Console.WriteLine("Printing all items:");
            manager.PrintAllItems();
        }
    }




}