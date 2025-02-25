/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

WeakKey.cs

*/

using System;

public readonly struct WeakKey<T> : IEquatable<WeakKey<T>>, IEquatable<T> where T : class
{
    public static implicit operator WeakKey<T> (T target)
    {
        return new WeakKey<T>(target);
    }

    public T Target
    {
        get
        {
            if (_weakReference == null)
                return null;

            return _weakReference.TryGetTarget(out var target) ? target : null;
        }
    }

    private readonly WeakReference<T> _weakReference;

    public WeakKey(T target)
    {
        _weakReference = new WeakReference<T>(target);
    }

    public bool Equals(T other)
    {
        return ReferenceEquals(Target, other);
    }

    public bool Equals(WeakKey<T> other)
    {
        return Equals(other.Target);
    }

    public override bool Equals(object obj)
    {
        if (obj is T target)
            return Equals(target);
        if (obj is WeakKey<T> other)
            return Equals(other);
        return false;
    }

    public override int GetHashCode()
    {
        return Target?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return Target?.ToString() ?? string.Empty;
    }
}
