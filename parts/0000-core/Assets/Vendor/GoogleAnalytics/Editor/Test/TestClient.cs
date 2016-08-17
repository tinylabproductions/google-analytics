using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics {
  public class GATestClient : IGAClient {
    public readonly List<GAEvent> events = new List<GAEvent>();
    public readonly List<GAAppView> appViews = new List<GAAppView>();
    public readonly List<GAItem> items = new List<GAItem>();
    public readonly List<GATiming> timings = new List<GATiming>();

    public void clear() {
      events.Clear();
      appViews.Clear();
      items.Clear();
      timings.Clear();
    }

    public void Event(GAEvent data) { events.Add(data); }
    public void AppView(GAAppView data) { appViews.Add(data); }
    public void Item(GAItem data) { items.Add(data); }
    public void Timing(GATiming data) { timings.Add(data); }
  }
}
