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
