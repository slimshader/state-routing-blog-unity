public class Router
{
    private readonly ViewStore _store;
    //private Dictionary<string, Action> _routes = new Dictionary<string, Action>();

    public Router(ViewStore store)
    {
        _store = store;
        //_routes.Add("/document/", () => store.ShowOverview());
    }

    public void Go(string url)
    {
        var parts = url.Remove(0).Split("/");

        if (parts.Length == 1)
        {
            if (parts[0] == "/document/")
                _store.ShowOverview();
        }
        else if (parts.Length == 2)
        {
            if (parts[0] == "/document/" && int.TryParse(parts[1], out var id))
            {
                _store.ShowDocument(id);
            }
        }

        //if (_routes.TryGetValue(url, out Action action))
        //{
        //    action();
        //}
        else
        {
            _store.ShowOverview();
        }
    }
}
