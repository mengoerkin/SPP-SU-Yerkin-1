public interface IDocument
{
    void Open();
}

public class Report : IDocument
{
    public void Open()
    {
        Console.WriteLine("Opening Report document...");
    }
}

public class Resume : IDocument
{
    public void Open()
    {
        Console.WriteLine("Opening Resume document...");
    }
}

public class Letter : IDocument
{
    public void Open()
    {
        Console.WriteLine("Opening Letter document...");
    }
}

public abstract class DocumentCreator
{
    // Фабричный метод
    public abstract IDocument CreateDocument();

    // Общая логика работы
    public void OpenDocument()
    {
        IDocument document = CreateDocument();
        document.Open();
    }
}

public class ReportCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Report();
    }
}

public class ResumeCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Resume();
    }
}

public class LetterCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Letter();
    }
}

public class Invoice : IDocument
{
    public void Open()
    {
        Console.WriteLine("Opening Invoice document...");
    }
}

public class InvoiceCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Invoice();
    }
}

using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Choose document type:");
        Console.WriteLine("report / resume / letter / invoice");

        string choice = Console.ReadLine()?.ToLower();

        DocumentCreator creator = choice switch
        {
            "report" => new ReportCreator(),
            "resume" => new ResumeCreator(),
            "letter" => new LetterCreator(),
            "invoice" => new InvoiceCreator(),
            _ => null
        };

        if (creator == null)
        {
            Console.WriteLine("Invalid document type!");
            return;
        }

        creator.OpenDocument();
    }
}
