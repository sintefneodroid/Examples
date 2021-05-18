namespace SceneAssets.Move {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  [UnityEngine.RequireComponent(requiredComponent : typeof(UnityEngine.Renderer))]
  public class Conveyor : UnityEngine.MonoBehaviour {
    static readonly int _bump_map = UnityEngine.Shader.PropertyToID("_BumpMap");

    /// <summary>
    /// </summary>
    [UnityEngine.SerializeField]
    float actualVelocity = 1f;

    /// <summary>
    /// </summary>
    [UnityEngine.SerializeField]
    float visualVelocity = 1f;

    [UnityEngine.SerializeField] UnityEngine.Vector3 _direction = UnityEngine.Vector3.zero;
    [UnityEngine.SerializeField] float _dampening = 0.2f;
    [UnityEngine.SerializeField] bool use_shared = true;
    float _current_scroll = 0f;
    UnityEngine.Material _material = null;
    UnityEngine.Renderer _renderer = null;

    void Start() {
      this._renderer = this.GetComponent<UnityEngine.Renderer>();
      if (this.use_shared) {
        this._material = this._renderer.sharedMaterial;
      } else {
        this._material = this._renderer.material;
      }
    }

    void Update() {
      // Scroll texture to fake it moving
      this._current_scroll = this._current_scroll
                             + UnityEngine.Time.deltaTime * this.actualVelocity * this.visualVelocity % 1.0f;
      var offset = new UnityEngine.Vector2(0, y : this._current_scroll);

      this._material.mainTextureOffset = offset;
      this._material.SetTextureOffset(nameID : _bump_map, value : offset);
    }

// Anything that is touching will move
// This function repeats as long as the object is touching
    void OnCollisionStay(UnityEngine.Collision other_thing) {
      // Get the direction of the conveyor belt
      // (transform.forward is a built in Vector3
      // which is used to get the forward facing direction)
      // * Remember Vector3's can used for position AND direction AND rotation
      var direction = this._direction * this.actualVelocity;

      // Add a WORLD force to the other objects
      // Ignore the mass of the other objects so they all go the same speed (ForceMode.Acceleration)
      //other_thing.rigidbody.AddForce(-this.transform.forward*this.speed, ForceMode
      //.VelocityChange);
      var force = -this.transform.TransformDirection(direction : direction) * this.actualVelocity;
      //other_thing.rigidbody.velocity = force;

      foreach (var c in other_thing.contacts) {
        other_thing.rigidbody.AddForceAtPosition(force : (force - other_thing.rigidbody.velocity)
                                                         * (1 / (c.separation + 1)),
                                                 position : c.point + c.normal);
      }

      /*foreach (var c in other_thing.contacts) {
        other_thing.rigidbody.AddForce((force-other_thing.rigidbody.velocity)*(1/(c.separation+1)));
        break;
      }*/

      var copy = -other_thing.rigidbody.angularVelocity;
      other_thing.rigidbody.AddTorque(torque : copy * this._dampening);
      var copy2 = -other_thing.rigidbody.velocity;
      other_thing.rigidbody.AddForce(force : copy2 * this._dampening);
    }
  }
}