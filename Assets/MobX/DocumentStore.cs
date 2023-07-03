using Cysharp.Threading.Tasks;
using UniMob;

public class DocumentStore : Store
{
    private readonly UniTask<Document> _loadingDocument;

    [Atom]
    public Document Value { get; private set; }

    public DocumentStore(UniTask<Document> loadingDocument)
    {
        _loadingDocument = loadingDocument;

        Load().Forget();
    }

    private async UniTask Load()
    {
        try
        {
            Value = await _loadingDocument;
        }
        finally { }
    }

    public int DocumentId { get; set; }
}
