using UnityEngine;
using System.Collections;

public class Tower_Crossbow_Visuals : MonoBehaviour {
    [SerializeField] private LineRenderer attackVisuals; // LineRenderer used for displaying the attack visual effect.
    [SerializeField] private float attackVisualsDuration = 0.2f; // Duration to display the attack visual effect.
    private Tower_Crossbow towerCrossbow; // Reference to the Tower_Crossbow component to control rotation.

    [Header("Glowing Visuals")]
    [SerializeField] private MeshRenderer stringRenderer; // MeshRenderer for the string of the crossbow.
    [SerializeField] private Material stringMaterial; // Material for the string of the crossbow.
    [SerializeField] private float currentIntensity = 0.0f; // Current intensity of the emissive material.
    [SerializeField] private float maxIntensity = 1.0f; // Maximum intensity of the emissive material.

    /// <summary>
    /// Unity Awake method for caching the Tower_Crossbow component.
    /// </summary>
    private void Awake() {
        towerCrossbow = GetComponent<Tower_Crossbow>(); // Cache the Tower_Crossbow component.

        stringMaterial = new Material(stringRenderer.material); // Create a new material from the stringRenderer material.

        stringRenderer.material = stringMaterial; // Set the material to the stringRenderer.
    }

    /// <summary>
    /// Plays the attack visual effects.
    /// </summary>
    /// <param name="startpoint">Starting point of the visual effect.</param>
    /// <param name="endpoint">Ending point of the visual effect.</param>
    public void PlayAttackVFX(Vector3 startpoint, Vector3 endpoint) {
        StartCoroutine(ChangeEmission(3));
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

    // New coroutine to increase emissive intensity of the stringMaterial.
    private IEnumerator ChangeEmission(float duration) {
        float startIntensity = 0;
        float startTime = Time.time;
        while (startTime < duration) {
            float tValue = (Time.time - startTime) / duration;
            currentIntensity = Mathf.Lerp(startIntensity, maxIntensity, tValue);
            yield return null;
        }
        currentIntensity = maxIntensity;
    }

}
