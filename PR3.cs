using System;
using System.Collections.Generic;

// SRP — Order отвечает только за данные заказа
class OrderItem
{
    public string Name;
    public int Quantity;
    public double Price;
}

class Order
{
    private List<OrderItem> items = new List<OrderItem>();

    public IPayment PaymentMethod { get; set; }
    public IDelivery DeliveryMethod { get; set; }

    public void AddItem(string name, int quantity, double price)
    {
        items.Add(new OrderItem { Name = name, Quantity = quantity, Price = price });
    }

    public double GetTotalPrice()
    {
        double total = 0;
        foreach (var item in items)
            total += item.Price * item.Quantity;

        return total;
    }
}

// OCP + DIP — Оплата
interface IPayment
{
    void ProcessPayment(double amount);
}

class CreditCardPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Payment by Credit Card: {amount}");
    }
}

class PayPalPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Payment by PayPal: {amount}");
    }
}

class BankTransferPayment : IPayment
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"Payment by Bank Transfer: {amount}");
    }
}

// OCP + LSP — Доставка
interface IDelivery
{
    void DeliverOrder(Order order);
}

class CourierDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Order delivered by courier");
    }
}

class PostDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Order delivered by post");
    }
}

class PickUpPointDelivery : IDelivery
{
    public void DeliverOrder(Order order)
    {
        Console.WriteLine("Order delivered to pickup point");
    }
}

// ISP + DIP — Уведомления
interface INotification
{
    void SendNotification(string message);
}

class EmailNotification : INotification
{
    public void SendNotification(string message)
    {
        Console.WriteLine($"Email notification: {message}");
    }
}

class SmsNotification : INotification
{
    public void SendNotification(string message)
    {
        Console.WriteLine($"SMS notification: {message}");
    }
}

// OCP — Скидки
interface IDiscountRule
{
    double Apply(double total);
}

class PercentageDiscount : IDiscountRule
{
    public double Apply(double total)
    {
        return total * 0.9; // 10% скидка
    }
}

class FixedDiscount : IDiscountRule
{
    public double Apply(double total)
    {
        return total - 500;
    }
}

class DiscountCalculator
{
    private List<IDiscountRule> discounts = new List<IDiscountRule>();

    public void AddDiscount(IDiscountRule discount)
    {
        discounts.Add(discount);
    }

    public double Calculate(double total)
    {
        foreach (var discount in discounts)
            total = discount.Apply(total);

        return total;
    }
}

//DIP
class OrderService
{
    private INotification notification;

    public OrderService(INotification notification)
    {
        this.notification = notification;
    }

    public void ProcessOrder(Order order, double finalPrice)
    {
        order.PaymentMethod.ProcessPayment(finalPrice);
        order.DeliveryMethod.DeliverOrder(order);
        notification.SendNotification("Order successfully processed");
    }
}

//MAIN
class Program
{
    static void Main()
    {
        //Создание заказа
        Order order = new Order();
        order.AddItem("Laptop", 1, 300000);
        order.AddItem("Mouse", 2, 5000);

        //Выбор оплаты и доставки (DIP)
        order.PaymentMethod = new PayPalPayment();
        order.DeliveryMethod = new CourierDelivery();

        //Расчет стоимости
        double total = order.GetTotalPrice();

        DiscountCalculator discountCalculator = new DiscountCalculator();
        discountCalculator.AddDiscount(new PercentageDiscount());

        double finalPrice = discountCalculator.Calculate(total);

        //Уведомление
        INotification notification = new EmailNotification();
        OrderService orderService = new OrderService(notification);

        orderService.ProcessOrder(order, finalPrice);

        Console.WriteLine($"Final price: {finalPrice}");
    }
}
