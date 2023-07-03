using System;
using UniMob;

public abstract class Store : ILifetimeScope, IDisposable
{
    private LifetimeController _lifetimeControler = new();

    public string Name { get; set; }

    public Lifetime Lifetime => _lifetimeControler.Lifetime;

    public void Dispose() => _lifetimeControler.Dispose();
}
