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
        var go = Instantiate(popupTemplate, position, Quaternion.identity, transform);

        var textPopup = go.GetComponent<TextPopup>();
        textPopup.Setup(popupText, playerTransform, timeToLive);
    }

    #region Pickup Callbacks

    private void Pickup_PickedUp(Pickup sender, GameObject target)
    {
        var attributeDataMap = GameData.Instance.AttributeDataMap;

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
            Vector3 popupSpawnPosition = target.transform.position + new Vector3(0f, 2.5f, 0f);

            var sb = new StringBuilder(48);

            foreach (var item in pickupWithEffect.AbsoluteAttributeValueChanges)
            {
                var attribute = item.Key;
                var delta = item.Value;

                // dynamically create popup string value (target = player)
                sb.Clear();

                
                if (delta < 0f)
                    sb.Append("-");
                else if (delta > 0f)
                    sb.Append("+");

                if (attribute.IsScaleAttribute())
                {
                    sb.Append(Mathf.Abs(delta * 100f).ToString("0.##", CultureInfo.InvariantCulture));
                    sb.Append("%");
                }
                else
                {
                    sb.Append(Mathf.Abs(delta).ToString("0.##", CultureInfo.InvariantCulture));
                }

                var attributeDisplayName = attributeDataMap.TryGetValue(attribute, out var attributeData) ? attributeData.DisplayName : attribute.GetName();

                sb.Append(" ");
                sb.Append(attributeDisplayName);

                CreateNewPopup(popupSpawnPosition, sb.ToString(), target.transform, _popupTimeToLive);

                popupSpawnPosition += popupOffset;
            }
        }
    }

    #endregion Pickup Callbacks
}