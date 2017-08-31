namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  public class MultiGAClient : IGAClient {
    readonly IGAClient[] clients;

    public MultiGAClient(IGAClient[] clients) { this.clients = clients; }

    public void Event(GAEvent data) {
      foreach (var c in clients) c.Event(data);
    }

    public void AppView(GAAppView data) {
      foreach (var c in clients) c.AppView(data);
    }

    public void Item(GAItem data) {
      foreach (var c in clients) c.Item(data);
    }

    public void Timing(GATiming data) {
      foreach (var c in clients) c.Timing(data);
    }
  }
}