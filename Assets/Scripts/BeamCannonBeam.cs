using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCannonBeam : MonoBehaviour
{
    private LineRenderer line;
    private Material lineMaterial;
    private float fadePerSec = 3f;
    private int beamDamageEnemy = 100;
    private int beamDamagePlayer = 50;

    private void Awake() {
        line = GetComponent<LineRenderer>();
        lineMaterial = line.material;
    }

    private void Update() {
        Color lineColor = lineMaterial.color;
        if (lineColor.a <= 0) {//once the lazer is no longer visible
            lineMaterial.color = new Color(lineColor.r, lineColor.g, lineColor.b, 1f);//bring back the faded material
            gameObject.SetActive(false);//disable the lazer
        } else {
            lineMaterial.color = new Color(lineColor.r, lineColor.g, lineColor.b, lineColor.a - (fadePerSec * Time.deltaTime));//fade out the lazer over time
        }
    }

    public void SetPoints(Vector3 startPoint, Vector3 endPoint) {
        Vector3[] linePositions = {startPoint, endPoint};//set the start and end points of the lazer
        line.SetPositions(linePositions);//apply the points
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy")) {
            other.GetComponent<EnemyStats>().DamageEnemy(beamDamageEnemy);
        } else if (other.CompareTag("Player")) {
            other.GetComponent<PlayerStats>().DamagePlayer(beamDamagePlayer);
        }
    }

}
