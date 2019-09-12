using System;
using droid.Runtime.Prototyping.ObjectiveFunctions.Spatial;

namespace SceneAssets.BalanceBall {
  /// <summary>
  ///
  /// </summary>
  public class CenterDevianceObjectionFunction : SpatialObjectionFunction {
    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Single InternalEvaluate() { return 0.1f; }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public override void InternalReset() { }
  }
}
