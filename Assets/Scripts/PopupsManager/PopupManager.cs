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
        if (sender.GetType().ToString() == "ExperienceOrbPickup")
            CreateNewPopup(target.transform.position, "+1 XP", target.transform, _popupTimeToLive);

        if (sender.GetType().ToString() == "PickupWithEffect")
        {
            var pwe = sender as PickupWithEffect;
            foreach (var e in pwe.Effect.Modifiers)
            {
                CreateNewPopup(target.transform.position, $"+{e.Value} {e.Attribute.ToString()}", target.transform, _popupTimeToLive);
            }

        }
    }

    #endregion Pickup Callbacks
}
