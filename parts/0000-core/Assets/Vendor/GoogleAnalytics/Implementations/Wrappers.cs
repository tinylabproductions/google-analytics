using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  class AlwaysIncludedDimensionsWrapper : IGAClient {
    public readonly IGAClient underlying;
    public readonly IDictionary<IDimension, string> dimensions;

    public AlwaysIncludedDimensionsWrapper(IGAClient underlying, IDictionary<IDimension, string> dimensions) {
      this.underlying = underlying;
      this.dimensions = dimensions;
    }

    public void Event(
      string category = null, string action = null, string label = null,
      int? value = null, IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      underlying.Event(
        category: category, action: action, label: label,
        value: value, metricValues: metricValues,
        dimensionValues: add(dimensionValues)
      );
    }

    public void AppView(
      string screenName,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      underlying.AppView(screenName, metricValues, add(dimensionValues));
    }

    public void Item(
      string name, float? price = null, int? quantity = null,
      string code = null, string category = null, string currencyCode = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      underlying.Item(
        name, price, quantity, code, category, currencyCode, metricValues,
        add(dimensionValues)
      );
    }

    public IGAClient withBaseDimensions(IDictionary<IDimension, string> dimensions) {
      return new AlwaysIncludedDimensionsWrapper(this, dimensions);
    }

    IDictionary<IDimension, string> add(IDictionary<IDimension, string> dims) {
      if (dims == null) return dimensions;
      var d = new Dictionary<IDimension, string>(dimensions);
      foreach (var kv in dims) d[kv.Key] = kv.Value;
      return d;
    }
  }
}
