public class DollyZoom : UnityEngine.MonoBehaviour {
  public UnityEngine.Transform target;

  UnityEngine.Camera cam;
  float distance = 0f;
  float fov = 60;

  float viewWidth = 10f;

  void Start() { this.cam = UnityEngine.Camera.main; }

  void Update() {
    var pos = this.target.transform.position;

    this.fov = this.cam.fieldOfView;
    this.distance = this.viewWidth
                    / (2f * UnityEngine.Mathf.Tan(f : 0.5f * this.fov * UnityEngine.Mathf.Deg2Rad));

    pos.z = -UnityEngine.Mathf.Abs(f : this.distance);
    this.cam.transform.position = pos;

    UnityEngine.Debug.Log(message : this.distance);
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