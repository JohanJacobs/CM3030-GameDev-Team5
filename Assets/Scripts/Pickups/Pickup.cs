using UnityEngine;

public class Pickup : MonoBehaviour
{
    public virtual bool HandlePickUp(GameObject target)
    {
        return false;
    }
}
