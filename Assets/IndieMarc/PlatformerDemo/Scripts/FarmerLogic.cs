using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteSwitcher))]
public class FarmerLogic : MonoBehaviour
{
    [Header("Lifecycle")]
    public float sleepDuration = 5;
    public float awakeDuration = 2;
    public bool isAlive = true;

    [Header("Connections")]
    public Bubble speechBubble;

    [HideInInspector]
    public bool isSleeping = true;

    private SpriteSwitcher spriteSwitcher;
    private GameObject anchor;

    void Start() {
        spriteSwitcher = GetComponent<SpriteSwitcher>();
        anchor = gameObject.transform.Find("Anchor").gameObject;
        StartCoroutine(Live());
    }

    public void HeartAttack() {
        isAlive = false;
        isSleeping = false;

        speechBubble.Call("AAAAAAAGHH!!!!", anchor);
    }

    private IEnumerator Live() {
        while (isAlive) {
            if (isSleeping) {
                yield return new WaitForSeconds(sleepDuration);
                isSleeping = false;
            } else {
                yield return new WaitForSeconds(awakeDuration);
                isSleeping = true;
            }

            if (isAlive) spriteSwitcher.Switch(isSleeping ? 0 : 1);
        }
    }
}
