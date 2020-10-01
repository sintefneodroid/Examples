using System;
using UnityEngine;

namespace SceneAssets.Clustering.Categorical {
  public class CategoricalDataPoint : DataPoint {
    [SerializeField] string category;

    public string Category { get { return this.category; } set { this.category = value; } }

    void OnDrawGizmosSelected() {
      var color = Color.green;
      color.g = Hash128.Compute(this.category).GetHashCode() % 255 / 256f;
      Gizmos.color = color;
      Gizmos.DrawSphere(center : this.transform.position,radius : this.gizmo_radius);
    }
  }
}
