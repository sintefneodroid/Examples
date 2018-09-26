#if UNITY_EDITOR
/*using Excluded.ListView.Scripts;
using UnityEngine;

namespace Excluded.ListView.Examples._8._Dictionary {
  public class DictionaryInputHandler : ListViewScroller {
    float _m_list_depth;

    protected override void HandleInput() {
      var screen_point = Input.mousePosition;
      if (Input.GetMouseButton(0)) {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
          var item = hit.collider.GetComponent<ListViewItemBase>();
          if (item) {
            this._m_list_depth = (hit.point - Camera.main.transform.position).magnitude;
            screen_point.z = this._m_list_depth;
            this.StartScrolling(Camera.main.ScreenToWorldPoint(screen_point));
          }
        }
      } else {
        this.StopScrolling();
      }

      screen_point.z = this._m_list_depth;
      this.Scroll(Camera.main.ScreenToWorldPoint(screen_point));
    }

    protected override void StartScrolling(Vector3 position) {
      base.StartScrolling(position);
      ((DictionaryList)this._ListView).OnStartScrolling();
    }

    protected override void Scroll(Vector3 position) {
      if (this._M_Scrolling) {
        this._ListView._ScrollOffset = this._M_StartOffset + this._M_StartPosition.y - position.y;
      }
    }

    protected override void StopScrolling() {
      base.StopScrolling();
      ((DictionaryList)this._ListView).OnStopScrolling();
    }
  }
}*/
#endif
