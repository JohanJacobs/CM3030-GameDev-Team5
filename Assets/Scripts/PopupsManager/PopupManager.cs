using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private float _popupTimeToLive = 1f;
    [SerializeField] private Transform popupTemplate;

    public void OnEnable()
    {
        //AttributeModifier
        Pickup.PickedUp += Pickup_PickedUp;
    }

    private void OnDisable()
    {
        Pickup.PickedUp -= Pickup_PickedUp;
    }

    public void CreateNewPopup(Vector3 position, string popupText,Transform playerTrasnform, float timeToLive)
    {
        var go = Instantiate(popupTemplate,position, Quaternion.identity);
        go.transform.SetParent(transform);

        var text_component = go.GetComponent<TextPopup>();
        text_component.Setup(popupText, playerTrasnform, timeToLive);
    }

    #region Pickup Callbacks
    private void Pickup_PickedUp(object sender, GameObject target)
    {
        // handle XP messages
        if (sender.GetType().ToString() == "ExperienceOrbPickup")
            CreateNewPopup(target.transform.position, "+1 XP", target.transform, _popupTimeToLive);

        // pickups that impact the players abilities
        if (sender.GetType().ToString() == "PickupWithEffect")
        {
            // create a message for each attribute that is changed
            var pwe = sender as PickupWithEffect;
            int attribute_counter = 0;
            var message_offset = new Vector3(0f, -0.5f, 0f);
            foreach (var e in pwe.Effect.Modifiers)
            {
                CreateNewPopup(target.transform.position - (message_offset * attribute_counter ), $"+{e.Value} {e.Attribute.ToString()}", target.transform, _popupTimeToLive);
                attribute_counter += 1;
            }

        }
    }

    #endregion Pickup Callbacks
}
