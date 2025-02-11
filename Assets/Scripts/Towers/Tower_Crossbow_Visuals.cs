using UnityEngine;
using System.Collections;

public class Tower_Crossbow_Visuals : MonoBehaviour {
    [SerializeField] private LineRenderer attackVisuals; // LineRenderer used for displaying the attack visual effect.
    [SerializeField] private float attackVisualsDuration = 0.2f; // Duration to display the attack visual effect.
    private Tower_Crossbow towerCrossbow; // Reference to the Tower_Crossbow component to control rotation.

    /// <summary>
    /// Unity Awake method for caching the Tower_Crossbow component.
    /// </summary>
    private void Awake() {
        towerCrossbow = GetComponent<Tower_Crossbow>();
    }

    /// <summary>
    /// Plays the attack visual effects.
    /// </summary>
    /// <param name="startpoint">Starting point of the visual effect.</param>
    /// <param name="endpoint">Ending point of the visual effect.</param>
    public void PlayAttackVFX(Vector3 startpoint, Vector3 endpoint) {
        StartCoroutine(VFXCoroutione(startpoint, endpoint));
    }

    /// <summary>
    /// Coroutine that displays the attack visual effect, disables tower rotation during the effect, then re-enables it.
    /// </summary>
    /// <param name="startpoint">Starting point of the visual effect.</param>
    /// <param name="endpoint">Ending point of the visual effect.</param>
    /// <returns>IEnumerator for coroutine handling.</returns>
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
