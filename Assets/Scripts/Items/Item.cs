/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

Item.cs

*/

using System.Linq;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string DisplayName;

    /// <summary>
    /// Slots this item can be equipped to. If not specified, item cannot be equipped.
    /// </summary>
    public EquipmentSlot[] EquipsInSlots;

    public bool CanEquip => EquipsInSlots != null && EquipsInSlots.Length > 0;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EquipsInSlots != null)
        {
            if (EquipsInSlots.Contains(EquipmentSlot.Undefined))
            {
                Debug.LogWarning("Equipment slot Undefined must not be used");
            }
        }
    }
#endif
}
