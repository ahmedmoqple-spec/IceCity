using System;
using System.Threading.Tasks;

#nullable enable

class Program
{
    static async Task Main(string[] args)
    {
        var house = new House();
        house.Heaters.Add(new ElectricHeater { PowerKw = 1500 });

        SaveDailyUsageDelegate saveDelegate = usage => house.DailyUsages.Add(usage);

        var heater = house.Heaters[0];
        if (heater != null)
        {
            heater.OpenHeater += (s, e) => Console.WriteLine($"[Event] Heater {((HeaterBase)s!).Id} Opened.");
            heater.CloseHeater += (s, e) => 
            {
                var h = (HeaterBase)s!;
                var usage = new DailyUsage { Date = DateTime.UtcNow.Date, HoursWorked = e.DurationHours, HeaterValue = h.PowerKw };
                saveDelegate(usage);
                Console.WriteLine($"[Event] Heater Closed. Usage saved!");
            };
        }

        try
        {
            if (house.Heaters[0] != null)
            {
                house.Heaters[0]!.Open();
                throw new HeaterFailedException("Heater exploded!"); 
            }
        }
        catch (HeaterFailedException ex)
        {
            Console.WriteLine($"\n[Error] {ex.Message}");
            var newHeater = await CityCenterService.RequestReplacementAsync(house.HouseId, house.Heaters[0]!.Id);
            house.Heaters[0] = null;
            house.Heaters[0] = newHeater;
        }

        if (house.Heaters[0] != null)
        {
            house.Heaters[0]!.Close();
        }

        var weatherService = new WeatherService();
        var apiData = await weatherService.GetLastMonthWeatherAsync();
        house.DailyUsages.AddRange(apiData);

        var printService = new PrintingService();
        printService.PrintLastMonthDailyUsageWithThreads(house);
        await printService.PrintLastMonthDailyUsageWithTasks(house);

        var service1 = new Service1();
        double median = service1.CalculateMedianHeaterValue(house.DailyUsages);
        Console.WriteLine($"\nMedian Heater Value: {median}");
    }
}
