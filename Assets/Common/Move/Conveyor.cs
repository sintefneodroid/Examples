using System;
using UnityEngine;
using Object = System.Object;

namespace Common.Move {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class Conveyor : MonoBehaviour {
    /// <summary>
    ///
    /// </summary>
    [SerializeField]
    float speed;

    /// <summary>
    ///
    /// </summary>
    [SerializeField]
    float visualSpeedScalar;

    [SerializeField] Vector3 _direction;
    [SerializeField] float _dampening = 0.2f;
    float _current_scroll;
    Renderer _renderer;
    Material _material;
    static readonly int _bump_map = Shader.PropertyToID("_BumpMap");

    void Start() {
      this._renderer = this.GetComponent<Renderer>();
      this._material = this._renderer.material;
    }

    void Update() {
      // Scroll texture to fake it moving
      this._current_scroll =
          this._current_scroll + Time.deltaTime * this.speed * this.visualSpeedScalar % 1.0f;
      var offset = new Vector2(0, this._current_scroll);

      this._material.mainTextureOffset = offset;
      this._material.SetTextureOffset(_bump_map, offset);
    }

// Anything that is touching will move
// This function repeats as long as the object is touching
    void OnCollisionStay(Collision other_thing) {
      // Get the direction of the conveyor belt
      // (transform.forward is a built in Vector3
      // which is used to get the forward facing direction)
      // * Remember Vector3's can used for position AND direction AND rotation
      this._direction = this._direction * this.speed;

      // Add a WORLD force to the other objects
      // Ignore the mass of the other objects so they all go the same speed (ForceMode.Acceleration)
      //other_thing.rigidbody.AddForce(-this.transform.forward*this.speed, ForceMode
      //.VelocityChange);
      other_thing.rigidbody.velocity = -this.transform.forward * this.speed;
      var copy = -other_thing.rigidbody.angularVelocity;
      other_thing.rigidbody.AddTorque(copy * this._dampening);
    }
  }
}
