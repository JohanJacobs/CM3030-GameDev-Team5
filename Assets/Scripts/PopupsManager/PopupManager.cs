using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
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

    public void CreateNewPopup(Vector3 position, string popupText, Transform playerTransform, float timeToLive)
    {
        var go = Instantiate(popupTemplate, position, Quaternion.identity);
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
            CreateNewPopup(target.transform.position, $"+{experienceOrbPickup.Experience} XP", target.transform, _popupTimeToLive);
            return;
        }

        // pickups that impact the players abilities
        if (sender is PickupWithEffect pickupWithEffect)
        {
            Vector3 popupOffset = new Vector3(0f, -0.5f, 0f);
            Vector3 popupSpawnPosition = target.transform.position;

            foreach (var change in pickupWithEffect.AbsoluteAttributeValueChanges)
            {
                var sb = new StringBuilder();

                if (change.Value < 0f)
                    sb.Append("-");
                else if (change.Value > 0f)
                    sb.Append("+");

                if (change.Key.IsScaleAttribute())
                {
                    sb.Append(Mathf.Abs(change.Value * 100f).ToString("0.##", CultureInfo.InvariantCulture));
                    sb.Append("%");
                }
                else
                {
                    sb.Append(Mathf.Abs(change.Value).ToString("0.##", CultureInfo.InvariantCulture));
                }

                sb.Append(" ");
                sb.Append(change.Key.GetName());

                // dynamically create popup string value (target = player)
                CreateNewPopup(popupSpawnPosition, sb.ToString(), target.transform, _popupTimeToLive);

                popupSpawnPosition += popupOffset;
            }
        }
    }

    #endregion Pickup Callbacks
}