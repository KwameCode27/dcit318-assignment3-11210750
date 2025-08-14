﻿using System;

public class Repository<T>
{
    public List<T> Items { get; set; }

    public void Add(T item)
    {
        if (Items == null)
        {
            Items = new List<T>();
        }
        Items.Add(item);
    }


    public List<T> GetAll()
    {
        return Items ?? new List<T>();
    }

    public T? GetById(Func<T, bool> predicate)
    {
        if (Items == null)
        {
            return default;
        }
        return Items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        if (Items == null)
        {
            return false;
        }
        var itemToRemove = Items.FirstOrDefault(predicate);
        if (itemToRemove != null)
        {
            Items.Remove(itemToRemove);
            return true;
        }
        return false;
    }
}


public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }


    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

public class Prescription 
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string MedicationName { get; set; }

    public DateTime DateIssued { get; set; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

}

public class HealthSystemApp
{
    Repository<Patient> _patientRepo = new Repository<Patient>();
    Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    Dictionary<int, List<Prescription>> _patientPrescriptions = new Dictionary<int, List<Prescription>>();

    public void seedData()
    {
        _patientRepo.Add(new Patient(1, "MICHAEL", 20, "MALE"));
        _patientRepo.Add(new Patient(2, "ROSEMARY", 25, "FEMALE"));
        _patientRepo.Add(new Patient(3, "GABRIEL", 30, "MALE"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Medication A", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(2, 2, "Medication B", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(3, 3, "Medication C", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(4, 1, "Medication D", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(5, 2, "Medication E", DateTime.Now));

    }

    public void BuildPrescriptionMap()
    {
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_patientPrescriptions.ContainsKey(prescription.PatientId))
            {
                _patientPrescriptions[prescription.PatientId] = new List<Prescription>();
            }
            _patientPrescriptions[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"Id: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public void PrintAllPrescriptions()
    {
        Console.WriteLine("Prescriptions:");
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            Console.WriteLine($"Id: {prescription.Id}, PatientId: {prescription.PatientId}, Medication: {prescription.MedicationName}, DateIssued: {prescription.DateIssued}");
        }

    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        if (_patientPrescriptions.TryGetValue(patientId, out var prescriptions))
        {
            Console.WriteLine($"Prescriptions for Patient Id {patientId}:");
            foreach (var prescription in prescriptions)
            {
                Console.WriteLine($"Id: {prescription.Id}, Medication: {prescription.MedicationName}, DateIssued: {prescription.DateIssued}");
            }
        }
        else
        {
            Console.WriteLine($"No prescriptions found for Patient Id {patientId}.");
        }
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        HealthSystemApp app = new HealthSystemApp();
        app.seedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintAllPrescriptions();
        Console.WriteLine();
        app.PrintPrescriptionsForPatient(1);
        Console.WriteLine();
        app.PrintPrescriptionsForPatient(2);
        Console.WriteLine();
        app.PrintPrescriptionsForPatient(3);
    }
}