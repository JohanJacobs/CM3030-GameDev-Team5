/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

EquipmentComponent.cs

*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class EquipmentComponent : MonoBehaviour
{
    public delegate void EquipDelegate(Item item, EquipmentItem equipmentItem, EquipmentSlot equipmentSlot);

    public event EquipDelegate ItemEquipped;
    public event EquipDelegate ItemUnequipped;

    private class InventorySlot
    {
        public Item Item;
        public EquipmentItem EquipmentItem;
        public bool Equipped;
        public EquipmentSlot EquippedSlot;
    }

    private static readonly EquipmentSlot[] AllEquipmentSlots = Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>().ToArray();

    private readonly List<InventorySlot> _slots = new List<InventorySlot>();
    private readonly EnumArray<InventorySlot, EquipmentSlot> _equipmentSlots = new EnumArray<InventorySlot, EquipmentSlot>();

    private Creature _creature;

    public bool HasItem(Item item)
    {
        var slot = _slots.Find(s => s.Item == item);
        if (slot == null)
            return false;

        return true;
    }

    public bool AddItem(Item item)
    {
        var slot = _slots.Find(s => s.Item == item);
        if (slot != null)
            return false;

        var newSlot = new InventorySlot()
        {
            Item = item,
            Equipped = false,
        };

        _slots.Add(newSlot);

        return true;
    }

    public bool RemoveItem(Item item)
    {
        var slot = _slots.Find(s => s.Item == item);
        if (slot == null)
            return false;

        UnequipItem(item);

        _slots.Remove(slot);

        return true;
    }

    public bool EquipItem(Item item)
    {
        var slot = _slots.Find(s => s.Item == item);
        if (slot == null)
            return false;

        if (slot.Equipped)
            return true;

        var equipmentSlot = FindFreeEquipmentSlot();
        if (equipmentSlot == null)
            return false;

        var attachmentSlot = FindEquipmentAttachmentSlot(equipmentSlot.Value);
        if (attachmentSlot == null)
            return false;

        switch (slot.Item)
        {
            case WeaponItem weaponItem:
                if (weaponItem.EquipmentItem)
                {
                    slot.EquipmentItem = Instantiate(weaponItem.EquipmentItem, attachmentSlot.Socket);
                }
                break;
            default:
                return false;
        }

        slot.Equipped = true;
        slot.EquippedSlot = equipmentSlot.Value;

        _equipmentSlots[equipmentSlot.Value] = slot;

        ItemEquipped?.Invoke(item, slot.EquipmentItem, slot.EquippedSlot);

        return true;
    }

    public bool UnequipItem(Item item)
    {
        var slot = _slots.Find(s => s.Item == item);
        if (slot == null)
            return false;

        if (!slot.Equipped)
            return true;

        ItemUnequipped?.Invoke(item, slot.EquipmentItem, slot.EquippedSlot);

        _equipmentSlots[slot.EquippedSlot] = null;

        if (slot.EquipmentItem)
        {
            Destroy(slot.EquipmentItem);
        }

        slot.EquipmentItem = null;
        slot.Equipped = false;
        slot.EquippedSlot = default;

        return true;
    }

    private void Awake()
    {
        _creature = GetComponent<Creature>();
    }

    private EquipmentSlot? FindFreeEquipmentSlot()
    {
        foreach (var equipmentSlot in AllEquipmentSlots)
        {
            if (_equipmentSlots[equipmentSlot] == null)
                return equipmentSlot;
        }

        return null;
    }

    private EquipmentAttachmentSlot FindEquipmentAttachmentSlot(EquipmentSlot equipmentSlot)
    {
        return _creature.EquipmentAttachmentSlots.FirstOrDefault(s => s.Slot == equipmentSlot);
    }
}
