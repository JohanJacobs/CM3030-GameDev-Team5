/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

EnumArray.cs

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnumArray<T, TEnum> : IEnumerable<T> where TEnum : Enum
{
    private readonly T[] _array;
    private readonly int _lower;

    public EnumArray()
    {
        var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();

        int lower = Convert.ToInt32(values.Min());
        int upper = Convert.ToInt32(values.Max());

        _array = new T[upper - lower + 1];
        _lower = lower;
    }

    public void Fill(T value)
    {
        for (var i = 0; i < _array.Length; ++i)
        {
            _array[i] = value;
        }
    }

    public T this[TEnum key]
    {
        get => _array[Convert.ToInt32(key) - _lower];
        set => _array[Convert.ToInt32(key) - _lower] = value;
    }

    public int Count => _array.Length;

    public IEnumerator<T> GetEnumerator()
    {
        return _array.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _array.GetEnumerator();
    }
}
