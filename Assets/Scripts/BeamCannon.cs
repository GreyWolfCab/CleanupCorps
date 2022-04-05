using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCannon : Gun
{
    private PlayerStats playerStats;

    [Header("Gun Stats")]
    [SerializeField] private int gunCost;
    [SerializeField] private int ammoCost;

    [Header("Bullet")]
    private Transform spawnPoint;
    private Transform cameraTransform;
    private LayerMask groundLayer;
    [SerializeField] private GameObject bulletHolder;
    [SerializeField] private GameObject lazerPrefab;
    private GameObject lazer;

    private void Awake() {
        cameraTransform = Camera.main.transform;
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        spawnPoint = transform.GetChild(0);//set spawn point for the bullet
        bulletHolder = GameObject.Find("BulletHolder");//store the bullets in an object
        lazer = Instantiate(lazerPrefab);//create lazer object
        lazer.SetActive(false);
        lazer.transform.parent = bulletHolder.transform;//set laser beam parent
        groundLayer = LayerMask.GetMask("Ground");
        //instantiate bullet render
    }

    public override void CraftWeapon() {
        playerStats.AddToBank(-gunCost);//deduct cost from player bank
        Debug.Log("Beam Cannon Built");
        //make the weapon available to the player
    }

    public override void ShootWeapon() {
        playerStats.AddToBank(-ammoCost);//deduct ammoCost from the player bank
        RaycastHit groundHit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward * 100f, out groundHit, 1000f, groundLayer, QueryTriggerInteraction.Ignore)) {//if raycast hits a ground object
            lazer.SetActive(true);//activate lazer beam
            lazer.transform.position = Midpoint(cameraTransform.position, groundHit.point);//set collider in the center between both points
            lazer.transform.rotation = cameraTransform.rotation;//set collider with the correct angle
            lazer.GetComponent<CapsuleCollider>().height = Vector3.Distance(cameraTransform.position, groundHit.point);//set the collider with the correct length
            lazer.GetComponent<BeamCannonBeam>().SetPoints(spawnPoint.transform.position, groundHit.point);//have the lazer stop at the ground object
        } else {//the raycast didn't hit a ground object
            lazer.SetActive(true);//activate lazer beam
            lazer.GetComponent<BeamCannonBeam>().SetPoints(spawnPoint.transform.position, spawnPoint.transform.forward * 1000f);//have the lazer shoot out very far
        }
        //activate bullet render
    }

    private Vector3 Midpoint(Vector3 startPoint, Vector3 endPoint) {
        return new Vector3((startPoint.x + endPoint.x) * 0.5f, (startPoint.y + endPoint.y) * 0.5f, (startPoint.z + endPoint.z) * 0.5f);//calculate the midpoint between 2 vectors
    }

}
