using UnityEngine
using System.Collections

public class DollyZoom : MonoBehaviour
{

    public Transform target;

    Camera cam;
    float distance = 0f;
    float fov = 60;

    float viewWidth = 10f;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 pos = target.transform.position;

        fov = cam.fieldOfView;
        distance = viewWidth / (2f * Mathf.Tan(0.5f * fov * Mathf.Deg2Rad));

        pos.z = -Mathf.Abs(distance);
        cam.transform.position = pos;


        Debug.Log(distance);
    }
}

/*

using UnityEngine;
using System.Collections;

public class ExampleScript : MonoBehaviour {
    public Transform target;
    public Camera camera;
    
    private float initHeightAtDist;
    private bool dzEnabled;

    // Calculate the frustum height at a given distance from the camera.
    void FrustumHeightAtDistance(float distance) {
        return 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    // Calculate the FOV needed to get a given frustum height at a given distance.
    void FOVForHeightAndDistance(float height, float distance) {
        return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }

    // Start the dolly zoom effect.
    void StartDZ() {
        var distance = Vector3.Distance(transform.position, target.position);
        initHeightAtDist = FrustumHeightAtDistance(distance);
        dzEnabled = true;
    }
    
    // Turn dolly zoom off.
    void StopDZ() {
        dzEnabled = false;
    }
    
    void Start() {
        StartDZ();
    }

    void Update () {
        if (dzEnabled) {
            // Measure the new distance and readjust the FOV accordingly.
            var currDistance = Vector3.Distance(transform.position, target.position);
            camera.fieldOfView = FOVForHeightAndDistance(initHeightAtDist, currDistance);
        }
        
        // Simple control to allow the camera to be moved in and out using the up/down arrows.
        transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * Time.deltaTime * 5f);
    }
}


*/