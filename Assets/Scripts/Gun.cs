using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public abstract void ShootWeapon();//shoot the weapon and deduct the ammo cost from the player
    public abstract void CraftWeapon();//activate the weapon for player use and deduct the cost from the player
}
