using UnityEngine;

[CreateAssetMenu(menuName = "Input/Input Mapping Context")]
public class InputMappingContext : ScriptableObject
{
    public int Priority;

    public ButtonInputMapping[] ButtonMappings;
}