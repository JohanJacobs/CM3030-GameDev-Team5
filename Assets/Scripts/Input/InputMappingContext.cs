/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

InputMappingContext.cs

*/
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Input Mapping Context")]
public class InputMappingContext : ScriptableObject
{
    public int Priority;

    public ButtonInputMapping[] ButtonMappings;
}
