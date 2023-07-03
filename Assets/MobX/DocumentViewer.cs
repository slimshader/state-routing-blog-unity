using Cysharp.Threading.Tasks;
using System.IO;
using UniMob;
using UnityEngine;
using UnityEngine.UIElements;

public class DocumentViewer : MonoBehaviour, IFetch, ILifetimeScope
{
    public UIDocument Ui;

    private readonly LifetimeController _lifetimeController = new LifetimeController();
    private ViewStore _store;
    private Router _router;
    private ListView _listView;
    
    private VisualElement _overview;
    private VisualElement _document;
    private Login _login;

    private VisualElement _current;

    Lifetime ILifetimeScope.Lifetime => _lifetimeController.Lifetime;

    void Start()
    {
        _store = new ViewStore(this);

        this.Reaction(() => _store.CurrentView, v =>
        {
            if (_current != null)
            {
                _current.style.display = DisplayStyle.None;
            }

            _current = RenderCurrentView(_store);

            if (_current != null)
            {
                _current.style.display = DisplayStyle.Flex;
            }
        });

        this.Reaction(() => _store.IsAuthenticated ? _store.CurrentUser.Name : "unknown user", username =>
        {
            Ui.rootVisualElement.Q<Label>("CurrentUserLabel").text = username;
        });

        var routeInputFiled = Ui.rootVisualElement.Q<TextField>();

        _router = new Router(_store, x => routeInputFiled.value = x);

        _router.Go("/document/");
        
        Ui.rootVisualElement.Q<Button>().clicked += () =>
        {
            _router.Go(routeInputFiled.text);
        };

        _listView = Ui.rootVisualElement.Q<ListView>();
        _listView.makeItem = () => new DocumentOverviewItem(_store.ShowDocument);

        _document = Ui.rootVisualElement.Q<VisualElement>("DocumentElement");
        _document.style.display = DisplayStyle.None;
        _document.Q<Button>().clicked += () =>
        {
            _store.ShowOverview();
        };

        _login = Ui.rootVisualElement.Q<Login>();
        _login.style.display = DisplayStyle.None;
        _login.Store = _store;
    }

    private VisualElement RenderCurrentView(ViewStore store)
    {
        var view = store.CurrentView;

        if (view?.Name == "overview")
        {
            return ShowOverview(store);
        }
        else if (view?.Name == "document")
        {
            return ShowDocument();
        }

        return null;
    }

    private VisualElement ShowDocument()
    {
        var document = (DocumentStore) _store.CurrentView;

        if (!_store.IsAuthenticated)
        {
            _login.AfterLogin = () => _store.ShowDocument(document.DocumentId);
            return _login;
        }

        document.Reaction(() => document.Value, x =>
        {
            _document.Q<Label>("NameLabel").text = x?.Name;
            _document.Q<Label>("TextLabel").text = x?.Text;
        });


        return _document;
    }

    private VisualElement ShowOverview(ViewStore store)
    {
        if (_overview ==  null)
        {
            _overview = Ui.rootVisualElement.Q<VisualElement>("OverviewElement");
            _overview.style.display = DisplayStyle.None;
        }

        var overviewStore = store.CurrentView as OverviewStore;

        overviewStore.Reaction(() => overviewStore.State, s =>
        {
            _overview.Q<Label>("StatusLabel").text = s.ToString();
        });

        overviewStore.Reaction(() => overviewStore.Documents, ds =>
        {
            _listView.itemsSource = ds;

            if (ds != null)
            {
                _listView.bindItem = (v, i) =>
                {
                    v.Q<Button>().text = ds[i].ToString();
                    (v as DocumentOverviewItem).Id = ds[i].Id;
                };
            }
        });

        return _overview;
    }

    public async UniTask<T> Fetch<T>(string url)
    {
        var dir = Application.streamingAssetsPath;

        var fullPath = dir + url;// Path.Combine(dir, url);

        var text = await File.ReadAllTextAsync(fullPath);

        await UniTask.Delay(UnityEngine.Random.Range(500, 1500));

        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
    }
}
