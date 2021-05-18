namespace SceneAssets.BalanceBall {
  /// <summary>
  /// </summary>
  public class MouseGimbal : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField]
    [UnityEngine.RangeAttribute(0, 89)]
    float maxRotationDegrees = 10.0f; // At 90+ gimbal oddities must be dealt with.

    [UnityEngine.SerializeField] bool ClampToMaxRotationDegrees = true; // Disable for free rotation.
    [UnityEngine.SerializeField] float rotationSpeed = 10.0f;

    void Update() {
      if (this.maxRotationDegrees > 0) {
        // Apply the 'pre-clamp' rotation (rotation-Z and rotation-X from X & Y of mouse, respectively).
        this.SimpleRotation(mouse_xy : this.GetMouseInput());
      }

      if (this.ClampToMaxRotationDegrees) {
        // Clamp rotation to maxRotationDegrees.
        this.transform.rotation =
            RotationClamping.ClampRotation(temp_eulers : this.transform.rotation.eulerAngles,
                                           low : this.maxRotationDegrees,
                                           high : this.maxRotationDegrees);
      }
    }

    UnityEngine.Vector2 GetMouseInput() {
      UnityEngine.Vector2 mouse_xy;
      mouse_xy.x = -UnityEngine.Input.GetAxis("Mouse X"); // MouseX -> rotZ.
      mouse_xy.y = UnityEngine.Input.GetAxis("Mouse Y"); // MouseY -> rotX.
      return mouse_xy;
    }

    void SimpleRotation(UnityEngine.Vector2 mouse_xy) {
      var rotation = UnityEngine.Vector3.zero;
      rotation.x = mouse_xy.y * UnityEngine.Time.deltaTime * this.rotationSpeed;
      rotation.z = mouse_xy.x * UnityEngine.Time.deltaTime * this.rotationSpeed;
      this.transform.Rotate(eulers : rotation, relativeTo : UnityEngine.Space.Self);
    }
  }
}