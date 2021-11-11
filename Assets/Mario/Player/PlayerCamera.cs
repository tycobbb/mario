using UnityEngine;

/// a camera the follows the player
public class PlayerCamera: MonoBehaviour {
    // -- tuning --
    [Header("tuning")]
    [Tooltip("the arm to the player on the XY (???) plane")]
    [SerializeField] Vector2 m_Arm = new Vector2(10.0f, 5.0f);

    [Tooltip("the maximum adjustment when the player moves")]
    [SerializeField] float m_Adjustment = 0.1f;

    [Tooltip("the camera smoothing duration")]
    [SerializeField] float m_SmoothTime = 0.1f;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the camera")]
    [SerializeField] Camera m_Camera;

    [Tooltip("the player state")]
    [SerializeField] PlayerMovement m_Player;

    // -- props --
    /// the target position
    Vector3 m_Position;

    /// the target rotation
    Quaternion m_Rotation;

    /// the time the last follow started
    float m_FollowTime;

    // -- lifecycle --
    void OnDrawGizmos() {
        var t = transform;
        var pos = t.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.5f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(pos, pos + t.forward * m_Arm.magnitude);
    }

    void Update() {
        if (!Application.isPlaying) {
            Play();
        }
    }

    void FixedUpdate() {
        Play();
    }

    // -- commands --
    /// run camera behavior
    void Play() {
        Move();
        Follow();
    }

    /// move the camera into position
    void Move() {
        var rt = transform;
        var ct = m_Camera.transform;

        var pct = Mathf.Clamp01((Time.time - m_FollowTime) / m_SmoothTime);
        rt.position = Vector3.Lerp(rt.position, m_Position, pct);
        ct.rotation = Quaternion.Slerp(ct.rotation, m_Rotation, pct);
    }

    /// follow the player's movement
    void Follow() {
        var rt = transform;
        var ct = m_Camera.transform;

        // add the camera arm to player's pos
        var p0 = m_Player.Position;
        var p1 = p0;
        p1 -= Vector3.forward * m_Arm.x;
        p1.y += m_Arm.y;

        // calculate look
        var fwd = p0 - p1;

        // adjust for player velocity
        p1 += m_Player.Velocity * m_Adjustment;

        // update state
        rt.position = p1;
        ct.rotation = Quaternion.LookRotation(fwd, Vector3.up);

        // keep following as long as the player is moving
        if (m_Player.IsMoving) {
            m_FollowTime = Time.time;
        }
    }
}
