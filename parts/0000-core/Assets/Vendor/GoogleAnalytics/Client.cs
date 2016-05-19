using System;
using System.Collections.Generic;
using com.tinylabproductions.GoogleAnalytics.Implementations;

namespace com.tinylabproductions.GoogleAnalytics {
  public struct GAEvent {
    public readonly string category, action, label;
    public readonly int? value;
    public readonly IDictionary<IMetric, uint> metricValues;
    public readonly IDictionary<IDimension, string> dimensionValues;

    public GAEvent(
      string category=null, string action=null, string label=null,
      int? value=null,
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null
    ) {
      if (category == null && action == null && label == null && value == null)
        throw new ArgumentException("All parameters cannot be null!");
      this.category = category;
      this.action = action;
      this.label = label;
      this.value = value;
      this.metricValues = metricValues;
      this.dimensionValues = dimensionValues;
    }

    public override string ToString() { return $"{nameof(GAEvent)}[category: {category}, action: {action}, label: {label}, value: {value}, metricValues: {metricValues}, dimensionValues: {dimensionValues}]"; }
  }

  public struct GAAppView {
    public readonly string screenName;
    public readonly IDictionary<IMetric, uint> metricValues;
    public readonly IDictionary<IDimension, string> dimensionValues;

    public GAAppView(string screenName, IDictionary<IMetric, uint> metricValues, IDictionary<IDimension, string> dimensionValues) {
      this.screenName = screenName;
      this.metricValues = metricValues;
      this.dimensionValues = dimensionValues;
    }

    public override string ToString() { return
        $"{nameof(GAAppView)}[" +
        $"screenName: {screenName}, " +
        $"metricValues: {metricValues}, " +
        $"dimensionValues: {dimensionValues}" +
        $"]"; }
  }

  /**
   * [param name]  Required. Specifies the item name.
   * [param price] Specifies the price for a single item / unit.
   * [param quantity] Specifies the number of items purchased.
   * [param code] Specifies the SKU or item code.
   * [param category] Specifies the category that the item belongs to.
   * [param currencyCode] When present indicates the local currency for all
   *                      transaction currency values. Value should be a valid
   *                      ISO 4217 currency code.
   */
  public struct GAItem {
    public readonly string name, code, category, currencyCode;
    public readonly float? price;
    public readonly int? quantity;
    public readonly IDictionary<IMetric, uint> metricValues;
    public readonly IDictionary<IDimension, string> dimensionValues;

    public GAItem(string name, string code, string category, string currencyCode, float? price, int? quantity, IDictionary<IMetric, uint> metricValues, IDictionary<IDimension, string> dimensionValues) {
      if (name == null) throw new ArgumentNullException(nameof(name));
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
      return $"{nameof(GAItem)}[" +
             $"name: {name}, code: {code}, category: {category}, currencyCode: {currencyCode}, " +
             $"price: {price}, " +
             $"quantity: {quantity}, " +
             $"{nameof(metricValues)}: {metricValues}, " +
             $"{nameof(dimensionValues)}: {dimensionValues}, " +
             $"]";
    }
  }

  public interface IGAClient {
    /** Posts an event. **/
    void Event(GAEvent data);

    /** Registers an app view that you can see under screens tab. **/
    void AppView(GAAppView data);

    /** Registers an item purchase. **/
    void Item(GAItem data);
  }

  public static class GAClient {
    /**
     * Returns pseudo-random client id. In reality it shouldn't collide
     * unless you have millions of users.
     *
     * You can use it like this:
     *
     *     private const string KEY_CLIENT_ID = "google-analytics-client-id";
     *
     *     private static string clientId { get {
     *       var id = PlayerPrefs.GetString(KEY_CLIENT_ID, "");
     *       if (id == "") {
     *         id = ClientUtil.randomClientId;
     *         PlayerPrefs.SetString(KEY_CLIENT_ID, id);
     *       }
     *       return id;
     *     } }
     *
     * And then use it in Client constructor.
     **/
    public static string randomClientId => Guid.NewGuid().ToString();

    public const int
      MAX_CATEGORY_LENGTH = 150, MAX_ACTION_LENGTH = 500,
      MAX_LABEL_LENGTH = 500;

    public const string DEFAULT_URL = "http://www.google-analytics.com/collect";

    /* Dimensions that are always added to all hits. */
    public static IGAClient withBaseDimensions(
      this IGAClient underlying, IDictionary<IDimension, string> dimensions
    ) {
      return new AlwaysIncludedDimensionsWrapper(underlying, dimensions);
    }

    [Obsolete("Use " + nameof(Event) + "(" + nameof(GAEvent) + ")")]
    public static void Event(
      this IGAClient client,

      string category = null, string action = null, string label = null,
      int? value = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      client.Event(new GAEvent(category, action, label, value, metricValues, dimensionValues));
    }

    [Obsolete("Use " + nameof(AppView) + "(" + nameof(GAAppView) + ")")]
    public static void AppView(
      this IGAClient client,

      string screenName, IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      client.AppView(new GAAppView(screenName, metricValues, dimensionValues));
    }

    [Obsolete("Use " + nameof(Item) + "(" + nameof(GAItem) + ")")]
    public static void Item(
      this IGAClient client,

      string name, float? price = null, int? quantity = null, string code = null,
      string category = null, string currencyCode = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      client.Item(new GAItem(
        name: name, price: price, quantity: quantity, code: code, category: category,
        currencyCode: currencyCode, metricValues: metricValues, dimensionValues: dimensionValues
      ));
    }
  }
}