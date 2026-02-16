using System;
using System.Collections.Generic;

namespace IceCity
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- IceCity Heating Calculator ---");
            Console.WriteLine("--- Developed by: Ahmed Mokbel ---");
            Console.WriteLine();

            Console.Write("Enter Owner Name: ");
            string ownerName = Console.ReadLine();

            List<double> heaterValues = new List<double>();
            List<double> dailyHours = new List<double>();

            int daysInMonth = 30;

            for (int i = 1; i <= daysInMonth; i++)
            {
                Console.WriteLine("\nDay " + i + ":");

                Console.Write("Enter Heater Power Value: ");
                double power = double.Parse(Console.ReadLine());
                heaterValues.Add(power);

                Console.Write("Enter Working Hours (0-24): ");
                double hours = double.Parse(Console.ReadLine());

                while(hours < 0 || hours > 24)
                {
                    Console.WriteLine("Invalid input. Hours must be between 0 and 24.");
                    Console.Write("Enter Working Hours (0-24): ");
                    hours = double.Parse(Console.ReadLine());
                }
                dailyHours.Add(hours);
            }

            double totalHours = CalculateTotalHours(dailyHours);
            double medianPower = GetMedianValue(heaterValues);
            double monthlyCost = CalculateAverageCost(medianPower, totalHours);

            Console.WriteLine("\n---------------------------");
            Console.WriteLine("Report for Owner: " + ownerName);
            Console.WriteLine("Total Cost: " + monthlyCost);
            Console.WriteLine("---------------------------");

            Console.ReadKey();
        }

        static double CalculateTotalHours(List<double> hoursList)
        {
            double sum = 0;
            foreach (double h in hoursList)
            {
                sum += h;
            }
            return sum;
        }

        static double GetMedianValue(List<double> valuesList)
        {
            List<double> sortedList = new List<double>(valuesList);
            int count = sortedList.Count;

            for (int i = 0; i < count - 1; i++)
            {
                for (int j = 0; j < count - i - 1; j++)
                {
                    if (sortedList[j] > sortedList[j + 1])
                    {
                        double temp = sortedList[j];
                        sortedList[j] = sortedList[j + 1];
                        sortedList[j + 1] = temp;
                    }
                }
            }

            int middleIndex = count / 2;
            return sortedList[middleIndex];
        }

        static double CalculateAverageCost(double medianValue, double totalWorkTime)
        {
            double result = medianValue * (totalWorkTime / (24 * 30));
            return result;
        }
    }
}
