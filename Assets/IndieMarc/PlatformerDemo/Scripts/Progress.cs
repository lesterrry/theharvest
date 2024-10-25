using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IndieMarc.Platformer {
    
    [InitializeOnLoad]
    public class GameProgress {
        [HideInInspector]
        public static bool enteredScarecrow = false;

        public static Dictionary<string, string> stories = new Dictionary<string, string>();

        public static void Reset()  {
            enteredScarecrow = false;
            stories = new Dictionary<string, string>();
        }

        static GameProgress() {
            EditorApplication.playModeStateChanged += OnPlayModeState;
        }


        private static void OnPlayModeState(PlayModeStateChange obj) {
            if (obj == PlayModeStateChange.ExitingPlayMode) {
                Reset();
                Debug.Log("Progress reset");
            }
        }

    }
}
