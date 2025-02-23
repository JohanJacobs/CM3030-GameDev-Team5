/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

InputAction.cs

*/

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Input Action")]
public sealed class InputAction : ScriptableObject
{
    public delegate void SelfDelegate(InputAction action);

    public Tag Tag;
}
