namespace SceneAssets.Games.Creatures2 {
  public class SelfNormalBounce : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] float _reflection_factor = 1.05f;

    // we want to store the laser's velocity every frame
    // so we can use this data during collisions to reflect
    public UnityEngine.Vector3 _OldVelocity;

    UnityEngine.Rigidbody _rb;
    //public float fireSpeed = 2f;

    public UnityEngine.AudioClip bounce_clip; //TODO: play sound for bounce

    void Start() {
      // set our laser on its merry way. no need to update transform manually
      //rigidbody.velocity = Vector3.forward * fireSpeed;

      // freeze the rotation so it does not go spinning after a collision
      //rigidbody.freezeRotation = true;
      this._rb = this.GetComponent<UnityEngine.Rigidbody>();
    }

    void FixedUpdate() {
      // because we want the velocity after physics, we put this in fixed update
      this._OldVelocity = this._rb.velocity;
    }

    void OnCollisionEnter(UnityEngine.Collision collision) {
      var contact_point = collision.contacts[0];

      this.BouncyReflection(contact_point : contact_point, laser : this);
    }

    void BouncyReflection(UnityEngine.ContactPoint contact_point, SelfNormalBounce laser) {
      var rb = laser.GetComponent<UnityEngine.Rigidbody>();
      //var point_velocity = rb.GetPointVelocity(contact_point.point);
      //var speed = point_velocity.magnitude * _dampening_factor;

      //var direction = Vector3.Reflect(rb.velocity.normalized, contact_point.normal);

      var speed = laser._OldVelocity.magnitude * this._reflection_factor;
      var direction = UnityEngine.Vector3.Reflect(inDirection : laser._OldVelocity.normalized,
                                                  inNormal : contact_point.normal);

      var reflection = direction * speed;

      UnityEngine.Debug.DrawRay(start : contact_point.point,
                                dir : reflection,
                                color : UnityEngine.Color.white,
                                duration : 4.0f);

      rb.velocity = reflection;
      //rb.AddForceAtPosition(reflection, contact_point.point);
    }
  }
}