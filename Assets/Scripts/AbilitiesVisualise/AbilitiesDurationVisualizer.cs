using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
    Visual feedback for the boost that a player has picked up
 */

public class AbilitiesDurationVisualizer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMesh;
    AbilitySystemComponent _abilitySystem;

    List<Effect> _currentEffects;

    string _playerTag = "Player";

    #region Unity Messages
    private void Awake()
    {
        var go = GameObject.FindGameObjectWithTag(_playerTag);
        _abilitySystem = go.GetComponent<AbilitySystemComponent>();
        _currentEffects = new List<Effect>();
    }

    private void OnEnable()
    {
        Pickup.GlobalPickedUp += Pickup_GlobalPickedUp;
    }

    private void OnDisable()
    {
        Pickup.GlobalPickedUp -= Pickup_GlobalPickedUp;
    }
    private void LateUpdate()
    {
        var attribute_times = GetEffectAttributeTimes();
        _textMesh.text = GenerateDisplayText(attribute_times);       
    }

    #endregion Unity Messages

    // Iterate over active effects and identify the modifiers active with their
    // longest active durations.
    private Dictionary<AttributeType, float> GetEffectAttributeTimes()
    {
        // keep track of effects that are still active
        List<Effect> remaining_effects = new List<Effect>();

        // determine the longest active time 
        Dictionary<AttributeType, float> attribute_duration= new Dictionary<AttributeType, float>();

        foreach (var e in _currentEffects)
        {
            var d = _abilitySystem.GetActiveEffectTimeRemainingFraction(e);
            if (d <= 0)
                continue;
            
            foreach (var m in e.Modifiers)
            {
                UpdateDataDictionary(attribute_duration, m.Attribute, d * e.Duration.Value);
            }

            remaining_effects.Add(e);            
        }

        // update the active list of effects
        _currentEffects.Clear();
        _currentEffects = remaining_effects;

        return attribute_duration;
    }

    void UpdateDataDictionary(Dictionary<AttributeType, float> attribute_durations, AttributeType Attribute, float value)
    {
        // Value not in the dictionary
        if (!attribute_durations.ContainsKey(Attribute))
            attribute_durations.Add(Attribute, value);

        // Update the value if its bigger
        else if (attribute_durations[Attribute] < value)
            attribute_durations[Attribute] = value;        
    }

    // Generate the display string
    string GenerateDisplayText(Dictionary<AttributeType, float> attribute_durations)
    {
        string text = "";
        foreach (var a in attribute_durations)
        {
            text += $" {a.Key} {Math.Round(a.Value,1)}s\n";
        }

        return text;
    }

    // Add new pickups to the effect list that is tracked.
    private void Pickup_GlobalPickedUp(Pickup sender, GameObject pickedUpBy)
    {

        if (sender is PickupWithEffect pwe)
        {
            _currentEffects.Add(pwe.Effect);
        }
    }
}

