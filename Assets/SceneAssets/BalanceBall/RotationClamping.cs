using UnityEngine;

namespace SceneAssets.BalanceBall {
  public static class RotationClamping {
    const float _full_arc = 360.0f;
    const float _half_arc = 180.0f;
    const float _null_arc = 0.0f;

    static void DoNothing() { }

    static bool OkayHigh(float test, float high_limit) {
      return test >= _full_arc - high_limit && test <= _full_arc;
    }

    static bool OkayLow(float test, float low_limit) { return test >= _null_arc && test <= low_limit; }

    static bool BadHigh(float test, float high_limit) {
      return test > _half_arc && !OkayHigh(test, high_limit);
    }

    static bool BadLow(float test, float low_limit) {
      return test < _half_arc && !OkayLow(test, low_limit);
    }

    public static Quaternion ClampRotation(Vector3 temp_eulers, float low, float high) {
      temp_eulers.x = ClampPlane(temp_eulers.x, low, high);
      temp_eulers.z = ClampPlane(temp_eulers.z, low, high);
      temp_eulers.y = _null_arc; // ClampPlane(tempEulers.y); // *See GIST note below...
      return Quaternion.Euler(temp_eulers);
      ///Debug.Log(tempEulers);
    }

    static float ClampPlane(float plane, float low, float high) {
      if (OkayLow(plane, low) || OkayHigh(plane, high)) {
        DoNothing(); // Plane 'in range'.
      } else if (BadLow(plane, low)) {
        plane = Mathf.Clamp(plane, _null_arc, low);
      } else if (BadHigh(plane, high)) {
        plane = Mathf.Clamp(plane, _full_arc - high, _full_arc);
      } else {
        Debug.LogWarning("WARN: invalid plane condition");
      }

      return plane;
    }
  }
}
