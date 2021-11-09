using UnityEngine;

/// mario
public class Player: MonoBehaviour {
    void Awake() {
        name = "Mario";
    }

    void Start() {
        Debug.Log($"it's a me {name.ToLower()}");
    }
}
