using UnityEngine;
using UnityEngine.InputSystem;

/// a wrapper for player input
public class PlayerActions {
    // -- props --
    /// the move action
    InputAction m_Move;

    /// the jump action
    InputAction m_Jump;

    // -- lifetime --
    public PlayerActions(PlayerInput input) {
        // set actions
        m_Move = input.currentActionMap["Move"];
        m_Jump = input.currentActionMap["Jump"];
    }

    // -- queries --
    /// the move action
    public InputAction Move {
        get => m_Move;
    }

    /// the move direction
    public Vector2 MoveDir {
        get => m_Move.ReadValue<Vector2>();
    }

    /// the jump action
    public InputAction Jump {
        get => m_Jump;
    }
}
