/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

PickupTrigger.cs

*/

using UnityEngine;

public static class LayerMaskHelper
{
    public static bool TestGameObjectLayer(this LayerMask self, GameObject gameObject)
    {
        return (self.value & (1 << gameObject.layer)) != 0;
    }
}

public class PickupTrigger : MonoBehaviour
{
    public LayerMask LayerMask;

    private Pickup _pickup;
    private Collider _collider;

    private void Awake()
    {
        _pickup = GetComponentInParent<Pickup>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickedUpBy = other.gameObject;

        if (!LayerMask.TestGameObjectLayer(pickedUpBy))
            return;

        if (_pickup.HandlePickUp(pickedUpBy))
        {
            OnPickedUp(pickedUpBy);
        }
    }

    private void OnPickedUp(GameObject pickedUpBy)
    {
        _collider.enabled = false;
    }
}
