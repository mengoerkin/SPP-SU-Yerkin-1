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
