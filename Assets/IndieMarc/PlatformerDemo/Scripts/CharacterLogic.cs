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
    [RequireComponent(typeof(CharacterStory))]
    public class CharacterLogic : MonoBehaviour
    {
        public int playerId;

        [Header("Main")]
        public bool isScarecrow;
        public bool isEnabled;

        [Header("Physics")]
        public float movementSpeed = 1f;
        public float jumpStrength = 100f;
        public bool moveByTorque;
        public Vector2 centerOfMass = new Vector2(0, 0);

        [Header("Connections")]
        public SceneSwitcher sceneSwitcher;
        public Bubble speechBubble;

        [HideInInspector]
        public bool isSpeaking = false;

        private GameObject anchor;

        private CharacterStory story;
        private Rigidbody2D rigidBody;
        private Collider2D physicsCollider;
        private Collider2D collisionCollider;

        private Collider2D activeCollision;

        private Vector3 start_scale;

        private Vector2 move;
        private Vector2 moveInput;
        private bool jumpPress;
        private bool actionPress;
        private bool isGrounded = true;

        private static readonly Dictionary<int, CharacterLogic> characterList = new();

        void Awake() {
            characterList[playerId] = this;
            
            rigidBody = GetComponent<Rigidbody2D>();
            story = GetComponent<CharacterStory>();
            anchor = gameObject.transform.Find("Anchor").gameObject;

            if (isScarecrow) {
                physicsCollider = GetComponent<BoxCollider2D>();
                collisionCollider = GetComponent<CapsuleCollider2D>();
                
                if (GameProgress.enteredScarecrow) {
                    isEnabled = true;
                    rigidBody.bodyType = RigidbodyType2D.Dynamic;
                }
            } else {
                physicsCollider = GetComponent<CapsuleCollider2D>();

                if (GameProgress.enteredScarecrow) isEnabled = false;
            }

            start_scale = transform.localScale;
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

        void FixedUpdate() {
            if (isSpeaking || !isEnabled) return;

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
            if (!isEnabled) return;

            CharacterControl controls = CharacterControl.Get(playerId);
            moveInput = controls.GetMove();
            jumpPress = controls.GetJumpDown();
            actionPress = controls.GetActionDown();

            if (jumpPress || moveInput.y > 0.5f) Jump();

            if (actionPress) {
                if (activeCollision) {
                    Bubble veggie = activeCollision.gameObject.GetComponent<Bubble>();
                    CallBubble("Вкусное");
                    veggie?.Hide();
                } else if (isSpeaking) {
                    speechBubble.Hide();
                    isSpeaking = false;
                } else if (story.currentEventId == "awaiting_scarecrow") {
                    GameProgress.enteredScarecrow = true;
                    GameProgress.stories["fox"] = "awaiting_scarecrow";
                    Reload();
                }
            }
        }

        public void CallBubble(string text) {
            speechBubble.Call(text, anchor);
            isSpeaking = true;
        }

        private void UpdateFacing()
        {
            if (Mathf.Abs(move.x) > 0.01f)
            {
                float side = (move.x < 0f) ? -1f : 1f;
                transform.localScale = new Vector3(start_scale.x * side, start_scale.y, start_scale.z);
            }
        }

        public void Jump()
        {
            if (!isGrounded || isSpeaking) return;

            Vector2 direction = transform.TransformDirection(Vector2.up);
            rigidBody.AddForce(direction * jumpStrength, ForceMode2D.Force);
        }

        public void Reload() {
            sceneSwitcher.StartSwitchScene("TheFarm");
        }

        public Vector2 GetMove() {
            return move;
        }

        public Vector2 GetFacing() {
            return Vector2.right * Mathf.Sign(transform.localScale.x);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (isScarecrow && collision.gameObject.tag == "Ground") {
                if (collisionCollider.IsTouching(collision.collider)) {
                    CallBubble("ау(");
                    Reload();
                } else if (physicsCollider.IsTouching(collision.collider)) {
                    isGrounded = true;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision) {
            if (collision.gameObject.tag == "Ground") {
                if (!physicsCollider.IsTouching(collision.collider)) {
                    isGrounded = false;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (isScarecrow && collision.gameObject.tag == "Veggie") {
                activeCollision = collision;
            } else if (collision.gameObject.tag == "SceneSwitcher") {
                SceneSwitchTrigger trigger = collision.gameObject.GetComponent<SceneSwitchTrigger>();
                sceneSwitcher.StartSwitchScene(trigger.targetScene);
            }
        }

        private void OnTriggerExit2D() {
            activeCollision = null;
        }

        public static CharacterLogic Get(int playerId)
        {
            foreach (CharacterLogic character in GetAll())
            {
                if (character.playerId == playerId)
                {
                    return character;
                }
            }
            return null;
        }

        public static CharacterLogic[] GetAll()
        {
            CharacterLogic[] list = new CharacterLogic[characterList.Count];
            characterList.Values.CopyTo(list, 0);
            return list;
        }
    }

}