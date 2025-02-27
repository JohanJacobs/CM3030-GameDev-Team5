/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

AbilityInstanceDataClassPropertyDrawer.cs


*/

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

using AbilityInstanceDataClassRegistry = ClassRegistry<object, AbilityInstanceDataClassAttribute>;

[CustomPropertyDrawer(typeof(AbilityInstanceDataClass))]
public class AbilityInstanceDataClassPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float propertyHeight = EditorGUI.GetPropertyHeight(property);
        float propertyHeightWithSpacing = propertyHeight + EditorGUIUtility.standardVerticalSpacing;

        // get inside AbilityInstanceDataClass to access its stored class name
        property.NextVisible(true);

        var classNames = AbilityInstanceDataClassRegistry.GetClassNames().ToArray();
        var classLabels = classNames.Select(className => new GUIContent(className)).ToArray();

        var selectedIndex = Array.IndexOf(classNames, property.stringValue);
        var newSelectedIndex = EditorGUI.Popup(position, label, selectedIndex, classLabels);

        if (selectedIndex != newSelectedIndex)
        {
            property.stringValue = classNames[newSelectedIndex];

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
