// Basic Shapes

using System;
using UnityEngine;

public static partial class ParametricFunc
{
    public static void Cube(double u, double v, double w, out double x, out double y, out double z)
    {
        x = u;
        y = v;
        z = w;
    }
    public static void Sphere(double u, double v, double w, out double x, out double y, out double z)
    {
        x = Math.Cos(u) * Math.Cos(v);
        y = Math.Sin(u) * Math.Cos(v);
        z = Math.Sin(v);
    }
}