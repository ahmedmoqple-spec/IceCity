using System;
using System.Threading.Tasks;

#nullable enable

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- IceCity: Week 4 SOLID & Design Patterns ---\n");

        var house = new House();
        house.Heaters.Add(new ElectricHeater { PowerKw = 2000 });
        house.Heaters.Add(new SolarHeater { PowerKw = 1500 }); 

        house.DailyUsages.Add(new DailyUsage { Date = DateTime.UtcNow, HoursWorked = 50, HeaterValue = 2000 });
        house.DailyUsages.Add(new DailyUsage { Date = DateTime.UtcNow.AddDays(-1), HoursWorked = 60, HeaterValue = 1800 });

        ICostStrategyFactory factory = new CostStrategyFactory();
        ICostService costService = new CostService(factory);

        double standardCost = costService.CalculateHouseCost(house, "Standard");
        Console.WriteLine($"Cost using Standard Strategy: {standardCost:F2}");

        double ecoCost = costService.CalculateHouseCost(house, "Eco");
        Console.WriteLine($"Cost using Eco Strategy: {ecoCost:F2}");

        Console.WriteLine("\nAll Design Patterns and SOLID Principles applied successfully!");
    }
}
