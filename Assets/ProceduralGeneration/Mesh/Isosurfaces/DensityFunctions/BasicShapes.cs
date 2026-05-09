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
    public static float SphereSDF(Vector3 pos, float radius)
    {
        return pos.magnitude - radius;
    }
    /// <summary>
    /// Box True Distance
    /// </summary>
    public static float BoxSDF(Vector3 pos, Vector3 size)
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

    public static float SphereImplicit(Vector3 pos, float radius)
    {
        return pos.x * pos.x + pos.y * pos.y + pos.z * pos.z - radius * radius;
    }
    public static float CubeImplicit(Vector3 pos, Vector3 size)
    {
        float halfX = size.x / 2;
        float halfY = size.y / 2;
        float halfZ = size.z / 2;

        return -Mathf.Min(
            halfX - pos.x, halfX + pos.x,
            halfY - pos.y, halfY + pos.y,
            halfZ - pos.z, halfZ + pos.z);
    }

    /// <summary>
    /// Schwartz P surface
    /// </summary>
    public static float SchwartzP(Vector3 pos)
    {
        return (Mathf.Cos(pos.x) + Mathf.Cos(pos.y) + Mathf.Cos(pos.z));
    }

    /// <summary>
    /// Hyperbolic surface
    /// </summary>
    public static float Hyperbolic(Vector3 pos)
    {
        return pos.x * pos.x + pos.y - pos.z * pos.z;
    }

    public static float SixTori(Vector3 pos)
    {
        float x = pos.x;
        float y = pos.y;
        float z = pos.z;

        float f1 = Mathf.Pow(Mathf.Sqrt(x * x + y * y) - 3f, 2f) + z * z - 0.4f;
        float f2 = Mathf.Pow(Mathf.Sqrt((x - 4.5f) * (x - 4.5f) + z * z) - 3f, 2f) + y * y - 0.4f;
        float f3 = Mathf.Pow(Mathf.Sqrt((x + 4.5f) * (x + 4.5f) + z * z) - 3f, 2f) + y * y - 0.4f;
        float f4 = Mathf.Pow(Mathf.Sqrt((y + 4.5f) * (y + 4.5f) + z * z) - 3f, 2f) + x * x - 0.4f;
        float f5 = Mathf.Pow(Mathf.Sqrt((y - 4.5f) * (y - 4.5f) + z * z) - 3f, 2f) + x * x - 0.4f;
        float f6 = Mathf.Pow(Mathf.Sqrt(x * x + y * y) - 5f, 2f) + z * z - 0.4f;

        return f1 * f2 * f3 * f4 * f5 * f6;
    }

    public static float CubeCutOutSphere(Vector3 pos)
    {
        var cube = CubeImplicit(pos, new Vector3(2, 2, 2));
        var sphere = SphereImplicit(pos, 1.25f);

        return Mathf.Max(cube, -sphere);
    }

    public static float SphereToCubeImplict(Vector3 pos)
    {
        var t = (Mathf.Cos(Time.time) + 1) / 2;

        var cube = CubeImplicit(pos, new Vector3(5, 5, 5));
        return Mathf.Min(SphereImplicit(pos, 5 * t), cube);
    }
}