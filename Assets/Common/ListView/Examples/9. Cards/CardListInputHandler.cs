using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._9._Cards {
  public class CardListInputHandler : ListViewScroller {
    float _m_list_depth;
    public float _ScrollWheelCoeff = 1;

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
      }

      screen_point.z = this._m_list_depth;
      var scroll_position = Camera.main.ScreenToWorldPoint(screen_point);
      this.Scroll(scroll_position);
      if (!Input.GetMouseButton(0)) {
        this.StopScrolling();
      }

      this._ListView._ScrollOffset += Input.mouseScrollDelta.y * this._ScrollWheelCoeff;
    }

    protected override void Scroll(Vector3 position) {
      if (this._M_Scrolling) {
        this._ListView._ScrollOffset = this._M_StartOffset + position.x - this._M_StartPosition.x;
      }
    }

    protected override void StopScrolling() {
      base.StopScrolling();
      ((CardList)this._ListView).OnStopScrolling();
    }
  }
}
