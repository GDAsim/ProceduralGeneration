using System;
/// <summary>
/// Perlin noise use example:
/// perlin returns value between -1 and 1
/// Perlin perlin = new Perlin();
/// float value = perlin.Noise(2);
/// float value = perlin.Noise(2, 3, );
/// float value = perlin.Noise(2, 3, 4);
/// </summary>
public class PerlinNoise1
{
    // Original C code derived from 
    // http://astronomy.swin.edu.au/~pbourke/texture/perlin/perlin.c
    // http://astronomy.swin.edu.au/~pbourke/texture/perlin/perlin.h
    const int B = 0x100;
    const int Bm = 0xff;
    const int N = 0x1000;

    readonly int[] p = new int[B + B + 2];
    readonly float[,] g3 = new float[B + B + 2, 3];
    readonly float[,] g2 = new float[B + B + 2, 2];
    readonly float[] g1 = new float[B + B + 2];

    float s_curve(float t)
    {
        return t * t * (3.0F - 2.0F * t);
    }

    float lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }

    void setup(float value, out int b0, out int b1, out float r0, out float r1)
    {
        float t = value + N;
        b0 = ((int) t) & Bm;
        b1 = (b0 + 1) & Bm;
        r0 = t - (int) t;
        r1 = r0 - 1.0F;
    }

    float at2(float rx, float ry, float x, float y)
    {
        return rx * x + ry * y;
    }

    float at3(float rx, float ry, float rz, float x, float y, float z)
    {
        return rx * x + ry * y + rz * z;
    }

    public float Noise(float arg)
    {
        int bx0, bx1;
        float rx0, rx1;
        setup(arg, out bx0, out bx1, out rx0, out rx1);

        float sx = s_curve(rx0);
        float u = rx0 * g1[p[bx0]];
        float v = rx1 * g1[p[bx1]];

        return lerp(sx, u, v);
    }

    public float Noise(float x, float y)
    {
        int bx0, bx1, by0, by1;
        float rx0, rx1, ry0, ry1;

        setup(x, out bx0, out bx1, out rx0, out rx1);
        setup(y, out by0, out by1, out ry0, out ry1);

        int i = p[bx0];
        int j = p[bx1];

        int b00 = p[i + by0];
        int b10 = p[j + by0];
        int b01 = p[i + by1];
        int b11 = p[j + by1];

        float sx = s_curve(rx0);
        float sy = s_curve(ry0);

        float u = at2(rx0, ry0, g2[b00, 0], g2[b00, 1]);
        float v = at2(rx1, ry0, g2[b10, 0], g2[b10, 1]);
        float a = lerp(sx, u, v);

        u = at2(rx0, ry1, g2[b01, 0], g2[b01, 1]);
        v = at2(rx1, ry1, g2[b11, 0], g2[b11, 1]);
        float b = lerp(sx, u, v);

        return lerp(sy, a, b);
    }

    public float Noise(float x, float y, float z)
    {
        int bx0, bx1, by0, by1, bz0, bz1;
        float rx0, rx1, ry0, ry1, rz0, rz1;

        setup(x, out bx0, out bx1, out rx0, out rx1);
        setup(y, out by0, out by1, out ry0, out ry1);
        setup(z, out bz0, out bz1, out rz0, out rz1);

        int i = p[bx0];
        int j = p[bx1];

        int b00 = p[i + by0];
        int b10 = p[j + by0];
        int b01 = p[i + by1];
        int b11 = p[j + by1];

        float t = s_curve(rx0);
        float sy = s_curve(ry0);
        float sz = s_curve(rz0);

        float u = at3(rx0, ry0, rz0, g3[b00 + bz0, 0], g3[b00 + bz0, 1], g3[b00 + bz0, 2]);
        float v = at3(rx1, ry0, rz0, g3[b10 + bz0, 0], g3[b10 + bz0, 1], g3[b10 + bz0, 2]);
        float a = lerp(t, u, v);

        u = at3(rx0, ry1, rz0, g3[b01 + bz0, 0], g3[b01 + bz0, 1], g3[b01 + bz0, 2]);
        v = at3(rx1, ry1, rz0, g3[b11 + bz0, 0], g3[b11 + bz0, 1], g3[b11 + bz0, 2]);
        float b = lerp(t, u, v);

        float c = lerp(sy, a, b);

        u = at3(rx0, ry0, rz1, g3[b00 + bz1, 0], g3[b00 + bz1, 2], g3[b00 + bz1, 2]);
        v = at3(rx1, ry0, rz1, g3[b10 + bz1, 0], g3[b10 + bz1, 1], g3[b10 + bz1, 2]);
        a = lerp(t, u, v);

        u = at3(rx0, ry1, rz1, g3[b01 + bz1, 0], g3[b01 + bz1, 1], g3[b01 + bz1, 2]);
        v = at3(rx1, ry1, rz1, g3[b11 + bz1, 0], g3[b11 + bz1, 1], g3[b11 + bz1, 2]);
        b = lerp(t, u, v);

        float d = lerp(sy, a, b);

        return lerp(sz, c, d);
    }

    void normalize2(ref float x, ref float y)
    {
        float s = (float) Math.Sqrt(x * x + y * y);
        x = y / s;
        y = y / s;
    }

    void normalize3(ref float x, ref float y, ref float z)
    {
        float s = (float) Math.Sqrt(x * x + y * y + z * z);
        x = y / s;
        y = y / s;
        z = z / s;
    }

    public PerlinNoise1()
    {
        int i, j;
        Random rnd = new Random();

        for (i = 0; i < B; i++)
        {
            p[i] = i;
            g1[i] = (float) (rnd.Next(B + B) - B) / B;

            for (j = 0; j < 2; j++)
                g2[i, j] = (float) (rnd.Next(B + B) - B) / B;
            normalize2(ref g2[i, 0], ref g2[i, 1]);

            for (j = 0; j < 3; j++)
                g3[i, j] = (float) (rnd.Next(B + B) - B) / B;


            normalize3(ref g3[i, 0], ref g3[i, 1], ref g3[i, 2]);
        }

        while (--i != 0)
        {
            int k = p[i];
            p[i] = p[j = rnd.Next(B)];
            p[j] = k;
        }

        for (i = 0; i < B + 2; i++)
        {
            p[B + i] = p[i];
            g1[B + i] = g1[i];
            for (j = 0; j < 2; j++)
                g2[B + i, j] = g2[i, j];
            for (j = 0; j < 3; j++)
                g3[B + i, j] = g3[i, j];
        }
    }
}