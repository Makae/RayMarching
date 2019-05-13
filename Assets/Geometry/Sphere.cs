using UnityEngine;

public class Sphere: Geometry {
    public float Radius;

    public Sphere(Pose pose, float radius=1.0f) : base(pose)
    {
        Radius = radius;
    }

    public override float GetDistanceTo(Vector3 position)
    {
        return Mathf.Abs(Vector3.Distance(this.Pose.position, position) - Radius);
    }

    public override Vector3 GetSurfaceNormal(Vector3 position)
    {
        return (position - this.Pose.position).normalized;
    }
}
