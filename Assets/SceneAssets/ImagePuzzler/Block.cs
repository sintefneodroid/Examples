using System;
using System.Collections;
using UnityEngine;

namespace SceneAssets.ImagePuzzler {
  public class Block : MonoBehaviour {
    [SerializeField] Vector2Int _coord;
    [SerializeField] Vector2Int _starting_coord;

    public Vector2Int Coord {
      get { return this._coord; }
      set { this._coord = value; }
    }

    public event Action<Block> OnBlockPressed;
    public event Action OnFinishedMoving;

    public void Init(Vector2Int starting_coord, Texture2D image) {
      this._starting_coord = starting_coord;
      this._coord = starting_coord;

      this.GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");
      this.GetComponent<MeshRenderer>().material.mainTexture = image;
    }

    void OnMouseDown() { this.OnBlockPressed?.Invoke(this); }

    public void MoveToPosition(Vector2 target, float duration) {
      this.StartCoroutine(this.AnimateMove(target, duration));
    }

    IEnumerator AnimateMove(Vector2 target, float duration) {
      Vector2 initial_pos = this.transform.position;
      float percent = 0;

      while (percent < 1) {
        percent += Time.deltaTime / duration;
        this.transform.position = Vector2.Lerp(initial_pos, target, percent);
        yield return null;
      }

      this.OnFinishedMoving?.Invoke();
    }

    public bool IsAtStartingCoord() { return this.Coord == this._starting_coord; }
  }
}