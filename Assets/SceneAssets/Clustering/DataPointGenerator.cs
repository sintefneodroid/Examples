using droid.Runtime.Interfaces;
using droid.Runtime.Messaging.Messages;
using droid.Runtime.Prototyping.Configurables;
using droid.Runtime.Structs.Space.Sample;
using UnityEngine;

namespace SceneAssets.Clustering {
  public class DataPointGenerator : Configurable {
    [SerializeField] SampleSpace3 distributions;

    [SerializeField] int _num_data_points = 2000;

    [SerializeField] DataPoint[] data_points = { };

    public override void ApplyConfiguration(IConfigurableConfiguration configuration) { return; }
    public override Configuration[] SampleConfigurations() { return new Configuration[] { }; }

    public override void UpdateCurrentConfiguration() { }

    public override void PrototypingReset() {
      base.PrototypingReset();
      this.ResampleDataPoints();
    }

    void ResampleDataPoints() {
      if (this.data_points != null)
        foreach (var data_point in this.data_points) {
          Destroy(data_point.gameObject);
        }

      this.data_points = new DataPoint[this._num_data_points];

      for (var i = 0; i < this._num_data_points; i++) {
        var go = new GameObject();
        go.transform.parent = this.transform;
        go.transform.position = distributions.Sample();
        var dp = go.AddComponent<DataPoint>();

        this.data_points[i] = dp;
      }
    }
  }
}
