using System;
using System.Collections.Generic;
using droid.Runtime.Interfaces;
using droid.Runtime.Messaging.Messages;
using droid.Runtime.Prototyping.Configurables;
using droid.Runtime.Sampling;
using droid.Runtime.Structs.Space.Sample;
using UnityEngine;

namespace SceneAssets.Clustering {
  public class CategoricalDataPointGenerator : Configurable {
    [SerializeField] SampleSpace3[] distributions;

    [SerializeField] int data_points_per_category = 2000;

    [SerializeField] Dictionary<string, GameObject> categories = new Dictionary<String, GameObject>();

    [SerializeField]
    Dictionary<string, List<DataPoint>> data_points = new Dictionary<String, List<DataPoint>>();

    public override void ApplyConfiguration(IConfigurableConfiguration configuration) { return; }
    public override Configuration[] SampleConfigurations() { return new Configuration[] { }; }

    public override void UpdateCurrentConfiguration() { }

    public override void PrototypingReset() {
      base.PrototypingReset();
      this.ResampleDataPoints();
    }

    void ResampleDataPoints() {
      if (this.categories == null)
        this.categories = new Dictionary<String, GameObject>();

      if (this.data_points == null)
        this.data_points = new Dictionary<String, List<DataPoint>>();

      foreach (var data_point in this.data_points) {
        foreach (var point in data_point.Value) {
          Destroy(point.gameObject);
        }
      }

      this.data_points.Clear();

      foreach (var category in this.categories) {
        Destroy(category.Value.gameObject);
      }

      this.categories.Clear();

      for (var index = 0; index < this.distributions.Length; index++) {
        var cat_name = $"category{index}";
        var cat_go = new GameObject(cat_name);
        cat_go.transform.parent = this.transform;
        cat_go.transform.position = this.distributions[index].Space.Mean;
        var sample_space3 = this.distributions[index];
        var list = new List<DataPoint>();
        for (var i = 0; i < this.data_points_per_category; i++) {
          var go = new GameObject();
          go.transform.parent = cat_go.transform;
          go.transform.position = sample_space3.Sample();
          var dp = go.AddComponent<CategoricalDataPoint>();
          dp.Category = cat_name;
          go.name = $"{cat_name}#{i}";
          list.Add(dp);
        }

        this.data_points.Add(cat_name, list);
        this.categories.Add(cat_name, cat_go);
      }
    }
  }
}
