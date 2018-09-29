using UnityEngine;

namespace Common.ListView.Scripts {
  public abstract class ListViewInputHandler : MonoBehaviour {
    public ListViewControllerBase _ListView;

    void Update() { this.HandleInput(); }

    protected abstract void HandleInput();
  }

  public abstract class ListViewScroller : ListViewInputHandler {
    protected bool _M_Scrolling;
    protected float _M_StartOffset;
    protected Vector3 _M_StartPosition;

    protected virtual void StartScrolling(Vector3 start) {
      if (this._M_Scrolling) {
        return;
      }

      this._M_Scrolling = true;
      this._M_StartPosition = start;
      this._M_StartOffset = this._ListView._ScrollOffset;
    }

    protected virtual void Scroll(Vector3 position) {
      if (this._M_Scrolling) {
        this._ListView._ScrollOffset = this._M_StartOffset + position.x - this._M_StartPosition.x;
      }
    }

    protected virtual void StopScrolling() { this._M_Scrolling = false; }
  }
}
