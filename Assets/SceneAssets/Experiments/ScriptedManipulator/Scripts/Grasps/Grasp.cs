using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps {
  [ExecuteInEditMode]
  public class Grasp : MonoBehaviour {
    [SerializeField] bool _draw_ray_cast;
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
        Debug.DrawLine(position,
                       position - transform1.forward * this._obstruction_cast_length,
                       color);
        var up = transform1.up;
        Debug.DrawLine(this.transform.position - up * this._obstruction_cast_radius,
                       position + up * this._obstruction_cast_radius,
                       color);
        var right = transform1.right;
        Debug.DrawLine(position - right * this._obstruction_cast_radius,
                       position + right * this._obstruction_cast_radius,
                       color);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsObstructed() {
      RaycastHit hit;
      if (Physics.Linecast(this.transform.position,
                           this.transform.position - this.transform.forward * this._obstruction_cast_length,
                           LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      if (Physics.SphereCast(this.transform.position,
                             this._obstruction_cast_radius,
                             -this.transform.forward,
                             out hit,
                             this._obstruction_cast_length,
                             LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      var transform1 = this.transform;
      var position = transform1.position;
      var forward = transform1.forward;
      return Physics
             .OverlapCapsule(position - forward * this._obstruction_cast_radius,
                             position - forward * this._obstruction_cast_length,
                             this._obstruction_cast_radius,
                             LayerMask.GetMask("Obstruction")).Length
             > 0;
    }
  }
}
