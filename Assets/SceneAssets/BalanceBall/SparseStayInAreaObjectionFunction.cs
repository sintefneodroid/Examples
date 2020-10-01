using System;
using droid.Runtime.Prototyping.ObjectiveFunctions.Spatial;

namespace SceneAssets.BalanceBall {
  /// <summary>
  ///
  /// </summary>
  public class SparseStayInAreaObjectionFunction : SpatialObjective {
    /// <inheritdoc />
    ///  <summary>
    ///  </summary>
    ///  <returns></returns>
    ///  <exception cref="T:System.NotImplementedException"></exception>
    public override float InternalEvaluate() { return 0.1f; }

    /// <inheritdoc />
    ///  <summary>
    ///  </summary>
    ///  <exception cref="T:System.NotImplementedException"></exception>
    public override void InternalReset() { }
  }
}
