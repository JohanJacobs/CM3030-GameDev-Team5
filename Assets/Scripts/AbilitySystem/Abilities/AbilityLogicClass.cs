using System;
using UnityEngine;

[Serializable]
public struct AbilityLogicClass
{
    public string ClassName;

    public Type ClassType => ClassRegistry.GetAbilityLogicClassType(ClassName);

    public AbilityLogic CreateInstance()
    {
        Debug.Assert(ClassType != null, $"Ability logic class \"{ClassName}\" not found");

        var instance = Activator.CreateInstance(ClassType ?? typeof(NullAbilityLogic)) as AbilityLogic;

        Debug.Assert(instance != null, $"Failed to create instance if ability logic class \"{ClassName}\"");

        return instance;
    }
}