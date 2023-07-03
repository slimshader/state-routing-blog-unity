using Cysharp.Threading.Tasks;
using System;
using UniMob;

public enum DState
{
    Pending,
    Rejected,
    Fulfilled
}

public class Document
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
}

public sealed class ReactiveDocuments : ILifetimeScope
{
    private readonly UniTask<DocumentInfo[]> _task;

    [Atom]
    public DState State { get; private set; }

    [Atom]
    public DocumentInfo[] Value { get; private set; }

    public ReactiveDocuments(Lifetime lifetime, UniTask<DocumentInfo[]> task)
    {
        Lifetime = lifetime;
        _task = task;
    }

    public async void Run()
    {
        State = DState.Pending;

        try
        {
            Value = await _task;
            State = DState.Fulfilled;
        }
        catch (Exception ex)
        {
            State = DState.Rejected;
        }
    }

    public Lifetime Lifetime { get; }
}

public sealed class DocumentInfo
{
    public int Id;
    public string Name;

    public override string ToString() => $"{Id} : {Name}";
}

public class ViewStore : Store
{
    private readonly IFetch _fetch;

    [Atom]
    public User CurrentUser {  get; private set; }

    [Atom]
    public Store CurrentView { get; private set; }

    [Atom]
    public bool IsAuthenticated => CurrentUser != null;

    [Atom]
    public string CurrentPath => CurrentView switch
    {
        OverviewStore => "/document/",
        DocumentStore d => $"/document/{d.DocumentId}",
        _ => "/document/"
    };

    public ViewStore(IFetch fetch)
    {
        _fetch = fetch;
    }

    public void ShowOverview()
    {
        CurrentView = new OverviewStore(_fetch.Fetch<DocumentInfo[]>("/json/documents.json"))
        {
            Name = "overview",
        };
    }

    public void ShowDocument(int id)
    {
        CurrentView = new DocumentStore(_fetch.Fetch<Document>($"/json/{id}.json"))
        {
            Name = "document",
            DocumentId = id
        };
    }

    public async void PerformLogin(string username, string password, Action<bool> callback)
    {
        try
        {
            CurrentUser = await _fetch.Fetch<User>($"/json/{username}-{password}.json");
            callback(true);
        }
        catch
        {

        }
        callback(false);
    }
}
