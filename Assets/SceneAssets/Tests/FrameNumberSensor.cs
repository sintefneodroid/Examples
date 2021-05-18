namespace SceneAssets.Tests {
  public class FrameNumberSensor : droid.Runtime.Prototyping.Sensors.Experimental.SingleValueSensor {
    public override System.Collections.Generic.IEnumerable<float> FloatEnumerable {
      get { yield return this.ParentEnvironment.StepI; }
    }

    public override void UpdateObservation() { }
  }
}