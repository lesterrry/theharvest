using System.Collections;
using System.Collections.Generic;
using IndieMarc.Platformer;
using UnityEngine;

public class DenSceneManager : MonoBehaviour {

    public Bubble bed;

    void Start() {
        if (GameProgress.IsTrue("is_night")) {
            bed.isEnabled = true;
        }
    }

}
