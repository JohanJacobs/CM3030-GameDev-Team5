using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public sealed class TagContainer : IReadOnlyCollection<Tag>
{
    private readonly HashSet<Tag> _tags = new HashSet<Tag>();

    public void Add(Tag tag)
    {
        _tags.Add(tag);
    }

    public void AddRange(IEnumerable<Tag> tags)
    {
        _tags.UnionWith(tags);
    }

    public void Remove(Tag tag)
    {
        _tags.Remove(tag);
    }

    public void RemoveRange(IEnumerable<Tag> tags)
    {
        _tags.ExceptWith(tags);
    }

    public bool Contains(Tag tag)
    {
        return _tags.Contains(tag);
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
        return _tags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tags.GetEnumerator();
    }

    public int Count => _tags.Count;
}
