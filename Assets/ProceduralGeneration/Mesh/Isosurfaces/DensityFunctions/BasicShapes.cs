
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
}