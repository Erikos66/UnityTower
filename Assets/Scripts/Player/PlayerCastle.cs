using UnityEngine;

public class PlayerCastle : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {
            Destroy(other.gameObject);
        }
    }
}