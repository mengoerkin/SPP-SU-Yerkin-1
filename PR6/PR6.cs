using System;
using System.Collections.Generic;

#region STRATEGY

// Интерфейс стратегии (контракт для всех способов оплаты)
public interface IPaymentStrategy
{
    void Pay(double amount); // Любая стратегия должна уметь платить
}

// Оплата картой
public class CreditCardPayment : IPaymentStrategy
{
    private string cardNumber; // номер карты

    public CreditCardPayment(string cardNumber)
    {
        this.cardNumber = cardNumber; // сохраняем номер
    }

    public void Pay(double amount)
    {
        // реализация оплаты
        Console.WriteLine($"Оплата {amount}$ картой {cardNumber}");
    }
}

// Оплата через PayPal
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

// Оплата криптой
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

// Контекст — тот, кто использует стратегию
public class PaymentContext
{
    private IPaymentStrategy _strategy; // текущая стратегия

    // Устанавливаем способ оплаты
    public void SetStrategy(IPaymentStrategy strategy)
    {
        // если null — ошибка
        _strategy = strategy ?? throw new Exception("Стратегия не выбрана!");
    }

    // Выполнение оплаты
    public void ExecutePayment(double amount)
    {
        // если стратегию забыли выбрать
        if (_strategy == null)
            throw new Exception("Сначала выберите оплату!");

        _strategy.Pay(amount); // вызываем нужный метод
    }
}

#endregion

#region OBSERVER + SINGLETON

// Интерфейс наблюдателя
public interface IObserver
{
    void Update(string currency, double rate); // что делать при обновлении
}

// Интерфейс субъекта (биржи)
public interface ISubject
{
    void Attach(IObserver observer);  // подписаться
    void Detach(IObserver observer);  // отписаться
    void Notify(string currency, double rate); // уведомить всех
}

// Singleton класс — биржа валют
public class CurrencyExchange : ISubject
{
    private static CurrencyExchange _instance; // единственный объект

    private List<IObserver> observers = new(); // список подписчиков
    private Dictionary<string, double> rates = new(); // курсы валют

    // Приватный конструктор — нельзя создать через new
    private CurrencyExchange() {}

    // Глобальный доступ к объекту (Singleton)
    public static CurrencyExchange Instance
    {
        get
        {
            // если еще не создан
            if (_instance == null)
                _instance = new CurrencyExchange(); // создаем

            return _instance; // всегда возвращаем один и тот же объект
        }
    }

    // Добавить наблюдателя
    public void Attach(IObserver observer)
    {
        observers.Add(observer);
        Console.WriteLine("[LOG] Подписчик добавлен");
    }

    // Удалить наблюдателя
    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
        Console.WriteLine("[LOG] Подписчик удален");
    }

    // Установить курс валюты
    public void SetRate(string currency, double rate)
    {
        rates[currency] = rate; // обновляем значение

        Console.WriteLine($"\n[Биржа] {currency} = {rate}");

        Notify(currency, rate); // уведомляем всех
    }

    // Уведомление всех подписчиков
    public void Notify(string currency, double rate)
    {
        foreach (var observer in observers)
        {
            observer.Update(currency, rate); // вызываем Update у каждого
        }
    }
}

// Наблюдатель — пользователь
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

// Наблюдатель — торговый бот
public class TradingBotObserver : IObserver
{
    private double threshold; // порог

    public TradingBotObserver(double threshold)
    {
        this.threshold = threshold;
    }

    public void Update(string currency, double rate)
    {
        // логика бота
        if (rate > threshold)
            Console.WriteLine($"[BOT] Продаем {currency}");
        else
            Console.WriteLine($"[BOT] Покупаем {currency}");
    }
}

// Наблюдатель — аналитика
public class AnalyticsObserver : IObserver
{
    public void Update(string currency, double rate)
    {
        Console.WriteLine($"[Analytics] {currency} обновлен до {rate}");
    }
}

#endregion

#region CLIENT

class Program
{
    static void Main()
    {
        Console.WriteLine("=== PAYMENT (Strategy) ===");

        // создаем контекст
        var context = new PaymentContext();

        // выбираем стратегию — карта
        context.SetStrategy(new CreditCardPayment("1234"));
        context.ExecutePayment(100);

        // меняем стратегию — PayPal
        context.SetStrategy(new PayPalPayment("mail@test.com"));
        context.ExecutePayment(200);

        // меняем стратегию — крипта
        context.SetStrategy(new CryptoPayment("0xABC"));
        context.ExecutePayment(300);

        Console.WriteLine("\n=== EXCHANGE (Observer + Singleton) ===");

        // получаем ЕДИНСТВЕННЫЙ объект биржи
        var exchange = CurrencyExchange.Instance;

        // создаем наблюдателей
        var user = new UserObserver("Alice");
        var bot = new TradingBotObserver(500);
        var analytics = new AnalyticsObserver();

        // подписываем их
        exchange.Attach(user);
        exchange.Attach(bot);
        exchange.Attach(analytics);

        // меняем курс — все получают уведомление
        exchange.SetRate("USD", 480);
        exchange.SetRate("EUR", 550);

        Console.ReadLine();
    }
}

#endregion
