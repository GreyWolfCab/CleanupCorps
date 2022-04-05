using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [Header("Movement")]
    public float moveSpeed = 120.0f;//move speed of the player
    public bool isMoving;//determine if the player is moving
    private float previousY;//determine if the player is falling
    public float deceleration = 0.4f;//decelerate the player faster
    private Vector3 moveDirection;//move direction of the player

    [Header("Jump")]
    public float jumpForce = 300.0f;
    public float airDrag = 1f;
    [SerializeField] private float airMultiplier = 0.1f;//lessen move speed in the air
    private float gravityMultiplier = 0.3f;//increase gravity when falling

    [Header("GroundDetection")]
    [SerializeField] LayerMask groundMask;
    public bool isGrounded;
    public float groundDrag = 12f;
    private float groundCheckRadius = 0.1f;

    [Header("Shooting")]
    [SerializeField] private Gun[] guns;
    private int currentGun;
    
    [Header("PlayerInfo")]
    private float playerHeight;
    [SerializeField] private Transform orientation;
    private Rigidbody playerBody;

    [Header("UILogic")]
    private GameManager gameManager;
    private bool isPaused;
    private bool lookingAtWeaponWheel;
    private GameObject nearbyDoor = null;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentGun = 1;//start with Beam Cannon
        playerBody = GetComponent<Rigidbody>();
        playerHeight = GetComponent<CapsuleCollider>().height;
        previousY = transform.position.y;//store starting y position
        isPaused = false;
        lookingAtWeaponWheel = false;
    }

    // Update is called once per frame
    private void Update() {

        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, playerHeight * 0.5f, 0), groundCheckRadius, groundMask);

        if (gameManager.isGameActive) {//deactivate player controls upon game over
            PlayerMovement();
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
                Jump();
            }

            if (Input.GetMouseButtonDown(0)) {//left click shoot
                guns[currentGun].ShootWeapon();
            }

            if (Input.GetKeyDown(KeyCode.Tab)) {
                gameManager.PauseGame();
                isPaused = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                gameManager.ShowWeaponWheel();
                lookingAtWeaponWheel = true;
            }

            if (Input.GetKeyDown(KeyCode.E) && nearbyDoor != null) {
                nearbyDoor.GetComponent<OpenDoor>().RemoveDoor();
                nearbyDoor = null;
            }

        } else {//game is in-active
            if (isPaused && Input.GetKeyDown(KeyCode.Tab)) {
                gameManager.UnPauseGame();
                isPaused = false;
            } else if (lookingAtWeaponWheel && Input.GetKeyDown(KeyCode.LeftShift)) {
                gameManager.HideWeaponWheel();
                lookingAtWeaponWheel = false;
            }
        }

        ControlDrag();
        MultiplyGravity();

    }

    void PlayerMovement() {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (verticalInput != 0 || horizontalInput != 0) {
            isMoving = true;
        } else {
            isMoving = false;
        }
    }

    void ControlDrag() {
        if (isGrounded) {
            playerBody.drag = groundDrag;
        } else {
            playerBody.drag = airDrag;
        }
    }

    void Jump() {
        playerBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void MultiplyGravity() {
        if(!isGrounded) {//player is in the air
            if (previousY > transform.position.y) {//player is falling
                playerBody.AddForce(Physics.gravity * (playerBody.mass * gravityMultiplier));//increase gravity
            }
            previousY = transform.position.y;
        }
    }

    void FixedUpdate()
    {
        if (gameManager.isGameActive) {//deactivate player movement upon game over
            MovePlayer();
        }
    }

    void MovePlayer() {
        if (isGrounded) {//on flat ground
            playerBody.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);
        }  else {//in the air
            playerBody.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Acceleration);
        }

        if (!isMoving) {//slow down at a faster rate
            Vector3 playerVelocity = playerBody.velocity;
            playerBody.velocity = new Vector3(playerVelocity.x * deceleration, playerVelocity.y, playerVelocity.z * deceleration);
        }
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Door")) {
            nearbyDoor = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Door")) {
            nearbyDoor = null;
        }
    }

    public void SetCurrentGun(int weaponID) {
        DeActivateGun();
        currentGun = weaponID;
        ActivateGun();
    }

    private void ActivateGun() {
        guns[currentGun].gameObject.SetActive(true);//activate current gun in the heirarchy
    }

    private void DeActivateGun() {
        guns[currentGun].gameObject.SetActive(false);//de-activate current gun in the heirarchy
    }

}
