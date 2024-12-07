#nullable enable

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IndieMarc.Platformer {
    
    [InitializeOnLoad]
    public class GameProgress {
        [HideInInspector]

        public static Dictionary<string, string> entries = new Dictionary<string, string>();

        public static string? Get(string key) {
            if (entries.TryGetValue(key, out var value)) {
                return value;
            }

            return null;
        }

        public static void Reset()  {
            entries = new Dictionary<string, string>();
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
