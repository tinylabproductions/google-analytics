using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics {
  public class GATestClient : IGAClient {
    public List<GAEvent> events = new List<GAEvent>();
    public List<GAAppView> appViews = new List<GAAppView>();
    public List<GAItem> items = new List<GAItem>();

    public void clear() {
      events.Clear();
      appViews.Clear();
      items.Clear();
    }

    public void Event(GAEvent data) { events.Add(data); }
    public void AppView(GAAppView data) { appViews.Add(data); }
    public void Item(GAItem data) { items.Add(data); }
  }
}
