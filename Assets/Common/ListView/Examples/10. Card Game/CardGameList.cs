using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.ListView.Examples._9._Cards;
using Common.ListView.Scripts;
using UnityEngine;
using Random = System.Random;

namespace Common.ListView.Examples._10._Card_Game {
  public class CardGameList : ListViewController<CardData, Card> {
    public string _DefaultTemplate = "Card";
    public float _Interpolate = 15f;
    public float _RecycleDuration = 0.3f;
    public int _DealMax = 5;
    public Transform _Deck;

    Vector3 _m_start_pos;

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

      this.Shuffle(data_list);

      this._Range = 0;
    }

    void Shuffle(List<CardData> data_list) {
      var rnd = new Random();
      this._Data = data_list.OrderBy(x => rnd.Next()).ToArray();
    }

    void OnDrawGizmos() {
      if (this.enabled) {
        Gizmos.DrawWireCube(
            this.transform.position + Vector3.left * (this.ItemSize.x * this._DealMax - this._Range) * 0.5f,
            new Vector3(this._Range, this.ItemSize.y, this.ItemSize.z));
      }
    }

    protected override void UpdateItems() {
      this._m_start_pos = this.transform.position + Vector3.left * this.ItemSize.x * this._DealMax * 0.5f;
      for (var i = 0; i < this._Data.Length; i++) {
        if (i + this._M_DataOffset < 0) {
          this.ExtremeLeft(this._Data[i]);
        } else if (i + this._M_DataOffset > this._M_NumItems - 1) {
          this.ExtremeRight(this._Data[i]);
        } else {
          this.ListMiddle(this._Data[i], i);
        }
      }
    }

    protected override void ExtremeLeft(CardData data) { this.RecycleItemAnimated(data, this._Deck); }

    protected override void ExtremeRight(CardData data) { this.RecycleItemAnimated(data, this._Deck); }

    protected override void ListMiddle(CardData data, int offset) {
      if (data._Item == null) {
        data._Item = this.GetItem(data);
        data._Item.transform.position = this._Deck.transform.position;
        data._Item.transform.rotation = this._Deck.transform.rotation;
      }

      this.Positioning(data._Item.transform, offset);
    }

    protected override Card GetItem(CardData data) {
      if (data == null) {
        Debug.LogWarning("Tried to get item with null data");
        return null;
      }

      if (!this._M_Templates.ContainsKey(data._Template)) {
        Debug.LogWarning("Cannot get item, template " + data._Template + " doesn't exist");
        return null;
      }

      Card item = null;
      if (this._M_Templates[data._Template]._Pool.Count > 0) {
        item = (Card)this._M_Templates[data._Template]._Pool[0];
        this._M_Templates[data._Template]._Pool.RemoveAt(0);

        item.gameObject.SetActive(true);
        item.GetComponent<BoxCollider>().enabled = true;
        item.Setup(data);
      } else {
        item = Instantiate(this._M_Templates[data._Template]._Prefab).GetComponent<Card>();
        item.transform.parent = this.transform;
        item.Setup(data);
      }

      return item;
    }

    void RecycleItemAnimated(CardData data, Transform destination) {
      if (data._Item == null) {
        return;
      }

      var item = data._Item;
      data._Item = null;
      item.GetComponent<BoxCollider>().enabled =
          false; //Disable collider so we can't click the card during the animation
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
          this._m_start_pos + (offset * this._M_ItemSize.x + this._ScrollOffset) * Vector3.right,
          this._Interpolate * Time.deltaTime);
      t.rotation = Quaternion.Lerp(t.rotation, Quaternion.identity, this._Interpolate * Time.deltaTime);
    }

    void RecycleCard(CardData data) { this.RecycleItemAnimated(data, this._Deck); }

    public CardData DrawCard() {
      if (this._Data.Length == 0) {
        Debug.Log("Out of Cards");
        return null;
      }

      var new_data = new List<CardData>(this._Data);
      var result = new_data[new_data.Count - 1];
      new_data.RemoveAt(new_data.Count - 1);
      if (result._Item == null) {
        this.GetItem(result);
      }

      this._Data = new_data.ToArray();
      return result;
    }

    public void RemoveCardFromDeck(CardData card_data) {
      var new_data = new List<CardData>(this._Data);
      new_data.Remove(card_data);
      this._Data = new_data.ToArray();
      if (this._Range > 0) {
        this._Range -= this.ItemSize.x;
      }
    }

    public void AddCardToDeck(CardData card_data) {
      this._Data = new List<CardData>(this._Data) {card_data}.ToArray();
      card_data._Item.transform.parent = this.transform;
      this.RecycleCard(card_data);
    }

    public void Deal() {
      this._Range += this.ItemSize.x;
      if (this._Range >= this.ItemSize.x * (this._DealMax + 1)) {
        this._ScrollOffset -= this.ItemSize.x * this._DealMax;
        this._Range = 0;
      }

      if (-this._ScrollOffset >= (this._Data.Length - this._DealMax) * this.ItemSize.x) { //reshuffle
        this.Shuffle(new List<CardData>(this._Data));
        this._ScrollOffset = this.ItemSize.x * 0.5f;
      }
    }
  }
}
