using Cysharp.Threading.Tasks;
using System;
using UniMob;
using UnityEngine;

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

    public override string ToString() => $"{Id} : {Name}";
}

public abstract class View
{
    public string Name { get; set; }
}

public class Overview : View, ILifetimeScope
{
    private readonly UniTask<DocumentInfo[]> _documentTask;

    public Overview(in Lifetime lifetime, UniTask<DocumentInfo[]> documentsTask)
    {
        Lifetime = Lifetime;
        _documentTask = documentsTask;

        Observe(documentsTask).Forget();
    }

    private async UniTask Observe(UniTask<DocumentInfo[]> task)
    {
        State = DState.Pending;

        try
        {
            Documents = await task;
            State = DState.Fulfilled;
        }
        catch (Exception ex)
        {
            State = DState.Rejected;
        }
    }


    [Atom]
    public DocumentInfo[] Documents { get; private set; }

    [Atom]
    public DState State { get; private set; }

    public Lifetime Lifetime { get; }
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
        CurrentView = new Overview(Lifetime, _fetch.Fetch<DocumentInfo[]>("/json/documents.json"))
        {
            Name = "overview",
        };
    }

    public void ShowDocument(int id)
    {
        Debug.Log($"Will show document {id}");
        CurrentView = new DocumentView
        {
            Name = "document",
            Id = id
        };
    }

    public void PerformLogin(string username, string password, Action<bool> callback)
    {
        callback(UnityEngine.Random.Range(0, 100) < 50);
    }
}
