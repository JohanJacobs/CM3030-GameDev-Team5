using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using AbilityLogicClassRegistry = ClassRegistry<AbilityLogic, AbilityLogicClassAttribute>;

public static class ClassRegistry<T, TagAttribute> where T : class where TagAttribute : Attribute
{
    private static readonly Dictionary<string, Type> ClassTypeDictionary;

    static ClassRegistry()
    {
        ClassTypeDictionary = CreateClassTypeDictionary();
    }

    public static IEnumerable<string> GetClassNames()
    {
        return ClassTypeDictionary.Keys;
    }

    public static Type GetClassType(string name)
    {
        if (ClassTypeDictionary.TryGetValue(name, out var type))
            return type;

        return null;
    }

    public static string GetClassTypeName(Type type)
    {
        if (IsClassType(type))
            return type.Name;

        return null;
    }

    private static bool IsClassType(Type type)
    {
        if (!type.IsClass)
            return false;
        if (type.IsAbstract)
            return false;

        var attrTag = type.GetCustomAttribute<TagAttribute>();
        if (attrTag == null)
            return false;

        return true;
    }

    private static IEnumerable<Type> GetClassTypes()
    {
        return Assembly.GetExecutingAssembly().ExportedTypes
            .Where(IsClassType);
    }

    private static Dictionary<string, Type> CreateClassTypeDictionary()
    {
        return GetClassTypes().ToDictionary(type => type.Name);
    }
}
