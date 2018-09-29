using UnityEngine;

namespace Common.EffectExamples.Shared.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class GunAim : MonoBehaviour {
    public int _BorderBottom;
    public int _BorderLeft;
    public int _BorderRight;
    public int _BorderTop;
    bool _is_out_of_bounds;

    Camera _parent_camera;

    void Start() { this._parent_camera = this.GetComponentInParent<Camera>(); }

    void Update() {
      var mouse_x = Input.mousePosition.x;
      var mouse_y = Input.mousePosition.y;

      if (mouse_x <= this._BorderLeft
          || mouse_x >= Screen.width - this._BorderRight
          || mouse_y <= this._BorderBottom
          || mouse_y >= Screen.height - this._BorderTop) {
        this._is_out_of_bounds = true;
      } else {
        this._is_out_of_bounds = false;
      }

      if (!this._is_out_of_bounds) {
        this.transform.LookAt(this._parent_camera.ScreenToWorldPoint(new Vector3(mouse_x, mouse_y, 5.0f)));
      }
    }

    public bool GetIsOutOfBounds() { return this._is_out_of_bounds; }
  }
}
