using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneAssets.Experiments.ScriptedManipulator.Navigation {
  /// <inheritdoc />
  /// <summary>
  ///   - Class for describing and drawing Bezier Curves
  ///   - Efficiently handles approximate length calculation through 'dirty' system
  ///   - Has static functions for getting points on curves constructed by Vector3 parameters (GetPoint,
  ///   GetCubicPoint,
  ///   GetQuadraticPoint, and GetLinearPoint)
  /// </summary>
  [ExecuteInEditMode, Serializable]
  public class BezierCurve : MonoBehaviour {
    #region PrivateVariables

    /// <summary>
    ///   - Array of point objects that make up this curve
    ///   - Populated through editor
    /// </summary>
    [SerializeField]
    BezierPoint[] _points = new BezierPoint[0];

    #endregion

    #region PublicVariables

    /// <summary>
    ///   - the number of mid-points calculated for each pair of bezier points
    ///   - used for drawing the curve in the editor
    ///   - used for calculating the "length" variable
    /// </summary>
    [SerializeField]
    int _resolution = 30;

    /// <summary>
    ///   Gets or sets a value indicating whether this <see cref="BezierCurve" /> is dirty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if dirty; otherwise, <c>false</c>.
    /// </value>
    public bool Dirty { get; private set; }

    [SerializeField] Color _draw_color = Color.white;

    [SerializeField] bool _draw_curve = true;

    #endregion

    #region PublicProperties

    /// <summary>
    ///   - set in the editor
    ///   - used to determine if the curve should be drawn as "closed" in the editor
    ///   - used to determine if the curve's length should include the curve between the first and the last points in
    ///   "points" array
    ///   - setting this value will cause the curve to become dirty
    /// </summary>
    [SerializeField]
    bool _close;

    public bool Close {
      get { return this._close; }
      set {
        if (this._close == value) {
          return;
        }

        this._close = value;
        this.Dirty = true;
      }
    }

    /// <summary>
    ///   - set internally
    ///   - gets point corresponding to "index" in "points" array
    ///   - does not allow direct set
    /// </summary>
    /// <param name='index'>
    ///   - the index
    /// </param>
    public BezierPoint this[int index] {
      get { return this._points[index]; }
    }

    /// <summary>
    ///   - number of points stored in 'points' variable
    ///   - set internally
    ///   - does not include "handles"
    /// </summary>
    /// <value>
    ///   - The point count
    /// </value>
    public int PointCount {
      get { return this._points.Length; }
    }

    /// <summary>
    ///   - The approximate length of the curve
    ///   - recalculates if the curve is "dirty"
    /// </summary>
    float _length;

    public float Length {
      get {
        if (this.Dirty) {
          this._length = 0;
          for (var i = 0; i < this._points.Length - 1; i++) {
            this._length += ApproximateLength(this._points[i], this._points[i + 1], this._resolution);
          }

          if (this.Close) {
            this._length += ApproximateLength(
                this._points[this._points.Length - 1],
                this._points[0],
                this._resolution);
          }

          this.Dirty = false;
        }

        return this._length;
      }
    }

    #endregion

    #region UnityFunctions

    void OnDrawGizmos() {
      if (this.enabled) {
        Gizmos.color = this._draw_color;
        if (!this._draw_curve) {
          this._draw_color.a = 0;
        } else if (this._draw_curve) {
          this._draw_color.a = 255;
        }

        if (this._points.Length > 1) {
          for (var i = 0; i < this._points.Length - 1; i++) {
            DrawCurve(this._points[i], this._points[i + 1], this._resolution);
          }

          if (this.Close) {
            DrawCurve(this._points[this._points.Length - 1], this._points[0], this._resolution);
          }
        }
      }
    }

    void Awake() { this.Dirty = true; }

    #endregion

    #region PublicFunctions

    /// <summary>
    ///   - Adds the given point to the end of the curve ("points" array)
    /// </summary>
    /// <param name='point'>
    ///   - The point to add.
    /// </param>
    public void AddPoint(BezierPoint point) {
      var temp_array = new List<BezierPoint>(this._points) {point};
      this._points = temp_array.ToArray();
      this.Dirty = true;
    }

    /// <summary>
    ///   - Adds a point at position
    /// </summary>
    /// <returns>
    ///   - The point object
    /// </returns>
    /// <param name='position'>
    ///   - Where to add the point
    /// </param>
    public BezierPoint AddPointAt(Vector3 position) {
      var point_object = new GameObject("Point " + this.PointCount);
      point_object.transform.parent = this.transform;
      point_object.transform.position = position;

      var new_point = point_object.AddComponent<BezierPoint>();
      new_point.Curve = this;
      new_point._Style = BezierPoint.HandleStyle.Broken_;
      return new_point;
    }

    /// <summary>
    ///   - Removes the given point from the curve ("points" array)
    /// </summary>
    /// <param name='point'>
    ///   - The point to remove
    /// </param>
    public void RemovePoint(BezierPoint point) {
      var temp_array = new List<BezierPoint>(this._points);
      temp_array.Remove(point);
      this._points = temp_array.ToArray();
      this.Dirty = false;
    }

    public void ClearPoints() { this._points = new BezierPoint[0]; }

    /// <summary>
    ///   - Gets a copy of the bezier point array used to define this curve
    /// </summary>
    /// <returns>
    ///   - The cloned array of points
    /// </returns>
    public BezierPoint[] GetAnchorPoints() { return (BezierPoint[])this._points.Clone(); }

    /// <summary>
    ///   - Gets the point at 't' percent along this curve
    /// </summary>
    /// <returns>
    ///   - Returns the point at 't' percent
    /// </returns>
    /// <param name='t'>
    ///   - Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
    /// </param>
    public Vector3 GetPointAt(float t) {
      if (t <= 0) {
        return this._points[0].Position;
      }

      if (t >= 1) {
        return this._points[this._points.Length - 1].Position;
      }

      float total_percent = 0;
      float curve_percent = 0;

      BezierPoint p1 = null;
      BezierPoint p2 = null;

      for (var i = 0; i < this._points.Length - 1; i++) {
        curve_percent = ApproximateLength(this._points[i], this._points[i + 1]) / this.Length;
        if (total_percent + curve_percent > t) {
          p1 = this._points[i];
          p2 = this._points[i + 1];
          break;
        }

        total_percent += curve_percent;
      }

      if (this.Close && p1 == null) {
        p1 = this._points[this._points.Length - 1];
        p2 = this._points[0];
      }

      t -= total_percent;

      return GetPoint(p1, p2, t / curve_percent);
    }

    /// <summary>
    ///   - Get the index of the given point in this curve
    /// </summary>
    /// <returns>
    ///   - The index, or -1 if the point is not found
    /// </returns>
    /// <param name='point'>
    ///   - Point to search for
    /// </param>
    public int GetPointIndex(BezierPoint point) {
      var result = -1;
      for (var i = 0; i < this._points.Length; i++) {
        if (this._points[i] == point) {
          result = i;
          break;
        }
      }

      return result;
    }

    /// <summary>
    ///   - Sets this curve to 'dirty'
    ///   - Forces the curve to recalculate its length
    /// </summary>
    public void SetDirty() { this.Dirty = true; }

    #endregion

    #region PublicStaticFunctions

    /// <summary>
    ///   - Draws the curve in the Editor
    /// </summary>
    /// <param name='p1'>
    ///   - The bezier point at the beginning of the curve
    /// </param>
    /// <param name='p2'>
    ///   - The bezier point at the end of the curve
    /// </param>
    /// <param name='resolution'>
    ///   - The number of segments along the curve to draw
    /// </param>
    public static void DrawCurve(BezierPoint p1, BezierPoint p2, int resolution) {
      var limit = resolution + 1;
      float res = resolution;
      var last_point = p1.Position;

      for (var i = 1; i < limit; i++) {
        var current_point = GetPoint(p1, p2, i / res);
        Gizmos.DrawLine(last_point, current_point);
        last_point = current_point;
      }
    }

    /// <summary>
    ///   - Gets the point 't' percent along a curve
    ///   - Automatically calculates for the number of relevant points
    /// </summary>
    /// <returns>
    ///   - The point 't' percent along the curve
    /// </returns>
    /// <param name='p1'>
    ///   - The bezier point at the beginning of the curve
    /// </param>
    /// <param name='p2'>
    ///   - The bezier point at the end of the curve
    /// </param>
    /// <param name='t'>
    ///   - Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
    /// </param>
    public static Vector3 GetPoint(BezierPoint p1, BezierPoint p2, float t) {
      if (p1 == null || p2 == null) {
        return Vector3.back;
      }

      if (p1.Handle2 != Vector3.zero) {
        if (p2.Handle1 != Vector3.zero) {
          return GetCubicCurvePoint(p1.Position, p1.GlobalHandle2, p2.GlobalHandle1, p2.Position, t);
        }

        return GetQuadraticCurvePoint(p1.Position, p1.GlobalHandle2, p2.Position, t);
      }

      if (p2.Handle1 != Vector3.zero) {
        return GetQuadraticCurvePoint(p1.Position, p2.GlobalHandle1, p2.Position, t);
      }

      return GetLinearPoint(p1.Position, p2.Position, t);
    }

    /// <summary>
    ///   - Gets the point 't' percent along a third-order curve
    /// </summary>
    /// <returns>
    ///   - The point 't' percent along the curve
    /// </returns>
    /// <param name='p1'>
    ///   - The point at the beginning of the curve
    /// </param>
    /// <param name='p2'>
    ///   - The second point along the curve
    /// </param>
    /// <param name='p3'>
    ///   - The third point along the curve
    /// </param>
    /// <param name='p4'>
    ///   - The point at the end of the curve
    /// </param>
    /// <param name='t'>
    ///   - Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
    /// </param>
    public static Vector3 GetCubicCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t) {
      t = Mathf.Clamp01(t);

      var part1 = Mathf.Pow(1 - t, 3) * p1;
      var part2 = 3 * Mathf.Pow(1 - t, 2) * t * p2;
      var part3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p3;
      var part4 = Mathf.Pow(t, 3) * p4;

      return part1 + part2 + part3 + part4;
    }

    /// <summary>
    ///   - Gets the point 't' percent along a second-order curve
    /// </summary>
    /// <returns>
    ///   - The point 't' percent along the curve
    /// </returns>
    /// <param name='p1'>
    ///   - The point at the beginning of the curve
    /// </param>
    /// <param name='p2'>
    ///   - The second point along the curve
    /// </param>
    /// <param name='p3'>
    ///   - The point at the end of the curve
    /// </param>
    /// <param name='t'>
    ///   - Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
    /// </param>
    public static Vector3 GetQuadraticCurvePoint(Vector3 p1, Vector3 p2, Vector3 p3, float t) {
      t = Mathf.Clamp01(t);

      var part1 = Mathf.Pow(1 - t, 2) * p1;
      var part2 = 2 * (1 - t) * t * p2;
      var part3 = Mathf.Pow(t, 2) * p3;

      return part1 + part2 + part3;
    }

    /// <summary>
    ///   - Gets point 't' percent along a linear "curve" (line)
    ///   - This is exactly equivalent to Vector3.Lerp
    /// </summary>
    /// <returns>
    ///   - The point 't' percent along the curve
    /// </returns>
    /// <param name='p1'>
    ///   - The point at the beginning of the line
    /// </param>
    /// <param name='p2'>
    ///   - The point at the end of the line
    /// </param>
    /// <param name='t'>
    ///   - Value between 0 and 1 representing the percent along the line (0 = 0%, 1 = 100%)
    /// </param>
    public static Vector3 GetLinearPoint(Vector3 p1, Vector3 p2, float t) { return p1 + (p2 - p1) * t; }

    /// <summary>
    ///   - Gets point 't' percent along n-order curve
    /// </summary>
    /// <returns>
    ///   - The point 't' percent along the curve
    /// </returns>
    /// <param name='t'>
    ///   - Value between 0 and 1 representing the percent along the curve (0 = 0%, 1 = 100%)
    /// </param>
    /// <param name='points'>
    ///   - The points used to define the curve
    /// </param>
    public static Vector3 GetPoint(float t, params Vector3[] points) {
      t = Mathf.Clamp01(t);

      var order = points.Length - 1;
      var point = Vector3.zero;

      for (var i = 0; i < points.Length; i++) {
        var vector_to_add = points[points.Length - i - 1]
                            * (BinomialCoefficient(i, order) * Mathf.Pow(t, order - i) * Mathf.Pow(1 - t, i));
        point += vector_to_add;
      }

      return point;
    }

    /// <summary>
    ///   - Approximates the length
    /// </summary>
    /// <returns>
    ///   - The approximate length
    /// </returns>
    /// <param name='p1'>
    ///   - The bezier point at the start of the curve
    /// </param>
    /// <param name='p2'>
    ///   - The bezier point at the end of the curve
    /// </param>
    /// <param name='resolution'>
    ///   - The number of points along the curve used to create measurable segments
    /// </param>
    public static float ApproximateLength(BezierPoint p1, BezierPoint p2, int resolution = 10) {
      float res = resolution;
      float total = 0;
      var last_position = p1.Position;

      for (var i = 0; i < resolution + 1; i++) {
        var current_position = GetPoint(p1, p2, i / res);
        total += (current_position - last_position).magnitude;
        last_position = current_position;
      }

      return total;
    }

    #endregion

    #region UtilityFunctions

    static int BinomialCoefficient(int i, int n) { return Factoral(n) / (Factoral(i) * Factoral(n - i)); }

    static int Factoral(int i) {
      if (i == 0) {
        return 1;
      }

      var total = 1;

      while (i - 1 >= 0) {
        total *= i;
        i--;
      }

      return total;
    }

    #endregion

    /* needs testing
        public Vector3 GetPointAtDistance(float distance)
        {
            if(close)
            {
                if(distance < 0) while(distance < 0) { distance += length; }
                else if(distance > length) while(distance > length) { distance -= length; }
            }

            else
            {
                if(distance <= 0) return points[0].position;
                else if(distance >= length) return points[points.Length - 1].position;
            }

            float totalLength = 0;
            float curveLength = 0;

            BezierPoint firstPoint = null;
            BezierPoint secondPoint = null;

            for(int i = 0; i < points.Length - 1; i++)
            {
                curveLength = ApproximateLength(points[i], points[i + 1], resolution);
                if(totalLength + curveLength >= distance)
                {
                    firstPoint = points[i];
                    secondPoint = points[i+1];
                    break;
                }
                else totalLength += curveLength;
            }

            if(firstPoint == null)
            {
                firstPoint = points[points.Length - 1];
                secondPoint = points[0];
                curveLength = ApproximateLength(firstPoint, secondPoint, resolution);
            }

            distance -= totalLength;
            return GetPoint(distance / curveLength, firstPoint, secondPoint);
        }
        */
  }
}