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
      return test > _half_arc && !OkayHigh(test : test, high_limit : high_limit);
    }

    static bool BadLow(float test, float low_limit) {
      return test < _half_arc && !OkayLow(test : test, low_limit : low_limit);
    }

    public static UnityEngine.Quaternion
        ClampRotation(UnityEngine.Vector3 temp_eulers, float low, float high) {
      temp_eulers.x = ClampPlane(plane : temp_eulers.x, low : low, high : high);
      temp_eulers.z = ClampPlane(plane : temp_eulers.z, low : low, high : high);
      temp_eulers.y = _null_arc; // ClampPlane(tempEulers.y); // *See GIST note below...
      return UnityEngine.Quaternion.Euler(euler : temp_eulers);
      //Debug.Log(tempEulers);
    }

    static float ClampPlane(float plane, float low, float high) {
      if (OkayLow(test : plane, low_limit : low) || OkayHigh(test : plane, high_limit : high)) {
        DoNothing(); // Plane 'in range'.
      } else if (BadLow(test : plane, low_limit : low)) {
        plane = UnityEngine.Mathf.Clamp(value : plane, min : _null_arc, max : low);
      } else if (BadHigh(test : plane, high_limit : high)) {
        plane = UnityEngine.Mathf.Clamp(value : plane, min : _full_arc - high, max : _full_arc);
      } else {
        UnityEngine.Debug.LogWarning("WARN: invalid plane condition");
      }

      return plane;
    }
  }
}