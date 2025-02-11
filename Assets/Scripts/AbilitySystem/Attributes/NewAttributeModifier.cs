using UnityEngine;

public enum NewAttributeModifierType
{
    Add,
    AddFraction,
    Multiply,
    Override,
}

[CreateAssetMenu]
public class NewAttributeModifier : ScriptableObject
{
    public AttributeType Attribute => _attribute;
    public NewAttributeModifierType Type => _type;
    public float Value => _value;
    public bool Permanent => _permanent;
    public bool Post => _post;

    [SerializeField]
    private AttributeType _attribute;

    [SerializeField]
    private NewAttributeModifierType _type;

    [SerializeField]
    private float _value;

    [SerializeField]
    private bool _permanent;

    [SerializeField]
    private bool _post;

    public NewAttributeModifierInstance CreateInstance()
    {
        NewAttributeModifierInstance instance;

        if (Type == NewAttributeModifierType.Override)
        {
            instance = new NewAttributeModifierInstanceWithOverride(this);
        }
        else
        {
            instance = new NewAttributeModifierInstanceWithModifier(this);
        }

        return instance;
    }
}
