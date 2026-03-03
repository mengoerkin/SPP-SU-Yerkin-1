using System;
using System.Collections.Generic;

#region STRATEGY

public interface IPaymentStrategy
{
    void Pay(double amount);
}

public class CreditCardPayment : IPaymentStrategy
{
    private string cardNumber;

    public CreditCardPayment(string cardNumber)
    {
        this.cardNumber = cardNumber;
    }

    public void Pay(double amount)
    {
        Console.WriteLine($"Оплата {amount}$ картой {cardNumber}");
    }
}

public class PayPalPayment : IPaymentStrategy
{
    private string email;

    public PayPalPayment(string email)
    {
        this.email = email;
    }

    public void Pay(double amount)
    {
        Console.WriteLine($"Оплата {amount}$ через PayPal ({email})");
    }
}

public class CryptoPayment : IPaymentStrategy
{
    private string wallet;

    public CryptoPayment(string wallet)
    {
        this.wallet = wallet;
    }

    public void Pay(double amount)
    {
        Console.WriteLine($"Оплата {amount}$ криптовалютой ({wallet})");
    }
}

public class PaymentContext
{
    private IPaymentStrategy _strategy;

    public void SetStrategy(IPaymentStrategy strategy)
    {
        _strategy = strategy ?? throw new Exception("Стратегия не выбрана!");
    }

    public void ExecutePayment(double amount)
    {
        if (_strategy == null)
            throw new Exception("Сначала выберите оплату!");

        _strategy.Pay(amount);
    }
}

#endregion

#region OBSERVER + SINGLETON

public interface IObserver
{
    void Update(string currency, double rate);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string currency, double rate);
}

// Singleton
public class CurrencyExchange : ISubject
{
    private static CurrencyExchange _instance;

    private List<IObserver> observers = new();
    private Dictionary<string, double> rates = new();

    // Приватный конструктор
    private CurrencyExchange() {}

    // Единственная точка доступа
    public static CurrencyExchange Instance
    {
        get
        {
            if (_instance == null)
                _instance = new CurrencyExchange();
            return _instance;
        }
    }

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
        Console.WriteLine("[LOG] Подписчик добавлен");
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
        Console.WriteLine("[LOG] Подписчик удален");
    }

    public void SetRate(string currency, double rate)
    {
        rates[currency] = rate;
        Console.WriteLine($"\n[Биржа] {currency} = {rate}");
        Notify(currency, rate);
    }

    public void Notify(string currency, double rate)
    {
        foreach (var observer in observers)
        {
            observer.Update(currency, rate);
        }
    }
}

// Наблюдатели

public class UserObserver : IObserver
{
    private string name;

    public UserObserver(string name)
    {
        this.name = name;
    }

    public void Update(string currency, double rate)
    {
        Console.WriteLine($"Пользователь {name}: {currency} = {rate}");
    }
}

public class TradingBotObserver : IObserver
{
    private double threshold;

    public TradingBotObserver(double threshold)
    {
        this.threshold = threshold;
    }

    public void Update(string currency, double rate)
    {
        if (rate > threshold)
            Console.WriteLine($"[BOT] Продаем {currency}");
        else
            Console.WriteLine($"[BOT] Покупаем {currency}");
    }
}

public class AnalyticsObserver : IObserver
{
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Analytics] {currency} обновлен до {rate}");
    }
}

#endregion

#region ================= CLIENT =================

class Program
{
    static void Main()
    {
        Console.WriteLine("=== PAYMENT (Strategy) ===");

        var context = new PaymentContext();

        context.SetStrategy(new CreditCardPayment("1234"));
        context.ExecutePayment(100);

        context.SetStrategy(new PayPalPayment("mail@test.com"));
        context.ExecutePayment(200);

        context.SetStrategy(new CryptoPayment("0xABC"));
        context.ExecutePayment(300);

        Console.WriteLine("\n=== EXCHANGE (Observer + Singleton) ===");

        // Singleton қолдану
        var exchange = CurrencyExchange.Instance;

        var user = new UserObserver("Alice");
        var bot = new TradingBotObserver(500);
        var analytics = new AnalyticsObserver();

        exchange.Attach(user);
        exchange.Attach(bot);
        exchange.Attach(analytics);

        exchange.SetRate("USD", 480);
        exchange.SetRate("EUR", 550);

        Console.ReadLine();
    }
}

#endregion
