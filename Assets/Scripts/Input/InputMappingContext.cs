/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

InputMappingContext.cs

*/
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Input Mapping Context")]
public class InputMappingContext : ScriptableObject
{
    public int Priority;

    public ButtonInputMapping[] ButtonMappings;
}
