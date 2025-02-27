/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AbilityLogicClass.cs

*/

using System;
using UnityEngine;

using AbilityLogicClassRegistry = ClassRegistry<AbilityLogic, AbilityLogicClassAttribute>;

[Serializable]
public struct AbilityLogicClass
{
    public string ClassName;

    public Type ClassType => AbilityLogicClassRegistry.GetClassType(ClassName);

    public AbilityLogic CreateInstance()
    {
        Debug.Assert(ClassType != null, $"Ability logic class \"{ClassName}\" not found");

        var instance = Activator.CreateInstance(ClassType ?? typeof(NullAbilityLogic)) as AbilityLogic;

        Debug.Assert(instance != null, $"Failed to create instance if ability logic class \"{ClassName}\"");

        return instance;
    }
}
