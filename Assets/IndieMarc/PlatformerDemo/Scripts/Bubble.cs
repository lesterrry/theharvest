#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Bubble : MonoBehaviour {

    public bool isEnabled;
    public bool showHideAnimation = false;
    public GameObject? anchor;
    
    private Text? textElement;
    private Animator? animator;
    private CircleCollider2D collider;

    void Awake() {
        textElement = GetComponentInChildren<Text>();
        animator = GetComponent<Animator>();
        collider = GetComponent<CircleCollider2D>();
        UpdateAppearance();
    }

    void Update() {
        if(anchor != null) { 
            Vector2 newPosition = new Vector2(anchor.transform.position.x + 1, anchor.transform.position.y + 1.75f);
            transform.position = newPosition;
        }
    }

    public void Hide() {
        isEnabled = false;
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
        if (showHideAnimation) {
            animator?.SetBool("shown", isEnabled);
            collider.enabled = isEnabled;
        } else {
            gameObject.SetActive(isEnabled);
        }

        if (!isEnabled) anchor = null;
    }
}
