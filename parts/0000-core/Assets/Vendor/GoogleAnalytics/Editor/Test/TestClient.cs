using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics {
  public class TestClient : Client {
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

    public struct ItemEntry {
      public readonly string name, code, category, currencyCode;
      public readonly float? price;
      public readonly int? quantity;

      public ItemEntry(
        string name, float? price = null, int? quantity = null,
        string code = null, string category = null,
        string currencyCode = null
      ) {
        this.name = name;
        this.code = code;
        this.category = category;
        this.currencyCode = currencyCode;
        this.price = price;
        this.quantity = quantity;
      }

      public override string ToString() { return $"{nameof(ItemEntry)}[name: {name}, code: {code}, category: {category}, currencyCode: {currencyCode}, price: {price}, quantity: {quantity}]"; }
    }

    public List<EventEntry> events = new List<EventEntry>();
    public List<string> appViews = new List<string>();
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

    public void AppView(string screenName) { appViews.Add(screenName); }

    public void Item(
      string name, float? price = null, int? quantity = null,
      string code = null, string category = null,
      string currencyCode = null
    ) {
      items.Add(new ItemEntry(
        name:name, price:price, quantity:quantity, code:code, 
        category:category, currencyCode:currencyCode
      ));
    }
  }
}
