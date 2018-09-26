using Common.ListView.Examples._9._Cards;
using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._10._Card_Game {
  public class CardGameInputHandler : ListViewInputHandler {
    public CardGameHand _Hand;

    protected override void HandleInput() {
      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Debug.DrawRay(ray.origin, ray.direction);
      if (Input.GetMouseButtonUp(0)) {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
          if (hit.collider.name.Equals("Deck")) {
            ((CardGameList)this._ListView).Deal();
          } else {
            var item = hit.collider.GetComponent<Card>();
            if (item) {
              if (item.transform.parent == this._ListView.transform) {
                this._Hand.DrawCard(item);
              } else if (item.transform.parent == this._Hand.transform) {
                this._Hand.ReturnCard(item);
              }
            }
          }
        }
      }
    }
  }
}
