using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IndieMarc.Platformer {
    public class BoundaryCheck : MonoBehaviour {
        public string targetScene;

        public Image fadeImage;

        IEnumerator FadeOut(float duration) {
            Color color = fadeImage.color;
            color.a = 0;
            float time = 0;

            while (time < duration) {
                fadeImage.color = new Color(color.r, color.g, color.b, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            fadeImage.color = new Color(color.r, color.g, color.b, 1);
        }

        IEnumerator SwitchScene(string sceneName, float duration) {
            yield return StartCoroutine(FadeOut(duration));
            SceneManager.LoadScene(sceneName);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag == "Player") StartCoroutine(SwitchScene(targetScene, 2f));
        }
    }
}
