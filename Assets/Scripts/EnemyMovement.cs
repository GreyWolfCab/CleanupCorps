using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private GameManager gameManager;

    [Header("ChasingPlayer")]
    private Transform playerTransform;
    [SerializeField] private Transform orientationTransform;//same y-axis value used by corners from navmesh
    private NavMeshAgent navMeshAgent;
    private Rigidbody body;
    private NavMeshPath playerPath;
    private float moveSpeed = 30f;
    private Vector3 prevPosition;
    private float playerRange = 4.5f;
    private bool useNavMesh;

    [Header("GroundDetection")]
    [SerializeField] LayerMask groundMask;
    private float enemyHeight;
    public bool isGrounded;
    private float groundCheckRadius = 0.1f;
    private float previousY;//determine if the player is falling
    private float gravityMultiplier = 5f;//increase gravity when falling

    private Quaternion startingRotation;

    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerTransform = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false;//prevent navmesh from moving enemy
        navMeshAgent.updateRotation = false;//prevent navmesh from rotating enemy
        body = GetComponent<Rigidbody>();
        playerPath = new NavMeshPath();
        enemyHeight = GetComponent<CapsuleCollider>().height;
        startingRotation = transform.rotation;
        useNavMesh = false;//start using rigidbody movement
    }

    private void Update() {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, enemyHeight * 0.5f, 0), groundCheckRadius, groundMask);
        MultiplyGravity();
        if (gameManager.isGameActive) {
            if (!useNavMesh) {//track player using rigidbody
                navMeshAgent.nextPosition = transform.position;//force navmeshagent to mimic physic control
            } else {
                navMeshAgent.destination = playerTransform.position;//track player along with nav mesh agent
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!useNavMesh) {//track player using rigidbody
            NavMesh.CalculatePath(orientationTransform.position, playerTransform.position, NavMesh.AllAreas, playerPath);//calculate waypoints to guide the enemy to the player
            Vector3 nextPosition;//come up with better solution to track player in mid air
                                 //raise nav mesh
                                 //use raycast pointing downward from player
            if (playerPath.corners.Length == 0) {//current player not on nav mesh solution
                nextPosition = prevPosition;
            }
            else {
                nextPosition = playerPath.corners[1];
                prevPosition = nextPosition;
            }
            Vector3 direction = nextPosition - orientationTransform.position;//calculate direction towards next destination
            if (gameManager.isGameActive) {
                if (playerPath.corners.Length <= 2 && CalculateDistance(direction) < playerRange) {//if there are no corners between the enemy and player AND the enemy is close to the player
                    body.velocity = Vector3.zero;//stop enemy movement
                }
                else {
                    body.AddForce(direction.normalized * moveSpeed, ForceMode.Acceleration);//move rigidbody toward destination
                }
                transform.rotation = Quaternion.LookRotation(direction);//rotate enemy toward direction of movement
            }
        }//else do not use rigidbody physics to move towards the player
        

        //DEBUG CODE draw corners
        for (int i = 0; i < playerPath.corners.Length - 1; i++)
            Debug.DrawLine(playerPath.corners[i], playerPath.corners[i + 1], Color.red);
        
    }

    //calculate the distance from a magnitude vector
    float CalculateDistance(Vector3 direction) {
        float distanceSqrd = direction.sqrMagnitude;//optimized way of calculating distance
        return distanceSqrd;
    }

    void MultiplyGravity() {
        if (!isGrounded) {//player is in the air
            if (previousY > transform.position.y) {//player is falling
                body.AddForce(Physics.gravity * (body.mass * gravityMultiplier));//increase gravity
            }
            previousY = transform.position.y;
        }
    }

    public void ResetEnemy(Vector3 spawnPoint) {
        useNavMesh = false;//start using rigidbody movement
        transform.position = spawnPoint;//respawn gameobject on the new spawn point
        navMeshAgent.Warp(spawnPoint);//respawn nav mesh agent to join with its new spawn point
        navMeshAgent.updatePosition = false;//prevent navmesh from moving enemy
        navMeshAgent.updateRotation = false;//prevent navmesh from rotating enemy
        transform.rotation = startingRotation;
    }

    public void SetMoveSpeed(float moveSpeed) {
        this.moveSpeed = moveSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        useNavMesh = true;//activate navmesh agent destination
        navMeshAgent.updatePosition = true;//allow navmesh to update enemy position
        navMeshAgent.updateRotation = true;//allow navmesh to update enemy rotation
        body.velocity = Vector3.zero;
    }

    private void OnTriggerExit(Collider other) {
        useNavMesh = false;//deactivate navmesh agent destination
        navMeshAgent.updatePosition = false;//prevent navmesh from moving enemy
        navMeshAgent.updateRotation = false;//prevent navmesh from rotating enemy
    }

}
