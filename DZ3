using System;

//SRP
// Order — только хранение данных
class Order
{
    public string ProductName;
    public int Quantity;
    public double Price;
}

//Расчет цены
class OrderPriceCalculator
{
    public double Calculate(Order order)
    {
        return order.Quantity * order.Price * 0.9;
    }
}

//Платеж
class PaymentService
{
    public void Pay(string details)
    {
        Console.WriteLine("Payment processed: " + details);
    }
}

//Уведомление
class EmailService
{
    public void Send(string email)
    {
        Console.WriteLine("Email sent to: " + email);
    }
}

//OCP
class Employee
{
    public double BaseSalary;
}

interface ISalaryCalculator
{
    double Calculate(Employee employee);
}

class PermanentEmployee : ISalaryCalculator
{
    public double Calculate(Employee employee)
    {
        return employee.BaseSalary * 1.2;
    }
}

class ContractEmployee : ISalaryCalculator
{
    public double Calculate(Employee employee)
    {
        return employee.BaseSalary * 1.1;
    }
}

class InternEmployee : ISalaryCalculator
{
    public double Calculate(Employee employee)
    {
        return employee.BaseSalary * 0.8;
    }
}

//ISP
interface IPrintable
{
    void Print(string text);
}

interface IScannable
{
    void Scan(string text);
}

interface IFaxable
{
    void Fax(string text);
}

class AllInOnePrinter : IPrintable, IScannable, IFaxable
{
    public void Print(string text)
    {
        Console.WriteLine("Print: " + text);
    }

    public void Scan(string text)
    {
        Console.WriteLine("Scan: " + text);
    }

    public void Fax(string text)
    {
        Console.WriteLine("Fax: " + text);
    }
}

class BasicPrinter : IPrintable
{
    public void Print(string text)
    {
        Console.WriteLine("Print: " + text);
    }
}

//DIP
interface ISender
{
    void Send(string message);
}

class EmailSender : ISender
{
    public void Send(string message)
    {
        Console.WriteLine("Email: " + message);
    }
}

class SmsSender : ISender
{
    public void Send(string message)
    {
        Console.WriteLine("SMS: " + message);
    }
}

class NotificationService
{
    private ISender sender;

    public NotificationService(ISender sender)
    {
        this.sender = sender;
    }

    public void Notify(string message)
    {
        sender.Send(message);
    }
}

//MAIN
class Program
{
    static void Main()
    {
        Console.WriteLine("=== SRP ===");
        Order order = new Order
        {
            ProductName = "Laptop",
            Quantity = 2,
            Price = 1000
        };

        OrderPriceCalculator priceCalculator = new OrderPriceCalculator();
        Console.WriteLine("Total price: " + priceCalculator.Calculate(order));

        PaymentService payment = new PaymentService();
        payment.Pay("VISA");

        EmailService email = new EmailService();
        email.Send("user@mail.com");

        Console.WriteLine("\nOCP");
        Employee employee = new Employee { BaseSalary = 1000 };
        ISalaryCalculator salaryCalculator = new PermanentEmployee();
        Console.WriteLine("Salary: " + salaryCalculator.Calculate(employee));

        Console.WriteLine("\nISP");
        IPrintable printer = new BasicPrinter();
        printer.Print("Hello ISP");

        Console.WriteLine("\nDIP");
        ISender sender = new EmailSender();
        NotificationService notification = new NotificationService(sender);
        notification.Notify("Hello DIP");
    }
}
