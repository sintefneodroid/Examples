using UnityEngine;

namespace Common.AsmDefTools {
  public static class VisibilityHelper {
    public static bool IsHidden(this string file_name_with_extension) {
      if (string.IsNullOrEmpty(file_name_with_extension)) {
        Debug.LogError("The file name is null or empty, therefore the path is considered hidden");
        return true;
      }

      return file_name_with_extension.EndsWith("~");
    }

    public static string Hide(this string file_name_with_extension) {
      if (!file_name_with_extension.IsHidden())
        file_name_with_extension += '~';
      return file_name_with_extension;
    }

    public static string Show(this string file_name_with_extension) {
      if (file_name_with_extension.IsHidden())
        file_name_with_extension = file_name_with_extension.Remove(file_name_with_extension.Length - 1, 1);
      return file_name_with_extension;
    }

    public static string Toggle(this string file_name_with_extension, bool enable) {
      if (enable)
        return file_name_with_extension.Show();
      return file_name_with_extension.Hide();
    }
  }
}