using System;
using System.Collections.Generic;

#nullable enable

public interface ICostStrategyFactory
{
    ICostCalculationStrategy GetStrategy(string type);
}

public class CostStrategyFactory : ICostStrategyFactory
{
    public ICostCalculationStrategy GetStrategy(string type)
    {
        if (type.Equals("Eco", StringComparison.OrdinalIgnoreCase))
        {
            return new EcoCostStrategy();
        }
        return new StandardCostStrategy(); 
    }
}

public interface ICostService
{
    double CalculateHouseCost(House house, string strategyType);
}

public class CostService : ICostService
{
    private readonly ICostStrategyFactory _factory;

    public CostService(ICostStrategyFactory factory)
    {
        _factory = factory;
    }

    public double CalculateHouseCost(House house, string strategyType)
    {
        ICostCalculationStrategy strategy = _factory.GetStrategy(strategyType);
        return strategy.CalculateCost(house.DailyUsages);
    }
}
