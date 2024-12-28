//https://github.com/Scrawk/Procedural-Noise
using System.Collections.Generic;
using UnityEngine;

public class FractalNoiseExample : MonoBehaviour
{
    public enum NOISE_TYPE { PERLIN, VALUE, SIMPLEX, VORONOI, WORLEY }
    public List<NOISE_TYPE> noiseTypeForEachOctave;
    public int octaves => noiseTypeForEachOctave.Count;
    public int seed = 0;
    public float frequency = 1.0f;
    public Vector2 offset;

    public int width = 512;
    public int height = 512;
    Texture2D texture;

    FractalNoise fractal;

    void Start()
    {
        texture = new Texture2D(width, height);

        INoise[] noiseArray = new INoise[octaves];
        for (int i = 0;i< octaves;i++)
        {
            switch (noiseTypeForEachOctave[i])
            {
                case NOISE_TYPE.PERLIN:
                    noiseArray[i] = new PerlinNoise(seed, 20);
                    break;

                case NOISE_TYPE.VALUE:
                    noiseArray[i] = new ValueNoise(seed, 20);
                    break;

                case NOISE_TYPE.SIMPLEX:
                    noiseArray[i] = new SimplexNoise(seed, 20);
                    break;

                case NOISE_TYPE.VORONOI:
                    noiseArray[i] = new VoronoiNoise(seed, 20);
                    break;

                case NOISE_TYPE.WORLEY:
                    noiseArray[i] = new WorleyNoise(seed, 20, 1.0f);
                    break;
            }
        }
        
        fractal = new FractalNoise(noiseArray, octaves, frequency);
    }
    void Update()
    {
        float[,] arr = new float[width, height];

        //Sample the 2D noise and add it into a array.
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float fx = (x + offset.x) / (width - 1.0f);
                float fy = (y + offset.y) / (height - 1.0f);

                float n = fractal.Sample2D(fx , fy);

                //Some of the noises range from -1-1 so normalize the data to 0-1 to make it easier to see.
                n = n.remap(-1, 1, 0, 1);

                texture.SetPixel(x, y, new Color(n, n, n, 1));
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

    private void NormalizeArray(float[,] arr)
    {
        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float v = arr[x, y];
                if (v < min) min = v;
                if (v > max) max = v;

            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float v = arr[x, y];
                arr[x, y] = (v - min) / (max - min);
            }
        }
    }
}