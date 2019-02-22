using UnityEngine;

namespace Common.Move {
  public class Conveyor : MonoBehaviour
  {
    public float speed;
    public float visualSpeedScalar;

    Vector3 _direction;
    float _current_scroll;
    Renderer _renderer;

    void Start() { this._renderer = this.GetComponent<Renderer>(); }

    void Update()
    {
      // Scroll texture to fake it moving
      this._current_scroll = this._current_scroll + Time.deltaTime* this.speed* this.visualSpeedScalar;
      this._renderer.material.mainTextureOffset = new Vector2(0, this._current_scroll);
    }

// Anything that is touching will move
// This function repeats as long as the object is touching
    void OnCollisionStay(Collision other_thing)
    {
      // Get the direction of the conveyor belt
      // (transform.forward is a built in Vector3
      // which is used to get the forward facing direction)
      // * Remember Vector3's can used for position AND direction AND rotation
      this._direction = this.transform.forward;
      this._direction = this._direction* this.speed;

      // Add a WORLD force to the other objects
      // Ignore the mass of the other objects so they all go the same speed (ForceMode.Acceleration)
      other_thing.rigidbody.AddForce(this._direction, ForceMode.Acceleration);
    }
  }
}