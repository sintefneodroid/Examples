namespace SceneAssets.Games.Creatures2 {
  [UnityEngine.RequireComponent(requiredComponent : typeof(UnityEngine.Rigidbody))]
  public class LaserBehaviour : UnityEngine.MonoBehaviour {
    // we want to store the laser's velocity every frame
    // so we can use this data during collisions to reflect
    public UnityEngine.Vector3 _OldVelocity;

    UnityEngine.Rigidbody _rb;
    //public float fireSpeed = 2f;

    void Start() {
      // set our laser on its merry way. no need to update transform manually
      //rigidbody.velocity = Vector3.forward * fireSpeed;

      // freeze the rotation so it doesnt go spinning after a collision
      //rigidbody.freezeRotation = true;
      this._rb = this.GetComponent<UnityEngine.Rigidbody>();
    }

    void FixedUpdate() {
      // because we want the velocity after physics, we put this in fixed update
      this._OldVelocity = this._rb.velocity;
    }

    // when a collision happens
    void OnCollisionEnter(UnityEngine.Collision collision) {
      // get the point of contact
      //ContactPoint contact = collision.contacts[0];

      // reflect our old velocity off the contact point's normal vector
      //Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);        

      // assign the reflected velocity back to the rigidbody
      //rigidbody.velocity = reflectedVelocity;
      // rotate the object by the same amount we changed its velocity
      //Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
      //transform.rotation = rotation * transform.rotation;
    }
  }
}