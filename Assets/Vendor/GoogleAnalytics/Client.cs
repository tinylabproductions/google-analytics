using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace com.tinylabproductions.GoogleAnalytics {
  public interface Client {
    /** 
     * Starts a user session that you can see under sessions tab.
     * 
     * You can still call other methods without a session.
     **/
    void SessionStart();

    /** Ends a user session. **/
    void SessionEnd();

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
      var parts = new string[5];
      for (var idx = 0; idx < parts.Length; idx++)
        parts[idx] = Convert.ToString(Mathf.RoundToInt(
          Random.value * Random.value * Random.value * 1000000
        ), 16);
      return string.Join("-", parts);
    } }

    public const string DEFAULT_URL = 
      "http://www.google-analytics.com/collect";

    private readonly string trackingId;
    private readonly string clientId;
    private readonly string appName;
    private readonly string appVersion;
    private readonly string url;
    private readonly IDictionary<IMetric, uint> customMetrics;
    private readonly IDictionary<IDimension, uint> customDimensions;
    private readonly string screenResolution;
    private readonly string userAgent;
    private readonly Dictionary<string,string> headers;
#if UNITY_EDITOR
    private readonly List<WWW> wwws = new List<WWW>();
#endif

    /**
     * [param trackingId] Tracking ID / Web property / Property ID.
     * [param clientId] Anonymous Client ID.
     * [param customMetrics] metric to metric id mapping.
     **/
    public ClientImpl(
      string trackingId, string clientId, 
      string appName, string appVersion,
      IDictionary<IMetric, uint> customMetrics = null,
      IDictionary<IDimension, uint> customDimensions = null,
      string url = DEFAULT_URL
    ) {
      appName.checkLength("appName", 100);
      appVersion.checkLength("appVersion", 100);

      this.trackingId = trackingId;
      this.clientId = clientId;
      this.appName = appName;
      this.appVersion = appVersion;
      this.customMetrics = customMetrics;
      this.customDimensions = customDimensions;
      this.url = url;

      screenResolution = 
        string.Format("{0}x{1}", Screen.width, Screen.height);

#if UNITY_IPHONE
      var os = SystemInfo.operatingSystem;
      var lowos = os.Replace(".", "_");
      var model = SystemInfo.deviceModel;

      if (model.StartsWith("iPhone")) model = "iPhone";
      else if (model.StartsWith("iPad")) model = "iPad";
      else if (model.StartsWith("iPod")) model = "iPod";

      userAgent = "Mozilla/5.0 (" + model + "; U; cpu " + lowos +
        " like Mac OS X) AppleWebKit/534.46.0 (KHTML, like Gecko) CriOS/19.0.1084.60 Mobile/9B206 Safari/7534.48.3";

      headers = new Dictionary<string, string>();
      headers.Add("User-Agent", userAgent);
 #endif
    }

    public void SessionStart() {
      var form = createForm();
      form.AddField("sc", "start");
      post(form);
    }
    
    public void SessionEnd() {
      var form = createForm();
      form.AddField("sc", "end");
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
      form.AddField("t", "event"); // Hit type
      form.add("category", category, "ec", 150);
      form.add("action", action, "ea", 500);
      form.add("label", label, "el", 500);
      if (value != null)
        form.add("value", value.ToString(), "ev");
      addMetrics(form, metricValues);
      addDimensions(form, dimensionValues);

      post(form);
    }

    public void AppView(string screenName) {
      var form = createForm();
      form.AddField("t", "appview"); // Hit type
      form.add("screenName", screenName, "cd", 1500);
      post(form);
    }

    public void Item(
      string name, float? price=null, int? quantity=null, string code=null, 
      string category=null, string currencyCode=null
    ) {
      if (name == null) throw new ArgumentNullException("name");

      var form = createForm();
      form.add("name", name, "in", 500);
      if (price != null) form.add("price", price.ToString(), "ip");
      if (quantity != null) form.add("quantity", quantity.ToString(), "iq");
      form.add("code", code, "ic", 500);
      form.add("category", category, "iv", 500);
      form.add("currencyCode", currencyCode, "cu", 10);
      post(form);
    }

    private void addMetrics(WWWForm form, IDictionary<IMetric, uint> values) {
      addCustom("metric", "cm", form, customMetrics, values);
    }

    private void addDimensions(WWWForm form, IDictionary<IDimension, string> values) {
      if (values == null) return;
      foreach (var pair in values)
        pair.Value.checkLength("dimension " + pair.Key + " value", 150);

      addCustom("dimension", "cd", form, customDimensions, values);
    }

    private static void addCustom<Metric, Value>(
      string name, string googleName, WWWForm form, 
      IDictionary<Metric, uint> indexes, 
      IDictionary<Metric, Value> values
    ) {
      if (values == null) return;
      if (indexes == null) throw new Exception(string.Format(
        "You haven't defined any custom {0}s in the constructor!", name
      ));

      foreach (var pair in values) {
        if (! indexes.ContainsKey(pair.Key))
          throw new ArgumentException(String.Format(
            "Unregistered {0}: {1} ", name, pair.Key
          ));

        var idx = indexes[pair.Key];
        form.AddField(googleName + idx, pair.Value.ToString());
      }
    }

    private WWWForm createForm() {
      var f = new WWWForm();
      f.AddField("v", 1); // version
      f.AddField("tid", trackingId);
      f.AddField("cid", clientId);
      f.AddField("an", appName);
      f.AddField("av", appVersion);
      f.AddField("sr", screenResolution);
      return f;
    }

    private void post(WWWForm form) {
      // Cleanup complete wwws.
      //wwws.RemoveAll(w => w.isDone);

#if UNITY_EDITOR
      Debug.Log(
        "Posting to Google Analytics: " + 
        Encoding.UTF8.GetString(form.data) +
        "\n\n" + debugCurrentWwws()
      );
#endif
#if !UNITY_IPHONE
      var www = new WWW(url, form);
#else
      var www = new WWW(url, form.data, headers);
#endif
#if UNITY_EDITOR
      wwws.Add(www);
#endif
    }

#if UNITY_EDITOR
    private string debugCurrentWwws() {
      var msg = "=== Executing WWW requests ===\n\n";
      foreach (var executingWww in wwws) {
        if (executingWww.isDone) {
          msg += "Headers:\n";
          foreach (var pair in executingWww.responseHeaders) {
            msg += "  " + pair.Key + " = " + pair.Value + "\n";
          }
          msg += "\n";

          if (executingWww.error != null)
            msg += "Error:\n" + executingWww.error;
          else
            msg += "Response:\n" + executingWww.text;
        }
        else {
          msg += "Fetching... " + executingWww.progress;
        }
      }
      return msg + "\n\n=== end ===";
    }
#endif
  }

  public class ClientNoOpImpl : Client {
    public void SessionStart() {}
    public void SessionEnd() {}
    public void Event(string category = null, string action = null, string label = null, int? value = null, IDictionary<IMetric, uint> metricValues = null, IDictionary<IDimension, string> dimensionValues = null) {}
    public void AppView(string screenName) {}
    public void Item(string name, float? price = null, int? quantity = null, string code = null, string category = null, string currencyCode = null) {}
  }
}
