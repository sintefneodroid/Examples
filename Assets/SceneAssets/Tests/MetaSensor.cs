namespace SceneAssets.Tests {
  public class MetaSensor : droid.Runtime.Prototyping.Sensors.Experimental.SingleValueSensor {
    public override System.Collections.Generic.IEnumerable<float> FloatEnumerable {
      get {
        yield return this.ParentEnvironment.StepI;
        yield return this.ParentEnvironment.Sensors.Count;
        yield return this.ParentEnvironment.Listeners.Count;
      }
    }

    public override void UpdateObservation() { }
  }
}