// using System.Collections;
// using System.Collections.Generic;
// using MutCommon;
// using UnityEngine;
// using UnityAtoms.BaseAtoms;
// using UnityEngine.Serialization;

// // someone else's ideas
// // Platformer stuff
// namespace MutPlatformer
// {
//     public enum PlatformerState
//     {
//         Idle,
//         StartingMovement,
//         Moving,
//         Falling,
//         StoppingMovement,
//     }

//     public class PlatformerMovement : MonoBehaviour
//     {
//         [Header("References")]
//         public CharacterModel CharacterModel;
//         private Transform model => CharacterModel?.Model ?? this.transform;

//         //public PlayerData data;
//         [Header("Tunables - Movement")]
//         [FormerlySerializedAs("Speed")]
//         [FormerlySerializedAs("maxSpeed")]
//         [SerializeField] private FloatReference groundedSpeedMax;
//         [FormerlySerializedAs("TurnSpeed")]
//         [FormerlySerializedAs("turnSpeed")]
//         [SerializeField] private FloatReference groundedTurnSpeed;
//         [FormerlySerializedAs("MinSpeedToTurn")]
//         [SerializeField] private FloatReference minSpeedToTurn;
//         [SerializeField] private FloatReference startMovingTime;
//         // [SerializeField] private AnimationCurve startMovingCurve;
//         [SerializeField] private FloatReference stopMovingTime;
//         // [SerializeField] private AnimationCurve stopMovingCurve;

//         [Header("Tunables - Jump")]
//         [FormerlySerializedAs("JumpSpeed")]
//         [FormerlySerializedAs("jumpSpeed")]
//         [SerializeField] private FloatReference jumpSpeedVertical;
//         [SerializeField] private FloatReference jumpSpeedPlanar;
//         [FormerlySerializedAs("Gravity")]
//         [SerializeField] private FloatReference gravity;
//         [FormerlySerializedAs("AirSpeed")]
//         [FormerlySerializedAs("airSpeed")]
//         [SerializeField] private FloatReference airSpeedMax;
//         [SerializeField] private FloatReference airTurnSpeed;
//         [SerializeField] private FloatReference airInputMaxAcceleration;
//         [SerializeField] private AnimationCurve airInputDecelerationCurve;

//         [Header("Tunables - Buffers")]
//         [Tooltip("Coyote time (in seconds), aka how long the character can still jump after they leave the ground without jumping")]
//         [FormerlySerializedAs("GroundedBuffer")]
//         [SerializeField] private FloatReference groundedBuffer;
//         [Tooltip("How long (in seconds) before touching the ground the player can press jump so that the character immediately jumps on land")]
//         [FormerlySerializedAs("JumpInputBuffer")]
//         [SerializeField] private FloatReference jumpInputBuffer;

//         [Header("Output")]
//         [SerializeField] private FloatReference speedPlanar;
//         [SerializeField] private FloatReference speedY;

//         [Header("Animator")]
//         [FormerlySerializedAs("RunningSpeedAnimation")]
//         [SerializeField] private FloatReference animMoveSpeedBlendMaxValue;

//         private CharacterController charController;
//         [Header("Debug")]
//         [SerializeField] private PlatformerState state;

//         private Vector3 modelInitialPosition;

//         /// Direction of the characters movement, will always have y = 0
//         public Vector3 HeadingDirection
//         {
//             get => isBlocked ? Vector3.zero : headingDirection;
//             set
//             {
//                 headingDirection = Vector3.ProjectOnPlane(value, Vector3.up).normalized;
//             }
//         }
//         private Vector3 headingDirection;
//         private Vector3 movingDirection;
//         private Vector3 fallingDirection;

//         private float timeNotGrounded;
//         private bool isJumping;
//         private bool wasRecentlyGrounded => timeNotGrounded <= groundedBuffer.Value;
//         private bool isGrounded => !isJumping && wasRecentlyGrounded;
//         private float turnSpeed => isGrounded ? groundedTurnSpeed.Value : airTurnSpeed.Value;

//         private bool shouldJump;
//         private Coroutine jumpInputBufferCoroutine;
//         public void TryJump()
//         {
//             shouldJump = true;
//             if (jumpInputBufferCoroutine != null) StopCoroutine(jumpInputBufferCoroutine);
//             jumpInputBufferCoroutine = StartCoroutine(
//               CoroutineHelpers.DoAfterTimeCoroutine(
//                 jumpInputBuffer.Value,
//                 () => shouldJump = false
//               ));
//         }


//         private Vector3 direction = Vector3.zero;

//         private bool isBlocked;

//         public void Block() => isBlocked = true;
//         public void Unblock() => isBlocked = false;

//         // Lifecycle
//         private void Awake()
//         {
//             charController = GetComponent<CharacterController>();
//             state = PlatformerState.Idle;
//             modelInitialPosition = model.transform.localPosition;
//         }

//         private void OnValidate()
//         {
//             if (CharacterModel == null)
//             {
//                 CharacterModel = GetComponentInChildren<CharacterModel>();
//             }
//         }

//         float startMoveTimer = 0;
//         float stopMoveTimer = 0;
//         float airSpeed = 0;
//         public float preMovePlanarSpeed = 0;
//         private void FixedUpdate()
//         {
//             // if (isBlocked) return;
//             if (charController.isGrounded)
//             {
//                 timeNotGrounded = 0.0f;
//                 isJumping = false;

//                 // TODO: apparently this breaks if the gameobject rotates
//                 float heading = Mathf.Atan2(HeadingDirection.z, headingDirection.x);
//                 float charFrontHeading = Mathf.Atan2(transform.right.z, transform.right.x);
//                 bool isFacingRightDirection = Mathf.Abs(charFrontHeading) - Mathf.Abs(heading) <= 0.5f;

//                 if (HeadingDirection.sqrMagnitude > 0.001f && isFacingRightDirection)
//                 {
//                     stopMoveTimer = 0;
//                     startMoveTimer += Time.deltaTime;
//                     var t = startMovingTime.Value == 0 ? 1 : startMoveTimer / startMovingTime.Value;
//                     t = Mathf.Clamp01(t);
//                     // var k = t*t*t*t;
//                     var k = t;
//                     preMovePlanarSpeed = Mathf.Lerp(0, groundedSpeedMax.Value, k);
//                     movingDirection = HeadingDirection;
//                 }
//                 else
//                 {
//                     startMoveTimer = 0;
//                     stopMoveTimer += Time.deltaTime;
//                     var t = stopMovingTime.Value == 0 ? 1 : stopMoveTimer / stopMovingTime.Value;
//                     t = Mathf.Clamp01(t);
//                     var k = t;
//                     preMovePlanarSpeed = Mathf.Lerp(groundedSpeedMax.Value, 0, k);
//                 }

//                 direction = preMovePlanarSpeed * movingDirection;
//             }
//             else
//             {
//                 if (!isJumping)
//                 {
//                     fallingDirection = direction.normalized;
//                 }
//                 timeNotGrounded += Time.deltaTime;
//                 var inputDot = Vector3.Dot(fallingDirection, HeadingDirection);
//                 var acceleration = airInputMaxAcceleration.Value * airInputDecelerationCurve.Evaluate(inputDot);
//                 airSpeed += acceleration * Time.deltaTime;

//                 airSpeed = Mathf.Clamp(airSpeed, -airSpeedMax.Value, airSpeedMax.Value);
//                 direction.x = fallingDirection.x * airSpeed;
//                 direction.z = fallingDirection.z * airSpeed;
//             }

//             if (wasRecentlyGrounded && !isJumping && shouldJump)
//             {
//                 shouldJump = false;
//                 isJumping = true;

//                 // TODO: remove (atoms event?)
//                 CharacterModel?.Anim?.SetTrigger("Jump");

//                 direction.y = jumpSpeedVertical.Value;
//                 fallingDirection = new Vector3(direction.x, 0, direction.z).normalized;
//                 direction.x = jumpSpeedPlanar.Value;
//                 direction.z = jumpSpeedPlanar.Value;
//                 //GameEvents.OnPlayerJumpEvent();
//             }

//             direction.y -= gravity.Value * Time.deltaTime;

//             var from = Vector3.Scale(
//              model.forward,
//              new Vector3(1, 0, 1)
//             );

//             var to = Vector3.Scale(
//              direction,
//              new Vector3(1, 0, 1)
//             );

//             if (to.magnitude > minSpeedToTurn.Value)
//             {
//                 model.rotation = Quaternion.Slerp(
//                   model.rotation,
//                   model.rotation * Quaternion.Euler(Vector3.Scale(Quaternion.FromToRotation(from, to).eulerAngles, Vector3.up)),
//                   turnSpeed * Time.deltaTime);
//             }

//             charController.Move(direction * Time.deltaTime);

//             // Animator update
//             speedPlanar.Value = Vector3.ProjectOnPlane(charController.velocity, Vector3.up).magnitude;
//             speedY.Value = charController.velocity.y;
//             CharacterModel?.Anim?.SetFloat("Speed", speedPlanar.Value / animMoveSpeedBlendMaxValue);

//             CharacterModel?.Anim?.SetBool("IsGrounded", isGrounded);

//             // prevent model from drifting
//             model.transform.localPosition = Vector3.Lerp(model.transform.localPosition, modelInitialPosition, 0.5f);
//         }
//     }
// }