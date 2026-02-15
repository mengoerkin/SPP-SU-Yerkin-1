public class ResumeCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Resume();
    }
}
