using NUnit.Framework.Constraints;
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
