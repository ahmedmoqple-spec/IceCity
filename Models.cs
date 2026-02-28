using System;
using System.Collections.Generic;

#nullable enable

public static class IntExtensions
{
    public static bool IsEven(this int number) => number % 2 == 0;
}

public class HeaterFailedException : Exception
{
    public HeaterFailedException(string message) : base(message) { }
}

public delegate void SaveDailyUsageDelegate(DailyUsage usage);
public delegate void HeaterEventHandler(object sender, EventArgs e);
public delegate void HeaterDurationHandler(object sender, HeaterDurationEventArgs e);

public class HeaterDurationEventArgs : EventArgs
{
    public double DurationHours { get; set; }
}

public class DailyUsage
{
    public DateTime Date { get; set; }
    public double HoursWorked { get; set; }
    public double HeaterValue { get; set; }
}

public abstract class HeaterBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public double PowerKw { get; set; }
    
    internal DateTime? _lastOpenTime;

    public event HeaterEventHandler? OpenHeater;
    public event HeaterDurationHandler? CloseHeater;

    public abstract double GetEffectivePower();

    public void Open()
    {
        _lastOpenTime = DateTime.UtcNow;
        OpenHeater?.Invoke(this, EventArgs.Empty);
    }

    public void Close()
    {
        if (_lastOpenTime == null) return;
        
        double hours = (DateTime.UtcNow - _lastOpenTime.Value).TotalHours;
        
        if (hours < 0.001) hours = 5.5; 

        _lastOpenTime = null;
        CloseHeater?.Invoke(this, new HeaterDurationEventArgs { DurationHours = hours });
    }
}

public class ElectricHeater : HeaterBase
{
    public override double GetEffectivePower() => PowerKw * 0.95;
}

public class GasHeater : HeaterBase
{
    public override double GetEffectivePower() => PowerKw * 0.85;
}

public class House
{
    public string HouseId { get; set; } = Guid.NewGuid().ToString();
    public List<HeaterBase?> Heaters { get; set; } = new List<HeaterBase?>();
    public List<DailyUsage> DailyUsages { get; set; } = new List<DailyUsage>();
}
