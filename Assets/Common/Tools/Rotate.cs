using UnityEngine;

namespace Excluded.Hide.SharpShadowLight.Scripts.Utilities {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class Rotate : MonoBehaviour {
    [SerializeField] Vector3 _rotation_vector = Vector3.up;
    [SerializeField] float _speed = 0.1f;

    void FixedUpdate() { this.transform.Rotate(this._rotation_vector, this._speed); }
  }
}