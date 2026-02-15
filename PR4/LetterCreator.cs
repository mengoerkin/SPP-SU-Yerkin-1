public class LetterCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        return new Letter();
    }
}
