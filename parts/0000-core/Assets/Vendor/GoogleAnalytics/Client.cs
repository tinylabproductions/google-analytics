using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.tinylabproductions.GoogleAnalytics {
  public interface Client {
    /** Posts an event. **/
    void Event(
      string category = null, string action = null, string label = null, 
      int? value = null, 
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null
    );

    /** Registers an app view that you can see under screens tab. **/
    void AppView(string screenName);

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
      string category=null, string currencyCode=null
    );
  }

  public class ClientImpl : Client {
    public const int 
      MAX_CATEGORY_LENGTH = 150, MAX_ACTION_LENGTH = 500, 
      MAX_LABEL_LENGTH = 500;

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
     *         id = Client.randomClientId;
     *         PlayerPrefs.SetString(KEY_CLIENT_ID, id);
     *       }
     *       return id;
     *     } }
     *     
     * And then use it in Client constructor.
     **/
    public static string randomClientId { get {
      return Guid.NewGuid().ToString();
    } }

    public const string DEFAULT_URL = 
      "http://www.google-analytics.com/collect";

    readonly ICollection<string> trackingIds;
    readonly string clientId, appName, appVersion, url, screenResolution;
    readonly IDictionary<IMetric, uint> customMetrics;
    readonly IDictionary<IDimension, uint> customDimensions;
#if UNITY_IPHONE
    readonly Dictionary<string,string> headers;
#endif
    string screenName = "Not Set";

    /**
     * [param trackingId] Tracking ID / Web property / Property ID.
     * [param clientId] Anonymous Client ID.
     * [param customMetrics] metric to metric id mapping.
     **/
    public ClientImpl(
      ICollection<string> trackingIds, string clientId, 
      string appName, string appVersion,
      IDictionary<IMetric, uint> customMetrics = null,
      IDictionary<IDimension, uint> customDimensions = null,
      string url = DEFAULT_URL
    ) {
      appName.checkLength("appName", 100);
      appVersion.checkLength("appVersion", 100);

      this.trackingIds = trackingIds;
      this.clientId = clientId;
      this.appName = appName;
      this.appVersion = appVersion;
      this.customMetrics = customMetrics;
      this.customDimensions = customDimensions;
      this.url = url;

      screenResolution = $"{Screen.width}x{Screen.height}";

#if UNITY_IPHONE
      var os = SystemInfo.operatingSystem;
      var lowos = os.Replace(".", "_");
      var model = SystemInfo.deviceModel;

      if (model.StartsWith("iPhone")) model = "iPhone";
      else if (model.StartsWith("iPad")) model = "iPad";
      else if (model.StartsWith("iPod")) model = "iPod";

      var userAgent = $"Mozilla/5.0 ({model}; U; cpu {lowos} like Mac OS X) AppleWebKit/534.46.0 (KHTML, like Gecko) CriOS/19.0.1084.60 Mobile/9B206 Safari/7534.48.3";

      headers = new Dictionary<string, string> {{"User-Agent", userAgent}};
#endif
    }

    public void SessionStart() {
      var form = createForm();
      form.Add("sc", "start");
      post(form);
    }
    
    public void SessionEnd() {
      var form = createForm();
      form.Add("sc", "end");
      post(form);
    }

    public void Event(
      string category = null, string action = null, string label = null, 
      int? value = null, 
      IDictionary<IMetric, uint> metricValues=null,
      IDictionary<IDimension, string> dimensionValues=null
    ) {
      if (category == null && action == null && label == null && value == null)
        throw new ArgumentException("All parameters cannot be null!");

      var form = createForm();
      form.Add("t", "event"); // Hit type
      form.add("ec", category, MAX_CATEGORY_LENGTH);
      form.add("ea", action, MAX_ACTION_LENGTH);
      form.add("el", label, MAX_LABEL_LENGTH);
      if (value != null) form.add("ev", value.ToString());
      addMetrics(form, metricValues);
      addDimensions(form, dimensionValues);

      post(form);
    }

    public void AppView(string screenName) {
      this.screenName = screenName;
      var form = createForm();
      form.Add("t", "appview"); // Hit type
      post(form);
    }

    public void Item(
      string name, float? price=null, int? quantity=null, string code=null, 
      string category=null, string currencyCode=null
    ) {
      if (name == null) throw new ArgumentNullException(nameof(name));

      var form = createForm();
      form.add("in", name, 500);
      if (price != null) form.add("ip", price.ToString());
      if (quantity != null) form.add("iq", quantity.ToString());
      form.add("ic", code, 500);
      form.add("iv", category, 500);
      form.add("cu", currencyCode, 10);
      post(form);
    }

    void addMetrics(IDictionary<string, string> form, IDictionary<IMetric, uint> values) {
      addCustom("metric", "cm", form, customMetrics, values);
    }

    void addDimensions(IDictionary<string, string> form, IDictionary<IDimension, string> values) {
      if (values == null) return;
      foreach (var pair in values)
        pair.Value.checkLength("dimension " + pair.Key + " value", 150);

      addCustom("dimension", "cd", form, customDimensions, values);
    }

    static void addCustom<Metric, Value>(
      string name, string googleName, IDictionary<string, string> form, 
      IDictionary<Metric, uint> indexes, 
      IDictionary<Metric, Value> values
    ) {
      if (values == null) return;
      if (indexes == null)
        throw new Exception($"You haven't defined any custom {name}s in the constructor!");

      foreach (var pair in values) {
        if (! indexes.ContainsKey(pair.Key))
          throw new ArgumentException($"Unregistered {name}: {pair.Key} ");

        var idx = indexes[pair.Key];
        form.Add(googleName + idx, pair.Value.ToString());
      }
    }

    Dictionary<string, string> createForm() {
      var f = new Dictionary<string, string> {
        {"v", "1"},
        {"cid", clientId},
        {"an", appName},
        {"av", appVersion},
        {"sr", screenResolution}
      };
      f.add("cd", screenName, 2048);
      return f;
    }

    void post(Dictionary<string, string> form) {
      foreach (var trackingId in trackingIds) {
        var wwwForm = new WWWForm();
        foreach (var kv in form) wwwForm.AddField(kv.Key, kv.Value);
        wwwForm.AddField("tid", trackingId);

        if (Application.isEditor || Debug.isDebugBuild) Debug.Log(
          "Posting to Google Analytics: " + Encoding.UTF8.GetString(wwwForm.data)
        );

#if !UNITY_IPHONE
        // ReSharper disable once ObjectCreationAsStatement
        new WWW(url, wwwForm);
#else
        // This is used because of il2cpp bug which crashes the runtime
        // if several wwws are running at the same time.
        ASync.oneAtATimeWWW(() => new WWW(url, form.data, headers));
#endif
      }
    }
  }

  public class ClientNoOpImpl : Client {
    public static readonly ClientNoOpImpl instance = new ClientNoOpImpl();
    ClientNoOpImpl() { }

    public void SessionStart() {}
    public void SessionEnd() {}
    public void Event(string category = null, string action = null, string label = null, int? value = null, IDictionary<IMetric, uint> metricValues = null, IDictionary<IDimension, string> dimensionValues = null) {}
    public void AppView(string screenName) {}
    public void Item(string name, float? price = null, int? quantity = null, string code = null, string category = null, string currencyCode = null) {}
  }
}
