using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._2._Custom_Scrolling {
  public class ListViewMouseScroller : ListViewScroller {
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
  }
}
