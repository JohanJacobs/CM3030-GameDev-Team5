using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
    private struct InputMappingEntry
    {
        public InputMapping Mapping;
        public int Priority;
    }

    private class InputActionEntry
    {
        private readonly List<InputMappingEntry> _mappings = new List<InputMappingEntry>();

        public void AddInputMapping(InputMapping mapping, int priority)
        {
            var entry = new InputMappingEntry()
            {
                Mapping = mapping,
                Priority = priority,
            };

            _mappings.Add(entry);

            // sort descending
            _mappings.Sort((lhs, rhs) => rhs.Priority - lhs.Priority);
        }

        public void RemoveInputMapping(InputMapping mapping)
        {
            var index = _mappings.FindIndex(entry => entry.Mapping == mapping);

            Debug.Assert(index >= 0);

            _mappings.RemoveAt(index);
        }

        public void Update()
        {
            // TODO: copy of mappings might be needed for iteration

            foreach (var entry in _mappings)
            {
                if (entry.Mapping.Update())
                    break;
            }
        }
    }

    public event InputAction.SelfDelegate InputActionPressed;
    public event InputAction.SelfDelegate InputActionReleased;

    private readonly Dictionary<InputAction, InputActionEntry> _inputActionEntries = new Dictionary<InputAction, InputActionEntry>();
    private readonly HashSet<InputMappingContext> _inputMappingContexts = new HashSet<InputMappingContext>();

    public void AddInputMappingContext(InputMappingContext imc)
    {
        if (_inputMappingContexts.Add(imc))
        {
            AddInputMappings(imc.ButtonMappings, imc.Priority);
        }
    }

    public void RemoveInputMappingContext(InputMappingContext imc)
    {
        if (_inputMappingContexts.Remove(imc))
        {
            RemoveInputMappings(imc.ButtonMappings);
        }
    }

    private void Awake()
    {
    }

    private void Update()
    {
        foreach (var inputActionEntry in _inputActionEntries.Values)
        {
            inputActionEntry.Update();
        }
    }

    private void AddInputMappings(IEnumerable<InputMapping> mappings, int priority)
    {
        foreach (var mapping in mappings)
        {
            if (_inputActionEntries.TryGetValue(mapping.InputAction, out var inputActionEntry))
            {
            }
            else
            {
                inputActionEntry = new InputActionEntry();

                _inputActionEntries.Add(mapping.InputAction, inputActionEntry);
            }

            mapping.Pressed += InputActionPressed;
            mapping.Released += InputActionReleased;

            inputActionEntry.AddInputMapping(mapping, priority);
        }
    }

    private void RemoveInputMappings(IEnumerable<InputMapping> mappings)
    {
        foreach (var mapping in mappings)
        {
            if (_inputActionEntries.TryGetValue(mapping.InputAction, out var inputActionEntry))
            {
                mapping.Pressed -= InputActionPressed;
                mapping.Released -= InputActionReleased;

                inputActionEntry.RemoveInputMapping(mapping);
            }
        }
    }
}