using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace NewFlowar
{
    public static class MathFlowar
    {
        ////public static Vector2 IntersectionSegment(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4)
        //{
        //    Vector2 intersectionPoint = Vector2.Zero;

        //    float ua = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
        //    float ub = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);
        //    float denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

        //    if (System.Math.Abs(denominator) <= 0.00001f)
        //    {
        //        if (System.Math.Abs(ua) <= 0.00001f && System.Math.Abs(ub) <= 0.00001f)
        //        {
        //            intersectionPoint = (point1 + point2) / 2;
        //        }
        //    }
        //    else
        //    {
        //        ua /= denominator;
        //        ub /= denominator;

        //        if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
        //        {
        //            intersectionPoint.X = point1.X + ua * (point2.X - point1.X);
        //            intersectionPoint.Y = point1.Y + ua * (point2.Y - point1.Y);
        //        }
        //    }

        //    return intersectionPoint;
        //}

        //public static Vector2 IntersectionTriangle(float defaultX, float defaultY, float triWidth, float triHeight, float gridWidth, float gridHeight, Vector2 origin, Vector2 direction, Flowar.Triangle.Triangle triangle,  ref Vector2 newDirection)
        //{
        //    return IntersectionTriangle(defaultX, defaultY, triWidth, triHeight, gridWidth, gridHeight, origin, direction, triangle, triangle.Angle, ref newDirection);
        //}

        //public static Vector2 IntersectionTriangle(float defaultX, float defaultY, float triWidth, float triHeight, float gridWidth, float gridHeight, Vector2 origin, Vector2 direction, Flowar.Triangle.Triangle triangle, float angleTriangle, ref Vector2 newDirection)
        //{
        //    float dist = float.MaxValue;
        //    Vector2 intersectionPoint = Vector2.Zero;

        //    Vector2 point1 = Vector2.Zero;
        //    Vector2 point2 = Vector2.Zero;
        //    Vector2 point3 = Vector2.Zero;

        //    //Vector2 trianglePos = new Vector2(
        //    //defaultX + ((float)triangle.PosGridX + 0.5f) * triWidth,
        //    //defaultY + triHeight * (float)(gridHeight - 1 - (float)triangle.PosGridY + 0.5f));

        //    Vector2 trianglePos = new Vector2(triangle.PosX, triangle.PosY);

        //    float w = triWidth * 0.45f;

        //    point1 = new Vector2(trianglePos.X + w * (float)System.Math.Cos(angleTriangle - MathHelper.PiOver2),
        //                         trianglePos.Y + w * (float)System.Math.Sin(angleTriangle - MathHelper.PiOver2));

        //    point2 = new Vector2(trianglePos.X + w * (float)System.Math.Cos(angleTriangle + MathHelper.Pi / 6f),
        //                         trianglePos.Y + w * (float)System.Math.Sin(angleTriangle + MathHelper.Pi / 6f));

        //    point3 = new Vector2(trianglePos.X + w * (float)System.Math.Cos(angleTriangle + MathHelper.Pi * 5f / 6f),
        //                         trianglePos.Y + w * (float)System.Math.Sin(angleTriangle + MathHelper.Pi * 5f / 6f));

        //    Vector2 intersection1 = MathFlowar.IntersectionSegment(origin, origin + direction * 5000f, point1, point2);
        //    Vector2 intersection2 = MathFlowar.IntersectionSegment(origin, origin + direction * 5000f, point2, point3);
        //    Vector2 intersection3 = MathFlowar.IntersectionSegment(origin, origin + direction * 5000f, point3, point1);

        //    direction.Normalize();

        //    if (intersection1 != Vector2.Zero && Vector2.Distance(origin, intersection1) < dist)
        //    {
        //        dist = Vector2.Distance(origin, intersection1);
        //        intersectionPoint = intersection1;
        //        newDirection = Vector2.Reflect(direction, new Vector2(point2.Y - point1.Y, -(point2.X - point1.X)));
        //        newDirection.Normalize();
        //    }
        //    if (intersection2 != Vector2.Zero && Vector2.Distance(origin, intersection2) < dist)
        //    {
        //        dist = Vector2.Distance(origin, intersection2);
        //        intersectionPoint = intersection2;
        //        newDirection = Vector2.Reflect(direction, new Vector2(point3.Y - point2.Y, -(point3.X - point2.X)));
        //        newDirection.Normalize();
        //    }
        //    if (intersection3 != Vector2.Zero && Vector2.Distance(origin, intersection3) < dist)
        //    {
        //        dist = Vector2.Distance(origin, intersection3);
        //        intersectionPoint = intersection3;
        //        newDirection = Vector2.Reflect(direction, new Vector2(point1.Y - point3.Y, -(point1.X - point3.X)));
        //        newDirection.Normalize();
        //    }

        //    return intersectionPoint;
        //}

        //public static bool IntersectionLineCircle(
        //    Vector2 origin,  // Line origin
        //    Vector2 direction,  // Line direction
        //    Vector2 triangleCenter,  // Circle center
        //    float radius,      // Circle radius
        //    ref float[] t,        // Parametric values at intersection points
        //    ref Vector2[] point,   // Intersection points
        //    ref Vector2[] normal) // Normals at intersection points
        //{

        //    Vector2 d = origin - triangleCenter;
        //    float a = Vector2.Dot(direction, direction);
        //    float b = Vector2.Dot(d, direction);// d.Dot(D);
        //    float c = Vector2.Dot(d, d) - radius * radius;
        //    //d.Dot(d) - radius * radius;

        //    float disc = b * b - a * c;
        //    if (disc < 0.0f)
        //    {
        //        return false;
        //    }

        //    float sqrtDisc = (float)Math.Sqrt(disc);
        //    float invA = 1.0f / a;
        //    t[0] = (-b - sqrtDisc) * invA;
        //    t[1] = (-b + sqrtDisc) * invA;

        //    float invRadius = 1.0f / radius;
        //    for (int i = 0; i < 2; ++i)
        //    {
        //        point[i] = origin + t[i] * direction;
        //        normal[i] = (point[i] - triangleCenter) * invRadius;
        //    }

        //    return true;
        //}

        //public static float GetAngle(float Xa, float Ya, float Xb, float Yb)
        //{
        //    return -(float)System.Math.Atan2(Xb - Xa, Yb - Ya) + MathHelper.PiOver2;
        //}
    }
}
