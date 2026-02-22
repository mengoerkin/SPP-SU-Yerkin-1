using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#region SINGLETON

public class ConfigurationManager
{
    private static ConfigurationManager _instance;
    private static readonly object _lock = new object();

    private Dictionary<string, string> _settings = new Dictionary<string, string>();

    // Приватный конструктор
    private ConfigurationManager() { }

    // Потокобезопасный Singleton
    public static ConfigurationManager GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new ConfigurationManager();
            }
        }
        return _instance;
    }

    public void Set(string key, string value)
    {
        _settings[key] = value;
    }

    public string Get(string key)
    {
        if (!_settings.ContainsKey(key))
            throw new Exception($"Setting '{key}' not found");

        return _settings[key];
    }

    public void SaveToFile(string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (var pair in _settings)
                writer.WriteLine($"{pair.Key}={pair.Value}");
        }
    }

    public void LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new Exception("Config file not found");

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split('=');
            if (parts.Length == 2)
                _settings[parts[0]] = parts[1];
        }
    }
}

#endregion

#region BUILDER

public class Report
{
    public string Header { get; set; }
    public string Content { get; set; }
    public string Footer { get; set; }

    public void Show()
    {
        Console.WriteLine(Header);
        Console.WriteLine(Content);
        Console.WriteLine(Footer);
        Console.WriteLine("----------------------");
    }
}

public interface IReportBuilder
{
    void SetHeader(string header);
    void SetContent(string content);
    void SetFooter(string footer);
    Report GetReport();
}

// Текстовый отчет
public class TextReportBuilder : IReportBuilder
{
    private Report _report = new Report();

    public void SetHeader(string header) => _report.Header = $"[TEXT HEADER]: {header}";
    public void SetContent(string content) => _report.Content = content;
    public void SetFooter(string footer) => _report.Footer = $"[TEXT FOOTER]: {footer}";
    public Report GetReport() => _report;
}

// HTML отчет
public class HtmlReportBuilder : IReportBuilder
{
    private Report _report = new Report();

    public void SetHeader(string header) => _report.Header = $"<h1>{header}</h1>";
    public void SetContent(string content) => _report.Content = $"<p>{content}</p>";
    public void SetFooter(string footer) => _report.Footer = $"<footer>{footer}</footer>";
    public Report GetReport() => _report;
}

// Director
public class ReportDirector
{
    public void ConstructReport(IReportBuilder builder)
    {
        builder.SetHeader("Report Title");
        builder.SetContent("Report Content...");
        builder.SetFooter("Report Footer");
    }
}

#endregion

#region PROTOTYPE

public class Product : ICloneable
{
    public string Name;
    public double Price;
    public int Quantity;

    public object Clone()
    {
        return new Product
        {
            Name = this.Name,
            Price = this.Price,
            Quantity = this.Quantity
        };
    }
}

public class Discount : ICloneable
{
    public string Name;
    public double Amount;

    public object Clone()
    {
        return new Discount
        {
            Name = this.Name,
            Amount = this.Amount
        };
    }
}

public class Order : ICloneable
{
    public List<Product> Products = new List<Product>();
    public List<Discount> Discounts = new List<Discount>();
    public double DeliveryCost;
    public string PaymentMethod;

    public object Clone()
    {
        Order clone = new Order();
        clone.DeliveryCost = this.DeliveryCost;
        clone.PaymentMethod = this.PaymentMethod;

        // Глубокое копирование
        foreach (var p in Products)
            clone.Products.Add((Product)p.Clone());

        foreach (var d in Discounts)
            clone.Discounts.Add((Discount)d.Clone());

        return clone;
    }

    public void Show()
    {
        Console.WriteLine("Order:");
        foreach (var p in Products)
            Console.WriteLine($"- {p.Name} x{p.Quantity} = {p.Price}");

        foreach (var d in Discounts)
            Console.WriteLine($"Discount: {d.Name} -{d.Amount}");

        Console.WriteLine($"Delivery: {DeliveryCost}");
        Console.WriteLine($"Payment: {PaymentMethod}");
        Console.WriteLine("----------------------");
    }
}

#endregion

#region MAIN

class Program
{
    static void Main()
    {
        Console.WriteLine("=== SINGLETON TEST ===");

        // Проверка многопоточности
        Parallel.For(0, 5, i =>
        {
            var config = ConfigurationManager.GetInstance();
            config.Set($"key{i}", $"value{i}");
            Console.WriteLine($"Thread {i}: {config.GetHashCode()}");
        });

        var cfg = ConfigurationManager.GetInstance();
        cfg.SaveToFile("config.txt");

        Console.WriteLine("\n=== BUILDER TEST ===");

        var director = new ReportDirector();

        var textBuilder = new TextReportBuilder();
        director.ConstructReport(textBuilder);
        var textReport = textBuilder.GetReport();
        textReport.Show();

        var htmlBuilder = new HtmlReportBuilder();
        director.ConstructReport(htmlBuilder);
        var htmlReport = htmlBuilder.GetReport();
        htmlReport.Show();

        Console.WriteLine("\n=== PROTOTYPE TEST ===");

        Order original = new Order();
        original.Products.Add(new Product { Name = "Laptop", Price = 1000, Quantity = 1 });
        original.Discounts.Add(new Discount { Name = "Sale", Amount = 100 });
        original.DeliveryCost = 20;
        original.PaymentMethod = "Card";

        var cloned = (Order)original.Clone();
        cloned.Products[0].Name = "Gaming Laptop";

        original.Show();
        cloned.Show();
    }
}

#endregio
