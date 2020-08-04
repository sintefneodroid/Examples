using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps {
  [ExecuteInEditMode]
  public class Grasp : MonoBehaviour {
    [SerializeField] bool _draw_ray_cast = false;
    [SerializeField] float _obstruction_cast_length = 0.1f;
    [SerializeField] float _obstruction_cast_radius = 0.1f;

    void Update() {
      var color = Color.white;
      if (this.IsObstructed()) {
        color = Color.red;
      }

      if (this._draw_ray_cast) {
        var transform1 = this.transform;
        var position = transform1.position;
        Debug.DrawLine(start : position, end : position - transform1.forward * this._obstruction_cast_length, color : color);
        var up = transform1.up;
        Debug.DrawLine(start : this.transform.position - up * this._obstruction_cast_radius,
                       end : position + up * this._obstruction_cast_radius,
                       color : color);
        var right = transform1.right;
        Debug.DrawLine(start : position - right * this._obstruction_cast_radius,
                       end : position + right * this._obstruction_cast_radius,
                       color : color);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsObstructed() {
      if (Physics.Linecast(start : this.transform.position,
                           end : this.transform.position - this.transform.forward * this._obstruction_cast_length,
                           layerMask : LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      if (Physics.SphereCast(origin : this.transform.position,
                             radius : this._obstruction_cast_radius,
                             direction : -this.transform.forward,
                             hitInfo : out var hit,
                             maxDistance : this._obstruction_cast_length,
                             layerMask : LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      var transform1 = this.transform;
      var position = transform1.position;
      var forward = transform1.forward;
      return Physics.OverlapCapsule(point0 : position - forward * this._obstruction_cast_radius,
                                    point1 : position - forward * this._obstruction_cast_length,
                                    radius : this._obstruction_cast_radius,
                                    layerMask : LayerMask.GetMask("Obstruction")).Length
             > 0;
    }
  }
}
