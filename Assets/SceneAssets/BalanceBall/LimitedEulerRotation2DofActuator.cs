#if UNITY_EDITOR

#endif

namespace SceneAssets.BalanceBall {
  /// <inheritdoc />
  [UnityEngine.AddComponentMenu(menuName :
                                 droid.Runtime.Prototyping.Actuators.ActuatorComponentMenuPath
                                      ._ComponentMenuPath
                                 + "LimitedEulerRotationActuator2Dof"
                                 + droid.Runtime.Prototyping.Actuators.ActuatorComponentMenuPath._Postfix)]
  public class LimitedEulerRotation2DofActuator : droid.Runtime.Prototyping.Actuators.Actuator {
    /// <summary>
    /// </summary>
    [UnityEngine.SerializeField]
    protected droid.Runtime.Structs.Space.Space2 limits =
        droid.Runtime.Structs.Space.Space2.MinusOneOne * 0.25f;

    [UnityEngine.SerializeField] string _x;
    [UnityEngine.SerializeField] string _z;

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public override string PrototypingTypeName { get { return "EulerRotation"; } }

    public override string[] InnerMotionNames { get; }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected() {
      if (this.enabled) {
        var transform1 = this.transform;
        var position = transform1.position;
        var right = transform1.right;
        var forward = transform1.forward;

        UnityEditor.Handles.DrawWireArc(center : position,
                                        normal : right,
                                        from : -forward,
                                        angle : 180,
                                        radius : 2);

        UnityEditor.Handles.DrawWireArc(center : position,
                                        normal : forward,
                                        from : -right,
                                        angle : 180,
                                        radius : 2);
      }
    }

    #endif

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    protected override void RegisterComponent() {
      this._x = this.Identifier + "RotX_";

      this._z = this.Identifier + "RotZ_";

      this.Parent =
          droid.Runtime.Utilities.NeodroidRegistrationUtilities.RegisterComponent(r : (droid.Runtime.
                Interfaces.IHasRegister<droid.Runtime.Interfaces.IActuator>)this.Parent,
            c : this,
            identifier : this._x);
      this.Parent =
          droid.Runtime.Utilities.NeodroidRegistrationUtilities.RegisterComponent(r : (droid.Runtime.
                Interfaces.IHasRegister<droid.Runtime.Interfaces.IActuator>)this.Parent,
            c : this,
            identifier : this._z);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    protected override void UnRegisterComponent() {
      this.Parent?.UnRegister(t : this, obj : this._x);
      this.Parent?.UnRegister(t : this, obj : this._z);
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <param name="motion"></param>
    /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
    protected override void InnerApplyMotion(droid.Runtime.Interfaces.IMotion motion) {
      var m = motion.Strength;
      #if NEODROID_DEBUG
      if (this.Debugging) {
        UnityEngine.Debug.Log(message : $"Inner Applying {m} To {this.name}");
        UnityEngine.Debug.Log(message : $"Current rotation of {this.name} is {this.transform.rotation}");
      }
      #endif

      if (motion.ActuatorName == this._x) {
        this.transform.Rotate(axis : UnityEngine.Vector3.forward, angle : m);
      } else if (motion.ActuatorName == this._z) {
        this.transform.Rotate(axis : UnityEngine.Vector3.right, angle : m);
      }

      this.transform.rotation =
          RotationClamping.ClampRotation(temp_eulers : this.transform.rotation.eulerAngles,
                                         low : this.limits.Max.x,
                                         high : this.limits.Max.x);
    }
  }
}