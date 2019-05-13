using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTexture : MonoBehaviour {

    public Camera camera;
    public Renderer renderer;
    private Texture2D texture;
    private int scale = 2;
    public Action OnInitialized;

    public bool Initialized { get; internal set; }

    // Use this for initialization
    void Start () {
        texture = new Texture2D(Screen.width, Screen.height);
        
        float ratio = ((float) Screen.width) /((float) Screen.height);
        this.transform.localScale = new Vector3(
            ratio,
            1f,
            1f
        );

        // The base plane width with scale=1 is 10
        var inGameScreenWidth = ratio * 10;
        var factor = (inGameScreenWidth / 2) / Mathf.Atan(camera.fieldOfView / 2);
        this.transform.position = camera.transform.position + camera.transform.forward * factor;

        renderer.material.mainTexture = texture;

        Initialized = true;
        OnInitialized?.Invoke();
    }

    private float updateInterval = 0.5f;
    private float nextUpdate = 0f;
    void Update()
    {
       if(nextUpdate < Time.fixedTime)
        {
            Debug.Log("Applying texture");
            this.texture.Apply();
            nextUpdate = Time.fixedTime + updateInterval;
        }
    }
	
    public Vector2Int GetTextureDimensions()
    {
        return new Vector2Int(texture.width / scale, texture.height / scale);
    }

    public Vector2 GetPhysicalDimensions()
    {
        return transform.localScale * 10;
    }

    public void SetPixel(Vector2Int position, Color color)
    {
        for (var x = 0; x < scale; x++)
        {
            for (var y = 0; y < scale; y++)
            {
                this.texture.SetPixel(position.x * scale + x, position.y * scale + y, color);
            }
        }
       
    }

}
