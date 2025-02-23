/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

WeaponItem.cs

*/

using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : Item
{
    public WeaponEquipmentItem EquipmentItem;
    public AttackAbility AttackAbility;
}
