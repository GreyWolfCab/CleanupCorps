using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMiner : Gun
{
    private PlayerStats playerStats;

    [Header("Gun Stats")]
    [SerializeField] private int gunCost;
    [SerializeField] private int ammoCost;
    [SerializeField] private int totalBullets;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Bullet")]
    private GameObject[] bullets;
    private Transform spawnPoint;
    [SerializeField] private float launchForce = 75f;
    [SerializeField] private GameObject bulletHolder;

    private void Awake() {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        spawnPoint = transform.GetChild(0);//set spawn point for the bullet
        bulletHolder = GameObject.Find("BulletHolder");//store the bullets in an object
        bullets = new GameObject[totalBullets];//instantiate the bullets
        for (int i = 0; i < bullets.Length; i++) {
            bullets[i] = Instantiate(bulletPrefab);
            bullets[i].transform.parent = bulletHolder.transform;//set bullet parent
            bullets[i].SetActive(false);//deactivate the bullets
        }
    }

    public override void CraftWeapon() {
        playerStats.AddToBank(-gunCost);//deduct gunCost from player bank
        Debug.Log("Capsule Miner Built");
        //make the weapon available to the player
    }

    public override void ShootWeapon() {
        //attempt next available bullet
        int nextBullet = 0;//restart bullet counter
        while (bullets[nextBullet].activeInHierarchy || bullets[nextBullet].GetComponent<CapMinerBullet>().AreMinesActive()) {//check if bullet is active or mines are active
            nextBullet++;//attempt next bullet
            if (nextBullet >= bullets.Length) {//stop check at max bullet
                return;//fire nothing
            }
        }

        playerStats.AddToBank(-ammoCost);//deduct ammoCost from the player bank
        bullets[nextBullet].SetActive(true);//activate the bullet
        bullets[nextBullet].transform.position = spawnPoint.position;//spawn the bullet at the end of the gun
        Rigidbody bulletBody = bullets[nextBullet].GetComponent<Rigidbody>();
        bulletBody.AddForce(spawnPoint.forward * launchForce, ForceMode.Impulse);//launch the bullet
        nextBullet++;//increment to next bullet
    }
}
