
namespace TBD.Views
{
  /// <summary>
  /// A samll data object for use by UIView's update function
  /// </summary>
  public struct UIData
  {
    public int time;
    public int attempt;

    public UIData(int time, int attempt)
    {
      this.time = time;
      this.attempt = attempt;
    }
  }
}
