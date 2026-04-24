// Basic Shapes SDF Sampling Functions

using UnityEngine;

public static partial class DensityFunc
{
    public static float Sphere_Implicit(float posX, float posY, float posZ)
    {
        float r = 0.5f;
        float x = posX;
        float y = posY;
        float z = posZ;

        return (r * r - x * x - y * y - z * z);
    }
    public static float Box_Implicit(float posX, float posY, float posZ)
    {
        float xt = posX - 0;
        float yt = posY - 0;
        float zt = posZ - 0;

        float xd = (xt * xt) - 0.5f * 0.5f;
        float yd = (yt * yt) - 0.5f * 0.5f;
        float zd = (zt * zt) - 0.5f * 0.5f;
        float d;

        if (xd > yd)
            if (xd > zd)
                d = xd;
            else
                d = zd;
        else if (yd > zd)
            d = yd;
        else
            d = zd;

        return -d;
    }

    /// <summary>
    /// Chebyshev-distance-squared
    /// not true distance
    /// </summary>
    public static float Box_Implicit2(float posX, float posY, float posZ)
    {
        float xt = posX - 0;
        float yt = posY - 0;
        float zt = posZ - 0;

        float xd = (xt * xt) - 0.5f * 0.5f;
        float yd = (yt * yt) - 0.5f * 0.5f;
        float zd = (zt * zt) - 0.5f * 0.5f;
        float d;

        if (xd > yd)
            if (xd > zd)
                d = xd;
            else
                d = zd;
        else if (yd > zd)
            d = yd;
        else
            d = zd;

        return -d;
    }

    /// <summary>
    /// Sphere True Distance
    /// </summary>
    public static float Sphere(Vector3 pos, float radius)
    {
        return pos.magnitude - radius;
    }
    /// <summary>
    /// Box True Distance
    /// </summary>
    public static float Box(Vector3 pos, Vector3 size)
    {
        // 1. Simplyfy by only calculating positive quadrant by converting negative pos into positive values using abs()
        // 2. calculate distance using length()
        // 3. using some math tricks, reduce the formula
        Vector3 quadrant;
        quadrant.x = Mathf.Abs(pos.x) - size.x;
        quadrant.y = Mathf.Abs(pos.y) - size.y;
        quadrant.z = Mathf.Abs(pos.z) - size.z;

        var max = Mathf.Max(quadrant.x, Mathf.Max(quadrant.y, quadrant.z));
        return Vector3.Max(quadrant, Vector3.zero).magnitude + Mathf.Min(max, 0.0f);
    }

    public static float SchwartzP(Vector3 pos)
    {
        return (Mathf.Cos(pos.x) + Mathf.Cos(pos.y) + Mathf.Cos(pos.z));
    }
     
}