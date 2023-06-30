using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UniMob;
using UnityEngine;
using UnityEngine.UIElements;

abstract class  VisualComponent : VisualElement, ILifetimeScope
{
    private readonly LifetimeController _lifetimeController = new();

    public Lifetime Lifetime => _lifetimeController.Lifetime;

    public VisualComponent()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChangedInit);
    }

    protected virtual void OnInit(Lifetime lifetime) { }

    protected void OnGeometryChangedInit(GeometryChangedEvent e)
    {
        UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedInit);

        OnInit(Lifetime);
    }
}

class Login : VisualComponent
{
    public new class UxmlFactory : UxmlFactory<Login> { }

    [Atom]
    private string Message { get; set; }

    private string Username { get; set; }
    private string Password { get; set; }

    public ViewStore Store { get; set; }

    public Login()
    {
        
    }

    protected override void OnInit(Lifetime lifetime)
    {
        var messageLabel = this.Q<Label>("MessageLabel");
        Atom.Reaction(lifetime, () => Message, x => messageLabel.text = x);

        this.Q<TextField>("UsernameTextField").RegisterValueChangedCallback(e =>
        {
            Username = e.newValue;
        });

        this.Q<TextField>("PasswordTextField").RegisterValueChangedCallback(e =>
        { Password = e.newValue; });

        this.Q<Button>().clicked += () =>
        {
            Debug.Log($"Will attempt login with {Username} and {Password}");
            Store.PerformLogin(Username, Password, isOk =>
            {
                Message = isOk ? "Success" : "Fail";
            });

        };
        Message = "YOLO";
    }

}

class DocumentOverviewItem : VisualElement
{
    public DocumentOverviewItem(Action<int> onClicked)
    {
        var btn = new Button();
        btn.clicked += () => onClicked(Id);
        Add(btn);
    }

    public int Id { get; set; }
}

public class DocumentViewer : MonoBehaviour, IFetch
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
    void Start()
    {
        var lifetime = _lifetimeController.Lifetime;

        _store = new ViewStore(lifetime, this);

        _router = new Router(_store);

        Atom.Reaction(lifetime, () => _store.CurrentView, v =>
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

        Atom.Reaction(lifetime, () => _store.IsAuthenticated ? _store.CurrentUser.Name : "unknown user", username =>
        {
            Ui.rootVisualElement.Q<Label>("CurrentUserLabel").text = username;
        });

        _router.Go("/document/");
        
        Ui.rootVisualElement.Q<Button>().clicked += () =>
        {
            _router.Go(Ui.rootVisualElement.Q<TextField>().text);
        };

        _listView = Ui.rootVisualElement.Q<ListView>();
        _listView.makeItem = () => new DocumentOverviewItem(_store.ShowDocument);

        _document = Ui.rootVisualElement.Q<VisualElement>("DocumentElement");
        _document.style.display = DisplayStyle.None;

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
        if (!_store.IsAuthenticated)
        {
            return _login;
        }

        return _document;
    }

    private VisualElement ShowOverview(ViewStore store)
    {
        if (_overview ==  null)
        {
            _overview = Ui.rootVisualElement.Q<VisualElement>("OverviewElement");
            _overview.style.display = DisplayStyle.None;
        }

        var overviewView = store.CurrentView as Overview;

        Atom.Reaction(overviewView.Lifetime, () => overviewView.State, s =>
        {
            _overview.Q<Label>("StatusLabel").text = s.ToString();
        });

        Atom.Reaction(overviewView.Lifetime, () => overviewView.Documents, ds =>
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

        await UniTask.Delay(1000);

        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(text);
    }
}
