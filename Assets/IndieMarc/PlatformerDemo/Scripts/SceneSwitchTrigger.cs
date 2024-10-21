using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IndieMarc.Platformer {
    public class SceneSwitchTrigger : MonoBehaviour {
        public string targetScene;

        public SceneSwitcher sceneSwitcher;

        void Awake() {
            StartCoroutine(sceneSwitcher.FadeIn());
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Player") sceneSwitcher.StartSwitchScene(targetScene);
        }
    }
}
