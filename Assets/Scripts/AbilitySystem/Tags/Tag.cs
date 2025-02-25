/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Tag.cs

*/

using System;
using UnityEngine;

[Serializable]
public struct Tag : IEquatable<Tag>, IComparable<Tag>, ISerializationCallbackReceiver
{
    public static implicit operator Tag(string s) => new Tag(s);
    public static explicit operator string(Tag tag) => tag._string;

    public static bool operator ==(Tag tag1, Tag tag2) => tag1.Equals(tag2);
    public static bool operator !=(Tag tag1, Tag tag2) => !(tag1 == tag2);

    [SerializeField]
    private string _string;

    public Tag(string s)
    {
        _string = string.IsNullOrWhiteSpace(s) ? null : string.Intern(s);
    }

    public int CompareTo(Tag other)
    {
        if (ReferenceEquals(_string, other._string))
            return 0;

        if (_string == null)
            return -1;
        if (other._string == null)
            return 1;

        return string.Compare(_string, other._string, StringComparison.Ordinal);
    }

    public bool Equals(Tag other)
    {
        return ReferenceEquals(_string, other._string);
    }

    public override int GetHashCode()
    {
        return _string.GetHashCode();
    }

    public override string ToString() => _string;

    public override bool Equals(object obj)
    {
        return obj is Tag other && Equals(other);
    }

    public void OnBeforeSerialize()
    {
    }

    public void OnAfterDeserialize()
    {
        _string = string.Intern(_string);
    }
}
