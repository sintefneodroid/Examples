using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.ListView.Scripts;
using UnityEngine;
using Random = System.Random;

//Images sourced from http://web.stanford.edu/~jlewis8/cs148/pokerscene/

namespace Common.ListView.Examples._9._Cards {
  public class CardList : ListViewController<CardData, Card> {
    public string _DefaultTemplate = "Card";
    public float _Interpolate = 15f;
    public float _RecycleDuration = 0.3f;
    public bool _AutoScroll;
    public float _ScrollSpeed = 1;
    public Transform _LeftDeck, _RightDeck;

    float _m_scroll_return = float.MaxValue;
    float _m_last_scroll_offset;

    protected override void Setup() {
      base.Setup();

      var data_list = new List<CardData>(52);
      for (var i = 0; i < 4; i++) {
        for (var j = 1; j < 14; j++) {
          var card = new CardData();
          switch (j) {
            case 1:
              card._Value = "A";
              break;
            case 11:
              card._Value = "J";
              break;
            case 12:
              card._Value = "Q";
              break;
            case 13:
              card._Value = "K";
              break;
            default:
              card._Value = j + "";
              break;
          }

          card._Suit = (Card.Suit)i;
          card._Template = this._DefaultTemplate;
          data_list.Add(card);
        }
      }

      var rnd = new Random();
      this._Data = data_list.OrderBy(x => rnd.Next()).ToArray();
    }

    void OnDrawGizmos() {
      if (this.enabled) {
        Gizmos.DrawWireCube(
            this.transform.position,
            new Vector3(this._Range, this.ItemSize.y, this.ItemSize.z));
      }
    }

    public void OnStopScrolling() {
      if (this._ScrollOffset > this.ItemSize.x) { //Let us over-scroll one whole card
        this._ScrollOffset = this.ItemSize.x * 0.5f;
      }

      if (this._m_scroll_return < float.MaxValue) {
        this._ScrollOffset = this._m_scroll_return;
        this._m_scroll_return = float.MaxValue;
      }
    }

    protected override void UpdateItems() {
      if (this._AutoScroll) {
        this._ScrollOffset -= this._ScrollSpeed * Time.deltaTime;
        if (-this._ScrollOffset > (this._Data.Length - this._M_NumItems) * this.ItemSize.x
            || this._ScrollOffset >= 0) {
          this._ScrollSpeed *= -1;
        }
      }

      for (var i = 0; i < this._Data.Length; i++) {
        if (i + this._M_DataOffset < 0) {
          this.ExtremeLeft(this._Data[i]);
        } else if (i + this._M_DataOffset > this._M_NumItems - 1) { //End the m_List one item early
          this.ExtremeRight(this._Data[i]);
        } else {
          this.ListMiddle(this._Data[i], i);
        }
      }

      this._m_last_scroll_offset = this._ScrollOffset;
    }

    protected override void ExtremeLeft(CardData data) { this.RecycleItemAnimated(data, this._LeftDeck); }

    protected override void ExtremeRight(CardData data) { this.RecycleItemAnimated(data, this._RightDeck); }

    protected override void ListMiddle(CardData data, int offset) {
      if (data._Item == null) {
        data._Item = this.GetItem(data);
        if (this._ScrollOffset - this._m_last_scroll_offset < 0) {
          data._Item.transform.position = this._RightDeck.transform.position;
          data._Item.transform.rotation = this._RightDeck.transform.rotation;
        } else {
          data._Item.transform.position = this._LeftDeck.transform.position;
          data._Item.transform.rotation = this._LeftDeck.transform.rotation;
        }
      }

      this.Positioning(data._Item.transform, offset);
    }

    void RecycleItemAnimated(CardData data, Transform destination) {
      if (data._Item == null) {
        return;
      }

      var item = data._Item;
      data._Item = null;
      this.StartCoroutine(this.RecycleAnimation(item, data._Template, destination, this._RecycleDuration));
    }

    IEnumerator RecycleAnimation(MonoBehaviour card, string template, Transform destination, float speed) {
      var start = Time.time;
      var start_rot = card.transform.rotation;
      var start_pos = card.transform.position;
      while (Time.time - start < speed) {
        card.transform.rotation = Quaternion.Lerp(
            start_rot,
            destination.rotation,
            (Time.time - start) / speed);
        card.transform.position = Vector3.Lerp(start_pos, destination.position, (Time.time - start) / speed);
        yield return null;
      }

      card.transform.rotation = destination.rotation;
      card.transform.position = destination.position;
      this.RecycleItem(template, card);
    }

    protected override void Positioning(Transform t, int offset) {
      t.position = Vector3.Lerp(
          t.position,
          this._M_LeftSide + (offset * this._M_ItemSize.x + this._ScrollOffset) * Vector3.right,
          this._Interpolate * Time.deltaTime);
      t.rotation = Quaternion.Lerp(t.rotation, Quaternion.identity, this._Interpolate * Time.deltaTime);
    }
  }
}
