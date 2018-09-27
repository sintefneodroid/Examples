using UnityEngine;

namespace Common.reference_checker.Tests {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class RefCheckerTestComponent : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    public int _ExampleInt;

    [IgnoreRefChecker] public MonoBehaviour _ExampleWithTag;
    public MonoBehaviour _WithoutReferenceWithoutTag; // Should print log
    public MonoBehaviour _WithoutReferenceWithoutTag2; // Should print log
    public MonoBehaviour _WithReference;

    MonoBehaviour _private_non_serializable;
    [SerializeField] MonoBehaviour _private_serializable; // Should print log

    [HideInInspector] public MonoBehaviour _HiddenInInspector;
  }
}
