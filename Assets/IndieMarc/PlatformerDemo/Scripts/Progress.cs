#nullable enable

using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

namespace IndieMarc.Platformer {
    
    [InitializeOnLoad]
    public class GameProgress {
        [HideInInspector]

        private static readonly IReadOnlyDictionary<string, string> entriesOverride = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string> {
                { "scene", "TheDen" },
                { "is_night", "true" },
                { "next_bg_index", "1" },
                // { "next_bg_index", "1" },
                }
        );

        public static Dictionary<string, string> entries = new Dictionary<string, string>(entriesOverride);

        public static string? Get(string key) {
            if (entries.TryGetValue(key, out var value)) {
                return value;
            }

            return null;
        }
    
        public static void Set(string key, string value) {
            entries[key] = value;
        }

        public static void Unset(string key) {
            entries.Remove(key);
        }

        public static bool IsTrue(string key) {
            return Get(key) == "true";
        }

        public static void Reset()  {
            entries = new Dictionary<string, string>(entriesOverride);
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
