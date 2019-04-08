using UnityEngine;

namespace SceneAssets.Experiments {
  /// <inheritdoc />
  /// <summary>
  /// </summary>
  public class Projectile : MonoBehaviour {
    // launch variables
    [SerializeField] Transform TargetTransform=null;

    /// <summary>
    /// 
    /// </summary>
    [Range(1.0f, 15.0f)]
    public float TargetRadius=1f;

    /// <summary>
    /// 
    /// </summary>
    [Range(20.0f, 75.0f)]
    public float LaunchAngle=20f;

    /// <summary>
    /// 
    /// </summary>
    [Range(0.0f, 10.0f)]
    public float TargetHeightOffsetFromGround=0f;

    [SerializeField] bool RandomizeHeightOffset=true;

    // state
    bool _b_target_ready=false;
    bool _b_touching_ground=false;

    // cache
    Rigidbody _rigidbody=null;
    Vector3 _initial_position=Vector3.zero;
    Quaternion _initial_rotation=Quaternion.identity;

    //-----------------------------------------------------------------------------------------------

    // Use this for initialization
    void Start() {
      this._rigidbody = this.GetComponent<Rigidbody>();
      this._b_target_ready = false;
      this._b_touching_ground = true;
      var transform1 = this.transform;
      this._initial_position = transform1.position;
      this._initial_rotation = transform1.rotation;
    }

    // resets the projectile to its initial position
    void ResetToInitialState() {
      this._rigidbody.velocity = Vector3.zero;
      this.transform.SetPositionAndRotation(this._initial_position, this._initial_rotation);
      this._b_touching_ground = true;
      this._b_target_ready = false;
    }

    // Update is called once per frame
    void Update() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        if (this._b_target_ready) {
          this.Launch();
        } else {
          this.ResetToInitialState();
          this.SetNewTarget();
        }
      }

      if (Input.GetKeyDown(KeyCode.R)) {
        this.ResetToInitialState();
      }

      if (!this._b_touching_ground && !this._b_target_ready) {
        // update the rotation of the projectile during trajectory motion
        this.transform.rotation = Quaternion.LookRotation(this._rigidbody.velocity) * this._initial_rotation;
      }
    }

    void OnCollisionEnter() { this._b_touching_ground = true; }

    void OnCollisionExit() { this._b_touching_ground = false; }

    // returns the distance between the red dot and the TargetObject's y-position
    // this is a very little offset considered the ranges in this demo so it shouldn't make a big difference.
    // however, if this code is tested on smaller values, the lack of this offset might introduce errors.
    // to be technically accurate, consider using this offset together with the target platform's y-position.
    float GetPlatformOffset() {
      var platform_offset = 0.0f;
      // 
      //          (SIDE VIEW OF THE PLATFORM)
      //
      //                   +------------------------- Mark (Sprite)
      //                   v
      //                  ___                                          -+-
      //    +-------------   ------------+         <- Platform (Cube)   |  platformOffset
      // ---|--------------X-------------|-----    <- TargetObject     -+-
      //    +----------------------------+
      //

      // we're iterating through Mark (Sprite) and Platform (Cube) Transforms. 
      foreach (var child_transform in this.TargetTransform.GetComponentsInChildren<Transform>()) {
        // take into account the y-offset of the Mark gameobject, which essentially
        // is (y-offset + y-scale/2) of the Platform as we've set earlier through the editor.
        if (child_transform.name == "Mark") {
          platform_offset = child_transform.localPosition.y;
          break;
        }
      }

      return platform_offset;
    }

    // launches the object towards the TargetObject with a given LaunchAngle
    void Launch() {
      // think of it as top-down view of vectors: 
      //   we don't care about the y-component(height) of the initial and target position.
      var position = this.transform.position;
      var projectile_xz_pos = new Vector3(position.x, 0.0f, position.z);
      var position1 = this.TargetTransform.position;
      var target_xz_pos = new Vector3(position1.x, 0.0f, position1.z);

      // rotate the object to face the target
      this.transform.LookAt(target_xz_pos);

      // shorthands for the formula
      var r = Vector3.Distance(projectile_xz_pos, target_xz_pos);
      var g = Physics.gravity.y;
      var tan_alpha = Mathf.Tan(this.LaunchAngle * Mathf.Deg2Rad);
      var h = (position1.y + this.GetPlatformOffset()) - position.y;

      // calculate the local space components of the velocity 
      // required to land the projectile on the target object 
      var vz = Mathf.Sqrt(g * r * r / (2.0f * (h - r * tan_alpha)));
      var vy = tan_alpha * vz;

      // create the velocity vector in local space and get it in global space
      var local_velocity = new Vector3(0f, vy, vz);
      var global_velocity = this.transform.TransformDirection(local_velocity);

      // launch the object by setting its initial velocity and flipping its state
      this._rigidbody.velocity = global_velocity;
      this._b_target_ready = false;
    }

    // Sets a random target around the object based on the TargetRadius
    void SetNewTarget() {
      // To acquire our new target from a point around the projectile object:
      // - we start with a vector in the XZ-Plane (ground), let's pick right (1, 0, 0).
      //    (or pick left, forward, back, or any perpendicular vector to the rotation axis, which is up)
      // - We'll use a quaternion to rotate our vector. To create a rotation quaternion, we'll be using
      //    the AngleAxis() function, which takes a rotation angle and a rotation amount in degrees as parameters.
      var rotation_axis =
          Vector3.up; // as our object is on the XZ-Plane, we'll use up vector as the rotation axis.
      var random_angle = Random.Range(0.0f, 360.0f);
      var random_vector_on_ground_plane = Quaternion.AngleAxis(random_angle, rotation_axis) * Vector3.right;

      // Add a random offset to the height of the target location:
      // - If the RandomizeHeightOffset flag is turned on, pick a random number between 0.2f and 1.0f to make sure
      //    we're somewhat above or below the ground. If the flag is off, just pick 1.0f. Finally, scale this number
      //    with the TargetHeightOffsetFromGround.
      // - We want to randomly determine if the target is above or below ground.
      //    Randomly assign the multiplier -1.0f or 1.0f
      // - Create an offset vector from the random height and add the offset vector to the random point on the plane
      var height_offset = (this.RandomizeHeightOffset ? Random.Range(0.2f, 1.0f) : 1.0f)
                          * this.TargetHeightOffsetFromGround;
      var above_or_below_ground = (Random.Range(0.0f, 1.0f) > 0.5f ? 1.0f : -1.0f);
      var height_offset_vector = new Vector3(0, height_offset, 0) * above_or_below_ground;
      var random_point = random_vector_on_ground_plane * this.TargetRadius + height_offset_vector;

      //  - finally, we'll set the target object's position and update our state.
      this.TargetTransform.SetPositionAndRotation(random_point, this.TargetTransform.rotation);
      this._b_target_ready = true;
    }
  }
}
