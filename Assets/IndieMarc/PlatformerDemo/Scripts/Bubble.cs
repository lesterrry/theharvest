#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace IndieMarc.Platformer {
    [RequireComponent(typeof(SpriteRenderer))]
    public class Bubble : MonoBehaviour {

        public bool isEnabled;
        public bool isExpectingProximity;
        public bool showHideAnimation = false;
        public GameObject? anchor;
        
        private Text? textElement;
        private Animator? animator;
        private Collider2D immediateCollider;
        private Collider2D proximityCollider;

        private bool inProximity = false;

        void Awake() {
            textElement = GetComponentInChildren<Text>();
            animator = GetComponent<Animator>();
            immediateCollider = GetComponent<CircleCollider2D>();
            proximityCollider = GetComponent<CapsuleCollider2D>();
            UpdateAppearance();
        }

        void Update() {
            if(anchor != null) { 
                Vector2 newPosition = new Vector2(anchor.transform.position.x + 1, anchor.transform.position.y + 1.75f);
                transform.position = newPosition;
            }
        }

        public void Hide(bool permanently = true) {
            if (!permanently && isExpectingProximity) {
                inProximity = false;
            } else {
                isEnabled = false;
            }
            UpdateAppearance();
        }

        public void Show() {
            isEnabled = true;
            UpdateAppearance();
        }

        public void SetText(string textString) {
            if (!textElement) return;

            textElement!.text = textString;
        }

        public void Call(string text, GameObject? bubbleAnchor) {
            if (bubbleAnchor != null) anchor = bubbleAnchor;
            isEnabled = true;
            SetText(text);
            UpdateAppearance();
        }

        private void UpdateAppearance() {
            bool shouldShow = isEnabled && ((isExpectingProximity && inProximity) || !isExpectingProximity);

            if (showHideAnimation) {
                animator?.SetBool("shown", shouldShow);
                immediateCollider.enabled = shouldShow;
            } else {
                gameObject.SetActive(shouldShow);
            }

            if (!shouldShow) anchor = null;
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            if (proximityCollider.IsTouching(collider) && collider.gameObject.tag == "Player") {
                inProximity = true;
                UpdateAppearance();
            }
        }

        private void OnTriggerExit2D(Collider2D collider) {
            if (!proximityCollider.IsTouching(collider) && collider.gameObject.tag == "Player") {
                inProximity = false;
                UpdateAppearance();
            }
        }
    }
}
