using UnityEngine;

/// mario
public class Player: MonoBehaviour {
    // -- nodes --
    [Header("nodes")]
    [Tooltip("the player's movement")]
    [SerializeField] PlayerMovement m_Movement;

    // -- lifecycle --
    void Awake() {
        name = "Mario";
    }

    void Start() {
        Debug.Log($"it's a me {name.ToLower()}");
    }

    // -- queries --
    /// the player's transform
    public Transform Transform {
        get => m_Movement.Transform;
    }

    /// the player's velocity scale
    public Vector3 FindVelocityScale() {
        return m_Movement.FindVelocityScale();
    }
}
