namespace Core.Utils
{
  public class Helpers
  {
    private static int _uniqueID = 0;

    /// <summary>
    /// A unique numerical ID for this session. You may also want to consider using a GUID
    /// </summary>
    public static int UniqueID
    {
      get
      {
        return _uniqueID++;
      }
    }
  }
}
