using System.Collections.Generic;
using com.tinylabproductions.GoogleAnalytics.Implementations;

namespace com.tinylabproductions.GoogleAnalytics {
  public class GATestClient : IGAClient {
    public struct EventEntry {
      public readonly string category, action, label;
      public readonly int? value;
      public readonly IDictionary<IMetric, uint> metricValues;
      public readonly IDictionary<IDimension, string> dimensionValues;

      public EventEntry(
        string category = null, string action = null,
        string label = null, int? value = null,
        IDictionary<IMetric, uint> metricValues = null,
        IDictionary<IDimension, string> dimensionValues = null
      ) {
        this.category = category;
        this.action = action;
        this.label = label;
        this.value = value;
        this.metricValues = metricValues;
        this.dimensionValues = dimensionValues;
      }

      public override string ToString() { return $"{nameof(EventEntry)}[category: {category}, action: {action}, label: {label}, value: {value}, metricValues: {metricValues}, dimensionValues: {dimensionValues}]"; }
    }

    public struct AppViewEntry {
      public readonly string screenName;
      public readonly IDictionary<IMetric, uint> metricValues;
      public readonly IDictionary<IDimension, string> dimensionValues;

      public AppViewEntry(string screenName, IDictionary<IMetric, uint> metricValues, IDictionary<IDimension, string> dimensionValues) {
        this.screenName = screenName;
        this.metricValues = metricValues;
        this.dimensionValues = dimensionValues;
      }

      public override string ToString() { return
          $"{nameof(AppViewEntry)}[" +
          $"screenName: {screenName}, " +
          $"metricValues: {metricValues}, " +
          $"dimensionValues: {dimensionValues}" +
          $"]"; }
    }

    public struct ItemEntry {
      public readonly string name, code, category, currencyCode;
      public readonly float? price;
      public readonly int? quantity;
      public readonly IDictionary<IMetric, uint> metricValues;
      public readonly IDictionary<IDimension, string> dimensionValues;

      public ItemEntry(
        string name, float? price = null, int? quantity = null,
        string code = null, string category = null,
        string currencyCode = null,
        IDictionary<IMetric, uint> metricValues = null,
        IDictionary<IDimension, string> dimensionValues = null
      ) {
        this.name = name;
        this.code = code;
        this.category = category;
        this.currencyCode = currencyCode;
        this.price = price;
        this.quantity = quantity;
        this.metricValues = metricValues;
        this.dimensionValues = dimensionValues;
      }

      public override string ToString() {
        return $"{nameof(ItemEntry)}[" +
               $"name: {name}, code: {code}, category: {category}, currencyCode: {currencyCode}, " +
               $"price: {price}, " +
               $"quantity: {quantity}, " +
               $"{nameof(metricValues)}: {metricValues}, " +
               $"{nameof(dimensionValues)}: {dimensionValues}, " +
               $"]";
      }
    }

    public List<EventEntry> events = new List<EventEntry>();
    public List<AppViewEntry> appViews = new List<AppViewEntry>();
    public List<ItemEntry> items = new List<ItemEntry>();

    public void clear() {
      events.Clear();
      appViews.Clear();
      items.Clear();
    }

    public void Event(
      string category = null, string action = null,
      string label = null, int? value = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      events.Add(new EventEntry(
        category:category, action:action, label:label, value:value,
        metricValues:metricValues, dimensionValues:dimensionValues
      ));
    }

    public void AppView(
      string screenName,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      appViews.Add(new AppViewEntry(
        screenName, metricValues, dimensionValues
      ));
    }

    public void Item(
      string name, float? price = null, int? quantity = null,
      string code = null, string category = null,
      string currencyCode = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      items.Add(new ItemEntry(
        name:name, price:price, quantity:quantity, code:code,
        category:category, currencyCode:currencyCode,
        metricValues:metricValues, dimensionValues:dimensionValues
      ));
    }
  }
}
