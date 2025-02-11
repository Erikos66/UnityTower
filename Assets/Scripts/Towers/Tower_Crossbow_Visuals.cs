using UnityEngine;
using System.Collections;

public class Tower_Crossbow_Visuals : MonoBehaviour {
    [SerializeField] private LineRenderer attackVisuals;
    [SerializeField] private float attackVisualsDuration = 0.2f;
    private Tower_Crossbow towerCrossbow;

    private void Awake() {
        towerCrossbow = GetComponent<Tower_Crossbow>();
    }

    public void PlayAttackVFX(Vector3 startpoint, Vector3 endpoint) {
        StartCoroutine(VFXCoroutione(startpoint, endpoint));
    }

    private IEnumerator VFXCoroutione(Vector3 startpoint, Vector3 endpoint) {
        towerCrossbow.EnableRotation(false);
        attackVisuals.enabled = true;
        attackVisuals.SetPosition(0, startpoint);
        attackVisuals.SetPosition(1, endpoint);

        yield return new WaitForSeconds(attackVisualsDuration);
        attackVisuals.enabled = false;
        towerCrossbow.EnableRotation(true);
    }
}
