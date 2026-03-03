using System;
using System.Collections.Generic;
using System.Text;

namespace Module06_Combined
{
    // ============================================================
    // ======================== 1) STRATEGY =======================
    // ============================================================

    public enum ServiceClass
    {
        Economy = 1,
        Business = 2
    }

    public enum DiscountType
    {
        None = 0,
        Child = 1,
        Pensioner = 2
    }

    public class TravelRequest
    {
        public double DistanceKm { get; }
        public ServiceClass ServiceClass { get; }
        public DiscountType Discount { get; }
        public int Passengers { get; }
        public int BaggageCount { get; }

        public TravelRequest(double distanceKm, ServiceClass serviceClass, DiscountType discount,
                             int passengers, int baggageCount)
        {
            if (distanceKm <= 0) throw new ArgumentException("Кашыктык 0-ден улкен болуы керек.");
            if (passengers <= 0) throw new ArgumentException("Жолаушылар саны 0-ден улкен болуы керек.");
            if (baggageCount < 0) throw new ArgumentException("Багаж саны терыс болмауы керек.");

            DistanceKm = distanceKm;
            ServiceClass = serviceClass;
            Discount = discount;
            Passengers = passengers;
            BaggageCount = baggageCount;
        }
    }

    public interface ICostCalculationStrategy
    {
        string Name { get; }
        decimal Calculate(TravelRequest request);
    }

    public abstract class BaseStrategy : ICostCalculationStrategy
    {
        public abstract string Name { get; }
        public abstract decimal CalculateBase(TravelRequest request);

        public decimal Calculate(TravelRequest request)
        {
            decimal cost = CalculateBase(request);

            cost *= request.ServiceClass switch
            {
                ServiceClass.Business => 1.8m,
                _ => 1.0m
            };

            cost *= request.Passengers;

            cost *= request.Discount switch
            {
                DiscountType.Child => 0.70m,
                DiscountType.Pensioner => 0.80m,
                _ => 1.0m
            };

            return Math.Round(cost, 2);
        }
    }

    public class AirplaneStrategy : BaseStrategy
    {
        public override string Name => "Самолет";
        public override decimal CalculateBase(TravelRequest request)
        {
            decimal baseCost = (decimal)request.DistanceKm * 0.50m;
            baseCost += request.BaggageCount * 20m;
            baseCost *= 1.12m; 
            return baseCost;
        }
    }

    public class TrainStrategy : BaseStrategy
    {
        public override string Name => "Поезд";
        public override decimal CalculateBase(TravelRequest request)
        {
            decimal baseCost = (decimal)request.DistanceKm * 0.30m;
            baseCost += request.BaggageCount * 10m;
            return baseCost;
        }
    }

    public class BusStrategy : BaseStrategy
    {
        public override string Name => "Автобус";
        public override decimal CalculateBase(TravelRequest request)
        {
            decimal baseCost = (decimal)request.DistanceKm * 0.20m;
            baseCost += request.BaggageCount * 5m;
            return baseCost;
        }
    }

    public class TravelBookingContext
    {
        private ICostCalculationStrategy? _strategy;

        public void SetStrategy(ICostCalculationStrategy strategy)
            => _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));

        public decimal Calculate(TravelRequest request)
        {
            if (_strategy == null) throw new InvalidOperationException("Стратегия тандалмаган!");
            return _strategy.Calculate(request);
        }

        public string CurrentStrategyName => _strategy?.Name ?? "None";
    }

    // ============================================================
    // ======================= 2) OBSERVER ========================
    // ============================================================

    public interface IObserver
    {
        void Update(string stockName, decimal newPrice);
    }

    public interface ISubject
    {
        void Subscribe(string stockName, IObserver observer);
        void Unsubscribe(string stockName, IObserver observer);
        void Notify(string stockName);
    }

    public class StockExchange : ISubject
    {
        private readonly Dictionary<string, decimal> _prices = new();
        private readonly Dictionary<string, List<IObserver>> _subscribers = new();

        public void AddStock(string name, decimal initialPrice)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Акция аты бос болмауы керек.");
            if (initialPrice <= 0) throw new ArgumentException("Бастапкы бага 0-ден улкен болуы керек.");

            _prices[name] = initialPrice;
            if (!_subscribers.ContainsKey(name))
                _subscribers[name] = new List<IObserver>();
        }

        public void UpdatePrice(string name, decimal newPrice)
        {
            if (!_prices.ContainsKey(name)) throw new KeyNotFoundException("Мундай акция жок: " + name);
            if (newPrice <= 0) throw new ArgumentException("Бага 0-ден улкен болуы керек.");

            _prices[name] = newPrice;
            Console.WriteLine($"[Биржа] {name} жана багасы: {newPrice}");
            Notify(name);
        }

        public decimal GetPrice(string name) => _prices[name];

        public void Subscribe(string stockName, IObserver observer)
        {
            if (!_subscribers.ContainsKey(stockName))
                throw new KeyNotFoundException("Подписка ушын акция табылмады: " + stockName);

            if (!_subscribers[stockName].Contains(observer))
                _subscribers[stockName].Add(observer);

            Console.WriteLine($"[Биржа] Подписка косылды: {observer.GetType().Name} -> {stockName}");
        }

        public void Unsubscribe(string stockName, IObserver observer)
        {
            if (_subscribers.ContainsKey(stockName) && _subscribers[stockName].Remove(observer))
                Console.WriteLine($"[Биржа] Подписка өшырылды: {observer.GetType().Name} -> {stockName}");
        }

        public void Notify(string stockName)
        {
            if (!_subscribers.ContainsKey(stockName)) return;

            var price = _prices[stockName];
            foreach (var obs in _subscribers[stockName])
                obs.Update(stockName, price);
        }
    }

    public class Trader : IObserver
    {
        private readonly string _name;
        public Trader(string name) => _name = name;

        public void Update(string stockName, decimal newPrice)
        {
            Console.WriteLine($"[Трейдер {_name}] Уведомление: {stockName} = {newPrice}");
        }
    }

    public class TradingRobot : IObserver
    {
        private readonly decimal _buyBelow;
        private readonly decimal _sellAbove;

        public TradingRobot(decimal buyBelow, decimal sellAbove)
        {
            if (buyBelow <= 0 || sellAbove <= 0) throw new ArgumentException("Шектер 0-ден үлкен болуы керек.");
            if (buyBelow >= sellAbove) throw new ArgumentException("buyBelow < sellAbove болуы керек.");
            _buyBelow = buyBelow;
            _sellAbove = sellAbove;
        }

        public void Update(string stockName, decimal newPrice)
        {
            if (newPrice <= _buyBelow)
                Console.WriteLine($"[Робот] BUY сигнал: {stockName} ({newPrice})");
            else if (newPrice >= _sellAbove)
                Console.WriteLine($"[Робот] SELL сигнал: {stockName} ({newPrice})");
            else
                Console.WriteLine($"[Робот] HOLD: {stockName} ({newPrice})");
        }
    }

    // ============================================================
    // =========================== DEMOS ==========================
    // ============================================================

    public static class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            while (true)
            {
                Console.WriteLine("\n=== Модуль 06 (Біріктірілген) ===");
                Console.WriteLine("1) Travel Booking (Strategy)");
                Console.WriteLine("2) Stock Exchange (Observer)");
                Console.WriteLine("0) Exit");
                Console.Write("Таңдау: ");

                var input = Console.ReadLine();
                Console.WriteLine();

                if (input == "0") break;
                if (input == "1") RunTravelBookingDemo();
                else if (input == "2") RunStockExchangeDemo();
                else Console.WriteLine("Қате таңдау.");
            }
        }

        private static void RunTravelBookingDemo()
        {
            var context = new TravelBookingContext();

            Console.WriteLine("=== Travel Booking (Strategy) ===");
            Console.WriteLine("Транспорт таңда: 1-Самолет, 2-Поезд, 3-Автобус");
            int transport = ReadInt("Транспорт: ", min: 1, max: 3);

            ICostCalculationStrategy strategy = transport switch
            {
                1 => new AirplaneStrategy(),
                2 => new TrainStrategy(),
                _ => new BusStrategy()
            };
            context.SetStrategy(strategy);

            double dist = ReadDouble("Қашықтық (км): ", min: 0.0001);
            int passengers = ReadInt("Жолаушылар саны: ", min: 1, max: 1000);
            int baggage = ReadInt("Багаж саны: ", min: 0, max: 1000);

            Console.WriteLine("Класс: 1-Economy, 2-Business");
            int cls = ReadInt("Класс: ", min: 1, max: 2);

            Console.WriteLine("Жеңілдік: 0-Жоқ, 1-Бала, 2-Зейнеткер");
            int disc = ReadInt("Жеңілдік: ", min: 0, max: 2);

            try
            {
                var request = new TravelRequest(
                    distanceKm: dist,
                    serviceClass: (ServiceClass)cls,
                    discount: (DiscountType)disc,
                    passengers: passengers,
                    baggageCount: baggage
                );

                var total = context.Calculate(request);

                Console.WriteLine($"\nСтратегия: {context.CurrentStrategyName}");
                Console.WriteLine($"Итоговая стоимость: {total}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Қате: " + ex.Message);
            }
        }

        private static void RunStockExchangeDemo()
        {
            Console.WriteLine("=== Stock Exchange (Observer) ===");

            var exchange = new StockExchange();
            exchange.AddStock("AAPL", 150m);
            exchange.AddStock("GOOG", 2800m);
            exchange.AddStock("TSLA", 700m);

            var trader = new Trader("Alex");
            var robot = new TradingRobot(buyBelow: 160m, sellAbove: 220m);

            exchange.Subscribe("AAPL", trader);
            exchange.Subscribe("AAPL", robot);

            Console.WriteLine("\nБағаларды өзгерту (демо):");
            exchange.UpdatePrice("AAPL", 210m);
            exchange.UpdatePrice("AAPL", 155m);

            exchange.Unsubscribe("AAPL", trader);
            Console.WriteLine("\nТрейдерді алып тастадық, енді тек робот алады:");
            exchange.UpdatePrice("AAPL", 230m);

            Console.WriteLine("\n(Демо бітті)");
        }

        private static int ReadInt(string label, int min, int max)
        {
            while (true)
            {
                Console.Write(label);
                if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max)
                    return v;
                Console.WriteLine($"Қате енгізу. [{min}..{max}] аралығы керек.");
            }
        }

        private static double ReadDouble(string label, double min)
        {
            while (true)
            {
                Console.Write(label);
                if (double.TryParse(Console.ReadLine(), out double v) && v >= min)
                    return v;
                Console.WriteLine($"Қате енгізу. >= {min} болуы керек.");
            }
        }
    }
}
