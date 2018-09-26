using System.Text;

namespace Common.reference_checker.Editor {
  /// <summary>
  /// 
  /// </summary>
  public class ColorfulLogBuilder {
    StringBuilder _log = new StringBuilder();
    bool _colorful = true;

    public void SetColorful(bool use) { this._colorful = use; }

    public void StartColor() {
      if (this._colorful) {
        this._log.Append("<color=red>");
      }
    }

    public void EndColor() {
      if (this._colorful) {
        this._log.Append("</color>");
      }
    }

    public void Append(string text) { this._log.Append(text); }

    public override string ToString() { return this._log.ToString(); }
  }
}
