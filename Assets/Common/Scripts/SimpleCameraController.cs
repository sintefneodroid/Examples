using UnityEngine;

namespace Common.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class SimpleCameraController : MonoBehaviour {
    [Header("Movement Settings")]
    [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
    public float _Boost = 3.5f;

    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool _InvertY;

    CameraState _m_interpolating_camera_state = new CameraState();

    CameraState _m_target_camera_state = new CameraState();

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public AnimationCurve _MouseSensitivityCurve = new AnimationCurve(
        new Keyframe(0f, 0.5f, 0f, 5f),
        new Keyframe(1f, 2.5f, 0f, 0f));

    [Tooltip("Time it takes to interpolate camera position 99% of the way to the target.")]
    [Range(0.001f, 1f)]
    public float _PositionLerpTime = 0.2f;

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target.")]
    [Range(0.001f, 1f)]
    public float _RotationLerpTime = 0.01f;

    void OnEnable() {
      this._m_target_camera_state.SetFromTransform(this.transform);
      this._m_interpolating_camera_state.SetFromTransform(this.transform);
    }

    Vector3 GetInputTranslationDirection() {
      var direction = new Vector3();
      if (Input.GetKey(KeyCode.W)) {
        direction += Vector3.forward;
      }

      if (Input.GetKey(KeyCode.S)) {
        direction += Vector3.back;
      }

      if (Input.GetKey(KeyCode.A)) {
        direction += Vector3.left;
      }

      if (Input.GetKey(KeyCode.D)) {
        direction += Vector3.right;
      }

      if (Input.GetKey(KeyCode.Q)) {
        direction += Vector3.down;
      }

      if (Input.GetKey(KeyCode.E)) {
        direction += Vector3.up;
      }

      return direction;
    }

    void Update() {
      // Hide and lock cursor when right mouse button pressed
      if (Input.GetMouseButtonDown(1)) {
        Cursor.lockState = CursorLockMode.Locked;
      }

      // Unlock and show cursor when right mouse button released
      if (Input.GetMouseButtonUp(1)) {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
      }

      // Rotation
      if (Input.GetMouseButton(1)) {
        var mouse_movement = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y") * (this._InvertY ? 1 : -1));

        var mouse_sensitivity_factor = this._MouseSensitivityCurve.Evaluate(mouse_movement.magnitude);

        this._m_target_camera_state._Yaw += mouse_movement.x * mouse_sensitivity_factor;
        this._m_target_camera_state._Pitch += mouse_movement.y * mouse_sensitivity_factor;
      }

      // Translation
      var translation = this.GetInputTranslationDirection() * Time.deltaTime;

      // Speed up movement when shift key held
      if (Input.GetKey(KeyCode.LeftShift)) {
        translation *= 10.0f;
      }

      // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
      this._Boost += Input.mouseScrollDelta.y * 0.2f;
      translation *= Mathf.Pow(2.0f, this._Boost);

      this._m_target_camera_state.Translate(translation);

      // Framerate-independent interpolation
      // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
      var position_lerp_pct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / this._PositionLerpTime * Time.deltaTime);
      var rotation_lerp_pct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / this._RotationLerpTime * Time.deltaTime);
      this._m_interpolating_camera_state.LerpTowards(
          this._m_target_camera_state,
          position_lerp_pct,
          rotation_lerp_pct);

      this._m_interpolating_camera_state.UpdateTransform(this.transform);
    }

    /// <summary>
    /// </summary>
    class CameraState {
      public float _Pitch;
      public float _Roll;
      public float _X;
      public float _Y;
      public float _Yaw;
      public float _Z;

      public void SetFromTransform(Transform t) {
        var euler_angles = t.eulerAngles;
        this._Pitch = euler_angles.x;
        this._Yaw = euler_angles.y;
        this._Roll = euler_angles.z;
        var position = t.position;
        this._X = position.x;
        this._Y = position.y;
        this._Z = position.z;
      }

      public void Translate(Vector3 translation) {
        var rotated_translation = Quaternion.Euler(this._Pitch, this._Yaw, this._Roll) * translation;

        this._X += rotated_translation.x;
        this._Y += rotated_translation.y;
        this._Z += rotated_translation.z;
      }

      public void LerpTowards(CameraState target, float position_lerp_pct, float rotation_lerp_pct) {
        this._Yaw = Mathf.Lerp(this._Yaw, target._Yaw, rotation_lerp_pct);
        this._Pitch = Mathf.Lerp(this._Pitch, target._Pitch, rotation_lerp_pct);
        this._Roll = Mathf.Lerp(this._Roll, target._Roll, rotation_lerp_pct);

        this._X = Mathf.Lerp(this._X, target._X, position_lerp_pct);
        this._Y = Mathf.Lerp(this._Y, target._Y, position_lerp_pct);
        this._Z = Mathf.Lerp(this._Z, target._Z, position_lerp_pct);
      }

      public void UpdateTransform(Transform t) {
        t.eulerAngles = new Vector3(this._Pitch, this._Yaw, this._Roll);
        t.position = new Vector3(this._X, this._Y, this._Z);
      }
    }
  }
}