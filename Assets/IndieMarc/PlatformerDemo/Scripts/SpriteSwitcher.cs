using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSwitcher : MonoBehaviour
{
    public Sprite[] sprites;

    public void Switch(int index) {
        GetComponent<SpriteRenderer>().sprite = sprites[index];
    }
}