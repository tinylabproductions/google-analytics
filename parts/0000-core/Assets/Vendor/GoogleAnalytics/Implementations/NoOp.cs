using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  public class GAClientNoOpImpl : IGAClient {
    public static readonly GAClientNoOpImpl instance = new GAClientNoOpImpl();
    GAClientNoOpImpl() { }

    public void Event(
      string category = null, string action = null, string label = null,
      int? value = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {}

    public void AppView(
      string screenName,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {}

    public void Item(
      string name, float? price = null, int? quantity = null, string code = null,
      string category = null, string currencyCode = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {}
  }
}
