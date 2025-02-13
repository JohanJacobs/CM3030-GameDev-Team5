using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AbilityLogicClass))]
public class AbilityLogicClassPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float propertyHeight = EditorGUI.GetPropertyHeight(property);
        float propertyHeightWithSpacing = propertyHeight + EditorGUIUtility.standardVerticalSpacing;

        // get inside AbilityLogicClass to access its stored class name
        property.NextVisible(true);

        var abilityLogicClassNames = ClassRegistry.GetAbilityLogicClassNames().ToArray();
        var abilityLogicClassItems = abilityLogicClassNames.Select(className => new GUIContent(className)).ToArray();

        var selectedIndex = Array.IndexOf(abilityLogicClassNames, property.stringValue);
        var newSelectedIndex = EditorGUI.Popup(position, label, selectedIndex, abilityLogicClassItems);

        if (selectedIndex != newSelectedIndex)
        {
            property.stringValue = abilityLogicClassNames[newSelectedIndex];

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}