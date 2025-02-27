/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

GenericInstanceHandle.cs

*/

using System;

public abstract class GenericInstanceHandle<T> where T: class
{
    public AbilitySystemComponent AbilitySystemComponent => _weakAbilitySystemComponent.TryGetTarget(out var asc) ? asc : null;
    public T Instance => _weakInstance.TryGetTarget(out var instance) ? instance : null;

    private readonly WeakReference<AbilitySystemComponent> _weakAbilitySystemComponent;
    private readonly WeakReference<T> _weakInstance;

    protected GenericInstanceHandle(AbilitySystemComponent asc, T instance)
    {
        _weakAbilitySystemComponent = new WeakReference<AbilitySystemComponent>(asc);
        _weakInstance = new WeakReference<T>(instance);
    }

    public virtual void Clear()
    {
        _weakInstance.SetTarget(null);
    }
}
