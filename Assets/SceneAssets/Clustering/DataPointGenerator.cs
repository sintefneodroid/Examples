namespace SceneAssets.Clustering {
  public class DataPointGenerator : droid.Runtime.Prototyping.Configurables.Configurable {
    [UnityEngine.SerializeField] droid.Runtime.Structs.Space.Sample.SampleSpace3 distributions;

    [UnityEngine.SerializeField] int _num_data_points = 2000;

    [UnityEngine.SerializeField] DataPoint[] data_points = { };

    public override void
        ApplyConfiguration(droid.Runtime.Interfaces.IConfigurableConfiguration configuration) { }

    public override droid.Runtime.Messaging.Messages.Configuration[] SampleConfigurations() {
      return new droid.Runtime.Messaging.Messages.Configuration[] { };
    }

    public override void UpdateCurrentConfiguration() { }

    public override void PrototypingReset() {
      base.PrototypingReset();
      this.ResampleDataPoints();
    }

    void ResampleDataPoints() {
      if (this.data_points != null) {
        foreach (var data_point in this.data_points) {
          Destroy(obj : data_point.gameObject);
        }
      }

      this.data_points = new DataPoint[this._num_data_points];

      for (var i = 0; i < this._num_data_points; i++) {
        var go = new UnityEngine.GameObject();
        go.transform.parent = this.transform;
        go.transform.position = this.distributions.Sample();
        var dp = go.AddComponent<DataPoint>();

        this.data_points[i] = dp;
      }
    }
  }
}