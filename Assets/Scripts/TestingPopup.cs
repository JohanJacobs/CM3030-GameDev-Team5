using UnityEngine;

public class TestingPopup : MonoBehaviour 
{
    [SerializeField] private Transform pfDamagePopup;

    private void Start()
    {
        Transform damagePopupTransform = Instantiate(pfDamagePopup, Vector3.zero, Quaternion.identity);      
        DamagePopup damagePopup = damagePopupTransform.GetComponentInChildren<DamagePopup>();
        damagePopup.Setup("+Attack Rate!!!!!");
    }
}
