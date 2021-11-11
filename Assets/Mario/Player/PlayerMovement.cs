using UnityEngine;
using UnityEngine.InputSystem;

/// mario's movement
public class PlayerMovement: MonoBehaviour {
    // -- tuning --
    [Header("tuning")]
    [Tooltip("the move speed")]
    [SerializeField] float m_MoveSpeed = 10.0f;

    [Tooltip("the move friction coefficient")]
    [SerializeField] float m_MoveFriction = 1.0f;

    [Tooltip("the dash curve duration")]
    [SerializeField] float m_DashDuration = 0.3f;

    [Tooltip("the dash speed over time")]
    [SerializeField] AnimationCurve m_DashCurve = CurveExt.One();

    [Tooltip("the turn speed in degrees / s")]
    [SerializeField] float m_TurnSpeed = 30.0f;

    [Tooltip("the jump speed")]
    [SerializeField] float m_JumpSpeed = 1.0f;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the character controller")]
    [SerializeField] CharacterController m_Controller;

    [Tooltip("the look direction")]
    [SerializeField] Transform m_Look;

    [Tooltip("the player input asset")]
    [SerializeField] PlayerInput m_Input;

    // -- props --
    /// the dash time
    float? m_DashTime;

    /// the move dir
    Vector2 m_MoveDir;

    /// the current velocity
    Vector3 m_Velocity;

    /// the target rotation
    Quaternion m_Rotation;

    /// the actions
    PlayerActions m_Actions;

    // -- lifecycle --
    void Awake() {
        m_Actions = new PlayerActions(m_Input);
    }

    void Start() {
        var t = transform;
        m_Rotation = t.rotation;
    }

    void Update() {
        Read();
    }

    void FixedUpdate() {
        Play();
    }

    // -- commands --
    /// read input
    void Read() {
        ReadMove();
    }

    /// read the move input
    void ReadMove() {
        // get input dir
        m_MoveDir = m_Actions.MoveDir;

        // if the move is idle, cancel
        if (m_MoveDir == Vector2.zero) {
            m_DashTime = null;
            return;
        }

        // if no move has started, this is a new move
        if (m_DashTime == null) {
            m_DashTime = Time.time;
        }
    }

    /// run movment behavior
    void Play() {
        // apply movement
        Move();
        Drag();

        // move character
        var c = m_Controller;
        c.Move(m_Velocity * Time.deltaTime);

        // turn towards look direction
        if (m_Rotation != Quaternion.identity) {
            var t = c.transform;

            t.rotation = Quaternion.RotateTowards(
                t.rotation,
                m_Rotation,
                m_TurnSpeed * Time.deltaTime
            );
        }
    }

    /// move the player
    void Move() {
        // if there is movement
        if (m_DashTime == null) {
            return;
        }

        // get move speed scaled by progress through dash
        var pct = Mathf.Clamp01((Time.time - m_DashTime.Value) / m_DashDuration);
        var spd = m_DashCurve.Evaluate(pct) * m_MoveSpeed;

        // move using input in player's facing direction
        var dir = Vector3.ProjectOnPlane(m_MoveDir.XZ(), m_Look.up);
        var move = dir * spd;

        // update acceleration
        m_Velocity = move;
        m_Rotation = Quaternion.LookRotation(dir.normalized);
    }

    /// apply friction
    void Drag() {
        // if the player is moving
        if (m_Velocity == Vector3.zero) {
            return;
        }

        // get grounded friction
        var dir = -Vector3.Normalize(m_Velocity);
        var friction = dir * m_MoveFriction;

        // apply friction
        var curr = m_Velocity;
        m_Velocity += friction;

        // zero out velocity if overcome by friction
        if (Vector3.Dot(curr, m_Velocity) < 0.0f) {
            m_Velocity = Vector3.zero;
        }
    }

    // -- PlayerState --
    /// if the player is moving
    public bool IsMoving {
        get => m_DashTime != null;
    }

    /// the player's position
    public Vector3 Position {
        get => m_Controller.transform.position;
    }

    /// the player's velocity scale
    public Vector3 Velocity {
        get => m_Velocity;
    }
}
