using System;
using UnityEngine;

public class RayMarcher : RayProcessor {

    private int MAX_STEPS = 8;
    private float MAX_DISTANCE = 500;
    private float MIN_DISTANCE = 0.001f;

    private Geometry[] geometries;

    public RayMarcher(Geometry[] geometries)
    {
        this.geometries = geometries;
    }

    public void SetGeometries(Geometry[] geometries)
    {
        this.geometries = geometries;
    }

    public Color NewRay(Ray ray)
    {
        Color color = Color.black;

        color = MarchRay(ray);

        return color;
    }

    public Color MarchRay(Ray ray, float totalDistance=0f)
    {
        var latestDistance = MAX_DISTANCE;
        Geometry closestGeometry = new NullGeometry();
        for(var c=0; c < MAX_STEPS; c++)
        {
            var rayPoint = ray.GetPoint(totalDistance);
            closestGeometry = GetClosestGeometry(rayPoint);

            if (closestGeometry is NullGeometry) break;

            latestDistance = closestGeometry.GetDistanceTo(rayPoint);
            totalDistance += latestDistance;

            if (latestDistance < MIN_DISTANCE) break;
            if (totalDistance >= MAX_DISTANCE) break;
        }

        if(latestDistance <= MIN_DISTANCE)
        {
            Vector3 surfaceNormal = closestGeometry.GetSurfaceNormal(ray.GetPoint(totalDistance));
            float t = Vector3.Angle(surfaceNormal, ray.direction.normalized) / 360;
            return Color.Lerp(Color.black, Color.green, t);
        }
        else if (totalDistance >= MAX_DISTANCE)
        {
            return Color.black;
        }
        return Color.black;
    }

    private Geometry GetClosestGeometry(Vector3 position)
    {
        var minimalDistance = MAX_DISTANCE;
        Geometry closestGeometry = new NullGeometry();
        foreach (var geometry in this.geometries)
        {
            var geometryDistance = geometry.GetDistanceTo(position);
            if (geometryDistance < minimalDistance)
            {
                minimalDistance = geometryDistance;
                closestGeometry = geometry;
            }
        }
        return closestGeometry;
    }
}
