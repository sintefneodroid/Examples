namespace SceneAssets.Clustering {
  public class DataPoint : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] protected float gizmo_radius = 0.1f;

    void OnDrawGizmosSelected() {
      UnityEngine.Gizmos.DrawSphere(center : this.transform.position, radius : this.gizmo_radius);
    }
  }
}