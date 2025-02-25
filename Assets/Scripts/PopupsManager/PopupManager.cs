using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private float _popupTimeToLive = 1f;
    [SerializeField] private Transform popupTemplate;

    private AbilitySystemComponent asc;

    public void OnEnable()
    {
        //AttributeModifier
        Pickup.GlobalPickedUp += Pickup_PickedUp;
    }

    private void OnDisable()
    {
        Pickup.GlobalPickedUp -= Pickup_PickedUp;
    }

    public void CreateNewPopup(Vector3 position, string popupText,Transform playerTransform, float timeToLive)
    {
        var go = Instantiate(popupTemplate,position, Quaternion.identity);
        go.transform.SetParent(transform);

        var text_component = go.GetComponent<TextPopup>();
        text_component.Setup(popupText, playerTransform, timeToLive);
    }

    #region Pickup Callbacks
    private void Pickup_PickedUp(Pickup sender, GameObject target)
    {
        // handle XP messages   
        if (sender is ExperienceOrbPickup experienceOrbPickup)
        {
            CreateNewPopup(target.transform.position, "+1 XP", target.transform, _popupTimeToLive);
            return;
        }
        // pickups that impact the players abilities
        if (sender is ExperienceOrbPickup pickupWithEffect)
        {
            var asc = target.GetComponent<AbilitySystemComponent>();

            // create a message for each attribute that is changed
            var pwe = sender as PickupWithEffect;
            int attribute_counter = 0;
            var message_offset = new Vector3(0f, -0.5f, 0f);
            foreach (var e in pwe.Effect.Modifiers)
            {
                var modifierValue = asc.GetAttributeValueWithExtraModifier(e.Attribute, ScalarModifier.MakeFromAttributeModifier(e), e.Post);
                
                // Find the text value, depending on if multiplier or increment
                string popupText;
                if (e.Type == NewAttributeModifierType.Multiply)
                {
                    popupText = $"x{modifierValue} {e.Attribute}";
                }
                else
                {
                    popupText = $"+{modifierValue} {e.Attribute}";
                }
        
                // dynamically create popup string value (target = player)
                CreateNewPopup(target.transform.position - (message_offset * attribute_counter ), popupText, target.transform, _popupTimeToLive);
                
                attribute_counter += 1;
            }

        }
    }

    #endregion Pickup Callbacks
}
