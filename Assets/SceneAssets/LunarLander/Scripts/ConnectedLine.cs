using UnityEngine;

namespace SceneAssets.LunarLander.Scripts {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [ExecuteInEditMode]
  [RequireComponent(typeof(LineRenderer))]
  public class ConnectedLine : MonoBehaviour {
    public Transform _Connection_To;
    LineRenderer _line_renderer;

    [SerializeField] Vector3 _offset = Vector3.up;
    [SerializeField] bool _use_local_transforms;

    // Use this for initialization
    /// <summary>
    /// </summary>
    void Start() {
      this._line_renderer = this.GetComponent<LineRenderer>();
      if (!this._Connection_To) {
        this._Connection_To = this.GetComponentInParent<Transform>();
      }
    }

    // Update is called once per frame
    void Update() {
      this._line_renderer.SetPosition(0, this.transform.position);

      if (this._Connection_To) {
        if (this._use_local_transforms) {
          this._line_renderer.SetPosition(
              1,
              this.transform.InverseTransformPoint(
                  this._Connection_To.TransformPoint(this._Connection_To.localPosition + this._offset)));
        } else {
          this._line_renderer.SetPosition(1, this._Connection_To.position);
        }
      }
    }
  }
}