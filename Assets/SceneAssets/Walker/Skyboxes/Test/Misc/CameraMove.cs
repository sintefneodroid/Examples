using UnityEngine;

namespace SceneAssets.Walker.Skyboxes.Test.Misc {
  public class CameraMove : MonoBehaviour {
    void Update() {
      this.transform.localRotation = Quaternion.AngleAxis(Time.time * 30.0f, Vector3.up)
                                     * Quaternion.AngleAxis(Mathf.Sin(Time.time * 0.37f) * 80.0f,
                                                            Vector3.right);
    }
  }
}
