using Cysharp.Threading.Tasks;
using System;
using UniMob;

public enum DState
{
    Pending,
    Rejected,
    Fulfilled
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

public class User
{
    public string Name { get; set; }
}

public sealed class DocumentInfo
{
    public int Id;
    public string Name;
}

public abstract class View
{
    public string Name { get; set; }
}

public class Overview : View
{
    public ReactiveDocuments Documents { get; set; }
}

public class DocumentView : View
{
    public int Id { get; set; }
}

public interface IFetch
{
    UniTask<T> Fetch<T>(string url);
}

public class ViewStore : ILifetimeScope
{
    public Lifetime Lifetime { get; }
    private readonly IFetch _fetch;

    [Atom]
    public User CurrentUser {  get; private set; }

    [Atom]
    public View CurrentView { get; private set; }

    [Atom]
    public bool IsAuthenticated => CurrentUser != null;

    public ViewStore(Lifetime lifetime, IFetch fetch)
    {
        Lifetime = lifetime;
        _fetch = fetch;
    }

    public void ShowOverview()
    {
        CurrentView = new Overview
        {
            Name = "overview",
            Documents = new ReactiveDocuments(Lifetime, _fetch.Fetch<DocumentInfo[]>("/json/documents.json"))
        };
    }

    public void ShowDocument(int id)
    {
        CurrentView = new DocumentView
        {
            Name = "document",
            Id = id
        };
    }

    public void PerformLogin(string username, string password, Action<bool> callback)
    {
    }
}
