using UnityEngine;
using System.Collections;
using System;

// This class handles the visual effects for the Tower_Crossbow including attack effects and emissive color updates.
public class Tower_Crossbow_Visuals : MonoBehaviour {
    [SerializeField] private LineRenderer attackVisuals; // LineRenderer used for displaying the attack visual effect. (Assigned via Inspector)
    [SerializeField] private float attackVisualsDuration = 0.2f; // Duration (in seconds) for which the attack visual effect is displayed.
    private Tower_Crossbow towerCrossbow; // Cached reference to the Tower_Crossbow component for controlling tower rotation.

    [Header("Glowing Visuals")]
    [SerializeField] private MeshRenderer stringRenderer; // MeshRenderer for the crossbow string, used to display glowing effects.
    [SerializeField] private Material stringMaterial; // Material used by the stringRenderer that supports emissive properties.
    [Space]
    private float currentIntensity = 0.0f; // Current intensity level for the emissive visual effect.
    private float maxIntensity = 1.0f; // Maximum emissive intensity achievable.
    [Space]
    [SerializeField] private Color startColor; // Starting color for the emissive effect.
    [SerializeField] private Color endColor; // Ending color representing full emissive intensity.
    [SerializeField] private LineRenderer[] stringRenderers; // Array of LineRenderers for the crossbow strings.

    [Header("Crossbow Line Renderers")]
    [SerializeField] private LineRenderer frontString_L; // LineRenderer for the front string of the crossbow.
    [SerializeField] private LineRenderer frontString_R;
    [SerializeField] private LineRenderer rearString_L; // LineRenderer for the rear string of the crossbow.
    [SerializeField] private LineRenderer rearString_R;

    [Header("Rotar Points")]
    [SerializeField] private Transform frontRotar_L;
    [SerializeField] private Transform frontRotar_R;
    [SerializeField] private Transform rearRotar_L;
    [SerializeField] private Transform rearRotar_R;

    [Header("Crossbow Points")]
    [SerializeField] private Transform frontCrossbow_L;
    [SerializeField] private Transform frontCrossbow_R;
    [SerializeField] private Transform rearCrossbow_L;
    [SerializeField] private Transform rearCrossbow_R;

    [Header("Rotar Visuals")]
    [SerializeField] private Transform rotarUnloaded;
    [SerializeField] private Transform rotarLoaded;
    [SerializeField] private Transform rotor; // New: reference to the rotor to animate.

    /// <summary>
    /// Unity Awake method for caching components and initializing the emissive material.
    /// </summary>
    private void Awake() {
        towerCrossbow = GetComponent<Tower_Crossbow>(); // Cache the Tower_Crossbow component.
        stringMaterial = new Material(stringRenderer.material); // Create a new instance of the string's material.
        stringRenderer.material = stringMaterial; // Assign the new material to the MeshRenderer.
        foreach (LineRenderer stringRenderer in stringRenderers) {
            stringRenderer.material = stringMaterial; // Assign the new material to all LineRenderers.
        }
        StartCoroutine(ChangeEmissionLevel(0.5f)); // Ramp up the emissive effect on awake over 0.5 seconds.
        ReloadDurationVFX(2); // New: trigger the reload visual effect on awake.
    }

    /// <summary>
    /// Plays the attack visual effects and resets the rotor.
    /// </summary>
    /// <param name="startpoint">Starting point of the visual effect.</param>
    /// <param name="endpoint">Ending point of the visual effect.</param>
    public void PlayAttackVFX(Vector3 startpoint, Vector3 endpoint) {
        ResetRotorPosition(); // New: reset rotor position when firing.
        currentIntensity = 0f; // Reset emissive intensity to base (firing state).
        StartCoroutine(VFXCoroutione(startpoint, endpoint)); // Start the coroutine to display attack visuals.
    }

    /// <summary>
    /// Unity Update method to update the emissive color every frame.
    /// </summary>
    private void Update() {
        UpdateEmissionColor(); // Update the material's emissive color based on the current intensity
        ConnectCrossbowStrings();
    }

    /// <summary>
    /// Initiates the reload visual effect including rotor animation.
    /// </summary>
    /// <param name="duration">Reload duration over which the emissive intensity increases.</param>
    public void ReloadDurationVFX(float duration) {
        duration -= 1f;

        StartCoroutine(ChangeEmissionLevel(duration)); // Start reload visual effect (reducing duration by 1 sec).
        StartCoroutine(AnimateRotorReload(duration)); // New: animate rotor movement during reload.
    }

    /// <summary>
    /// Updates the emission color of the string material based on the current emissive intensity.
    /// </summary>
    private void UpdateEmissionColor() {
        Color emissionColor = Color.Lerp(startColor, endColor, currentIntensity / maxIntensity); // Interpolate between start and end colors.
        emissionColor *= Mathf.LinearToGammaSpace(currentIntensity); // Adjust the brightness based on the intensity.
        stringMaterial.SetColor("_EmissionColor", emissionColor); // Apply the calculated color to the material.
    }

    /// <summary>
    /// Coroutine that displays the attack visual effect.
    /// It disables tower rotation, activates the LineRenderer, waits, then resets the state.
    /// </summary>
    /// <param name="startpoint">Starting point of the visual effect.</param>
    /// <param name="endpoint">Ending point of the visual effect.</param>
    /// <returns>IEnumerator for coroutine handling.</returns>
    private IEnumerator VFXCoroutione(Vector3 startpoint, Vector3 endpoint) {
        towerCrossbow.EnableRotation(false); // Disable tower rotation during visual effect.
        attackVisuals.enabled = true; // Enable the LineRenderer.
        attackVisuals.SetPosition(0, startpoint); // Set the start position of the line.
        attackVisuals.SetPosition(1, endpoint);   // Set the end position of the line.
        yield return new WaitForSeconds(attackVisualsDuration); // Wait for the set duration.
        attackVisuals.enabled = false; // Disable the LineRenderer after effect.
        towerCrossbow.EnableRotation(true); // Re-enable tower rotation.
    }

    /// <summary>
    /// Coroutine to gradually increase the emissive intensity of the string material over a given duration.
    /// </summary>
    /// <param name="duration">The time in seconds over which the emission level is increased.</param>
    /// <returns>IEnumerator for coroutine management.</returns>
    private IEnumerator ChangeEmissionLevel(float duration) {
        float startTime = Time.time; // Record the starting time.
        float startIntensity = 0f; // Initial emissive intensity.
        while ((Time.time - startTime) < duration) { // Continue until the duration has passed.
            float tValue = (Time.time - startTime) / duration; // Calculate normalized elapsed time.
            currentIntensity = Mathf.Lerp(startIntensity, maxIntensity, tValue); // Gradually interpolate intensity.
            yield return null; // Wait for next frame.
        }
        currentIntensity = maxIntensity; // Ensure the intensity is at maximum when done.
    }

    // New: resets the rotor position to the starting point.
    private void ResetRotorPosition() {
        if (rotor != null) {
            rotor.position = rotarUnloaded.position;
        }
    }

    // New: coroutine that moves the rotor from start to cocked (rotarEnd) over the reload duration.
    private IEnumerator AnimateRotorReload(float reloadDuration) {
        float startTime = Time.time;

        while (Time.time - startTime < reloadDuration) {
            float tValue = (Time.time - startTime) / reloadDuration;
            rotor.position = Vector3.Lerp(rotarUnloaded.position, rotarLoaded.position, tValue);
            yield return null;
        }

    }

    // New: continuously update the attack visual so it follows the enemy.
    public IEnumerator TrackEnemyVFX(Transform gunPoint, Transform enemyTransform, float duration) {
        float timer = 0f;
        while (timer < duration && enemyTransform != null) {
            attackVisuals.enabled = true;
            attackVisuals.SetPosition(0, gunPoint.position);
            attackVisuals.SetPosition(1, enemyTransform.position);
            timer += Time.deltaTime;
            yield return null;
        }
        attackVisuals.enabled = false;
    }

    public void ConnectCrossbowStrings() {
        // Connect front left: from frontRotar_L to frontCrossbow_L
        if (frontString_L != null && frontRotar_L != null && frontCrossbow_L != null) {
            frontString_L.positionCount = 2;
            frontString_L.SetPosition(0, frontRotar_L.position);
            frontString_L.SetPosition(1, frontCrossbow_L.position);
        }

        // Connect front right: from frontRotar_R to frontCrossbow_R
        if (frontString_R != null && frontRotar_R != null && frontCrossbow_R != null) {
            frontString_R.positionCount = 2;
            frontString_R.SetPosition(0, frontRotar_R.position);
            frontString_R.SetPosition(1, frontCrossbow_R.position);
        }

        // Connect rear left: from rearRotar_L to rearCrossbow_L
        if (rearString_L != null && rearRotar_L != null && rearCrossbow_L != null) {
            rearString_L.positionCount = 2;
            rearString_L.SetPosition(0, rearRotar_L.position);
            rearString_L.SetPosition(1, rearCrossbow_L.position);
        }

        // Connect rear right: from rearRotar_R to rearCrossbow_R
        if (rearString_R != null && rearRotar_R != null && rearCrossbow_R != null) {
            rearString_R.positionCount = 2;
            rearString_R.SetPosition(0, rearRotar_R.position);
            rearString_R.SetPosition(1, rearCrossbow_R.position);
        }
    }
}
