using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Magnitude))]
public class MagnitudePropertyDrawer : PropertyDrawer
{
    private static readonly GUIStyle AdvancedButtonStyle;

    // property drawers are created anew for each displayed property but better make sure we keep track of all instances in case it's reused
    private readonly Dictionary<WeakKey<Magnitude>, bool> _advancedPropertyDisplay = new Dictionary<WeakKey<Magnitude>, bool>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var targetMagnitude = fieldInfo.GetValue(property.serializedObject.targetObject) as Magnitude;
        if (targetMagnitude == null)
            throw new ArgumentException("Unable to access property's Magnitude object");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float lineHeightWithSpacing = lineHeight + spacing;

        var propCalculation = property.FindPropertyRelative("Calculation");
        var propAttributeProvider = property.FindPropertyRelative("AttributeProvider");
        var propAttribute = property.FindPropertyRelative("Attribute");
        var propValue = property.FindPropertyRelative("Value");

        var advancedDisplay = GetAdvancedDisplay(targetMagnitude) || targetMagnitude.Calculation != MagnitudeCalculation.Simple;

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        using (var indentLevelScope = new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
        {
            if (advancedDisplay)
            {
                var valueRect = new Rect(position.x, position.y, position.width * 0.25f, lineHeight);
                var calculationRect = new Rect(valueRect.xMax + spacing, valueRect.y, position.width * 0.75f - spacing, lineHeight);

                EditorGUI.PropertyField(valueRect, propValue, GUIContent.none);
                EditorGUI.PropertyField(calculationRect, propCalculation, GUIContent.none);

                if (targetMagnitude.Calculation != MagnitudeCalculation.Simple)
                {
                    var attributeProviderRect = new Rect(position.x, position.y + lineHeightWithSpacing, position.width * 0.25f, lineHeight);
                    var attributeRect = new Rect(attributeProviderRect.xMax + spacing, attributeProviderRect.y, position.width * 0.75f - spacing, lineHeight);

                    EditorGUI.PropertyField(attributeProviderRect, propAttributeProvider, GUIContent.none);
                    EditorGUI.PropertyField(attributeRect, propAttribute, GUIContent.none);
                }
            }
            else
            {
                var valueRect = new Rect(position.x, position.y, position.width - lineHeightWithSpacing, lineHeight);
                var advancedRect = new Rect(valueRect.xMax + spacing, position.y, lineHeight, lineHeight);

                EditorGUI.PropertyField(valueRect, propValue, GUIContent.none);

                if (GUI.Button(advancedRect, "+", AdvancedButtonStyle))
                {
                    SetAdvancedDisplay(targetMagnitude, true);
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var targetMagnitude = fieldInfo.GetValue(property.serializedObject.targetObject) as Magnitude;
        if (targetMagnitude == null)
            throw new ArgumentException("Unable to access property's Magnitude object");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float lineHeightWithSpacing = lineHeight + EditorGUIUtility.standardVerticalSpacing;

        var height = lineHeight;

        if (targetMagnitude.Calculation != MagnitudeCalculation.Simple)
        {
            height += lineHeightWithSpacing;
        }

        return height;
    }

    static MagnitudePropertyDrawer()
    {
        // for some reason, + sign is not centered inside button - move it a bit
        AdvancedButtonStyle = new GUIStyle(GUI.skin.button)
        {
            contentOffset = new Vector2(0, -1),
        };
    }

    private bool GetAdvancedDisplay(Magnitude magnitude)
    {
        if (_advancedPropertyDisplay.TryGetValue(magnitude, out var advancedDisplay))
            return advancedDisplay;

        advancedDisplay = magnitude.Calculation != MagnitudeCalculation.Simple;
        
        _advancedPropertyDisplay.Add(magnitude, advancedDisplay);

        return advancedDisplay;
    }

    private void SetAdvancedDisplay(Magnitude magnitude, bool advancedDisplay)
    {
        _advancedPropertyDisplay[magnitude] = advancedDisplay;
    }

    private void ResetAdvancedDisplay(Magnitude magnitude)
    {
        _advancedPropertyDisplay.Remove(magnitude);
    }

}
