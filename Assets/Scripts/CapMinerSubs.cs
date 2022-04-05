using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapMinerSubs : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffect;
    private GameObject explosion;
    private ParticleSystem explosionPlayback;
    private Rigidbody mineBody;
    private LayerMask enemyMask;
    private LayerMask playerMask;
    private float explosionForce = 150f;
    private float explosionRadius = 1.5f;
    private int explosionDamageEnemy = 20;
    private int explosionDamagePlayer = 9;
    private float seekerForce = 10f;
    private float seekerRadius = 3f;
    private float detonationTime = 3.0f;
    private GameObject bulletHolder;//parent object for all bullets

    private void Awake() {
        Physics.IgnoreLayerCollision(13, 13);//have bullets ignore other bullets
        explosion = Instantiate(explosionEffect);//create explosion effect
        explosion.SetActive(false);//disable effect
        explosionPlayback = explosion.GetComponent<ParticleSystem>();//store explosion playback
        enemyMask = LayerMask.GetMask("Enemy");
        playerMask = LayerMask.GetMask("Player");
        mineBody = GetComponent<Rigidbody>();
        bulletHolder = GameObject.Find("BulletHolder");//store the bullets in an object
        explosion.transform.parent = bulletHolder.transform;//store explosion effects in bullet holder
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            DetonateMines();//explode sub mine
        } else if (collision.gameObject.CompareTag("Environment")) {
            SeekEnemy();//move towards nearby enemies
        }
    }

    void DetonateMines() {
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;//reset the velocity
        CancelInvoke("DetonateMines");//prevent mines from premature detonation
        Explode();
        gameObject.SetActive(false);
    }

    void SeekEnemy() {
        //play bounce soundeffect

        Collider[] enemyObjects = Physics.OverlapSphere(transform.position, seekerRadius, enemyMask);//get nearby enemy objects

        if (enemyObjects.Length >= 1) {
            Vector3 direction = enemyObjects[0].transform.position - transform.position;//find enemy direction
            mineBody.AddForce(direction.normalized * seekerForce, ForceMode.Impulse);//move mine toward enemy
        }
    }

    void Explode() {
        explosion.SetActive(true);//reactivate effect
        explosion.transform.position = transform.position;//reset explosion position
        explosion.transform.rotation = transform.rotation;//reset explosion rotation
        explosionPlayback.Play();//replay explosion effect

        Collider[] enemyObjects = Physics.OverlapSphere(transform.position, explosionRadius, enemyMask);//get nearby enemy objects

        foreach (Collider nearbyEnemy in enemyObjects) {//iterate through all nearby enemies
            Rigidbody body = nearbyEnemy.GetComponent<Rigidbody>();
            if (body != null) {
                Vector3 direction = nearbyEnemy.transform.position - transform.position;//find direction from detonation
                int distanceSqrd = Mathf.RoundToInt(direction.sqrMagnitude);//optimized way of calculating distance
                //for radius 1.5f max distance is 4 (16 is least possible damage)
                nearbyEnemy.GetComponent<EnemyStats>().DamageEnemy(explosionDamageEnemy - distanceSqrd);//deal damage to enemy based on proximity
                body.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);//deal explosive force to enemy
            }
        }

        Collider[] playerObjects = Physics.OverlapSphere(transform.position, explosionRadius, playerMask);//get nearby player objects

        foreach (Collider nearbyPlayer in playerObjects) {
            Rigidbody body = nearbyPlayer.GetComponent<Rigidbody>();
            if (body != null) {
                Vector3 direction = nearbyPlayer.transform.position - transform.position;//find direction from detonation
                int distanceSqrd = Mathf.RoundToInt(direction.sqrMagnitude);//optimized way of calculating distance
                //for radius 1.5f max distance is 4 (5 is least possible damage)
                nearbyPlayer.GetComponent<PlayerStats>().DamagePlayer(explosionDamagePlayer - distanceSqrd);//deal damage to player based on proximity
                body.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);//deal explosive force to player
            }
        }
    }

    public void TimedDetonation() {
        Invoke("DetonateMines", detonationTime);//explode the mines after some time
    }
}
