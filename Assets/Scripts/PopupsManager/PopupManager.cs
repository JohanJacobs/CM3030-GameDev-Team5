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

    public void CreateNewPopup(Vector3 position, string popupText,float timeToLive)
    {
        var go = Instantiate(popupTemplate,position, Quaternion.identity);
        go.transform.parent = transform;

        var text_component = go.GetComponent<TextPopup>();
        text_component.Setup(popupText, timeToLive);
    }

    #region Pickup Callbacks
    private void Pickup_PickedUp(object sender, GameObject target)
    {
        CreateNewPopup(target.transform.position, "PICKED UP!", _popupTimeToLive);
    }

    #endregion Pickup Callbacks
}
