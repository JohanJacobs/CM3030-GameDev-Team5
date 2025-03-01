/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

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

    public ref T this[TEnum key] => ref _array[Convert.ToInt32(key) - _lower];

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
