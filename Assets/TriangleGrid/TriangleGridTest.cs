using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleGridTest : MonoBehaviour {

    public int width;
    public int height;

    public TriangleGrid triangleGrid;

	void Start () {
        if(width < 0) width = 1;
        if(height < 0) height = 1;

        triangleGrid.Setup(width, height);

        Camera.main.transform.position = new Vector3(width/2f, height/2f, Camera.main.transform.position.z);

        // TEST
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                triangleGrid.Set(i, j, new Color(j / (float) height, 0, 0, TriangleGrid.BOTTOM_LEFT), Color.green);
            }
        }

        triangleGrid.Apply();

        StartCoroutine(PunchHoles());
	}

    IEnumerator PunchHoles() {
        const int SIZE = 5;
        const int SQUARE_SIZE = SIZE * SIZE;

        Color empty = TriangleGrid.CreateColor(Color.black, TileType.EMPTY);

        while(true) {
            int centerX = Random.Range(0, width);
            int centerY = Random.Range(0, height);

            int minX = Mathf.Max(centerX - SIZE, 0);
            int minY = Mathf.Max(centerY - SIZE, 0);

            int maxX = Mathf.Min(centerX + SIZE, width);
            int maxY = Mathf.Min(centerY + SIZE, height);

            for(int i = minX; i < maxX; i++) {
                for(int j = minY; j < maxY; j++) {
                    int dx = i - centerX;
                    int dy = j - centerY;

                    if(dx * dx + dy * dy < SQUARE_SIZE) {
                        triangleGrid.Set(i, j, empty);
                    }
                }
            }

            triangleGrid.Apply();

            yield return null;
            //yield return new WaitForSeconds(0.5f);
        }
    }
}
