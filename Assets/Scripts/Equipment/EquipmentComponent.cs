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
        public EquipmentSlot EquippedSlot = EquipmentSlot.Undefined;

        public bool Equipped => EquippedSlot != EquipmentSlot.Undefined;
    }

    private static readonly EquipmentSlot[] AllEquipmentSlots = Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>().ToArray();
    // NOTE: need to keep this up-to-date with equipment slots
    private static readonly EquipmentSlot[] UsableEquipmentSlots =
    {
        EquipmentSlot.MainHand,
        EquipmentSlot.OffHand,
    };

    private readonly List<InventorySlot> _slots = new List<InventorySlot>();
    private readonly EnumArray<InventorySlot, EquipmentSlot> _equipmentSlots = new EnumArray<InventorySlot, EquipmentSlot>();

    private Creature _creature;

    public bool HasItem(Item item)
    {
        var itemSlot = FindItemSlot(item);
        if (itemSlot == null)
            return false;

        return true;
    }

    public bool AddItem(Item item)
    {
        var itemSlot = FindItemSlot(item);
        if (itemSlot != null)
            return false;

        var newSlot = new InventorySlot()
        {
            Item = item,
        };

        _slots.Add(newSlot);

        return true;
    }

    public bool RemoveItem(Item item)
    {
        var itemSlot = FindItemSlot(item);
        if (itemSlot == null)
            return false;

        UnequipItem(item);

        _slots.Remove(itemSlot);

        return true;
    }

    public bool EquipItem(Item item)
    {
        if (!item.CanEquip)
            return false;

        var itemSlot = FindItemSlot(item);
        if (itemSlot == null)
            return false;

        if (itemSlot.Equipped)
            return true;

        if (!TryFindFreeEquipmentSlotOf(item.EquipsInSlots, out var equipmentSlot))
            return false;

        if (!TryFindEquipmentAttachmentSlot(equipmentSlot, out var attachmentSlot))
            return false;

        switch (itemSlot.Item)
        {
            case WeaponItem weaponItem:
                if (weaponItem.EquipmentItem)
                {
                    itemSlot.EquipmentItem = Instantiate(weaponItem.EquipmentItem, attachmentSlot.Socket);
                }

                break;
            default:
                return false;
        }

        itemSlot.EquippedSlot = equipmentSlot;

        _equipmentSlots[equipmentSlot] = itemSlot;

        ItemEquipped?.Invoke(item, itemSlot.EquipmentItem, itemSlot.EquippedSlot);

        return true;
    }

    public bool UnequipItem(Item item)
    {
        if (!item.CanEquip)
            return false;

        var itemSlot = FindItemSlot(item);
        if (itemSlot == null)
            return false;

        if (!itemSlot.Equipped)
            return true;

        ItemUnequipped?.Invoke(item, itemSlot.EquipmentItem, itemSlot.EquippedSlot);

        _equipmentSlots[itemSlot.EquippedSlot] = null;

        if (itemSlot.EquipmentItem)
        {
            Destroy(itemSlot.EquipmentItem);
        }

        itemSlot.EquipmentItem = null;
        itemSlot.EquippedSlot = EquipmentSlot.Undefined;

        return true;
    }

    public EquipmentItem GetEquippedItem(EquipmentSlot equipmentSlot)
    {
        var itemSlot = _equipmentSlots[equipmentSlot];
        if (itemSlot == null)
            return null;

        return itemSlot.EquipmentItem;
    }

    private void Awake()
    {
        _creature = GetComponent<Creature>();
    }

    private InventorySlot FindItemSlot(Item item)
    {
        return _slots.Find(slot => slot.Item == item);
    }

    private bool TryFindFreeEquipmentSlot(out EquipmentSlot slot)
    {
        foreach (var equipmentSlot in UsableEquipmentSlots)
        {
            if (_equipmentSlots[equipmentSlot] == null)
            {
                slot = equipmentSlot;
                return true;
            }
        }

        slot = EquipmentSlot.Undefined;
        return false;
    }

    private bool TryFindFreeEquipmentSlotOf(IEnumerable<EquipmentSlot> desiredSlots, out EquipmentSlot slot)
    {
        var suitableSlots = UsableEquipmentSlots
            .Where(equipmentSlot => _equipmentSlots[equipmentSlot] == null)
            .Intersect(desiredSlots)
            .ToArray();

        if (suitableSlots.Any())
        {
            slot = suitableSlots.First();
            return true;
        }

        slot = EquipmentSlot.Undefined;
        return false;
    }

    public bool TryFindEquipmentAttachmentSlot(EquipmentSlot equipmentSlot, out EquipmentAttachmentSlot attachmentSlot)
    {
        return _creature.TryFindEquipmentAttachmentSlot(equipmentSlot, out attachmentSlot);
    }
}