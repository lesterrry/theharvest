#nullable enable

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class Bubble : MonoBehaviour {

    public bool isEnabled;
    public Text? textElement;
    public GameObject? anchor;

    void Awake() {
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
        gameObject.SetActive(isEnabled);
        if (!isEnabled) anchor = null;
    }
}
