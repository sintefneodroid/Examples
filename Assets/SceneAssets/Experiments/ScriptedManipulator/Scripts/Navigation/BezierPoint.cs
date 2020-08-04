using System;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Scripts.Navigation {
  /// <inheritdoc />
  /// <summary>
  ///   - Helper class for storing and manipulating Bezier Point data
  ///   - Ensures that handles are in correct relation to one another
  ///   - Handles adding/removing self from curve point lists
  ///   - Calls SetDirty() on curve when edited
  /// </summary>
  [Serializable]
  public class BezierPoint : MonoBehaviour {
    #region PublicEnumerations

    /// <summary>
    ///   - Enumeration describing the relationship between a point's handles
    ///   - Connected : The point's handles are mirrored across the point
    ///   - Broken : Each handle moves independently of the other
    ///   - None : This point has no handles (both handles are located ON the point)
    /// </summary>
    public enum HandleStyle {
      Connected_,
      Broken_,
      None_
    }

    #endregion

    #region PrivateVariables

    /// <summary>
    ///   - Used to determine if this point has moved since the last frame
    /// </summary>
    Vector3 _last_position;

    #endregion

    #region MonoBehaviourFunctions

    void Update() {
      if (!this._curve.Dirty && this.transform.position != this._last_position) {
        this._curve.SetDirty();
        this._last_position = this.transform.position;
      }
    }

    #endregion

    #region PublicProperties

    /// <summary>
    ///   - Curve this point belongs to
    ///   - Changing this value will automatically remove this point from the current curve and add it to the new one
    /// </summary>
    [SerializeField]
    BezierCurve _curve;

    public BezierCurve Curve {
      get { return this._curve; }
      set {
        if (this._curve) {
          this._curve.RemovePoint(point : this);
        }

        this._curve = value;
        this._curve.AddPoint(point : this);
      }
    }

    /// <summary>
    ///   - Value describing the relationship between this point's handles
    /// </summary>
    public HandleStyle _Style;

    /// <summary>
    ///   - Shortcut to transform.position
    /// </summary>
    /// <value>
    ///   - The point's world position
    /// </value>
    public Vector3 Position {
      get { return this.transform.position; }
      set { this.transform.position = value; }
    }

    /// <summary>
    ///   - Shortcut to transform.localPosition
    /// </summary>
    /// <value>
    ///   - The point's local position.
    /// </value>
    public Vector3 LocalPosition {
      get { return this.transform.localPosition; }
      set { this.transform.localPosition = value; }
    }

    /// <summary>
    ///   - Local position of the first handle
    ///   - Setting this value will cause the curve to become dirty
    ///   - This handle effects the curve generated from this point and the point proceeding it in curve.points
    /// </summary>
    [SerializeField]
    Vector3 _handle1;

    public Vector3 Handle1 {
      get { return this._handle1; }
      set {
        if (this._handle1 == value) {
          return;
        }

        this._handle1 = value;
        if (this._Style == HandleStyle.None_) {
          this._Style = HandleStyle.Broken_;
        } else if (this._Style == HandleStyle.Connected_) {
          this._handle2 = -value;
        }

        this._curve.SetDirty();
      }
    }

    /// <summary>
    ///   - Global position of the first handle
    ///   - Ultimately stored in the 'handle1' variable
    ///   - Setting this value will cause the curve to become dirty
    ///   - This handle effects the curve generated from this point and the point proceeding it in curve.points
    /// </summary>
    public Vector3 GlobalHandle1 {
      get { return this.transform.TransformPoint(position : this.Handle1); }
      set { this.Handle1 = this.transform.InverseTransformPoint(position : value); }
    }

    /// <summary>
    ///   - Local position of the second handle
    ///   - Setting this value will cause the curve to become dirty
    ///   - This handle effects the curve generated from this point and the point coming after it in curve.points
    /// </summary>
    [SerializeField]
    Vector3 _handle2;

    public Vector3 Handle2 {
      get { return this._handle2; }
      set {
        if (this._handle2 == value) {
          return;
        }

        this._handle2 = value;
        if (this._Style == HandleStyle.None_) {
          this._Style = HandleStyle.Broken_;
        } else if (this._Style == HandleStyle.Connected_) {
          this._handle1 = -value;
        }

        this._curve.SetDirty();
      }
    }

    /// <summary>
    ///   - Global position of the second handle
    ///   - Ultimately stored in the 'handle2' variable
    ///   - Setting this value will cause the curve to become dirty
    ///   - This handle effects the curve generated from this point and the point coming after it in curve.points
    /// </summary>
    public Vector3 GlobalHandle2 {
      get { return this.transform.TransformPoint(position : this.Handle2); }
      set { this.Handle2 = this.transform.InverseTransformPoint(position : value); }
    }

    #endregion
  }
}
