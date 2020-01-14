using System;
using UnityEngine;
using Object = System.Object;

namespace SceneAssets.Clustering {
  public class CategoricalDataPoint : DataPoint {
    [SerializeField] string category;

    public String Category { get { return this.category; } set { this.category = value; } }

    void OnDrawGizmosSelected() {
      var color = Color.green;
      color.g = (Hash128.Compute(this.category).GetHashCode() % 255) / 256f;
      Gizmos.color = color;
      Gizmos.DrawSphere(this.transform.position,this.gizmo_radius);
    }
  }
}
