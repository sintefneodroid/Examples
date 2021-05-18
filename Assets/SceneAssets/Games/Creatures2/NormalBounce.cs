namespace SceneAssets.Games.Creatures2 {
  public class NormalBounce : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] float _reflection_factor = 1.1f;

    void OnCollisionEnter(UnityEngine.Collision collision) {
      var contact_point = collision.contacts[0];
      var rigbody = collision.rigidbody;
      var laser = rigbody.GetComponent<LaserBehaviour>();
      if (laser != null) {
        this.BouncyReflection(contact_point : contact_point, laser : laser);
      }
    }

    void BouncyReflection(UnityEngine.ContactPoint contact_point, LaserBehaviour laser) {
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