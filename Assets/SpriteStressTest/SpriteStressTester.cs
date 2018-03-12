using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteStressTester : MonoBehaviour {

    const int EXTRA_SPRITES = 100;

    public Sprite sprite;

    int spriteCount;

    void OnGUI() {
        
        if(GUI.Button(new Rect(10, 10, 200, 30), "Create more sprites")) {
            for(int i = 0; i < EXTRA_SPRITES; i++) {
                Vector2 pos = 10 * Random.insideUnitCircle;

                GameObject obj = new GameObject();
                obj.transform.position = pos;
                SpriteRenderer rend = obj.AddComponent<SpriteRenderer>();

                rend.sprite = sprite;
            }

            spriteCount += EXTRA_SPRITES;
        }

        GUI.Label(new Rect(10, 50, 200, 30), "Sprite count=" + spriteCount);
    }
}
