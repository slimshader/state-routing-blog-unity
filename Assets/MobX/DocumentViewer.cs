using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UniMob;
using UnityEngine;
using UnityEngine.UIElements;

public class Router
{
    private Dictionary<string, Action> _routes = new Dictionary<string, Action>();

    public Router(ViewStore store)
    {
        _routes.Add("/document/", () => store.ShowOverview());
    }

    public void Go(string url)
    {
        if (_routes.TryGetValue(url, out Action action))
        {
            action();
        }
    }
}

public class DocumentViewer : MonoBehaviour, IFetch
{
    public UIDocument Ui;

    private readonly LifetimeController _lifetimeController = new LifetimeController();
    private ViewStore _store;
    private Router _router;

    void Start()
    {
        var lifetime = _lifetimeController.Lifetime;

        _store = new ViewStore(lifetime, this);

        _router = new Router(_store);

        Atom.Reaction(lifetime, () => _store.CurrentView, v => RenderCurrentView(_store));

        Atom.Reaction(lifetime, () => _store.IsAuthenticated ? _store.CurrentUser.Name : "unknown user", username =>
        {
            Ui.rootVisualElement.Q<Label>("CurrentUserLabel").text = username;
        });

        _router.Go("/document/");
    }

    private void RenderCurrentView(ViewStore store)
    {
        var view = store.CurrentView;

        var overview = Ui.rootVisualElement.Q<VisualElement>("OverviewElement");
        var document = Ui.rootVisualElement.Q<VisualElement>("DocumentElement");

        if (view?.Name == "overview")
        {
            overview.style.display = DisplayStyle.Flex;
            document.style.display = DisplayStyle.None;
        }
        else if (view?.Name == "document")
        {
            overview.style.display = DisplayStyle.None;
            document.style.display = DisplayStyle.Flex;
        }
    }

    public async UniTask<T> Fetch<T>(string url)
    {
        var dir = Application.streamingAssetsPath;

        var fullPath = dir + url;// Path.Combine(dir, url);

        var text = await File.ReadAllTextAsync(fullPath);

        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
    }
}
