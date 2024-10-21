using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Platformer character movement
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerCharacter : MonoBehaviour
    {
        public int playerId;

        [Header("Main")]
        public bool isScarecrow;

        [Header("Physics")]
        public float movementSpeed = 1f;
        public float jumpStrength = 100f;
        public bool moveByTorque;
        public Vector2 centerOfMass = new Vector2(0, 0);

        [Header("Connections")]
        public SceneSwitcher sceneSwitcher;

        private Rigidbody2D rigidBody;
        private Collider2D physicsCollider;
        private Collider2D collisionCollider;

        private Vector3 returnPosition;
        private Vector3 start_scale;

        private Vector2 move;
        private Vector2 moveInput;
        private bool jumpPress;

        private static Dictionary<int, PlayerCharacter> characterList = new Dictionary<int, PlayerCharacter>();

        void Awake() {
            characterList[playerId] = this;
            
            rigidBody = GetComponent<Rigidbody2D>();

            if (isScarecrow) {
                physicsCollider = GetComponent<BoxCollider2D>();
                collisionCollider = GetComponent<CapsuleCollider2D>();
            } else {
                physicsCollider = GetComponent<CapsuleCollider2D>();
            }

            start_scale = transform.localScale;
            returnPosition = transform.position;
        }

        void Start() {
            rigidBody.centerOfMass = centerOfMass;
        }

        void OnDrawGizmos() {
            if (rigidBody != null)
            {
                Gizmos.color = Color.red;
                Vector3 com = transform.position + (Vector3)rigidBody.centerOfMass;
                Gizmos.DrawSphere(com, 0.1f);
            }
        }

        void FixedUpdate()
        {
            if (isScarecrow) {
                move.x = -moveInput.x;
            } else {
                move.x = moveInput.x;
            }

            UpdateFacing();

            if (moveByTorque) {
                if (moveInput.x == 0) return;
                rigidBody.AddTorque(-moveInput.x * movementSpeed * Time.deltaTime);
            } else {
                transform.Translate(moveInput.x * movementSpeed * Time.deltaTime, 0, 0);
            }
        }

        void Update() {
            PlayerControls controls = PlayerControls.Get(playerId);
            moveInput = controls.GetMove();
            jumpPress = controls.GetJumpDown();

            if (jumpPress || moveInput.y > 0.5f) Jump();
        }

        private void UpdateFacing()
        {
            if (Mathf.Abs(move.x) > 0.01f)
            {
                float side = (move.x < 0f) ? -1f : 1f;
                transform.localScale = new Vector3(start_scale.x * side, start_scale.y, start_scale.z);
            }
        }

        public void Jump(bool force_jump = false)
        {
            Debug.Log("bebra");

            Vector2 direction = transform.TransformDirection(Vector2.up);
            rigidBody.AddForce(direction * jumpStrength, ForceMode2D.Force);
        }

        private bool DetectGrounded(bool detect_ceiled)
        {
            return true;
        }

        public void Teleport(Vector3 pos)
        {
            // rigidBody.angularVelocity = 0f;
            // rigidBody.velocity = Vector2.zero;
            // transform.position = pos;
            // transform.rotation = new Quaternion(0, 0, 0, 0);
            // move = Vector2.zero;

            sceneSwitcher.StartSwitchScene("TheFarm");
        }

        public Vector2 GetMove()
        {
            return move;
        }

        public Vector2 GetFacing()
        {
            return Vector2.right * Mathf.Sign(transform.localScale.x);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.tag == "Ground" && collisionCollider.IsTouching(collision.collider)) {
                Debug.Log("Fell");
                Teleport(returnPosition);
            }
        }

        public static PlayerCharacter Get(int playerId)
        {
            foreach (PlayerCharacter character in GetAll())
            {
                if (character.playerId == playerId)
                {
                    return character;
                }
            }
            return null;
        }

        public static PlayerCharacter[] GetAll()
        {
            PlayerCharacter[] list = new PlayerCharacter[characterList.Count];
            characterList.Values.CopyTo(list, 0);
            return list;
        }
    }

}