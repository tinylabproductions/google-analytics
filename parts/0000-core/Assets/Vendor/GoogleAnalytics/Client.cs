using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.tinylabproductions.GoogleAnalytics.Implementations;
using com.tinylabproductions.TLPLib.Extensions;
using com.tinylabproductions.TLPLib.Functional;
using com.tinylabproductions.TLPLib.Net;

namespace com.tinylabproductions.GoogleAnalytics {
  public struct GAEvent {
    public readonly string category, action, label;
    public readonly int? value;
    public readonly IDictionary<IMetric, uint> metricValues;
    public readonly IDictionary<IDimension, string> dimensionValues;
    public readonly Option<GAReferrer> referrer;

    public GAEvent(
      string category=null, string action=null, string label=null,
      int? value=null,
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null,
      Option<GAReferrer> referrer=default(Option<GAReferrer>)
    ) {
      if (category == null && action == null && label == null && value == null)
        throw new ArgumentException("All parameters cannot be null!");
      Option.ensureValue(ref referrer);
      this.referrer = referrer;
      this.category = category;
      this.action = action;
      this.label = label;
      this.value = value;
      this.metricValues = metricValues;
      this.dimensionValues = dimensionValues;
    }

    public GAEvent withReferrer(Option<GAReferrer> referrer) =>
      new GAEvent(category, action, label, value, metricValues, dimensionValues, referrer);

    public override string ToString() => 
      $"{nameof(GAEvent)}[" +
      $"category: {category}, " +
      $"action: {action}, " +
      $"label: {label}, " +
      $"value: {value}, " +
      $"metricValues: {metricValues.asString()}, " +
      $"dimensionValues: {dimensionValues.asString()}, " +
      $"{referrer}" +
      $"]";
  }

  public struct GAAppView {
    public readonly string screenName;
    public readonly IDictionary<IMetric, uint> metricValues;
    public readonly IDictionary<IDimension, string> dimensionValues;
    public readonly Option<GAReferrer> referrer;

    public GAAppView(
      string screenName,
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null,
      Option<GAReferrer> referrer=default(Option<GAReferrer>)
    ) {
      this.screenName = screenName;
      this.metricValues = metricValues;
      this.dimensionValues = dimensionValues;
      Option.ensureValue(ref referrer);
      this.referrer = referrer;
    }

    public GAAppView withReferrer(Option<GAReferrer> referrer) =>
      new GAAppView(screenName, metricValues, dimensionValues, referrer);

    public override string ToString() { return
        $"{nameof(GAAppView)}[" +
        $"screenName: {screenName}, " +
        $"metricValues: {metricValues.asString()}, " +
        $"dimensionValues: {dimensionValues.asString()}, " +
        $"{referrer}" +
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
    public readonly Option<GAReferrer> referrer;

    public GAItem(
      string name, string code=null, string category=null, string currencyCode = null,
      float? price=null, int? quantity=null,
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null,
      Option<GAReferrer> referrer = default(Option<GAReferrer>)
    ) {
      if (name == null) throw new ArgumentNullException(nameof(name));
      this.name = name;
      this.code = code;
      this.category = category;
      this.currencyCode = currencyCode;
      this.price = price;
      this.quantity = quantity;
      this.metricValues = metricValues;
      this.dimensionValues = dimensionValues;
      Option.ensureValue(ref referrer);
      this.referrer = referrer;
    }

    public GAItem withReferrer(Option<GAReferrer> referrer) => new GAItem(
      name, code, category, currencyCode, price, quantity, metricValues, dimensionValues, referrer
    );

    public override string ToString() {
      return $"{nameof(GAItem)}[" +
             $"name: {name}, code: {code}, category: {category}, currencyCode: {currencyCode}, " +
             $"price: {price}, " +
             $"quantity: {quantity}, " +
             $"{nameof(metricValues)}: {metricValues}, " +
             $"{nameof(dimensionValues)}: {dimensionValues}, " +
             $"{referrer}" +
             $"]";
    }
  }

  /**
   * [param category] Required. Specifies the user timing category.
   * [param name] Required. Specifies the user timing variable.
   * [param time] Required. Specifies the user timing value. The value is in milliseconds.
   * [param label] Specifies the user timing label.
   */
  public struct GATiming {
    public readonly string category, name, label;
    public readonly int timeMs;
    public readonly IDictionary<IMetric, uint> metricValues;
    public readonly IDictionary<IDimension, string> dimensionValues;
    public readonly Option<GAReferrer> referrer;

    public GATiming(
      string category, string name, int timeMs,
      string label = null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null,
      Option<GAReferrer> referrer = default(Option<GAReferrer>)
    ) {
      this.category = category;
      this.name = name;
      this.timeMs = timeMs;
      this.label = label;
      this.metricValues = metricValues;
      this.dimensionValues = dimensionValues;
      Option.ensureValue(ref referrer);
      this.referrer = referrer;
    }

    public GATiming withReferrer(Option<GAReferrer> referrer) => new GATiming(
      category, name, timeMs, label, metricValues, dimensionValues, referrer
    );

    public override string ToString() => 
      $"{nameof(GATiming)}[" +
      $"category: {category}, " +
      $"name: {name}, " +
      $"time: {timeMs}, " +
      $"label: {label}, " +
      $"metricValues: {metricValues.asString()}, " +
      $"dimensionValues: {dimensionValues.asString()}, " +
      $"{referrer}" +
      $"]";
  }

  public struct GAReferrer {
    /**
     * Specifies which referral source brought traffic to a website. This value is 
     * also used to compute the traffic source. The format of this value is a URL.
     **/
    public readonly Option<string> documentReferrer;
    public readonly Option<string> 
      campaignName, campaignSource, campaignMedium, campaignKeyword, 
      campaignContent, campaignId;

    public GAReferrer(
      Option<string> documentReferrer = default(Option<string>), 
      Option<string> campaignName = default(Option<string>), 
      Option<string> campaignSource = default(Option<string>), 
      Option<string> campaignMedium = default(Option<string>), 
      Option<string> campaignKeyword = default(Option<string>), 
      Option<string> campaignContent = default(Option<string>), 
      Option<string> campaignId = default(Option<string>)
    ) {
      Option.ensureValue(ref documentReferrer);
      this.documentReferrer = documentReferrer;
      Option.ensureValue(ref campaignName);
      this.campaignName = campaignName;
      Option.ensureValue(ref campaignSource);
      this.campaignSource = campaignSource;
      Option.ensureValue(ref campaignMedium);
      this.campaignMedium = campaignMedium;
      Option.ensureValue(ref campaignKeyword);
      this.campaignKeyword = campaignKeyword;
      Option.ensureValue(ref campaignContent);
      this.campaignContent = campaignContent;
      Option.ensureValue(ref campaignId);
      this.campaignId = campaignId;
    }

    public static Try<GAReferrer> fromQueryString(string qs) {
      return QueryString.parseKV(qs).map(list => {
        var dict = list.ToDictionary(t => t._1, t => t._2);
        return new GAReferrer(
          campaignName: dict.get("utm_campaign"),
          campaignSource: dict.get("utm_source"),
          campaignMedium: dict.get("utm_medium"),
          campaignKeyword: dict.get("utm_term"),
          campaignContent: dict.get("utm_content")
        );
      });
    }

    static void addPart(StringBuilder sb, string name, Option<string> optVal, ref bool first) {
      foreach (var val in optVal) {
        if (first) first = false;
        else sb.Append(", ");

        sb.Append(name);
        sb.Append(": ");
        sb.Append(val);
      }
    }

    public override string ToString() {
      var first = true;
      var sb = new StringBuilder(nameof(GAReferrer));
      sb.Append('[');
      addPart(sb, nameof(documentReferrer), documentReferrer, ref first);
      addPart(sb, nameof(campaignName), campaignName, ref first);
      addPart(sb, nameof(campaignSource), campaignSource, ref first);
      addPart(sb, nameof(campaignMedium), campaignMedium, ref first);
      addPart(sb, nameof(campaignKeyword), campaignKeyword, ref first);
      addPart(sb, nameof(campaignContent), campaignContent, ref first);
      addPart(sb, nameof(campaignId), campaignId, ref first);
      sb.Append(']');
      return sb.ToString();
    }
  }

  public interface IGAClient {
    /** Posts an event. **/
    void Event(GAEvent data);

    /** Registers an app view that you can see under screens tab. **/
    void AppView(GAAppView data);

    /** Registers an item purchase. **/
    void Item(GAItem data);

    /** User Timing Tracking. **/
    void Timing(GATiming data);
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
    ) => new AlwaysIncludedDimensionsWrapper(underlying, dimensions);

    /** Adds specified referrer if no referrer is specified with the hit. */
    public static IGAClient withDefaultReferrer(
      this IGAClient underlying, GAReferrer referrer
    ) => new DefaultReferrerWrapper(underlying, referrer);

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