using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameManager gameManager;

    private float xSensitivity = 100f;
    private float xMultiplier = 1f;
    private float ySensitivity = 100f;
    private float yMultiplier = 1f;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;

    private float xRotation;
    private float yRotation;
    private float xRange = 60f;
    private float sensitivityMultiplier = 0.01f;

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive) {
            MouseMovement();

            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    void MouseMovement() {

        float xInput = Input.GetAxisRaw("Mouse X");
        float yInput = Input.GetAxisRaw("Mouse Y");
        yRotation += xInput * (xSensitivity * xMultiplier) * sensitivityMultiplier;
        xRotation -= yInput * (ySensitivity * yMultiplier) * sensitivityMultiplier;

        xRotation = Mathf.Clamp(xRotation, -xRange, xRange);
    }

    public void SetXMultiplier(float xMultiplier) {
        this.xMultiplier = xMultiplier;
        gameManager.SetXMultiplierText(this.xMultiplier);
    }

    public void SetYMultiplier(float yMultiplier) {
        this.yMultiplier = yMultiplier;
        gameManager.SetYMultiplierText(this.yMultiplier);
    }
}
