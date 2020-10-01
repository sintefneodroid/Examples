using System;
using UnityEngine;
using Object = System.Object;

namespace SceneAssets.Clustering {
  public class DataPoint : MonoBehaviour {
    [SerializeField] protected float gizmo_radius = 0.1f;

    void OnDrawGizmosSelected() {
      Gizmos.DrawSphere(center : this.transform.position,radius : this.gizmo_radius);
    }
  }
}
