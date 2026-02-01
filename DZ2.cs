using System;

// DRY
// Қайталанатын логиканы параметр арқылы басқару
enum LogLevel
{
    Error,
    Warning,
    Info
}

class Logger
{
    public void Log(LogLevel level, string message)
    {
        Console.WriteLine($"{level.ToString().ToUpper()}: {message}");
    }
}

// Ортақ конфигурация
static class AppConfig
{
    public static string ConnectionString =
        "Server=myServer;Database=myDb;User Id=myUser;Password=myPass;";
}

class DatabaseService
{
    public void Connect()
    {
        string cs = AppConfig.ConnectionString;
        // Базаға қосылу логикасы
    }
}

class LoggingService
{
    public void Log(string message)
    {
        string cs = AppConfig.ConnectionString;
        // Логты базаға жазу логикасы
    }
}

// KISS

// Артық if пен вложениені болдырмау
class NumberProcessor
{
    public void ProcessNumbers(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
            return;

        foreach (var number in numbers)
        {
            if (number > 0)
                Console.WriteLine(number);
        }
    }

    // Артық LINQ қолданбау
    public void PrintPositiveNumbers(int[] numbers)
    {
        if (numbers == null)
            return;

        foreach (var number in numbers)
        {
            if (number > 0)
                Console.WriteLine(number);
        }
    }

    // Exception-ды логика үшін қолданбау
    public int Divide(int a, int b)
    {
        if (b == 0)
            return 0;

        return a / b;
    }
}

// YAGNI

// Қазір қажет емес функционалды қоспау
class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Артық параметрсіз қарапайым класс
class FileReader
{
    public string ReadFile(string filePath)
    {
        return "file content";
    }
}

// Қазір бір ғана отчет керек
class ReportGenerator
{
    public void GeneratePdfReport()
    {
        // PDF отчет генерациясы
    }
}

// MAIN(TEST)

class Program
{
    static void Main()
    {
        Logger logger = new Logger();
        logger.Log(LogLevel.Info, "Application started");

        NumberProcessor processor = new NumberProcessor();
        processor.ProcessNumbers(new int[] { -1, 2, 3, 0 });

        User user = new User
        {
            Name = "Ahmed",
            Email = "ahmed@gmail.com"
        };

        Console.WriteLine("Done");
    }
}