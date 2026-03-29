using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

public interface ICostCalculationStrategy
{
    double CalculateMedian(List<DailyUsage> usages);
    double CalculateTotalHours(List<DailyUsage> usages);
    double CalculateCost(List<DailyUsage> usages);
}

public class StandardCostStrategy : ICostCalculationStrategy
{
    public double CalculateMedian(List<DailyUsage> usages)
    {
        if (usages.Count == 0) return 0;
        var sorted = usages.Select(u => u.HeaterValue).OrderBy(v => v).ToList();
        int count = sorted.Count;
        return count % 2 == 0 ? (sorted[(count / 2) - 1] + sorted[count / 2]) / 2.0 : sorted[count / 2];
    }

    public double CalculateTotalHours(List<DailyUsage> usages)
    {
        return usages.Sum(u => u.HoursWorked);
    }

    public double CalculateCost(List<DailyUsage> usages)
    {
        double median = CalculateMedian(usages);
        double totalHours = CalculateTotalHours(usages);
        return median * (totalHours / (24.0 * 30.0));
    }
}

public class EcoCostStrategy : ICostCalculationStrategy
{
    private readonly StandardCostStrategy _baseStrategy = new StandardCostStrategy();

    public double CalculateMedian(List<DailyUsage> usages) => _baseStrategy.CalculateMedian(usages);
    
    public double CalculateTotalHours(List<DailyUsage> usages) => _baseStrategy.CalculateTotalHours(usages);

    public double CalculateCost(List<DailyUsage> usages)
    {
        double baseCost = _baseStrategy.CalculateCost(usages);
        double totalHours = CalculateTotalHours(usages);
        
        if (totalHours < 120)
        {
            return baseCost * 0.9; 
        }
        return baseCost;
    }
}
