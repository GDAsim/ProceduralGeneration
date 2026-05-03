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
    /// uDomain = new Vector2(0, 12 * Mathf.PI);
    /// sampleresolution_U = 3000;
    /// </summary>
    public static void Torus(double u, double v, double w, out double x, out double y, out double z)
    {
        x = (1.0 + 0.5 * Math.Cos(u)) * Math.Cos(v);
        y = 0.5 * Math.Sin(u);
        z = (1.0 + 0.5 * Math.Cos(u)) * Math.Sin(v);
    }

    public static void Torus2(double u, double v, double w, out double x, out double y, out double z)
    {
        // 1. Create a Circle Surface on xy plane using u,v
        x = v * Math.Cos(u * 2 * Math.PI);
        y = v * Math.Sin(u * 2 * Math.PI);

        // 2. Extend al Rotating along xz
        z = (2 + x) * Math.Cos(w * 2 * Math.PI);
        x = (2 + x) * Math.Sin(w * 2 * Math.PI);
    }

    public static void Horn(double u, double v, double w, out double x, out double y, out double z)
    {
        x = (2 + u * Math.Cos(v)) * Math.Sin(2 * Math.PI * u);
        y = u * Math.Sin(v);
        z = (2 + u * Math.Cos(v)) * Math.Cos(2 * Math.PI * u) + 2 * u;
    }

    public static void HelixCurve(double u, double v, double w, out double x, out double y, out double z)
    {
        x = Math.Sin(u);
        y = (u * u) / 100;
        z = Math.Cos(u);
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
    public static void HeartCurve(double u, double v, double w, out double x, out double y, out double z)
    {
        var a = 2 * Math.PI * u;
        x = 16 * Math.Pow(Math.Sin(a), 3);
        y = 13 * Math.Cos(a) - 5 * Math.Cos(2 * a) - 2 * Math.Cos(3 * a) - Math.Cos(4 * a);
        z = 0;

        x /= 5;
        y /= 5;
        z /= 5;
    }
    public static void Shell(double u, double v, double w, out double x, out double y, out double z)
    {
        x = ((0.2 + 0.8 * v) * Math.Cos(u * 2 * Math.PI) + v) * Math.Sin(v * 12 * Math.PI) + 2;
        y = (0.2 + 0.8 * v) * Math.Sin(u * 2 * Math.PI) + 3 * (v) + 0.5;
        z = ((0.2 + 0.8 * v) * Math.Cos(u * 2 * Math.PI) + v) * Math.Cos(v * 12 * Math.PI) + 2;
    }

    public static void RoseCurve(double u, double v, double w, out double x, out double y, out double z)
    {
        var pedelsNum = 6;
        var angle = u * 2 * Math.PI;

        x = Math.Cos(pedelsNum * angle) * Math.Cos(angle);
        y = Math.Cos(pedelsNum * angle) * Math.Sin(angle);
        z = 0;
    }
    public static void Spiral(double u, double v, double w, out double x, out double y, out double z)
    {
        var radius = 0.1;
        var numOfRevolutions = 3;
        var angle = numOfRevolutions * u * 2 * Math.PI;

        x = radius * angle * Math.Cos(angle);
        y = radius * angle * Math.Sin(angle);
        z = 0;
    }

    public static void LogSpiral(double u, double v, double w, out double x, out double y, out double z)
    {
        var innerRadius = 0.1;
        var windingTightness = 0.1;
        var numOfRevolutions = 3;
        var angle = numOfRevolutions * u * 2 * Math.PI;

        x = innerRadius * Math.Exp(windingTightness * angle) * Math.Cos(angle);
        y = innerRadius * Math.Exp(windingTightness * angle) * Math.Sin(angle);
        z = 0;
    }
}