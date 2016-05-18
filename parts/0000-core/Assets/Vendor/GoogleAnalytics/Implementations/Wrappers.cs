using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  class AlwaysIncludedDimensionsWrapper : IGAClient {
    public readonly IGAClient underlying;
    public readonly IDictionary<IDimension, string> dimensions;

    public AlwaysIncludedDimensionsWrapper(IGAClient underlying, IDictionary<IDimension, string> dimensions) {
      this.underlying = underlying;
      this.dimensions = dimensions;
    }

    public void Event(GAEvent data) {
      underlying.Event(new GAEvent(
        category: data.category, action: data.action, label: data.label,
        value: data.value, metricValues: data.metricValues,
        dimensionValues: add(data.dimensionValues)
      ));
    }

    public void AppView(GAAppView data) {
      underlying.AppView(new GAAppView(
        data.screenName, data.metricValues, add(data.dimensionValues)
      ));
    }

    public void Item(GAItem data) {
      underlying.Item(new GAItem(
        name: data.name, price: data.price, quantity: data.quantity,
        code: data.code, category: data.category, currencyCode: data.currencyCode,
        metricValues: data.metricValues, dimensionValues: add(data.dimensionValues)
      ));
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
