using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SceneRenderer : MonoBehaviour {
    public Camera cam;
    public CanvasTexture texture;
    public Transform sceneContainer;
    public int BATCH_SIZE = 100;

    private Geometry[] geometries;
    private RayProcessor rayProcessor;
    private bool _running;

    private void Start()
    {

        if(texture.Initialized)
        {
            OnCanvasInitialized();
        } else
        {
            texture.OnInitialized += OnCanvasInitialized;
        }
    }

    public void OnCanvasInitialized()
    {
        var dimensions = texture.GetTextureDimensions();
        rayProcessor = new RayMarcher(GetSceneObjects());
        StartCoroutine(Run(dimensions));
    }

    private Geometry[] GetSceneObjects()
    {
        var list = new List<Geometry>();
        for (var i = 0; i < sceneContainer.childCount; i++)
        {
            var child = sceneContainer.GetChild(i);
            if (child.gameObject.activeSelf != true) continue;
            list.Add(GetGeomComponent(child.gameObject));
        }

        return list.ToArray();
    }

    private Geometry GetGeomComponent(GameObject go)
    {
        var mesh = go.GetComponent<MeshFilter>().mesh;
        var pose = new Pose(go.transform.position, go.transform.rotation);
        switch (mesh.name)
        {
            case "Sphere Instance":
                return new Sphere(pose, go.transform.localScale.x / 2);
                break;
        }
        return null;
    }

    IEnumerator Run(Vector2Int dimensions)
    {
        this._running = true;
        var totalRays = dimensions.x * dimensions.y;
        while(true)
        {
            rayProcessor.SetGeometries(GetSceneObjects());
            if (this._running == false)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            for (var i = 0; i < totalRays; i += BATCH_SIZE)
            {
                // Task.Run(() =>
                //{
                SendRayBundle(dimensions, i, i + BATCH_SIZE);
                // });
            }
            Debug.Log("End of render RAys");
            // _running = false;
            yield return new WaitForSeconds(1f);
        }
    }

    public void SendRayBundle(Vector2Int dimensions, int start, int end)
    {
        int currentPixel = start;
        int maxPixel = Math.Min(end, (int)dimensions.x * (int)dimensions.y);
        while (currentPixel < maxPixel)
        {
            Vector2Int pixelCoordinates = new Vector2Int(
                currentPixel % dimensions.x,
                Mathf.FloorToInt(currentPixel / dimensions.x)
            );

            var color = SendRay(pixelCoordinates);
            texture.SetPixel(pixelCoordinates, color);
            currentPixel++;
        }
    }

    public Color SendRay(Vector2Int pixel)
    {
        var ray = PixelRay(pixel);
        // Debug.DrawLine(ray.origin, ray.origin + ray.direction * 1000f, Color.red, 1000f);
        return this.rayProcessor.NewRay(ray);
        
    }

    private Ray PixelRay(Vector2Int pixel)
    {
        var textureTransform = texture.transform;
        var canvasCenter = Vector2.zero;
        var canvasDimensions = texture.GetPhysicalDimensions();
        var pixelDimensions = canvasDimensions / texture.GetTextureDimensions();

        var topLeft = new Vector2(
            -canvasDimensions.x / 2,
            canvasDimensions.y / 2
        );
        var localPixelPosition = topLeft + new Vector2(
            pixelDimensions.x * pixel.x,
            pixelDimensions.y * -pixel.y
        );

        var transformed = textureTransform.TransformDirection(localPixelPosition);

        return new Ray(
            cam.transform.position,
            (textureTransform.position + transformed) - cam.transform.position
        );
    }
}
