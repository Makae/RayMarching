using System;
using UnityEngine;

public abstract class Geometry {
    public Pose Pose;

    protected Geometry(Pose pose)
    {
        Pose = pose;
    }

    abstract public float GetDistanceTo(Vector3 position);

    abstract public Vector3 GetSurfaceNormal(Vector3 vector3);

}
public class NullGeometry : Geometry
{
    public NullGeometry() : base(new Pose())
    {
    }

    public NullGeometry(Pose pose) : base(pose)
    {

    }

    public override float GetDistanceTo(Vector3 position)
    {
        return -1f;
    }

    public override Vector3 GetSurfaceNormal(Vector3 vector3)
    {
        throw new NotImplementedException();
    }
}
