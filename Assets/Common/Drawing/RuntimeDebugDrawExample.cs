#if UNITY_EDITOR
using System.Globalization;
using Common.Drawing.RuntimeDebugDraw;
using UnityEngine;

namespace Common.Drawing {
  /// <summary>
  /// </summary>
  public class RuntimeDebugDrawExample : MonoBehaviour {
    static float _born_radian_offset;

    Vector3 _born_pos;
    int _color_ix;
    Color[] _colors = {Color.red, Color.blue, Color.cyan, Color.magenta, Color.yellow};
    float _interval_timer;

    float _timer;

    Color GetNextColor() {
      this._color_ix = (this._color_ix + 1) % this._colors.Length;
      return this._colors[this._color_ix];
    }

    void Awake() {
      this._born_pos = this.transform.position;
      this._timer = _born_radian_offset;
      _born_radian_offset += Mathf.PI * 0.3f;

      Draw.AttachText(
          this.transform,
          () => this.transform.position.y.ToString(CultureInfo.InvariantCulture),
          Vector3.up,
          Color.white,
          12);
    }

    void Update() {
      this._timer += Time.deltaTime * 1.5f;

      var dx = Mathf.Cos(this._timer) * 5f;
      var dy = Mathf.Sin(this._timer) * 3f;
      this.transform.position = new Vector3(dx, dy, 0) + this._born_pos;

      Draw.DrawText(this.transform.position, this.transform.position.ToString(), Color.green, 16, 0f);
      Draw.DrawLine(Vector3.zero, this.transform.position, Color.green, 0f, false);

      this._interval_timer += Time.deltaTime;

      if (this._interval_timer > 0.3f) {
        Draw.DrawText(
            this.transform.position,
            Time.frameCount.ToString(),
            this.GetNextColor(),
            16,
            0.5f,
            true);
        Draw.DrawLine(
            this.transform.position,
            this.transform.position + Vector3.up * 1.5f,
            this.GetNextColor(),
            0.5f,
            true);
        this._interval_timer = 0f;
      }
    }
  }
}
#endif