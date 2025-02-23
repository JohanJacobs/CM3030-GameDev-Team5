/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

TagContainer.cs

*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class TagContainer : IReadOnlyCollection<Tag>
{
    private readonly HashSet<Tag> _tagSet = new HashSet<Tag>();

    public TagContainer()
    {

    }

    public TagContainer(IEnumerable<Tag> tags)
    {
        AddRange(tags);
    }

    public void Add(Tag tag)
    {
        _tagSet.Add(tag);
    }

    public void AddRange(IEnumerable<Tag> tags)
    {
        _tagSet.UnionWith(tags);
    }

    public void Remove(Tag tag)
    {
        _tagSet.Remove(tag);
    }

    public void RemoveRange(IEnumerable<Tag> tags)
    {
        _tagSet.ExceptWith(tags);
    }

    public void Clear()
    {
        _tagSet.Clear();
    }

    public bool Contains(Tag tag)
    {
        return _tagSet.Contains(tag);
    }

    public bool ContainsAny(IEnumerable<Tag> tags)
    {
        return tags.Any(Contains);
    }

    public bool ContainsAll(IEnumerable<Tag> tags)
    {
        return tags.All(Contains);
    }

    public IEnumerator<Tag> GetEnumerator()
    {
        return _tagSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tagSet.GetEnumerator();
    }

    public int Count => _tagSet.Count;
}
