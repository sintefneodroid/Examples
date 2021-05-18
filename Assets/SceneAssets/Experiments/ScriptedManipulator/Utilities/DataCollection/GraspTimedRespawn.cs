namespace SceneAssets.Experiments.ScriptedManipulator.Utilities.DataCollection {
  /// <summary>
  /// </summary>
  public class GraspTimedRespawn : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] bool _debugging = false;

    [UnityEngine.SerializeField]
    SceneAssets.Experiments.ScriptedManipulator.Scripts.Grasps.Grasp _grasp = null;

    [UnityEngine.SerializeField]
    SceneAssets.Experiments.ScriptedManipulator.Scripts.GraspableObject _graspable_object = null;

    [UnityEngine.SerializeField]
    SceneAssets.Experiments.ScriptedManipulator.Scripts.ScriptedGrasping _grasping = null;

    [UnityEngine.SerializeField] UnityEngine.Vector3 _initial_position = UnityEngine.Vector3.zero;

    [UnityEngine.SerializeField]
    UnityEngine.Quaternion _initial_rotation = Unity.Mathematics.quaternion.identity;

    [UnityEngine.SerializeField] UnityEngine.Rigidbody[] _rigid_bodies = null;
    [UnityEngine.SerializeField] UnityEngine.Rigidbody _rigid_body = null;
    UnityEngine.WaitForSeconds _wait_for_seconds = new UnityEngine.WaitForSeconds(.5f);

    // Use this for initialization
    void Start() {
      if (!this._graspable_object) {
        this._graspable_object =
            this.GetComponent<SceneAssets.Experiments.ScriptedManipulator.Scripts.GraspableObject>();
      }

      if (!this._grasping) {
        this._grasping =
            FindObjectOfType<SceneAssets.Experiments.ScriptedManipulator.Scripts.ScriptedGrasping>();
      }

      this._grasp = this._graspable_object.GetOptimalGrasp(grasping : this._grasping).Item1;
      this._rigid_body = this._grasp.GetComponentInParent<UnityEngine.Rigidbody>();
      this._rigid_bodies = this._graspable_object.GetComponentsInChildren<UnityEngine.Rigidbody>();
      var transform1 = this._rigid_body.transform;
      this._initial_position = transform1.position;
      this._initial_rotation = transform1.rotation;

      droid.Runtime.Utilities.NeodroidRegistrationUtilities
           .RegisterCollisionTriggerCallbacksOnChildren<
               droid.Runtime.GameObjects.ChildSensors.ChildCollider3DSensor, UnityEngine.Collider,
               UnityEngine.Collision>(caller : this,
                                      parent : this.transform,
                                      on_collision_enter_child : this.OnCollisionEnterChild,
                                      on_trigger_enter_child : this.OnTriggerEnterChild,
                                      on_collision_exit_child : this.OnCollisionExitChild,
                                      on_trigger_exit_child : this.OnTriggerExitChild,
                                      on_collision_stay_child : this.OnCollisionStayChild,
                                      on_trigger_stay_child : this.OnTriggerStayChild,
                                      debug : this._debugging);
    }

    // Update is called once per frame
    void Update() { }

    void OnTriggerStayChild(UnityEngine.GameObject child_game_object, UnityEngine.Collider col) { }

    void OnCollisionStayChild(UnityEngine.GameObject child_game_object, UnityEngine.Collision collision) { }

    void OnCollisionEnterChild(UnityEngine.GameObject child_game_object, UnityEngine.Collision collision) {
      if (collision.gameObject.CompareTag("Floor")) {
        this.StopCoroutine(methodName : nameof(this.RespawnObject));
        this.StartCoroutine(methodName : nameof(this.RespawnObject));
      }
    }

    System.Collections.IEnumerator RespawnObject() {
      yield return this._wait_for_seconds;
      this.StopCoroutine(methodName : nameof(this.MakeObjectVisible));
      this._graspable_object.GetComponentInChildren<UnityEngine.SkinnedMeshRenderer>().enabled = false;
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

    System.Collections.IEnumerator MakeObjectVisible() {
      yield return this._wait_for_seconds;
      var transform1 = this._rigid_body.transform;
      transform1.position = this._initial_position;
      transform1.rotation = this._initial_rotation;
      this.WakeUpRigidBodies();
      this._graspable_object.GetComponentInChildren<UnityEngine.SkinnedMeshRenderer>().enabled = true;
    }

    void OnTriggerEnterChild(UnityEngine.GameObject child_game_object, UnityEngine.Collider col) { }

    void OnCollisionExitChild(UnityEngine.GameObject child_game_object, UnityEngine.Collision collision) {
      if (collision.gameObject.CompareTag("Floor")) {
        this.StopCoroutine(methodName : nameof(this.RespawnObject));
      }
    }

    void OnTriggerExitChild(UnityEngine.GameObject child_game_object, UnityEngine.Collider col) { }
  }
}