using System.Collections;
using Neodroid.Utilities;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedGrasper.Utilities.DataCollection {
  public class TimedRespawn : MonoBehaviour {
    [SerializeField] bool _debugging;
    [SerializeField] Grasps.Grasp _grasp;
    [SerializeField] Scripts.GraspableObject _graspable_object;
    [SerializeField] Scripts.ScriptedGripper _gripper;
    [SerializeField] Vector3 _initial_position;
    [SerializeField] Quaternion _initial_rotation;
    [SerializeField] Rigidbody[] _rigid_bodies;
    [SerializeField] Rigidbody _rigid_body;

    // Use this for initialization
    void Start() {
      if (!this._graspable_object) {
        this._graspable_object = this.GetComponent<Scripts.GraspableObject>();
      }

      if (!this._gripper) {
        this._gripper = FindObjectOfType<Scripts.ScriptedGripper>();
      }

      this._grasp = this._graspable_object.GetOptimalGrasp(this._gripper).Item1;
      this._rigid_body = this._grasp.GetComponentInParent<Rigidbody>();
      this._rigid_bodies = this._graspable_object.GetComponentsInChildren<Rigidbody>();
      this._initial_position = this._rigid_body.transform.position;
      this._initial_rotation = this._rigid_body.transform.rotation;

      Neodroid.Utilities.Unsorted.NeodroidUtilities.RegisterCollisionTriggerCallbacksOnChildren(
          this,
          this.transform,
          this.OnCollisionEnterChild,
          this.OnTriggerEnterChild,
          this.OnCollisionExitChild,
          this.OnTriggerExitChild,
          this.OnCollisionStayChild,
          this.OnTriggerStayChild,
          this._debugging);
    }

    void OnTriggerStayChild(GameObject child_game_object, Collider col) { }

    void OnCollisionStayChild(GameObject child_game_object, Collision collision) { }

    void OnCollisionEnterChild(GameObject child_game_object, Collision collision) {
      if (collision.gameObject.CompareTag("Floor")) {
        this.StopCoroutine(nameof(this.RespawnObject));
        this.StartCoroutine(nameof(this.RespawnObject));
      }
    }

    IEnumerator RespawnObject() {
      yield return new WaitForSeconds(.5f);
      this.StopCoroutine(nameof(this.MakeObjectVisible));
      this._graspable_object.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
      this._rigid_body.transform.position = this._initial_position;
      this._rigid_body.transform.rotation = this._initial_rotation;
      this.MakeRigidBodiesSleep();
      this.StartCoroutine(nameof(this.MakeObjectVisible));
    }

    void MakeRigidBodiesSleep() {
      foreach (var body in this._rigid_bodies) {
        body.useGravity = false;
        //body.isKinematic = true;
        body.Sleep();
      }

      //_rigid_body.isKinematic = true;
      //_rigid_body.useGravity = false;
      //_rigid_body.Sleep ();
    }

    void WakeUpRigidBodies() {
      foreach (var body in this._rigid_bodies) {
        //body.isKinematic = false;
        body.useGravity = true;
        body.WakeUp();
      }

      //_rigid_body.isKinematic = false;
      //_rigid_body.useGravity = true;
      //_rigid_body.WakeUp ();
    }

    IEnumerator MakeObjectVisible() {
      yield return new WaitForSeconds(.5f);
      this._rigid_body.transform.position = this._initial_position;
      this._rigid_body.transform.rotation = this._initial_rotation;
      this.WakeUpRigidBodies();
      this._graspable_object.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    }

    void OnTriggerEnterChild(GameObject child_game_object, Collider col) { }

    void OnCollisionExitChild(GameObject child_game_object, Collision collision) {
      if (collision.gameObject.CompareTag("Floor")) {
        this.StopCoroutine(nameof(this.RespawnObject));
      }
    }

    void OnTriggerExitChild(GameObject child_game_object, Collider col) { }

    // Update is called once per frame
    void Update() { }
  }
}
