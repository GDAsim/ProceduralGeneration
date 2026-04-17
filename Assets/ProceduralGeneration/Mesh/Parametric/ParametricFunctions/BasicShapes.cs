// Basic Shapes

using System;
using UnityEngine;

public static partial class ParametricFunc
{
    /// <summary>
    /// uDomain = new Vector2(-1, 1);
    /// vDomain = new Vector2(-1, 1);
    /// wDomain = new Vector2(-1, 1);
    /// </summary>
    public static void Cube(double u, double v, double w, out double x, out double y, out double z)
    {
        x = u;
        y = v;
        z = w;
    }

    /// <summary>
    /// uDomain = new Vector2(-Mathf.PI, Mathf.PI);
    /// vDomain = new Vector2(-Mathf.PI, Mathf.PI);
    /// </summary>
    public static void Sphere(double u, double v, double w, out double x, out double y, out double z)
    {
        x = Math.Cos(u) * Math.Cos(v);
        y = Math.Sin(u) * Math.Cos(v);
        z = Math.Sin(v);
    }

    /// <summary>
    /// uDomain = new Vector2(-0.4f, 0.4f);
    /// vDomain = new Vector2(0, 2 * Mathf.PI);
    /// </summary>
    public static void Moebius(double u, double v, double w, out double x, out double y, out double z)
    {
        x = Math.Cos(v) + u * Math.Cos(v * 0.5) * Math.Cos(v);
        y = u * Math.Sin(v * 0.5);
        z = Math.Sin(v) + u * Math.Cos(v * 0.5) * Math.Sin(v);
    }
}