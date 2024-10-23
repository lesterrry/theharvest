using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IndieMarc.Platformer {
    public class SceneSwitcher : MonoBehaviour {
        public Image fadeImage;

        public float fadeSpeed = 1f;

        void Awake() {
            StartCoroutine(FadeIn());
        }

        IEnumerator Fade(bool isOut, float duration) {
            Color color = fadeImage.color;
            float startAlpha = isOut ? 1 : 0;
            float endAlpha = isOut ? 0 : 1;
            float time = 0;

            while (time < duration) {
                float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                time += Time.deltaTime;
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
        }

        IEnumerator SwitchScene(string sceneName) {
            yield return StartCoroutine(Fade(false, fadeSpeed));
            SceneManager.LoadScene(sceneName);
        }

        public IEnumerator FadeIn() {
            yield return StartCoroutine(Fade(true, fadeSpeed));
        }

        public void StartSwitchScene(string targetScene) {
            StartCoroutine(SwitchScene(targetScene));
        }
    }
}
