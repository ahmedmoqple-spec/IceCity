using System;
using System.Collections.Generic;
using System.Linq;

namespace IceCity
{
    class Program
    {
        static void Main(string[] args)
        {
            CalculationService calcService = new CalculationService();
            ReportService reportService = new ReportService();

            Console.WriteLine("--- IceCity OOP Heating System ---");

            Console.Write("Enter Owner Name: ");
            string name = Console.ReadLine();
            Owner owner = new Owner(name);

            House house = new House(owner, calcService);

            int daysInMonth = 30;

            for (int i = 1; i <= daysInMonth; i++)
            {
                Console.WriteLine($"\n--- Day {i} Data ---");

                Console.Write("Enter Heater Power: ");
                double power = double.Parse(Console.ReadLine());

                HeaterBase heater;
                if (i % 2 == 0)
                    heater = new ElectricHeater(power);
                else
                    heater = new GasHeater(power);

                house.AddHeater(heater);

                Console.Write("Enter Working Hours (0-24): ");
                double hours = double.Parse(Console.ReadLine());

                DailyUsage usage = new DailyUsage(i, hours, power);
                house.AddDailyUsage(usage);
            }

            string resultReport = house.GenerateHouseReport(reportService);

            Console.WriteLine(resultReport);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    public class Owner
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Owner(string name)
        {
            _name = name;
        }
    }

    public class DailyUsage
    {
        private int _day;
        private double _hours;
        private double _powerUsed;

        public double Hours
        {
            get { return _hours; }
            set
            {
                if (value < 0 || value > 24)
                {
                    Console.WriteLine("Warning: Invalid hours. Setting to 0.");
                    _hours = 0;
                }
                else
                {
                    _hours = value;
                }
            }
        }

        public double PowerUsed { get { return _powerUsed; } }

        public DailyUsage(int day, double hours, double power)
        {
            _day = day;
            _powerUsed = power;
            Hours = hours;
        }
    }

    public abstract class HeaterBase
    {
        protected double _power;

        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }

        public HeaterBase(double power)
        {
            _power = power;
        }

        public abstract double GetEffectivePower();
    }

    public class ElectricHeater : HeaterBase
    {
        public ElectricHeater(double power) : base(power) { }

        public override double GetEffectivePower()
        {
            return _power;
        }
    }

    public class GasHeater : HeaterBase
    {
        public GasHeater(double power) : base(power) { }

        public override double GetEffectivePower()
        {
            return _power;
        }
    }

    public class House
    {
        private Owner _owner;
        private List<HeaterBase> _heaters;
        private List<DailyUsage> _usageHistory;
        private CalculationService _calcService;

        public House(Owner owner, CalculationService service)
        {
            _owner = owner;
            _calcService = service;
            _heaters = new List<HeaterBase>();
            _usageHistory = new List<DailyUsage>();
        }

        public void AddHeater(HeaterBase heater)
        {
            _heaters.Add(heater);
        }

        public void AddDailyUsage(DailyUsage usage)
        {
            _usageHistory.Add(usage);
        }

        public string GenerateHouseReport(ReportService reporter)
        {
            List<double> powerValues = new List<double>();
            foreach (var heater in _heaters)
            {
                powerValues.Add(heater.GetEffectivePower());
            }

            double totalHours = _calcService.CalculateTotalWorkingTime(_usageHistory);
            double medianPower = _calcService.CalculateMedianHeaterValue(powerValues);
            double cost = _calcService.CalculateMonthlyAverageCost(medianPower, totalHours);

            return reporter.FormatReport(_owner.Name, cost);
        }
    }

    public class CalculationService
    {
        public double CalculateTotalWorkingTime(List<DailyUsage> usageList)
        {
            double sum = 0;
            foreach (var item in usageList)
            {
                sum += item.Hours;
            }
            return sum;
        }

        public double CalculateMedianHeaterValue(List<double> values)
        {
            if (values.Count == 0) return 0;

            values.Sort();

            int count = values.Count;
            int middleIndex = count / 2;
            return values[middleIndex];
        }

        public double CalculateMonthlyAverageCost(double median, double totalHours)
        {
            return median * (totalHours / (24 * 30));
        }
    }

    public class ReportService
    {
        public string FormatReport(string ownerName, double cost)
        {
            return $"\n---------------------------\n" +
                   $"OOP Report for Owner: {ownerName}\n" +
                   $"Total Cost: {cost:F2}\n" +
                   $"---------------------------";
        }
    }
}
