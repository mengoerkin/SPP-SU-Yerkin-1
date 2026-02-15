public class InvoiceCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Invoice();
    }
}
