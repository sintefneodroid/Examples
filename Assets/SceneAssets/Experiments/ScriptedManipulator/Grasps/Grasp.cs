using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Grasps {
  [ExecuteInEditMode]
  public class Grasp : MonoBehaviour {
    [SerializeField] float _obstruction_cast_length = 0.1f;
    [SerializeField] float _obstruction_cast_radius = 0.1f;
    [SerializeField] bool _draw_ray_cast;

    void Update() {
      var color = Color.white;
      if (this.IsObstructed()) {
        color = Color.red;
      }

      if (this._draw_ray_cast) {
        Debug.DrawLine(
            this.transform.position,
            this.transform.position - this.transform.forward * this._obstruction_cast_length,
            color);
        Debug.DrawLine(
            this.transform.position - this.transform.up * this._obstruction_cast_radius,
            this.transform.position + this.transform.up * this._obstruction_cast_radius,
            color);
        Debug.DrawLine(
            this.transform.position - this.transform.right * this._obstruction_cast_radius,
            this.transform.position + this.transform.right * this._obstruction_cast_radius,
            color);
      }
    }

    public bool IsObstructed() {
      RaycastHit hit;
      if (Physics.Linecast(
          this.transform.position,
          this.transform.position - this.transform.forward * this._obstruction_cast_length,
          LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      if (Physics.SphereCast(
          this.transform.position,
          this._obstruction_cast_radius,
          -this.transform.forward,
          out hit,
          this._obstruction_cast_length,
          LayerMask.GetMask("Obstruction"))) {
        return true;
      }

      return Physics.OverlapCapsule(
                 this.transform.position - this.transform.forward * this._obstruction_cast_radius,
                 this.transform.position - this.transform.forward * this._obstruction_cast_length,
                 this._obstruction_cast_radius,
                 LayerMask.GetMask("Obstruction")).Length
             > 0;
    }
  }
}