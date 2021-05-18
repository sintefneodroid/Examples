namespace SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps {
  [UnityEngine.ExecuteInEditMode]
  public class Grasp : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] bool _draw_ray_cast = false;
    [UnityEngine.SerializeField] float _obstruction_cast_length = 0.1f;
    [UnityEngine.SerializeField] float _obstruction_cast_radius = 0.1f;

    void Update() {
      var color = UnityEngine.Color.white;
      if (this.IsObstructed()) {
        color = UnityEngine.Color.red;
      }

      if (this._draw_ray_cast) {
        var transform1 = this.transform;
        var position = transform1.position;
        UnityEngine.Debug.DrawLine(start : position,
                                   end : position - transform1.forward * this._obstruction_cast_length,
                                   color : color);
        var up = transform1.up;
        UnityEngine.Debug.DrawLine(start : this.transform.position - up * this._obstruction_cast_radius,
                                   end : position + up * this._obstruction_cast_radius,
                                   color : color);
        var right = transform1.right;
        UnityEngine.Debug.DrawLine(start : position - right * this._obstruction_cast_radius,
                                   end : position + right * this._obstruction_cast_radius,
                                   color : color);
      }
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public bool IsObstructed() {
      if (UnityEngine.Physics.Linecast(start : this.transform.position,
                                       end : this.transform.position
                                             - this.transform.forward * this._obstruction_cast_length,
                                       layerMask : UnityEngine.LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      if (UnityEngine.Physics.SphereCast(origin : this.transform.position,
                                         radius : this._obstruction_cast_radius,
                                         direction : -this.transform.forward,
                                         hitInfo : out var hit,
                                         maxDistance : this._obstruction_cast_length,
                                         layerMask : UnityEngine.LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      var transform1 = this.transform;
      var position = transform1.position;
      var forward = transform1.forward;
      return UnityEngine.Physics.OverlapCapsule(point0 : position - forward * this._obstruction_cast_radius,
                                                point1 : position - forward * this._obstruction_cast_length,
                                                radius : this._obstruction_cast_radius,
                                                layerMask : UnityEngine.LayerMask.GetMask("Obstruction"))
                        .Length
             > 0;
    }
  }
}