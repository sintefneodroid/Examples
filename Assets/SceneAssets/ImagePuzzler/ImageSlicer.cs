using UnityEngine;

namespace SceneAssets.ImagePuzzler {
  public static class ImageSlicer {
    public static Texture2D[,] GetSlices(Texture2D image, int horisontal_divisions, int vertical_divisions) {
      var image_size = Mathf.Min(image.width, image.height);

      var block_size = image_size / Mathf.Max(vertical_divisions, horisontal_divisions);

      var blocks = new Texture2D[horisontal_divisions, vertical_divisions];

      //var horisontal_center_offset = image_size / horisontal_divisions;
      //var vertical_center_offset = image_size / vertical_divisions;

      for (var y = 0; y < vertical_divisions; y++) {
        for (var x = 0; x < horisontal_divisions; x++) {
          var block = new Texture2D(block_size, block_size) {wrapMode = TextureWrapMode.Clamp};
          block.SetPixels(image.GetPixels(x * block_size, y * block_size, block_size, block_size));
          block.Apply();
          blocks[x, y] = block;
        }
      }

      return blocks;
    }
  }
}
