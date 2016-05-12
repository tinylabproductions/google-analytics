using System;
using System.Collections.Generic;
using com.tinylabproductions.GoogleAnalytics.Implementations;

namespace com.tinylabproductions.GoogleAnalytics {
  public interface IGAClient {
    /** Posts an event. **/
    void Event(
      string category = null, string action = null, string label = null,
      int? value = null,
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null
    );

    /** Registers an app view that you can see under screens tab. **/
    void AppView(
      string screenName,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    );

    /**
     * Registers an item purchase.
     *
     * [param name]  Required. Specifies the item name.
     * [param price] Specifies the price for a single item / unit.
     * [param quantity] Specifies the number of items purchased.
     * [param code] Specifies the SKU or item code.
     * [param category] Specifies the category that the item belongs to.
     * [param currencyCode] When present indicates the local currency for all
     *                      transaction currency values. Value should be a valid
     *                      ISO 4217 currency code.
     **/
    void Item(
      string name, float? price=null, int? quantity=null, string code=null,
      string category=null, string currencyCode=null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    );
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
  }
}