using System;
using UniMob;

public class Router : ILifetimeScope
{
    private readonly LifetimeController _lifetimeController = new();
    private readonly ViewStore _store;

    public Lifetime Lifetime => _lifetimeController.Lifetime;

    public Router(ViewStore store, Action<string> historyPush)
    {
        _store = store;

        Atom.Reaction(Lifetime, () => _store.CurrentPath, historyPush);
    }

    void Add(string path, Action<int> action)
    {
        var parts = path.Split(":");
    }

    public void Go(string url)
    {
        var parts = url.Split("/");

        if (parts.Length == 3)
        {
            if (parts[1] == "document")
            {
                if (int.TryParse(parts[2], out var id))
                {
                    _store.ShowDocument(id);
                }
                else
                {
                    _store.ShowOverview();
                }
            }
        }
        else
        {
            _store.ShowOverview();
        }
    }
}
