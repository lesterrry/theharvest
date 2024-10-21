using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(SpriteRenderer))]
public class VeggieCharacter : MonoBehaviour
{
    public bool isTaken = false;

    private SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Take() {
        isTaken = true;

        UpdateAppearance();
    }

    private void UpdateAppearance() {
        spriteRenderer.enabled = !isTaken;
    }
}
