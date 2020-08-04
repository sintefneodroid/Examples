using System.Collections;
using droid.Runtime.GameObjects.ChildSensors;
using droid.Runtime.Utilities;
using SceneAssets.Experiments.ScriptedManipulator.Scripts;
using SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps;
using Unity.Mathematics;
using UnityEngine;
using NeodroidUtilities = droid.Runtime.Utilities.Extensions.NeodroidUtilities;

namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <summary>
  ///
  /// </summary>
  public class GraspTimedRespawn : MonoBehaviour {
    [SerializeField] bool _debugging = false;
    [SerializeField] Grasp _grasp = null;
    [SerializeField] GraspableObject _graspable_object = null;
    [SerializeField] ScriptedGrasping _grasping = null;
    [SerializeField] Vector3 _initial_position = Vector3.zero;
    [SerializeField] Quaternion _initial_rotation = quaternion.identity;
    [SerializeField] Rigidbody[] _rigid_bodies = null;
    [SerializeField] Rigidbody _rigid_body = null;
    WaitForSeconds _wait_for_seconds = new WaitForSeconds(.5f);

    // Use this for initialization
    void Start() {
      if (!this._graspable_object) {
        this._graspable_object = this.GetComponent<GraspableObject>();
      }

      if (!this._grasping) {
        this._grasping = FindObjectOfType<ScriptedGrasping>();
      }

      this._grasp = this._graspable_object.GetOptimalGrasp(grasping : this._grasping).Item1;
      this._rigid_body = this._grasp.GetComponentInParent<Rigidbody>();
      this._rigid_bodies = this._graspable_object.GetComponentsInChildren<Rigidbody>();
      var transform1 = this._rigid_body.transform;
      this._initial_position = transform1.position;
      this._initial_rotation = transform1.rotation;

      NeodroidRegistrationUtilities
          .RegisterCollisionTriggerCallbacksOnChildren<ChildCollider3DSensor, Collider, Collision>(caller : this,
                                                                                                   parent : this
                                                                                                       .transform,
                                                                                                   on_collision_enter_child : this
                                                                                                       .OnCollisionEnterChild,
                                                                                                   on_trigger_enter_child : this
                                                                                                       .OnTriggerEnterChild,
                                                                                                   on_collision_exit_child : this
                                                                                                       .OnCollisionExitChild,
                                                                                                   on_trigger_exit_child : this
                                                                                                       .OnTriggerExitChild,
                                                                                                   on_collision_stay_child : this
                                                                                                       .OnCollisionStayChild,
                                                                                                   on_trigger_stay_child : this
                                                                                                       .OnTriggerStayChild,
                                                                                                   debug : this
                                                                                                       ._debugging);
    }

    void OnTriggerStayChild(GameObject child_game_object, Collider col) { }

    void OnCollisionStayChild(GameObject child_game_object, Collision collision) { }

    void OnCollisionEnterChild(GameObject child_game_object, Collision collision) {
      if (collision.gameObject.CompareTag("Floor")) {
        this.StopCoroutine(methodName : nameof(this.RespawnObject));
        this.StartCoroutine(methodName : nameof(this.RespawnObject));
      }
    }

    IEnumerator RespawnObject() {
      yield return this._wait_for_seconds;
      this.StopCoroutine(methodName : nameof(this.MakeObjectVisible));
      this._graspable_object.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
      var transform1 = this._rigid_body.transform;
      transform1.position = this._initial_position;
      transform1.rotation = this._initial_rotation;
      this.MakeRigidBodiesSleep();
      this.StartCoroutine(methodName : nameof(this.MakeObjectVisible));
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
      yield return this._wait_for_seconds;
      var transform1 = this._rigid_body.transform;
      transform1.position = this._initial_position;
      transform1.rotation = this._initial_rotation;
      this.WakeUpRigidBodies();
      this._graspable_object.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    }

    void OnTriggerEnterChild(GameObject child_game_object, Collider col) { }

    void OnCollisionExitChild(GameObject child_game_object, Collision collision) {
      if (collision.gameObject.CompareTag("Floor")) {
        this.StopCoroutine(methodName : nameof(this.RespawnObject));
      }
    }

    void OnTriggerExitChild(GameObject child_game_object, Collider col) { }

    // Update is called once per frame
    void Update() { }
  }
}
