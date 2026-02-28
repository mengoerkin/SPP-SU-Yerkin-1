using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#region STRATEGY PATTERN

// poezdka turaly aqparat
public class TravelRequest
{
    public double Distance { get; set; }
    public string ServiceClass { get; set; } // econom / biznes
    public int Passengers { get; set; }
    public bool HasDiscount { get; set; }
    public bool HasLuggage { get; set; }
}

// Stratgia interfeisi
public interface ICostCalculationStrategy
{
    double CalculateCost(TravelRequest request);
}

// Ushaq
public class PlaneStrategy : ICostCalculationStrategy
{
    public double CalculateCost(TravelRequest request)
    {
        double baseCost = request.Distance * 0.5;

        if (request.ServiceClass == "business")
            baseCost *= 2;

        if (request.HasLuggage)
            baseCost += 50;

        if (request.HasDiscount)
            baseCost *= 0.9;

        return baseCost * request.Passengers;
    }
}

// Poezd
public class TrainStrategy : ICostCalculationStrategy
{
    public double CalculateCost(TravelRequest request)
    {
        double baseCost = request.Distance * 0.2;

        if (request.ServiceClass == "business")
            baseCost *= 1.5;

        if (request.HasDiscount)
            baseCost *= 0.85;

        return baseCost * request.Passengers;
    }
}

// Avtobus
public class BusStrategy : ICostCalculationStrategy
{
    public double CalculateCost(TravelRequest request)
    {
        double baseCost = request.Distance * 0.1;

        if (request.HasDiscount)
            baseCost *= 0.8;

        return baseCost * request.Passengers;
    }
}

// Kontekst
public class TravelBookingContext
{
    private ICostCalculationStrategy _strategy;

    public void SetStrategy(ICostCalculationStrategy strategy)
    {
        _strategy = strategy ?? throw new Exception("Стратегия не выбрана!");
    }

    public double Calculate(TravelRequest request)
    {
        if (_strategy == null)
            throw new Exception("Сначала выберите стратегию!");

        if (request.Distance <= 0 || request.Passengers <= 0)
            throw new Exception("Некорректные данные!");

        return _strategy.CalculateCost(request);
    }
}

#endregion

#region OBSERVER PATTERN

// Baqylaushi interfeisi
public interface IObserver
{
    void Update(string stock, double price);
}

// Subekt interfeisi
public interface ISubject
{
    void Subscribe(string stock, IObserver observer);
    void Unsubscribe(string stock, IObserver observer);
    void Notify(string stock, double price);
}

// Birzha
public class StockExchange : ISubject
{
    private Dictionary<string, List<IObserver>> observers = new();
    private Dictionary<string, double> stocks = new();

    public void AddStock(string name, double price)
    {
        stocks[name] = price;
        observers[name] = new List<IObserver>();
    }

    public void Subscribe(string stock, IObserver observer)
    {
        if (observers.ContainsKey(stock))
        {
            observers[stock].Add(observer);
            Console.WriteLine($"[LOG] Подписка на {stock}");
        }
    }

    public void Unsubscribe(string stock, IObserver observer)
    {
        if (observers.ContainsKey(stock))
        {
            observers[stock].Remove(observer);
            Console.WriteLine($"[LOG] Отписка от {stock}");
        }
    }

    public async void ChangePrice(string stock, double newPrice)
    {
        if (!stocks.ContainsKey(stock)) return;

        stocks[stock] = newPrice;

        Console.WriteLine($"\n[Биржа] {stock} новая цена: {newPrice}");

        await NotifyAsync(stock, newPrice);
    }

    public void Notify(string stock, double price)
    {
        if (!observers.ContainsKey(stock)) return;

        foreach (var observer in observers[stock])
        {
            observer.Update(stock, price);
        }
    }

    private async Task NotifyAsync(string stock, double price)
    {
        if (!observers.ContainsKey(stock)) return;

        List<Task> tasks = new();

        foreach (var observer in observers[stock])
        {
            tasks.Add(Task.Run(() => observer.Update(stock, price)));
        }

        await Task.WhenAll(tasks);
    }
}

// Treider
public class Trader : IObserver
{
    private string name;

    public Trader(string name)
    {
        this.name = name;
    }

    public void Update(string stock, double price)
    {
        Console.WriteLine($"Трейдер {name}: {stock} = {price}");
    }
}

// Robot
public class TradingBot : IObserver
{
    private double threshold;

    public TradingBot(double threshold)
    {
        this.threshold = threshold;
    }

    public void Update(string stock, double price)
    {
        if (price > threshold)
            Console.WriteLine($"[BOT] Продаем {stock} по {price}");
        else
            Console.WriteLine($"[BOT] Покупаем {stock} по {price}");
    }
}

#endregion

#region CLIENT

class Program
{
    static void Main()
    {
        Console.WriteLine("=== TRAVEL BOOKING (Strategy) ===");

        var context = new TravelBookingContext();

        var request = new TravelRequest
        {
            Distance = 1000,
            ServiceClass = "business",
            Passengers = 2,
            HasDiscount = true,
            HasLuggage = true
        };

        // strategia tandau
        context.SetStrategy(new PlaneStrategy());
        Console.WriteLine("Самолет: " + context.Calculate(request));

        context.SetStrategy(new TrainStrategy());
        Console.WriteLine("Поезд: " + context.Calculate(request));

        context.SetStrategy(new BusStrategy());
        Console.WriteLine("Автобус: " + context.Calculate(request));

        Console.WriteLine("\n=== STOCK EXCHANGE (Observer) ===");

        var exchange = new StockExchange();

        exchange.AddStock("AAPL", 150);
        exchange.AddStock("GOOG", 2800);

        var trader1 = new Trader("Alice");
        var trader2 = new Trader("Bob");
        var bot = new TradingBot(200);

        exchange.Subscribe("AAPL", trader1);
        exchange.Subscribe("AAPL", bot);
        exchange.Subscribe("GOOG", trader2);

        // baga ozgertu
        exchange.ChangePrice("AAPL", 180);
        exchange.ChangePrice("GOOG", 2900);

        Console.ReadLine();
    }
}

#endregion
