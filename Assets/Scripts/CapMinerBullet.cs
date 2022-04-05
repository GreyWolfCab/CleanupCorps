using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapMinerBullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    private GameObject explosion;
    private ParticleSystem explosionPlayback;
    private float explosionRadius = 2.5f;
    private float explosionForce = 200f;
    private int explosionDamageEnemy = 40;
    private int explosionDamagePlayer = 15;
    private LayerMask playerMask;
    private LayerMask enemyMask;
    [SerializeField] private GameObject capMinerSub;//mines released from big bullet
    private GameObject[] subBullets;//all mines stored in big bullet
    [SerializeField] private int totalBullets;//total mines in the big bullet
    [SerializeField] private float launchForce = 5f;//launch force of the mines
    private GameObject bulletHolder;//parent object for all bullets

    private void Awake() {
        playerMask = LayerMask.GetMask("Player");//setup player layer
        enemyMask = LayerMask.GetMask("Enemy");//setup enemy layer
        explosion = Instantiate(explosionEffect, transform.position, transform.rotation);//create explosion effect
        explosion.SetActive(false);//disable effect
        explosionPlayback = explosion.GetComponent<ParticleSystem>();//store explosion particle system
        subBullets = new GameObject[totalBullets];//instantiate the bullets
        bulletHolder = GameObject.Find("BulletHolder");//store the bullets in an object
        explosion.transform.parent = bulletHolder.transform;//store explosion effects in bullet holder
        Physics.IgnoreLayerCollision(13, 13);//have bullets ignore other bullets
        for (int i = 0; i < subBullets.Length; i++) {
            subBullets[i] = Instantiate(capMinerSub);
            subBullets[i].transform.parent = bulletHolder.transform;//set bullet parent
            subBullets[i].SetActive(false);//deactivate the bullets
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Environment")) {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;//reset the velocity
            Vector3 direction = collision.GetContact(0).normal;//get perpendicular direction of landing position
            Explode();//apply explosive force to enemies and player
            RespawnSubBullets(direction);//respawn the mines at the bullets landing position
            gameObject.SetActive(false);//deactivate capsule
        }
    }

    void RespawnSubBullets(Vector3 direction) {
        for (int i = 0; i < subBullets.Length; i++) {
            subBullets[i].SetActive(true);
            subBullets[i].transform.position = transform.position;//set position of mines
            subBullets[i].GetComponent<CapMinerSubs>().TimedDetonation();//set a timer to detonate the mines upon release
            Rigidbody bulletBody = subBullets[i].GetComponent<Rigidbody>();
            Vector3 randomDirection = (direction + AddRandomDirection(direction));//apply random direction
            bulletBody.AddForce(randomDirection * launchForce, ForceMode.Impulse);//launch the mines in random directions
        }
    }

    void Explode() {
        explosion.SetActive(true);//reactivate effect
        explosion.transform.position = transform.position;//reset explosion position
        explosion.transform.rotation = transform.rotation;//reset explosion rotation
        explosionPlayback.Play();//replay explosion effect

        Collider[] enemyObjects = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);//get nearby enemy objects
        
        foreach (Collider nearbyEnemy in enemyObjects) {
            Rigidbody body = nearbyEnemy.GetComponent<Rigidbody>();
            if (body != null) {
                Vector3 direction = nearbyEnemy.transform.position - transform.position;//calculate the difference in enemy and spawn position
                int distanceSqrd = Mathf.RoundToInt(direction.sqrMagnitude);//optimized way of calculating distance
                //with radius of 2.5f max distance is 9 (31 is least possible damage)
                nearbyEnemy.GetComponent<EnemyStats>().DamageEnemy(explosionDamageEnemy - distanceSqrd);//deal damage to enemy based on proximity
                body.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        Collider[] playerObjects = Physics.OverlapSphere(transform.position, explosionRadius, playerMask);//get nearby player objects

        foreach (Collider nearbyPlayer in playerObjects) {
            Rigidbody body = nearbyPlayer.GetComponent<Rigidbody>();
            if (body != null) {
                Vector3 direction = nearbyPlayer.transform.position - transform.position;//calculate the difference in player and spawn position
                int distanceSqrd = Mathf.RoundToInt(direction.sqrMagnitude);//optimized way of calculating distance
                //with radius of 2.5f max distance is 9 (6 is least possible damage)
                nearbyPlayer.GetComponent<PlayerStats>().DamagePlayer(explosionDamagePlayer - distanceSqrd);//deal damage to player based on proximity
                body.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
        }
    }

    //Generate a random vector to apply bounce force to
    Vector3 AddRandomDirection(Vector3 direction) {
        float x;
        float y;
        float z;
        float randomRange = 1f;//range of directions to use
        float zeroRange = 0.001f;//use to check for zero values
        //generate random direction ignoring perpendicular(up) direction
        if (direction.x < zeroRange && direction.x > -zeroRange) {//for some reason walls do not generate an exact zero
            x = Random.Range(-randomRange, randomRange);
        } else {
            x = 0;
        }
        if (direction.y < zeroRange && direction.y > -zeroRange) {
            y = Random.Range(-randomRange, randomRange);
        }
        else {
            y = 0;
        }
        if (direction.z < zeroRange && direction.z > -zeroRange) {
            z = Random.Range(-randomRange, randomRange);
        }
        else {
            z = 0;
        }
        Vector3 randomVector = new Vector3(x, y, z);//random direction to move
        return randomVector;
    }

    //determines if any mines in the same group are still active
    public bool AreMinesActive() {
        for (int i = 0; i < subBullets.Length; i++) {
            if (subBullets[i].activeInHierarchy) {
                return true;//still have active mines
            }
        }
        return false;//no more active mines
    }
}
