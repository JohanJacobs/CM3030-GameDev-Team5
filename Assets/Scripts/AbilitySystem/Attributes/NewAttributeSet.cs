using System;
using UnityEngine;

[Serializable]
public sealed class NewAttributeDefinition
{
    public AttributeType Attribute => _attribute;

    public float DefaultValue => _defaultValue;

    [SerializeField]
    private AttributeType _attribute;

    [SerializeField]
    private float _defaultValue;
}

[CreateAssetMenu]
public class NewAttributeSet : ScriptableObject
{
    public NewAttributeDefinition[] DefaultAttributes => _defaultAttributes;

    [SerializeField]
    private NewAttributeDefinition[] _defaultAttributes;

    public NewAttributeSetInstance CreateInstance()
    {
        var instance = new NewAttributeSetInstance(this);

        return instance;
    }
}
