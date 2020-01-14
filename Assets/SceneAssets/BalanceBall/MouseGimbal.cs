using UnityEngine;

namespace SceneAssets.BalanceBall {
  /// <summary>
  /// 
  /// </summary>
  public class MouseGimbal : MonoBehaviour {
    [SerializeField]
    [Range(0, 89)]
    float maxRotationDegrees = 10.0f; // At 90+ gimbal oddities must be dealt with.

    [SerializeField] bool ClampToMaxRotationDegrees = true; // Disable for free rotation.
    [SerializeField] float rotationSpeed = 10.0f;

    void Update() {
      if (this.maxRotationDegrees > 0) {
        // Apply the 'pre-clamp' rotation (rotation-Z and rotation-X from X & Y of mouse, respectively).
        this.SimpleRotation(this.GetMouseInput());
      }

      if (this.ClampToMaxRotationDegrees) {
        // Clamp rotation to maxRotationDegrees.
        this.transform.rotation = RotationClamping.ClampRotation(temp_eulers : this.transform.rotation.eulerAngles,
                                                                 low : this.maxRotationDegrees,
                                                                 high : this.maxRotationDegrees);
      }
    }

    Vector2 GetMouseInput() {
      Vector2 mouse_xy;
      mouse_xy.x = -Input.GetAxis("Mouse X"); // MouseX -> rotZ.
      mouse_xy.y = Input.GetAxis("Mouse Y"); // MouseY -> rotX.
      return mouse_xy;
    }

    void SimpleRotation(Vector2 mouse_xy) {
      var rotation = Vector3.zero;
      rotation.x = mouse_xy.y * Time.deltaTime * this.rotationSpeed;
      rotation.z = mouse_xy.x * Time.deltaTime * this.rotationSpeed;
      this.transform.Rotate(eulers : rotation, relativeTo : Space.Self);
    }
  }
}
