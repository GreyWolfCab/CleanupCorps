using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public bool isGameActive;

    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI waveTextOutline;
    [SerializeField] private TextMeshProUGUI enemyText;
    [SerializeField] private TextMeshProUGUI enemyTextOutline;
    [SerializeField] private TextMeshProUGUI bankText;
    [SerializeField] private TextMeshProUGUI bankTextOutline;
    [SerializeField] private TextMeshProUGUI openDoorText;
    [SerializeField] private TextMeshProUGUI openDoorTextOutline;

    [SerializeField] private GameObject openDoorObject;
    [SerializeField] private GameObject openDoorOutlineObject;
    [SerializeField] private GameObject gameOverUI;

    [Header("Pause-Game UI")]
    [SerializeField] private TextMeshProUGUI xMultiplierText;
    [SerializeField] private TextMeshProUGUI xMultiplierTextOutline;
    [SerializeField] private TextMeshProUGUI yMultiplierText;
    [SerializeField] private TextMeshProUGUI yMultiplierTextOutline;

    [SerializeField] private GameObject pauseGameUI;

    [Header("WeaponWheel UI")]
    [SerializeField] private GameObject weaponWheelUI;

    // Start is called before the first frame update
    void Start()
    {
        SetWaveText(0);
        SetEnemyText(0);
        SetBankText(0);
        gameOverUI.SetActive(false);
        isGameActive = true;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetWaveText(int wave) {
        waveText.text = wave.ToString();
        waveTextOutline.text = waveText.text;
    }

    public void SetEnemyText(int enemy) {
        enemyText.text = enemy.ToString();
        enemyTextOutline.text = enemyText.text;
    }

    public void SetBankText(int bank) {
        bankText.text = "* " + bank.ToString();
        bankTextOutline.text = bankText.text;
    }

    public void SetDoorText(int door) {
        openDoorText.text = "E to open door for * " + door.ToString();
        openDoorTextOutline.text = openDoorText.text;
    }

    public void SetXMultiplierText(float xMultiplier) {
        xMultiplierText.text = "X Sensitivity: " + xMultiplier.ToString("0.00");
        xMultiplierTextOutline.text = xMultiplierText.text;
    }

    public void SetYMultiplierText(float yMultiplier) {
        yMultiplierText.text = "Y Sensitivity: " + yMultiplier.ToString("0.00");
        yMultiplierTextOutline.text = yMultiplierText.text;
    }

    public void ShowDoorText() {
        openDoorObject.SetActive(true);//show open door ui
        openDoorOutlineObject.SetActive(true);
    }

    public void HideDoorText() {
        openDoorObject.SetActive(false);//hide open door ui
        openDoorOutlineObject.SetActive(false);
    }

    public void PauseGame() {
        FreezeGame();
        pauseGameUI.SetActive(true);//display pause menu
    }

    public void UnPauseGame() {
        UnFreezeGame();
        pauseGameUI.SetActive(false);//hide pause menu
    }

    public void ShowWeaponWheel() {
        FreezeGame();
        weaponWheelUI.SetActive(true);//display weapon wheel menu
    }

    public void HideWeaponWheel() {
        UnFreezeGame();
        weaponWheelUI.SetActive(false);//hide weapon wheel menu
    }

    private void FreezeGame() {
        Time.timeScale = 0;//pause the game
        isGameActive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void UnFreezeGame() {
        Time.timeScale = 1;
        isGameActive = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        Time.timeScale = 0;
        isGameActive = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gameOverUI.SetActive(true);
    }
}
