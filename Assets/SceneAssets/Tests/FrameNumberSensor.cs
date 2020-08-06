using System;
using System.Collections.Generic;
using droid.Runtime.Prototyping.Sensors.Experimental;

namespace SceneAssets.Tests {
  public class FrameNumberSensor : SingleValueSensor {
    public override IEnumerable<float> FloatEnumerable { get { yield return this.ParentEnvironment.StepI; } }
    public override void UpdateObservation() { }
  }
}
