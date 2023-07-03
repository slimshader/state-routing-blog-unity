using System;
using UniMob;

public static class UniMobExtensions
{
    public static Reaction Reaction<T, U>(this T observable, Func<U> data, Action<U> effect) where T : ILifetimeScope
    {
        return Atom.Reaction(observable.Lifetime, data, effect);
    }
}
