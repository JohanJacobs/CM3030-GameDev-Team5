using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public class ClassRegistry
{
    private static readonly Dictionary<string, Type> AbilityLogicClasses = GetAbilityLogicClassesDictionary();

    private static IEnumerable<Type> GetAbilityLogicClasses()
    {
        var assembly = Assembly.GetExecutingAssembly();

        foreach (var type in assembly.ExportedTypes)
        {
            if (!type.IsClass)
                continue;
            if (type.IsAbstract)
                continue;

            var attrAbilityLogicClass = type.GetCustomAttribute<AbilityLogicClassAttribute>();
            if (attrAbilityLogicClass == null)
                continue;

            yield return type;
        }
    }

    private static Dictionary<string, Type> GetAbilityLogicClassesDictionary()
    {
        return GetAbilityLogicClasses().ToDictionary(type => type.Name);
    }

    public static IEnumerable<string> GetAbilityLogicClassNames()
    {
        return AbilityLogicClasses.Keys;
    }

    public static Type GetAbilityLogicClassType(string name)
    {
        if (AbilityLogicClasses.TryGetValue(name, out var type))
            return type;

        return null;
    }
}
