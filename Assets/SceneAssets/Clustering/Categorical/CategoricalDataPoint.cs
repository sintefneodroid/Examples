namespace SceneAssets.Clustering.Categorical {
  public class CategoricalDataPoint : DataPoint {
    [UnityEngine.SerializeField] string category;

    public string Category { get { return this.category; } set { this.category = value; } }

    void OnDrawGizmosSelected() {
      var color = UnityEngine.Color.green;
      color.g = UnityEngine.Hash128.Compute(data : this.category).GetHashCode() % 255 / 256f;
      UnityEngine.Gizmos.color = color;
      UnityEngine.Gizmos.DrawSphere(center : this.transform.position, radius : this.gizmo_radius);
    }
  }
}