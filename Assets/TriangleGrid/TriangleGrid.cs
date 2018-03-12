using System.Collections;

using UnityEngine;

public enum TileType {
    EMPTY,
    FULL,
    BOTTOM_LEFT,
    BOTTOM_RIGHT,
    TOP_RIGHT,
    TOP_LEFT
}

/// <summary>
/// A MonoBehaviour to render a "Grid of triangles" (a grid where every cell can
/// either be a square or two triangles splitting said cell diagonally).
/// 
/// TriangleGrid uses a specific shader to render the triangles. That shader uses
/// a texture to know what to render in each cell. Each color of that texture determines
/// both the "shape" of the cell and the colors in it.
/// </summary>
public class TriangleGrid : MonoBehaviour {

    public static Color EMPTY_COLOR;

    public const float EMPTY = 0;
    public const float FULL = 0.15f;
    public const float BOTTOM_LEFT = 0.25f;
    public const float BOTTOM_RIGHT = 0.35f;
    public const float TOP_RIGHT = 0.45f;
    public const float TOP_LEFT = 0.55f;

    public Material material;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    Mesh mesh;

    Texture2D texture;

    int width;
    int height;

    /// <summary>
    /// Needs to be called before it can be used
    /// </summary>
    /// <param name="width">The grid's width</param>
    /// <param name="height">The grid's height</param>
	public void Setup (int width, int height) {
        this.width = width;
        this.height = height;

        // Create components
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();

        // Define the material properties
        material.SetFloat("_Width", width);
        material.SetFloat("_Height", height);

        texture = new Texture2D(2 * width, height);
        texture.filterMode = FilterMode.Point;

        material.SetTexture("_MainTex", texture);

        meshRenderer.material = material;

        // Create the mesh
        mesh = CreateMesh(width, height);
        meshFilter.mesh = mesh;

        EMPTY_COLOR = new Color(0, 0, 0, EMPTY);

        // Render a clear grid when initialized
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                Set(i, j, EMPTY_COLOR);
            }
        }

        Apply();
    }

    static Mesh CreateMesh(int width, int height) {
        Mesh mesh = new Mesh();
        mesh.name = "TriangleGridMesh";

        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0, height);
        vertices[2] = new Vector3(width, 0, height);
        vertices[3] = new Vector3(width, 0, 0);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        return mesh;
    }

    /// <summary>
    /// Define what to render in a specific cell. Requires Apply to be called for the
    /// changes to be considered.
    /// </summary>
    /// <param name="color1">
    /// The first color, the alpha value is used to determine
    /// the shape of the cell
    /// </param>
    public void Set(int x, int y, Color color1) {
        texture.SetPixel(x, y, color1);
        texture.SetPixel(width + x, y, Color.clear);
    }
    /// <summary>
    /// Define what to render in a specific cell. Requires Apply to be called for the
    /// changes to be considered.
    /// </summary>
    /// <param name="color1">
    /// The first color, the alpha value is used to determine
    /// the shape of the cell
    /// </param>
    /// <param name="color2">
    /// The second color, fills the space the first color doesn't fill
    /// </param>
    public void Set(int x, int y, Color color1, Color color2) {
        texture.SetPixel(x, y, color1);
        texture.SetPixel(width + x, y, color2);
    }

    /// <summary>
    /// Apply every changes made through Set.
    /// </summary>
    public void Apply() {
        texture.Apply();
    }

    /// <summary>
    /// Creates a color with the correct shape "command"
    /// </summary>
    /// <param name="tileType">The shape of the command</param>
    public static Color CreateColor(Color color, TileType tileType) {
        switch(tileType) {
            case TileType.EMPTY : return Color.clear;
            case TileType.FULL : return new Color(color.r, color.g, color.b, FULL);
            case TileType.BOTTOM_LEFT : return new Color(color.r, color.g, color.b, BOTTOM_LEFT);
            case TileType.BOTTOM_RIGHT : return new Color(color.r, color.g, color.b, BOTTOM_RIGHT);
            case TileType.TOP_RIGHT : return new Color(color.r, color.g, color.b, TOP_RIGHT);
            case TileType.TOP_LEFT: return new Color(color.r, color.g, color.b, TOP_LEFT);
            default : return color;
        }
    }
}
