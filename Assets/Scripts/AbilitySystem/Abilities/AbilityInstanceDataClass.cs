/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AbilityInstanceDataClass.cs

*/

using System;
using UnityEngine;

using AbilityInstanceDataClassRegistry = ClassRegistry<AbilityInstanceData, AbilityInstanceDataClassAttribute>;

[Serializable]
public struct AbilityInstanceDataClass
{
    public string ClassName;

    public Type ClassType => AbilityInstanceDataClassRegistry.GetClassType(ClassName);

    public AbilityInstanceDataClass(Type classType)
    {
        ClassName = AbilityInstanceDataClassRegistry.GetClassTypeName(classType);

        if (string.IsNullOrWhiteSpace(ClassName))
            throw new ArgumentOutOfRangeException(nameof(classType), "Invalid ability instance data class type");
    }

    public AbilityInstanceData CreateInstance()
    {
        Debug.Assert(ClassType != null, $"Ability instance data class \"{ClassName}\" not found");

        if (ClassType == null)
            return null;

        var instance = Activator.CreateInstance(ClassType) as AbilityInstanceData;

        Debug.Assert(instance != null, $"Failed to create ability instance data object \"{ClassName}\"");

        return instance;
    }
}
