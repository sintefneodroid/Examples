using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.ListView.Examples._9._Cards;
using Common.ListView.Scripts;
using UnityEngine;

namespace Common.ListView.Examples._10._Card_Game {
  public class CardGameHand : ListViewController<CardData, Card> {
    public float _Radius = 0.25f;
    public float _Interpolate = 15f;
    public float _StackOffset = 0.01f;
    public int _HandSize = 5;
    public float _IndicatorTime = 0.5f;
    public CardGameList _Controller;
    public Renderer _Indicator;

    float _m_card_degrees, _m_cards_offset;

    new Vector3 ItemSize { get { return this._Controller.ItemSize; } }

    protected override void Setup() {
      this._Data = new CardData[this._HandSize];
      for (var i = 0; i < this._HandSize; i++) {
        this._Data[i] = this._Controller.DrawCard();
        this._Data[i]._Item.transform.parent = this.transform;
      }
    }

    protected override void ComputeConditions() {
      this._m_card_degrees = Mathf.Atan((this.ItemSize.x + this._Padding) / this._Radius) * Mathf.Rad2Deg;
      this._m_cards_offset = this._m_card_degrees * (this._Data.Length - 1) * 0.5f;
    }

    protected override void UpdateItems() {
      DebugDrawCircle(
          this._Radius - this.ItemSize.z * 0.5f,
          24,
          this.transform.position,
          this.transform.rotation);
      DebugDrawCircle(
          this._Radius + this.ItemSize.z * 0.5f,
          24,
          this.transform.position,
          this.transform.rotation);
      for (var i = 0; i < this._Data.Length; i++) {
        this.Positioning(this._Data[i]._Item.transform, i);
      }
    }

    protected override void Positioning(Transform t, int offset) {
      var slice_rotation = Quaternion.AngleAxis(
          90 - this._m_cards_offset + this._m_card_degrees * offset,
          Vector3.up);
      t.localPosition = Vector3.Lerp(
          t.localPosition,
          slice_rotation * (Vector3.left * this._Radius) + Vector3.up * this._StackOffset * offset,
          this._Interpolate * Time.deltaTime);
      t.localRotation = Quaternion.Lerp(
          t.localRotation,
          slice_rotation * Quaternion.AngleAxis(90, Vector3.down),
          this._Interpolate * Time.deltaTime);
    }

    public static void DebugDrawCircle(float radius, int slices, Vector3 center) {
      DebugDrawCircle(radius, slices, center, Quaternion.identity);
    }

    public static void DebugDrawCircle(float radius, int slices, Vector3 center, Quaternion orientation) {
      for (var i = 0; i < slices; i++) {
        Debug.DrawLine(
            center
            + orientation
            * Quaternion.AngleAxis((float)360 * i / slices, Vector3.up)
            * Vector3.forward
            * radius,
            center
            + orientation
            * Quaternion.AngleAxis((float)360 * (i + 1) / slices, Vector3.up)
            * Vector3.forward
            * radius);
      }
    }

    public void DrawCard(Card item) {
      if (this._Data.Length < this._HandSize) {
        if (item._Data == null) {
          Debug.Log("aaah!");
        }

        var new_data = new List<CardData>(this._Data) {item._Data};
        this._Data = new_data.ToArray();
        this._Controller.RemoveCardFromDeck(item._Data);
        item.transform.parent = this.transform;
      } else {
        this.Indicate();
        Debug.Log("Can't draw card, hand is full!");
      }
    }

    public void ReturnCard(Card item) {
      if (this._Data.Contains(item._Data)) {
        var new_data = new List<CardData>(this._Data);
        new_data.Remove(item._Data);
        this._Data = new_data.ToArray();
        this._Controller.AddCardToDeck(item._Data);
      } else {
        this.Indicate();
        Debug.Log("Something went wrong... This card is not in your hand");
      }
    }

    void Indicate() { this.StartCoroutine(this.DoIndicate()); }

    IEnumerator DoIndicate() {
      this._Indicator.enabled = true;
      yield return new WaitForSeconds(this._IndicatorTime);
      this._Indicator.enabled = false;
    }
  }
}
