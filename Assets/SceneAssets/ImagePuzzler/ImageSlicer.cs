using UnityEngine;

namespace SceneAssets.ImagePuzzler {
  /// <summary>
  ///
  /// </summary>
  public static class ImageSlicer {
    /// <summary>
    ///
    /// </summary>
    /// <param name="image"></param>
    /// <param name="horisontal_divisions"></param>
    /// <param name="vertical_divisions"></param>
    /// <returns></returns>
    public static Texture2D[,] GetSlices(Texture2D image, int horisontal_divisions, int vertical_divisions) {
      var image_size = Mathf.Min(a : image.width, b : image.height);

      var block_size = image_size / Mathf.Max(a : vertical_divisions, b : horisontal_divisions);

      var blocks = new Texture2D[horisontal_divisions, vertical_divisions];

      //var horisontal_center_offset = image_size / horisontal_divisions;
      //var vertical_center_offset = image_size / vertical_divisions;

      for (var y = 0; y < vertical_divisions; y++) {
        for (var x = 0; x < horisontal_divisions; x++) {
          var block = new Texture2D(width : block_size, height : block_size) {wrapMode = TextureWrapMode.Clamp};
          block.SetPixels(colors : image.GetPixels(x : x * block_size,
                                                   y : y * block_size,
                                                   blockWidth : block_size,
                                                   blockHeight : block_size));
          block.Apply();
          blocks[x, y] = block;
        }
      }

      return blocks;
    }
  }
}
