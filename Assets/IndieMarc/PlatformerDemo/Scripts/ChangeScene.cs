using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IndieMarc.Platformer {
    public class ChangeScene : MonoBehaviour {
        public string targetScene;

        public Image fadeImage;

        private float defaultSpeed = 1f;

        IEnumerator Fade(bool isOut, float duration) {
            Color color = fadeImage.color;
            float startAlpha = isOut ? 1 : 0;
            float endAlpha = isOut ? 0 : 1;
            float time = 0;

            while (time < duration)
            {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            time += Time.deltaTime;
            yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
        }

        IEnumerator SwitchScene(string sceneName, float duration) {
            yield return StartCoroutine(Fade(false, duration));
            SceneManager.LoadScene(sceneName);
        }

        void Awake() {
            StartCoroutine(Fade(true, defaultSpeed));
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Player") StartCoroutine(SwitchScene(targetScene, defaultSpeed));
        }
    }
}
