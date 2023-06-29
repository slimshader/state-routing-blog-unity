using System;

public class Router
{
    private readonly ViewStore _store;
    //private Dictionary<string, Action> _routes = new Dictionary<string, Action>();

    public Router(ViewStore store)
    {
        _store = store;
        //_routes.Add("/document/", () => store.ShowOverview());
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
