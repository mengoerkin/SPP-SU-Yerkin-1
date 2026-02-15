public class ReportCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Report();
    }
}
