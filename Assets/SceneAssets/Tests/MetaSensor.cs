using System;
using System.Collections.Generic;
using droid.Runtime.Prototyping.Sensors.Experimental;

namespace SceneAssets.Tests {
  public class MetaSensor : SingleValueSensor {
    public override IEnumerable<float> FloatEnumerable { get { yield return this.ParentEnvironment.StepI;
      yield return this.ParentEnvironment.Sensors.Count;
      yield return this.ParentEnvironment.Listeners.Count;
    } }
    public override void UpdateObservation() { }
  }
}
