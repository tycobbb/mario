using UnityEngine;

/// a camera the follows the player
[ExecuteAlways]
public class PlayerCamera: MonoBehaviour {
    // -- tuning --
    [Header("tuning")]
    [Tooltip("the arm to the player on the XY (???) plane")]
    [SerializeField] Vector2 m_Arm = new Vector2(10.0f, 5.0f);

    [Tooltip("the maximum adjustment when the player moves")]
    [SerializeField] float m_AdjustmentMax = 1.0f;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the player")]
    [SerializeField] Player m_Player;

    // -- lifecycle --
    void Update() {
        if (!Application.isPlaying) {
            FixedUpdate();
        }
    }

    void FixedUpdate() {
        Track();
    }

    void OnDrawGizmos() {
        var t = transform;
        var pos = t.position;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.5f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(pos, pos + t.forward * m_Arm.magnitude);
    }

    // -- commands --
    /// track the player's movement
    void Track() {
        var ct = transform;
        var pt = m_Player.transform;

        // add the camera arm to the player's pos
        var p0 = pt.position;
        var p1 = p0;
        p1 -= pt.forward * m_Arm.x;
        p1.y += m_Arm.y;

        // update position
        ct.position = p1;
        ct.LookAt(pt);

        // adjust for player velocity
        var pv = m_Player.FindVelocityScale();
        var cr = ct.right;
        p1 += cr * Vector3.Dot(pv, cr) * m_AdjustmentMax;
        ct.position = p1;
    }
}
