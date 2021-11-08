using UnityEngine;

/// the mario controller
public class Player: MonoBehaviour {
    void Awake() {
        name = "mario";
    }

    void Start() {
        Debug.Log($"it's a me {name}");
    }
}
