using UnityEditor;

public class NewAttributeModifierInstance
{
    public NewAttributeModifier Modifier { get; }
    public ScalarModifier ScalarModifier => _scalarModifier;
    public bool Permanent { get; }
    public bool Post { get; }
    public int Index { get; } = ++_nextIndex;

    private static int _nextIndex = 0;

    private ScalarModifier _scalarModifier;

    public NewAttributeModifierInstance(NewAttributeModifier modifier)
    {
        Modifier = modifier;
        Permanent = Modifier.Permanent;
        Post = Modifier.Post;

        _scalarModifier = ScalarModifier.MakeFromAttributeModifier(modifier);
    }

    public NewAttributeModifierInstance(ScalarModifier scalarModifier, bool permanent, bool post)
    {
        Permanent = permanent;
        Post = post;

        _scalarModifier = scalarModifier;
    }
}
