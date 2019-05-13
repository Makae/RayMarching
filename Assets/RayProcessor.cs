using UnityEngine;

public interface RayProcessor {
    Color NewRay(Ray ray);
    void SetGeometries(Geometry[] geometry);
}
