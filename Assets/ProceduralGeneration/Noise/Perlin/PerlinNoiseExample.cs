using UnityEditor;
using UnityEngine;

public class PerlinNoiseExample : MonoBehaviour
{
    Texture2D texture;
    public int width = 512;
    public int height = 512;
    public Vector2 offset = Vector2.zero;
    public int scale = 5;
    public bool useTime = false;

    PerlinNoise1 perlin1 = new PerlinNoise1();
    PerlinNoise2 perlin2 = new PerlinNoise2();

    void Start()
    {
        texture = new Texture2D(width, height);
    }

    void Update()
    {
        float timeoffset = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (useTime)
                    timeoffset = Time.time;

                float fx = x / (width - 0.1f);
                float fy = y / (height - 0.1f);

                //Use PerlinNoise1
                //float v = perlin1.Noise((fx + offset.x + timeoffset) * frequency, (fy + offset.y + timeoffset) * frequency);

                //Use PerlinNoise2
                //float v = perlin2.Noise((fx + offset.x + timeoffset) * frequency, (fy + offset.y + timeoffset) * frequency);

                //Use PerlinNoise3
                float v = PerlinNoise3.Noise((fx + offset.x + timeoffset) * scale, (fy + offset.y + timeoffset) * scale);

                //Use Unity's Perlin
                //float v = Mathf.PerlinNoise((fx + offset.x + timeoffset) * frequency, (fy + offset.y + timeoffset) * frequency);
               
                v = v.remap(-1,1,1,0);
                texture.SetPixel(x, y, new Color(v, v, v, 1));
            }
        }

        texture.Apply();
    }

    void OnGUI()
    {
        Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 offset = new Vector2(width / 2, height / 2);

        Rect rect = new Rect();
        rect.min = center - offset;
        rect.max = center + offset;

        GUI.DrawTexture(rect, texture);
    }
}
