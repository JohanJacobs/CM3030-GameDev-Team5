/*

University of London
BsC Computer Science Course
Games Development
Final Assignment - Streets of Fire Game

Group 5 

Please refer to the README file for detailled information

TagPropertyDrawer.cs

*/

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Tag))]
public class TagPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float propertyHeight = EditorGUI.GetPropertyHeight(property);
        float propertyHeightWithSpacing = propertyHeight + EditorGUIUtility.standardVerticalSpacing;

        // get inside Tag to access its stored string
        property.NextVisible(true);

        var newTagValue = EditorGUI.TagField(position, label, property.stringValue);

        if (string.CompareOrdinal(property.stringValue, newTagValue) != 0)
        {
            property.stringValue = newTagValue;

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
