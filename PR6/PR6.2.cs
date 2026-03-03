using System;
using System.Collections.Generic;

#region STRATEGY

public interface ICostCalculationStrategy
{
    double CalculateCost(TravelRequest request);
}

// ✈ Самолет
public class PlaneStrategy : ICostCalculationStrategy
{
    public double CalculateCost(TravelRequest request)
    {
        double basePrice = request.Distance * 0.5;

        if (request.Class == TravelClass.Business)
            basePrice *= 2;

        basePrice += request.BaggageWeight * 2;

        return ApplyDiscount(basePrice, request);
    }

    private double ApplyDiscount(double price, TravelRequest request)
    {
        if (request.IsChild) price *= 0.7;
        if (request.IsSenior) price *= 0.8;

        return price * request.PassengersCount;
    }
}

// 🚆 Поезд
public class TrainStrategy : ICostCalculationStrategy
{
    public double CalculateCost(TravelRequest request)
    {
        double basePrice = request.Distance * 0.2;

        if (request.Class == TravelClass.Business)
            basePrice *= 1.5;

        return ApplyDiscount(basePrice, request);
    }

    private double ApplyDiscount(double price, TravelRequest request)
    {
        if (request.IsChild) price *= 0.8;
        if (request.IsSenior) price *= 0.85;

        return price * request.PassengersCount;
    }
}

// 🚌 Автобус
public class BusStrategy : ICostCalculationStrategy
{
    public double CalculateCost(TravelRequest request)
    {
        double basePrice = request.Distance * 0.1;

        return ApplyDiscount(basePrice, request);
    }

    private double ApplyDiscount(double price, TravelRequest request)
    {
        if (request.IsChild) price *= 0.85;
        if (request.IsSenior) price *= 0.9;

        return price * request.PassengersCount;
    }
}

#endregion

#region OBSERVER

public interface IObserver
{
    void Update(double newPrice);
}

public class UserNotification : IObserver
{
    public void Update(double newPrice)
    {
        Console.WriteLine($"[Уведомление] Новая стоимость поездки: {newPrice}$");
    }
}

#endregion

#region MODEL

public enum TravelClass
{
    Economy,
    Business
}

public class TravelRequest
{
    public double Distance { get; set; }
    public TravelClass Class { get; set; }
    public bool IsChild { get; set; }
    public bool IsSenior { get; set; }
    public int PassengersCount { get; set; }
    public double BaggageWeight { get; set; }
}

#endregion

#region CONTEXT

public class TravelBookingContext
{
    private ICostCalculationStrategy _strategy;
    private List<IObserver> _observers = new List<IObserver>();

    public void SetStrategy(ICostCalculationStrategy strategy)
    {
        _strategy = strategy ?? throw new Exception("Стратегия не выбрана!");
    }

    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    public double Calculate(TravelRequest request)
    {
        if (_strategy == null)
            throw new Exception("Стратегия не установлена!");

        if (request.Distance <= 0 || request.PassengersCount <= 0)
            throw new Exception("Некорректные данные!");

        double price = _strategy.CalculateCost(request);

        Notify(price);

        return price;
    }

    private void Notify(double price)
    {
        foreach (var obs in _observers)
        {
            obs.Update(price);
        }
    }
}

#endregion

#region CLIENT

class Program
{
    static void Main()
    {
        var context = new TravelBookingContext();

        // Подписка на уведомления
        context.AddObserver(new UserNotification());

        Console.WriteLine("Выберите транспорт: 1 - Самолет, 2 - Поезд, 3 - Автобус");
        int choice = int.Parse(Console.ReadLine());

        switch (choice)
        {
            case 1:
                context.SetStrategy(new PlaneStrategy());
                break;
            case 2:
                context.SetStrategy(new TrainStrategy());
                break;
            case 3:
                context.SetStrategy(new BusStrategy());
                break;
            default:
                Console.WriteLine("Неверный выбор!");
                return;
        }

        var request = new TravelRequest();

        Console.Write("Расстояние (км): ");
        request.Distance = double.Parse(Console.ReadLine());

        Console.Write("Класс (0 - Economy, 1 - Business): ");
        request.Class = (TravelClass)int.Parse(Console.ReadLine());

        Console.Write("Количество пассажиров: ");
        request.PassengersCount = int.Parse(Console.ReadLine());

        Console.Write("Вес багажа: ");
        request.BaggageWeight = double.Parse(Console.ReadLine());

        Console.Write("Ребенок? (true/false): ");
        request.IsChild = bool.Parse(Console.ReadLine());

        Console.Write("Пенсионер? (true/false): ");
        request.IsSenior = bool.Parse(Console.ReadLine());

        try
        {
            double result = context.Calculate(request);
            Console.WriteLine($"Итоговая стоимость: {result}$");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}

#endregion
