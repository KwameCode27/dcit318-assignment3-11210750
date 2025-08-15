namespace GradingSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    // Custom exception for invalid scores
    public class InvalidScoreException : Exception
    {
        public InvalidScoreException(string message) : base(message) { }
    }

    // Custom exception for missing fields
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public string GetGrade()
        {
            if (Score >= 80)
            {
                return "A";
            }
            else if (Score >= 70)
            {
                return "B";
            }
            else if (Score >= 60)
            {
                return "C";
            }
            else if (Score >= 50)
            {
                return "D";
            }
            else
            {
                return "F";
            }
        }
    }

    public class StudentResultProcessor
    {
        public List<Student> Students { get; private set; } = new List<Student>();

        public List<Student> ReadStudentFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            Console.WriteLine($"Reading student data from {filePath}");

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length < 3)
                    {
                        throw new MissingFieldException("Each line must contain three fields: Id, FullName, and Score.");
                    }
                    if (!int.TryParse(parts[0], out int id))
                    {
                        throw new InvalidScoreException($"Invalid Id: {parts[0]}");
                    }
                    var fullName = parts[1];
                    if (string.IsNullOrEmpty(fullName))
                    {
                        throw new MissingFieldException("FullName cannot be empty.");
                    }
                    if (!int.TryParse(parts[2], out int score) || score < 0 || score > 100)
                    {
                        throw new InvalidScoreException($"Invalid Score: {parts[2]}");
                    }
                    var student = new Student
                    {
                        Id = id,
                        FullName = fullName,
                        Score = score
                    };
                    Students.Add(student);
                }
            }
            return Students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            if (students == null || students.Count == 0)
            {
                throw new ArgumentException("Student list cannot be null or empty.", nameof(students));
            }
            if (string.IsNullOrEmpty(outputFilePath))
            {
                throw new ArgumentException("Output file path cannot be null or empty.", nameof(outputFilePath));
            }
            Console.WriteLine($"Writing report to {outputFilePath}");
            using (var writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.Id},{student.FullName},{student.Score},{student.GetGrade()}");
                    Console.WriteLine($"{student.FullName} (ID:{student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var processor = new StudentResultProcessor();
                var students = processor.ReadStudentFromFile("C:\\Users\\fente\\Desktop\\dcit318-assignment3-11210750\\GradeSystem\\student_report.txt");
                processor.WriteReportToFile(students, "C:\\Users\\fente\\Desktop\\dcit318-assignment3-11210750\\GradeSystem\\student_report.txt");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found. {ex.Message}");
            }
            
            catch (InvalidScoreException ex)
            {
                Console.WriteLine($"Error: Invalid score. {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: Missing field. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }

}