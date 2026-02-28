using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

public class Service1
{
    public double CalculateMedianHeaterValue(List<DailyUsage> usages)
    {
        if (usages.Count == 0) return 0;
        var sorted = usages.Select(u => u.HeaterValue).OrderBy(v => v).ToList();
        int count = sorted.Count;

        if (count.IsEven()) 
            return (sorted[(count / 2) - 1] + sorted[count / 2]) / 2.0;
        else 
            return sorted[count / 2];
    }
}

public class CityCenterService
{
    public static async Task<HeaterBase?> RequestReplacementAsync(string houseId, string heaterId)
    {
        Console.WriteLine($"[City Center] Replacing heater {heaterId} for house {houseId}...");
        await Task.Delay(500);
        return new ElectricHeater { PowerKw = 2000 };
    }
}

public class WeatherService
{
    public async Task<List<DailyUsage>> GetLastMonthWeatherAsync()
    {
        var usages = new List<DailyUsage>();
        using var httpClient = new HttpClient();
        
        DateTime now = DateTime.UtcNow;
        DateTime start = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
        DateTime end = new DateTime(now.Year, now.Month, 1).AddDays(-1);

        string url = $"https://archive-api.open-meteo.com/v1/archive?latitude=31.0409&longitude=31.3785&start_date={start:yyyy-MM-dd}&end_date={end:yyyy-MM-dd}&daily=temperature_2m_max,temperature_2m_min,precipitation_sum";

        var response = await httpClient.GetStringAsync(url);
        using var json = JsonDocument.Parse(response);
        var daily = json.RootElement.GetProperty("daily");
        
        var dates = daily.GetProperty("time").EnumerateArray();
        var maxTemps = daily.GetProperty("temperature_2m_max").EnumerateArray();
        var minTemps = daily.GetProperty("temperature_2m_min").EnumerateArray();
        var rain = daily.GetProperty("precipitation_sum").EnumerateArray();

        while (dates.MoveNext() && maxTemps.MoveNext() && minTemps.MoveNext() && rain.MoveNext())
        {
            usages.Add(new DailyUsage
            {
                Date = DateTime.Parse(dates.Current.GetString()!),
                HeaterValue = maxTemps.Current.GetDouble(),
                HoursWorked = 0 
            });
        }
        return usages;
    }
}

public class PrintingService
{
    public void PrintLastMonthDailyUsageWithThreads(House house)
    {
        Console.WriteLine("\n--- Threads Printing ---");
        var t1 = new Thread(() => PrintUsageWithThreadId(house.DailyUsages));
        var t2 = new Thread(() => PrintUsageWithThreadId(house.DailyUsages));
        t1.Start(); t2.Start();
        t1.Join(); t2.Join();
    }

    public async Task PrintLastMonthDailyUsageWithTasks(House house)
    {
        Console.WriteLine("\n--- Tasks Printing ---");
        var tasks = new[]
        {
            Task.Run(() => PrintUsageWithTaskId(house.DailyUsages)),
            Task.Run(() => PrintUsageWithTaskId(house.DailyUsages))
        };
        await Task.WhenAll(tasks);
    }

    private void PrintUsageWithThreadId(IEnumerable<DailyUsage> usages)
    {
        foreach (var u in usages)
            Console.WriteLine($"{u.Date:yyyy-MM-dd} | Val={u.HeaterValue} | Hrs={u.HoursWorked} | Thread={Thread.CurrentThread.ManagedThreadId}");
    }

    private void PrintUsageWithTaskId(IEnumerable<DailyUsage> usages)
    {
        foreach (var u in usages)
            Console.WriteLine($"{u.Date:yyyy-MM-dd} | Val={u.HeaterValue} | Hrs={u.HoursWorked} | Thread={Thread.CurrentThread.ManagedThreadId} | Task={Task.CurrentId}");
    }
}
