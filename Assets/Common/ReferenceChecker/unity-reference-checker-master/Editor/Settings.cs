using System;
using UnityEngine;

namespace Common.reference_checker.Editor {
  /// <summary>
  /// 
  /// </summary>
  public static class Settings {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool GetCheckOnCompilation() { return GetPlayerPrefsBool(Keys._CheckOnCompilation, false); }

    public static void SetCheckOnCompilation(bool check) {
      SetPlayerPrefsBool(Keys._CheckOnCompilation, check);
    }

    public static LogType GetLogSeverity() {
      var saved_str = PlayerPrefs.GetString(Keys._LogSeverity, "Error");
      switch (saved_str) {
        case "Error":
          return LogType.Error;
        case "Assert":
          return LogType.Assert;
        case "Warning":
          return LogType.Warning;
        case "Log":
          return LogType.Log;
        case "Exception":
          return LogType.Exception;
        default:
          ClearSettings();
          throw new IndexOutOfRangeException();
      }
    }

    public static void SetLogSeverity(LogType type) {
      var type_str = type.ToString();
      PlayerPrefs.SetString(Keys._LogSeverity, type_str);
    }

    public static bool GetColorfulLogs() { return GetPlayerPrefsBool(Keys._ColorfulLogs, true); }

    public static void SetColorfulLogs(bool colorful) { SetPlayerPrefsBool(Keys._ColorfulLogs, colorful); }

    public static void ClearSettings() {
      PlayerPrefs.DeleteKey(Keys._CheckOnCompilation);
      PlayerPrefs.DeleteKey(Keys._LogSeverity);
      PlayerPrefs.DeleteKey(Keys._ColorfulLogs);
    }

    static bool GetPlayerPrefsBool(string key, bool default_value) {
      var default_value_int = default_value ? 1 : 0;
      return PlayerPrefs.GetInt(key, default_value_int) == 1;
    }

    static void SetPlayerPrefsBool(string key, bool value) {
      var value_int = value ? 1 : 0;
      PlayerPrefs.SetInt(key, value_int);
    }

    /// <summary>
    /// 
    /// </summary>
    static class Keys {
      static string _refchecker_pretext = "RefChecker:";
      public static string _CheckOnCompilation = _refchecker_pretext + "checkOnCompilation";
      public static string _LogSeverity = _refchecker_pretext + "logSeverity";
      public static string _ColorfulLogs = _refchecker_pretext + "colorfulLogs";
    }
  }
}
