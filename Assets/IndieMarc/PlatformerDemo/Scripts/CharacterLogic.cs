using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace IndieMarc.Platformer {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Speaker))]
    public class CharacterLogic : MonoBehaviour {
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
        public StoryManager storyManager;
        public FarmerLogic farmer;

        private GameObject anchor;

        private SpriteSwitcher spriteSwitcher;
        private Speaker speaker;
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
        private bool isBusted = false;
        private int veggiesTaken = 0;

        private static readonly Dictionary<int, CharacterLogic> characterList = new();

        void Awake() {
            characterList[playerId] = this;
            
            rigidBody = GetComponent<Rigidbody2D>();
            speaker = GetComponent<Speaker>();
            anchor = gameObject.transform.Find("Anchor").gameObject;

            if (isScarecrow) {
                physicsCollider = GetComponent<BoxCollider2D>();
                collisionCollider = GetComponent<CapsuleCollider2D>();
                spriteSwitcher = GetComponent<SpriteSwitcher>();
                
                if (GameProgress.IsTrue("in_scarecrow")) {
                    isEnabled = true;
                    rigidBody.bodyType = RigidbodyType2D.Dynamic;
                    spriteSwitcher.Switch(1);

                    if (GameProgress.Get("day") != "1") {
                        if (!GameProgress.IsTrue("has_been_reset")) storyManager.RunStoryline("scarecrow_first_time");
                    } else if (GameProgress.Get("day") == "1") {
                        if (!GameProgress.IsTrue("has_been_reset")) storyManager.RunStoryline("scarecrow_second_time");
                        farmer.Resurrect();
                    }
                }
            } else {
                physicsCollider = GetComponent<CapsuleCollider2D>();

                if (GameProgress.IsTrue("in_scarecrow")) {
                    gameObject.SetActive(false);
                    isEnabled = false;
                } else {
                    if (GameProgress.Get("scene") == "TheDen") {
                        if (GameProgress.IsTrue("is_night")) {
                            storyManager.RunStoryline("fox_first_night");
                        } else {
                            if (GameProgress.Get("day") == "1") {
                                storyManager.RunStoryline("fox_first_day");
                            }
                        }
                    } else if (GameProgress.Get("scene") == "TheFarm") {
                        if (GameProgress.Get("day") != "1") {
                            storyManager.RunStoryline("fox_first_time");
                        } else if (GameProgress.Get("day") == "1") {
                            storyManager.RunStoryline("fox_second_time");
                        }
                    } else {
                        storyManager.RunStoryline("fox_awake");
                    }
                }
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
            if (speaker.isSpeaking || !isEnabled) return;

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
            CharacterControl controls = CharacterControl.Get(playerId);
            moveInput = controls.GetMove();
            jumpPress = controls.GetJumpDown();
            actionPress = controls.GetActionDown();

            if (isScarecrow && isBusted && actionPress) {
                CallBubble("АААААА!!!!");
                GameProgress.Set("has_been_reset", "true");
                Reload();
            }

            if (veggiesTaken >= 3 && GameProgress.IsTrue("wolf_summon_attempted")) {
                GameProgress.Reset();
                GameProgress.Set("next_bg_index", "1");
                GameProgress.Set("is_night", "true");
                sceneSwitcher.StartSwitchScene("TheDen");
            }

            if (!isEnabled) return;

            if (isScarecrow && !farmer.isSleeping && !storyManager.isRunning && (moveInput.x != 0 || jumpPress)) {
                farmer.HeartAttack();
                isEnabled = false;
                isBusted = true;
            }

            if (jumpPress) Jump();

            if (actionPress) {
                if (storyManager.isRunning) {
                    storyManager.ResetSpeaking();
                } else if (activeCollision) {
                    GameObject gameObject = activeCollision.gameObject;
                    Bubble bubble = gameObject.GetComponent<Bubble>();
                    bubble?.Hide(false);

                    if (gameObject.tag == "Veggie") {
                        bubble?.Hide(true);

                        veggiesTaken++;

                        if (veggiesTaken >= 3) {
                            storyManager.RunStoryline("scarecrow_ate_all");
                            GameProgress.Set("total_veggies_taken", "3");
                        } else {
                            storyManager.RunStoryline("scarecrow_ate");
                        }
                    } else if (gameObject.tag == "HouseEntry") {
                        storyManager.RunStoryline("early_house_enter");
                    } else if (gameObject.tag == "WolfEntry") {
                        if (!GameProgress.IsTrue("wolf_summon_attempted")) {
                            storyManager.RunStoryline("early_wolf_summon");
                        } else {
                            Debug.Log("aaa");
                            storyManager.RunStoryline("second_wolf_summon");
                        }
                    } else if (gameObject.tag == "Bed") {
                        GameProgress.Unset("is_night");
                        GameProgress.Set("day", "1");
                        sceneSwitcher.StartSwitchScene("TheDen");
                    }
                } else if (storyManager.currentEventId == "awaiting_scarecrow") {
                    GameProgress.entries["in_scarecrow"] = "true";
                    Reload();
                }
            }
        }

        public void CallBubble(string text) {
            storyManager.speechBubble.Call(text, anchor);
            speaker.isSpeaking = true;
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
            if (!isGrounded || speaker.isSpeaking) return;

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
            if (isScarecrow && isEnabled && collision.gameObject.tag == "Ground") {
                if (collisionCollider.IsTouching(collision.collider)) {
                    CallBubble("Ouch :(");
                    GameProgress.Set("has_been_reset", "true");
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

        private void OnTriggerEnter2D(Collider2D collider) {
            if (collider is CircleCollider2D) {
                activeCollision = collider;
            } else if (collider.gameObject.tag == "SceneSwitcher") {
                SceneSwitchTrigger trigger = collider.gameObject.GetComponent<SceneSwitchTrigger>();
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