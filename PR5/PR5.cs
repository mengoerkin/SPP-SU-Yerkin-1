using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#region SINGLETON LOGGER

public enum LogLevel
{
    INFO = 1,
    WARNING = 2,
    ERROR = 3
}

public class LoggerConfig
{
    public string FilePath { get; set; }
    public LogLevel Level { get; set; }
}

public class Logger
{
    private static Logger _instance;
    private static readonly object _lock = new object();

    private string _filePath = "log.txt";
    private LogLevel _currentLevel = LogLevel.INFO;

    private readonly object _fileLock = new object();

    private Logger() { }

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new Logger();
            }
        }
        return _instance;
    }

    public void SetLogLevel(LogLevel level)
    {
        _currentLevel = level;
    }

    public void LoadConfig(string path)
    {
        if (!File.Exists(path))
            throw new Exception("Config file not found");

        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<LoggerConfig>(json);

        _filePath = config.FilePath;
        _currentLevel = config.Level;
    }

    public void Log(string message, LogLevel level)
    {
        if (level < _currentLevel) return;

        string logMessage = $"{DateTime.Now} [{level}] {message}";

        lock (_fileLock)
        {
            File.AppendAllText(_filePath, logMessage + "\n");
        }

        Console.WriteLine(logMessage); // доп: вывод в консоль
    }
}

public class LogReader
{
    private string _filePath;

    public LogReader(string path)
    {
        _filePath = path;
    }

    public void Read(LogLevel minLevel)
    {
        if (!File.Exists(_filePath))
            return;

        foreach (var line in File.ReadAllLines(_filePath))
        {
            if (line.Contains(minLevel.ToString()) ||
                (minLevel == LogLevel.INFO) ||
                (minLevel == LogLevel.WARNING && !line.Contains("INFO")))
            {
                Console.WriteLine(line);
            }
        }
    }
}

#endregion

#region BUILDER

public class ReportStyle
{
    public string BackgroundColor { get; set; }
    public string FontColor { get; set; }
    public int FontSize { get; set; }
}

public class Report
{
    public string Header;
    public string Content;
    public string Footer;
    public List<string> Sections = new List<string>();
    public ReportStyle Style;

    public void Export()
    {
        Console.WriteLine("=== REPORT ===");
        Console.WriteLine(Header);
        foreach (var s in Sections)
            Console.WriteLine(s);
        Console.WriteLine(Content);
        Console.WriteLine(Footer);
        Console.WriteLine($"Style: {Style?.FontColor}, {Style?.FontSize}");
        Console.WriteLine("----------------------");
    }
}

public interface IReportBuilder
{
    void SetHeader(string header);
    void SetContent(string content);
    void SetFooter(string footer);
    void AddSection(string name, string content);
    void SetStyle(ReportStyle style);
    Report GetReport();
}

public class TextReportBuilder : IReportBuilder
{
    private Report report = new Report();

    public void SetHeader(string h) => report.Header = $"TEXT: {h}";
    public void SetContent(string c) => report.Content = c;
    public void SetFooter(string f) => report.Footer = f;
    public void AddSection(string n, string c) => report.Sections.Add($"[{n}] {c}");
    public void SetStyle(ReportStyle s) => report.Style = s;
    public Report GetReport() => report;
}

public class HtmlReportBuilder : IReportBuilder
{
    private Report report = new Report();

    public void SetHeader(string h) => report.Header = $"<h1>{h}</h1>";
    public void SetContent(string c) => report.Content = $"<p>{c}</p>";
    public void SetFooter(string f) => report.Footer = $"<footer>{f}</footer>";
    public void AddSection(string n, string c) => report.Sections.Add($"<section><h2>{n}</h2><p>{c}</p></section>");
    public void SetStyle(ReportStyle s) => report.Style = s;
    public Report GetReport() => report;
}

// PDF (заглушка без библиотеки)
public class PdfReportBuilder : IReportBuilder
{
    private Report report = new Report();

    public void SetHeader(string h) => report.Header = $"PDF HEADER: {h}";
    public void SetContent(string c) => report.Content = c;
    public void SetFooter(string f) => report.Footer = f;
    public void AddSection(string n, string c) => report.Sections.Add($"PDF SECTION: {n} - {c}");
    public void SetStyle(ReportStyle s) => report.Style = s;
    public Report GetReport() => report;
}

public class ReportDirector
{
    public void Construct(IReportBuilder builder, ReportStyle style)
    {
        builder.SetHeader("Report Title");
        builder.AddSection("Intro", "This is intro");
        builder.SetContent("Main content");
        builder.SetFooter("Footer");
        builder.SetStyle(style);
    }
}

#endregion

#region PROTOTYPE

public class Weapon : ICloneable
{
    public string Name;
    public int Damage;

    public object Clone()
    {
        return new Weapon { Name = Name, Damage = Damage };
    }
}

public class Armor : ICloneable
{
    public string Name;
    public int Defense;

    public object Clone()
    {
        return new Armor { Name = Name, Defense = Defense };
    }
}

public class Skill : ICloneable
{
    public string Name;
    public int Power;

    public object Clone()
    {
        return new Skill { Name = Name, Power = Power };
    }
}

public class Character : ICloneable
{
    public int Health;
    public int Strength;
    public int Agility;
    public int Intelligence;

    public Weapon Weapon;
    public Armor Armor;
    public List<Skill> Skills = new List<Skill>();

    public object Clone()
    {
        Character clone = new Character();

        clone.Health = Health;
        clone.Strength = Strength;
        clone.Agility = Agility;
        clone.Intelligence = Intelligence;

        clone.Weapon = (Weapon)Weapon.Clone();
        clone.Armor = (Armor)Armor.Clone();

        foreach (var s in Skills)
            clone.Skills.Add((Skill)s.Clone());

        return clone;
    }

    public void Show()
    {
        Console.WriteLine($"HP:{Health} STR:{Strength}");
        Console.WriteLine($"Weapon: {Weapon.Name}");
        foreach (var s in Skills)
            Console.WriteLine($"Skill: {s.Name}");
        Console.WriteLine("------------------");
    }
}

#endregion

#region MAIN

class Program
{
    static void Main()
    {
        Console.WriteLine("=== LOGGER TEST ===");

        var logger = Logger.GetInstance();

        // создаем config.json
        File.WriteAllText("config.json",
            JsonSerializer.Serialize(new LoggerConfig
            {
                FilePath = "log.txt",
                Level = LogLevel.INFO
            }));

        logger.LoadConfig("config.json");

        Parallel.For(0, 5, i =>
        {
            var log = Logger.GetInstance();
            log.Log($"Message {i}", (LogLevel)((i % 3) + 1));
        });

        Console.WriteLine("\n=== READ ERRORS ===");
        new LogReader("log.txt").Read(LogLevel.ERROR);

        Console.WriteLine("\n=== BUILDER TEST ===");

        var director = new ReportDirector();
        var style = new ReportStyle { FontColor = "Black", FontSize = 12 };

        var textBuilder = new TextReportBuilder();
        director.Construct(textBuilder, style);
        textBuilder.GetReport().Export();

        var htmlBuilder = new HtmlReportBuilder();
        director.Construct(htmlBuilder, style);
        htmlBuilder.GetReport().Export();

        Console.WriteLine("\n=== PROTOTYPE TEST ===");

        Character hero = new Character
        {
            Health = 100,
            Strength = 20,
            Weapon = new Weapon { Name = "Sword", Damage = 50 },
            Armor = new Armor { Name = "Armor", Defense = 30 }
        };
        hero.Skills.Add(new Skill { Name = "Fireball", Power = 40 });

        var clone = (Character)hero.Clone();
        clone.Weapon.Name = "Axe";

        hero.Show();
        clone.Show();
    }
}

#endregio
