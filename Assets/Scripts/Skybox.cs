using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    private float targetFPS = 75;
    private float rotation  = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation += rotationSpeed * Time.deltaTime * targetFPS;
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}
