using System;
using System.Collections.Generic;

#region =STRATEGY (Payment)

// strategia interfeisi
public interface IPaymentStrategy
{
    void Pay(double amount);
}

// kartamen tolem
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

// paypal
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

// kriptovaliuta
public class CryptoPayment : IPaymentStrategy
{
    private string wallet;

    public CryptoPayment(string wallet)
    {
        this.wallet = wallet;
    }

    public void Pay(double amount)
    {
        Console.WriteLine($"Оплата {amount}$ криптовалютой (кошелек: {wallet})");
    }
}

// kontekst
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
            throw new Exception("Сначала выберите способ оплаты!");

        if (amount <= 0)
            throw new Exception("Сумма должна быть больше 0!");

        _strategy.Pay(amount);
    }
}

#endregion

#region =OBSERVER (Currency)

// baqylaushi interfeisi
public interface IObserver
{
    void Update(string currency, double rate);
}

// subekt interfeisi
public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify(string currency, double rate);
}

// subekt — valuta birzhasy
public class CurrencyExchange : ISubject
{
    private List<IObserver> observers = new();
    private Dictionary<string, double> rates = new();

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

// koldanushy
public class UserObserver : IObserver
{
    private string name;

    public UserObserver(string name)
    {
        this.name = name;
    }

    public void Update(string currency, double rate)
    {
        Console.WriteLine($"Пользователь {name}: курс {currency} = {rate}");
    }
}

// Bot
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
            Console.WriteLine($"[BOT] Продаем {currency} (курс {rate})");
        else
            Console.WriteLine($"[BOT] Покупаем {currency} (курс {rate})");
    }
}

// analitika
public class AnalyticsObserver : IObserver
{
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Analytics] Обновление: {currency} -> {rate}");
    }
}

#endregion

#region CLIENT

class Program
{
    static void Main()
    {
        Console.WriteLine(" PAYMENT SYSTEM (Strategy)");

        var context = new PaymentContext();

        Console.WriteLine("Выберите способ оплаты: 1-Карта 2-PayPal 3-Крипта");
        int choice = int.Parse(Console.ReadLine());

        switch (choice)
        {
            case 1:
                context.SetStrategy(new CreditCardPayment("1234-5678-9999"));
                break;
            case 2:
                context.SetStrategy(new PayPalPayment("test@mail.com"));
                break;
            case 3:
                context.SetStrategy(new CryptoPayment("0xABC123"));
                break;
            default:
                Console.WriteLine("Неверный выбор!");
                return;
        }

        Console.Write("Введите сумму: ");
        double amount = double.Parse(Console.ReadLine());

        context.ExecutePayment(amount);

        Console.WriteLine("\n CURRENCY EXCHANGE (Observer)");

        var exchange = new CurrencyExchange();

        var user1 = new UserObserver("Alice");
        var user2 = new UserObserver("Bob");
        var bot = new TradingBotObserver(500);
        var analytics = new AnalyticsObserver();

        exchange.Attach(user1);
        exchange.Attach(user2);
        exchange.Attach(bot);
        exchange.Attach(analytics);

        // kurs zhanartu
        exchange.SetRate("USD", 480);
        exchange.SetRate("EUR", 520);

        // bireuin oshiru
        exchange.Detach(user2);

        exchange.SetRate("USD", 550);

        Console.ReadLine();
    }
}

#endregion
