/*

University of London
BsC Computer Science Course
Games Design
Final Assignment - Streets of Fire Game

Group 5 

InputMappings.cs

*/
using System;
using UnityEngine;

[Serializable]
public abstract class InputMapping
{
    public event InputAction.SelfDelegate Pressed;
    public event InputAction.SelfDelegate Released;

    public InputAction InputAction;

    public abstract bool Update();

    protected void DispatchPressed()
    {
        Pressed?.Invoke(InputAction);
    }

    protected void DispatchReleased()
    {
        Released?.Invoke(InputAction);
    }
}

[Serializable, InputActionMapping]
public class ButtonInputMapping : InputMapping
{
    public string Button;

    public override bool Update()
    {
        if (Input.GetButtonDown(Button))
        {
            DispatchPressed();
            return true;
        }

        if (Input.GetButtonUp(Button))
        {
            DispatchReleased();
            return true;
        }

        return false;
    }
}
