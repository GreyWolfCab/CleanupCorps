using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUIElements : MonoBehaviour
{
    public RectTransform[] uiTransforms;

    private void Awake() {
        uiTransforms = new RectTransform[transform.childCount];
        for (int i = 0; i < uiTransforms.Length; i++) {
            uiTransforms[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    private void Start() {
        ResizeScreen();
    }

    public void ResizeScreen() {
        for (int i = 0; i < uiTransforms.Length; i++) {
            uiTransforms[i].sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }
}
