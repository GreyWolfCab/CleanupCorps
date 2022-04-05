using UnityEngine;
using TMPro;

public class WeaponWheelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI selectedWeapon;
    [SerializeField] private TextMeshProUGUI selectedWeaponOutline;
    [SerializeField] private TextMeshProUGUI buildCostText;
    [SerializeField] private TextMeshProUGUI buildCostOutline;
    [SerializeField] private TextMeshProUGUI ammoCostText;
    [SerializeField] private TextMeshProUGUI ammoCostOutline;
    [SerializeField] private string weaponName;
    [SerializeField] private int buildCost;
    [SerializeField] private int ammoCost;
    [SerializeField] private int weaponID;
    private PlayerInput player;

    private void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerInput>();
    }

    public void HoverEnter() {
        selectedWeapon.text = weaponName;
        selectedWeaponOutline.text = selectedWeapon.text;
        buildCostText.text = "Build: " + buildCost.ToString();
        buildCostOutline.text = buildCostText.text;
        ammoCostText.text = "Ammo: " + ammoCost.ToString();
        ammoCostOutline.text = ammoCostText.text;
    }

    public void Selected() {
        player.SetCurrentGun(weaponID);
    }
}
