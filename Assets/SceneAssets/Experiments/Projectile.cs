using UnityEngine;

namespace SceneAssets.Experiments
{
    public class Projectile : MonoBehaviour
    {
        // launch variables
        [SerializeField] Transform TargetTransform;
        [Range(1.0f, 15.0f)] public float TargetRadius;
        [Range(20.0f, 75.0f)] public float LaunchAngle;
        [Range(0.0f, 10.0f)] public float TargetHeightOffsetFromGround;
        [SerializeField] bool RandomizeHeightOffset;

        // state
        private bool bTargetReady;
        private bool bTouchingGround;

        // cache
        private Rigidbody rigid;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
    
        //-----------------------------------------------------------------------------------------------

        // Use this for initialization
        void Start()
        {   
            this.rigid = this.GetComponent<Rigidbody>();
            this.bTargetReady = false;
            this.bTouchingGround = true;
            var transform1 = this.transform;
            this.initialPosition = transform1.position;
            this.initialRotation = transform1.rotation;
        }

        // resets the projectile to its initial position
        void ResetToInitialState()
        {
            this.rigid.velocity = Vector3.zero;
            this.transform.SetPositionAndRotation(this.initialPosition, this.initialRotation);
            this.bTouchingGround = true;
            this.bTargetReady = false;
        }

        // Update is called once per frame
        void Update ()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (this.bTargetReady)
                {
                    this.Launch();
                }
                else
                {
                    this.ResetToInitialState();
                    this.SetNewTarget();
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                this.ResetToInitialState();
            }

            if (!this.bTouchingGround && !this.bTargetReady)
            {
                // update the rotation of the projectile during trajectory motion
                this.transform.rotation = Quaternion.LookRotation(this.rigid.velocity) * this.initialRotation;
            }
        }

        void OnCollisionEnter()
        {
            this.bTouchingGround = true;
        }

        void OnCollisionExit()
        {
            this.bTouchingGround = false;
        }

        // returns the distance between the red dot and the TargetObject's y-position
        // this is a very little offset considered the ranges in this demo so it shouldn't make a big difference.
        // however, if this code is tested on smaller values, the lack of this offset might introduce errors.
        // to be technically accurate, consider using this offset together with the target platform's y-position.
        float GetPlatformOffset()
        {
            var platformOffset = 0.0f;
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
            foreach (var childTransform in this.TargetTransform.GetComponentsInChildren<Transform>())
            {
                // take into account the y-offset of the Mark gameobject, which essentially
                // is (y-offset + y-scale/2) of the Platform as we've set earlier through the editor.
                if (childTransform.name == "Mark")
                {
                    platformOffset = childTransform.localPosition.y;
                    break;
                }
            }
            return platformOffset;
        }

        // launches the object towards the TargetObject with a given LaunchAngle
        void Launch()
        {
            // think of it as top-down view of vectors: 
            //   we don't care about the y-component(height) of the initial and target position.
            var position = this.transform.position;
            var projectileXZPos = new Vector3(position.x, 0.0f, position.z);
            var position1 = this.TargetTransform.position;
            var targetXZPos = new Vector3(position1.x, 0.0f, position1.z);
        
            // rotate the object to face the target
            this.transform.LookAt(targetXZPos);

            // shorthands for the formula
            var R = Vector3.Distance(projectileXZPos, targetXZPos);
            var G = Physics.gravity.y;
            var tanAlpha = Mathf.Tan(this.LaunchAngle * Mathf.Deg2Rad);
            var H = (position1.y + this.GetPlatformOffset()) - position.y;

            // calculate the local space components of the velocity 
            // required to land the projectile on the target object 
            var Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)) );
            var Vy = tanAlpha * Vz;

            // create the velocity vector in local space and get it in global space
            var localVelocity = new Vector3(0f, Vy, Vz);
            var globalVelocity = this.transform.TransformDirection(localVelocity);

            // launch the object by setting its initial velocity and flipping its state
            this.rigid.velocity = globalVelocity;
            this.bTargetReady = false;
        }

        // Sets a random target around the object based on the TargetRadius
        void SetNewTarget(){

            // To acquire our new target from a point around the projectile object:
            // - we start with a vector in the XZ-Plane (ground), let's pick right (1, 0, 0).
            //    (or pick left, forward, back, or any perpendicular vector to the rotation axis, which is up)
            // - We'll use a quaternion to rotate our vector. To create a rotation quaternion, we'll be using
            //    the AngleAxis() function, which takes a rotation angle and a rotation amount in degrees as parameters.
            var rotationAxis = Vector3.up;  // as our object is on the XZ-Plane, we'll use up vector as the rotation axis.
            var randomAngle = Random.Range(0.0f, 360.0f);
            var randomVectorOnGroundPlane = Quaternion.AngleAxis(randomAngle, rotationAxis) * Vector3.right;

            // Add a random offset to the height of the target location:
            // - If the RandomizeHeightOffset flag is turned on, pick a random number between 0.2f and 1.0f to make sure
            //    we're somewhat above or below the ground. If the flag is off, just pick 1.0f. Finally, scale this number
            //    with the TargetHeightOffsetFromGround.
            // - We want to randomly determine if the target is above or below ground.
            //    Randomly assign the multiplier -1.0f or 1.0f
            // - Create an offset vector from the random height and add the offset vector to the random point on the plane
            var heightOffset = (this.RandomizeHeightOffset ? Random.Range(0.2f, 1.0f) : 1.0f) * this.TargetHeightOffsetFromGround;
            var aboveOrBelowGround = (Random.Range(0.0f, 1.0f) > 0.5f ? 1.0f : -1.0f);
            var heightOffsetVector = new Vector3(0, heightOffset, 0) * aboveOrBelowGround;
            var randomPoint = randomVectorOnGroundPlane * this.TargetRadius + heightOffsetVector;

            //  - finally, we'll set the target object's position and update our state.
            this.TargetTransform.SetPositionAndRotation(randomPoint, this.TargetTransform.rotation);
            this.bTargetReady = true;
        }
    }
}
