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

    [Tooltip("the turn speed")]
    [SerializeField] float m_TurnSpeed = 1.0f;

    [Tooltip("the jump speed")]
    [SerializeField] float m_JumpSpeed = 1.0f;

    // -- nodes --
    [Header("nodes")]
    [Tooltip("the character controller")]
    [SerializeField] CharacterController m_Controller;

    [Tooltip("the player input asset")]
    [SerializeField] PlayerInput m_Input;

    // -- props --
    /// the dash time
    float? m_DashTime;

    /// the move dir
    Vector2 m_MoveDir;

    /// the current velocity
    Vector3 m_Velocity;

    /// the actions
    PlayerActions m_Actions;

    // -- lifecycle --
    void Awake() {
        m_Actions = new PlayerActions(m_Input);
    }

    void Update() {
        // read input
        ReadMove();
    }

    void FixedUpdate() {
        // apply movement
        Move();
        Drag();

        // move character
        m_Controller.Move(m_Velocity * Time.deltaTime);
    }

    // -- commands --
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

    /// move the player
    void Move() {
        // if there is movement
        if (m_DashTime == null) {
            return;
        }

        var t = m_Controller.transform;

        // get move speed scaled by progress through dash
        var pct = Mathf.Clamp01((Time.time - m_DashTime.Value) / m_DashDuration);
        var spd = m_DashCurve.Evaluate(pct) * m_MoveSpeed;

        // move using y-axis input in player's facing direction
        var dirM = Vector3.ProjectOnPlane(m_MoveDir.XZ(), t.up);
        var move = dirM * spd;

        // update acceleration
        m_Velocity = move;
    }

    /// apply friction
    void Drag() {
        // if the player is moving
        if (m_Velocity == Vector3.zero) {
            return;
        }

        // get grounded friction
        var dirF = -Vector3.Normalize(m_Velocity);
        var friction = dirF * m_MoveFriction;

        // apply friction
        var curr = m_Velocity;
        m_Velocity += friction;

        // zero out velocity if overcome by friction
        if (Vector3.Dot(curr, m_Velocity) < 0.0f) {
            m_Velocity = Vector3.zero;
        }
    }

    // -- queries --
    /// the player's transform
    public Transform Transform {
        get => m_Controller.transform;
    }

    /// the player's velocity scale
    public Vector3 FindVelocityScale() {
        return m_Velocity / m_MoveSpeed;
    }
}
