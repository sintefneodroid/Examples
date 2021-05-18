namespace SceneAssets.Walker.Skyboxes.Test.Misc {
  public class CameraMove : UnityEngine.MonoBehaviour {
    void Update() {
      this.transform.localRotation =
          UnityEngine.Quaternion.AngleAxis(angle : UnityEngine.Time.time * 30.0f,
                                           axis : UnityEngine.Vector3.up)
          * UnityEngine.Quaternion.AngleAxis(angle : UnityEngine.Mathf.Sin(f : UnityEngine.Time.time * 0.37f)
                                                     * 80.0f,
                                             axis : UnityEngine.Vector3.right);
    }
  }
}