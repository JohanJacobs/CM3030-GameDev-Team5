using UnityEngine;

internal class CueEffectInstance : EffectInstance
{
    private GameObject _cue;

    public CueEffectInstance(Effect effect, EffectContext context, GameObject cue)
        : base(effect, context)
    {
        _cue = Object.Instantiate(cue, context.Target.transform);
    }

    // TODO: override Destroy()?
    public override void Destroy()
    {
        Object.Destroy(_cue);

        _cue = null;

        base.Destroy();
    }
}

[CreateAssetMenu(menuName = "Effects/Cue")]
public class CueEffect : Effect
{
    public GameObject Cue;

    public override EffectInstance CreateEffectInstance(Effect effect, EffectContext context)
    {
        return new CueEffectInstance(this, context, Cue);
    }
}