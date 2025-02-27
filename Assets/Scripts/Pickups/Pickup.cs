/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Pickup.cs

*/
using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    public delegate void PickedUpDelegate(Pickup pickup, GameObject pickedUpBy);

    public static GameObject GetPickupGameObject(Component context)
    {
        switch (context)
        {
            case Pickup pickup:
                return pickup.gameObject;
            case PickupTrigger pickupTrigger:
                // NOTE: specific to current pickup prefab hierarchy
                return pickupTrigger.gameObject.transform.parent.gameObject;
        }

        {
            var siblingContext = context.gameObject.GetComponent<PickupTrigger>();
            if (siblingContext != null)
                return GetPickupGameObject(siblingContext);
        }

        {
            var siblingContext = context.gameObject.GetComponent<Pickup>();
            if (siblingContext != null)
                return GetPickupGameObject(siblingContext);
        }

        return null;
    }

    public static Pickup GetPickupComponent(Component context)
    {
        switch (context)
        {
            case Pickup pickup:
                return pickup;

            // TODO: more shortcuts?
        }

        var go = GetPickupGameObject(context);
        if (go)
            return go.GetComponent<Pickup>();

        return null;
    }

    public event PickedUpDelegate PickedUp;
    public static event PickedUpDelegate GlobalPickedUp; 

    /// <summary>
    /// Handle picking up logic. Checks conditions and performs all the actions specific to this pickup.
    /// </summary>
    /// <param name="target">Object that is picking up this pickup</param>
    /// <returns><see langword="true"/> if pickup is picked up by given <paramref name="target" />, <see langword="false" /> otherwise</returns>
    public bool HandlePickUp(GameObject target)
    {
        if (HandlePickUpImpl(target))
        {
            PickedUp?.Invoke(this, target);
            GlobalPickedUp?.Invoke(this, target);
            Destroy(gameObject);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Implementation if pickup logic
    /// </summary>
    /// <param name="target">Object that is picking up this pickup</param>
    /// <returns><see langword="true"/> if pickup is picked up by given <paramref name="target" />, <see langword="false" /> otherwise</returns>
    protected virtual bool HandlePickUpImpl(GameObject target)
    {
        return false;
    }
}
