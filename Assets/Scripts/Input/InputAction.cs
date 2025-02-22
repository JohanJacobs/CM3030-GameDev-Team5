using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Input Action")]
public sealed class InputAction : ScriptableObject
{
    public delegate void SelfDelegate(InputAction action);

    public Tag Tag;
}