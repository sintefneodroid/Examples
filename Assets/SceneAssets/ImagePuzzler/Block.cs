namespace SceneAssets.ImagePuzzler {
  /// <summary>
  /// </summary>
  public class Block : UnityEngine.MonoBehaviour {
    [UnityEngine.SerializeField] UnityEngine.Vector2Int _coord;
    [UnityEngine.SerializeField] UnityEngine.Vector2Int _starting_coord;

    /// <summary>
    /// </summary>
    public UnityEngine.Vector2Int Coord { get { return this._coord; } set { this._coord = value; } }

    void OnMouseDown() { this.OnBlockPressed?.Invoke(obj : this); }

    /// <summary>
    /// </summary>
    public event System.Action<Block> OnBlockPressed;

    /// <summary>
    /// </summary>
    public event System.Action OnFinishedMoving;

    /// <summary>
    /// </summary>
    /// <param name="starting_coord"></param>
    /// <param name="image"></param>
    public void Init(UnityEngine.Vector2Int starting_coord, UnityEngine.Texture2D image) {
      this._starting_coord = starting_coord;
      this._coord = starting_coord;

      this.GetComponent<UnityEngine.MeshRenderer>().material.shader =
          UnityEngine.Shader.Find("Unlit/Texture");
      this.GetComponent<UnityEngine.MeshRenderer>().material.mainTexture = image;
    }

    /// <summary>
    /// </summary>
    /// <param name="target"></param>
    /// <param name="duration"></param>
    public void MoveToPosition(UnityEngine.Vector2 target, float duration) {
      this.StartCoroutine(routine : this.AnimateMove(target : target, duration : duration));
    }

    System.Collections.IEnumerator AnimateMove(UnityEngine.Vector2 target, float duration) {
      UnityEngine.Vector2 initial_pos = this.transform.position;
      float percent = 0;

      while (percent < 1) {
        percent += UnityEngine.Time.deltaTime / duration;
        this.transform.position = UnityEngine.Vector2.Lerp(a : initial_pos, b : target, t : percent);
        yield return null;
      }

      this.OnFinishedMoving?.Invoke();
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public bool IsAtStartingCoord() { return this.Coord == this._starting_coord; }
  }
}