using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteSwitcher))]
public class Speaker : MonoBehaviour
{
    public GameObject anchor;

    [HideInInspector]
    public bool isSpeaking = false;

    void Awake() {
        anchor = gameObject.transform.Find("Anchor").gameObject;
    }
}
