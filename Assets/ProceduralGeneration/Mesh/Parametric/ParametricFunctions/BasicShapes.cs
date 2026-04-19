// Basic Shapes

using System;
using UnityEngine;

public static partial class ParametricFunc
{
    /// <summary>
    /// uDomain = new Vector2(-1, 1);
    /// vDomain = new Vector2(-1, 1);
    /// </summary>
    public static void Plane(double u, double v, double w, out double x, out double y, out double z)
    {
        x = u;
        y = v;
        z = 0;
    }

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

    /// <summary>
    /// Butterfly curve by Temple H. Fay
    /// uDomain = new Vector2(0, 12 * Mathf.PI);
    /// sampleresolution_U = 3000;
    /// </summary>
    public static void ButterflyCurve(double u, double v, double w, out double x, out double y, out double z)
    {
        var radius = Math.Pow(Math.E, Math.Sin(u)) - 2 * Math.Cos(4 * u) + Math.Pow(Math.Sin(u / 12), 5);
        x = Math.Cos(u) * radius;
        y = Math.Sin(u) * radius;
        z = 0;
    }

    //public static void Plane2(double u, double v, double w, out double x, out double y, out double z)
    //{
    //    x = cos(u) * (4 + 3.8 * cos(v))
    //    y = (cos(v) + sin(v) - 1) * (1 + sin(v)) * log(1 - pi * v / 10) + 7.5 * sin(v)
    //    z = sin(u) * (4 + 3.8 * cos(v))
    //}
    //public static void Horn(double u, double v, double w, out double x, out double y, out double z)
    //{
    //    x = (2 + u * cos(v)) * sin(2 * pi * u)
    //    y = u * sin(v)
    //    z = 0
    //}
    //public static void Trumpet(double u, double v, double w, out double x, out double y, out double z)
    //{
    //    x = cos(u) * sin(v)
    //    y = (cos(v) + log(tan(1 / 2 * v)))
    //    z = sin(u) * sin(v)
    //}
}