namespace SceneAssets.BalanceBall {
  /// <summary>
  /// </summary>
  public class SparseStayInAreaObjectionFunction :
      droid.Runtime.Prototyping.ObjectiveFunctions.Spatial.SpatialObjective {
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public override float InternalEvaluate() { return 0.1f; }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <exception cref="T:System.NotImplementedException"></exception>
    public override void InternalReset() { }
  }
}