using UnityEngine;

namespace EffectExamples.Shared.Scripts {
  public class GunAim : MonoBehaviour {
    public int _BorderLeft;
    public int _BorderRight;
    public int _BorderTop;
    public int _BorderBottom;

    Camera _parent_camera;
    bool _is_out_of_bounds;

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
