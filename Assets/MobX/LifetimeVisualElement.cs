using UniMob;
using UnityEngine.UIElements;

abstract class  LifetimeVisualElement : VisualElement, ILifetimeScope
{
    private readonly LifetimeController _lifetimeController = new();

    public Lifetime Lifetime => _lifetimeController.Lifetime;

    public LifetimeVisualElement()
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
