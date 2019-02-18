using UnityEngine;

namespace Common.Move {
    public class ConveyorBelt : MonoBehaviour
    {
        /// <summary>
        ///
        /// </summary>
        public float speed;
        public float visualSpeedScalar;

        Vector3 direction;
        float currentScroll;

        Renderer _renderer;

        void Start() { this._renderer = this.GetComponent<Renderer>(); }

        void Update()
        {
            // Scroll texture to fake it moving
            this.currentScroll = this.currentScroll + Time.deltaTime*this.speed*this.visualSpeedScalar;
            this._renderer.material.mainTextureOffset = new Vector2(0, this.currentScroll);
        }

// Anything that is touching will move
// This function repeats as long as the object is touching
        void OnCollisionStay(Collision other_thing)
        {
            // Get the direction of the conveyor belt
            // (transform.forward is a built in Vector3
            // which is used to get the forward facing direction)
            // * Remember Vector3's can used for position AND direction AND rotation
            this.direction = this.transform.forward;
            this.direction = this.direction*this.speed;

            // Add a WORLD force to the other objects
            // Ignore the mass of the other objects so they all go the same speed (ForceMode.Acceleration)
            other_thing.rigidbody.AddForce(this.direction, ForceMode.Acceleration);
        }
    }
}