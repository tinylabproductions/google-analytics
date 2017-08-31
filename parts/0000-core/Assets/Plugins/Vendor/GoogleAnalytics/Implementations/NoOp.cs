namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  public class GAClientNoOpImpl : IGAClient {
    public static readonly GAClientNoOpImpl instance = new GAClientNoOpImpl();
    GAClientNoOpImpl() { }

    public void Event(GAEvent _) {}
    public void AppView(GAAppView _) {}
    public void Item(GAItem _) {}
    public void Timing(GATiming _) {}
  }
}
