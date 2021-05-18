namespace SceneAssets.Clustering.Categorical {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class CategoricalDataPointGenerator : droid.Runtime.Prototyping.Configurables.Configurable {
    [UnityEngine.SerializeField] droid.Runtime.Structs.Space.Sample.SampleSpace3[] distributions;

    [UnityEngine.SerializeField] int data_points_per_category = 2000;

    [UnityEngine.SerializeField]
    System.Collections.Generic.Dictionary<string, UnityEngine.GameObject> categories =
        new System.Collections.Generic.Dictionary<string, UnityEngine.GameObject>();

    [UnityEngine.SerializeField]
    System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<DataPoint>> data_points =
        new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<DataPoint>>();

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    public override void
        ApplyConfiguration(droid.Runtime.Interfaces.IConfigurableConfiguration configuration) { }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override droid.Runtime.Messaging.Messages.Configuration[] SampleConfigurations() {
      return new droid.Runtime.Messaging.Messages.Configuration[] { };
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void UpdateCurrentConfiguration() { }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override void PrototypingReset() {
      base.PrototypingReset();
      this.ResampleDataPoints();
    }

    void ResampleDataPoints() {
      if (this.categories == null) {
        this.categories = new System.Collections.Generic.Dictionary<string, UnityEngine.GameObject>();
      }

      if (this.data_points == null) {
        this.data_points =
            new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<DataPoint>>();
      }

      foreach (var data_point in this.data_points) {
        foreach (var point in data_point.Value) {
          Destroy(obj : point.gameObject);
        }
      }

      this.data_points.Clear();

      foreach (var category in this.categories) {
        Destroy(obj : category.Value.gameObject);
      }

      this.categories.Clear();

      for (var index = 0; index < this.distributions.Length; index++) {
        var cat_name = $"category{index}";
        var cat_go = new UnityEngine.GameObject(name : cat_name);
        cat_go.transform.parent = this.transform;
        cat_go.transform.position = this.distributions[index].Space.Mean;
        var sample_space3 = this.distributions[index];
        var list = new System.Collections.Generic.List<DataPoint>();
        for (var i = 0; i < this.data_points_per_category; i++) {
          var go = new UnityEngine.GameObject();
          go.transform.parent = cat_go.transform;
          go.transform.position = sample_space3.Sample();
          var dp = go.AddComponent<CategoricalDataPoint>();
          dp.Category = cat_name;
          go.name = $"{cat_name}#{i}";
          list.Add(item : dp);
        }

        this.data_points.Add(key : cat_name, value : list);
        this.categories.Add(key : cat_name, value : cat_go);
      }
    }
  }
}