using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  public class GAClientImpl : IGAClient {
    readonly ICollection<string> trackingIds;
    readonly string clientId, appName, appVersion, url, screenResolution;
    readonly IDictionary<IMetric, uint> metricIndexes;
    readonly IDictionary<IDimension, uint> dimensionIndexes;
    readonly Dictionary<string,string> headers;
    string screenName = "Not Set";

    /**
     * [param trackingId] Tracking ID / Web property / Property ID.
     * [param clientId] Anonymous Client ID.
     * [param customMetrics] metric to metric id mapping.
     **/
    public GAClientImpl(
      ICollection<string> trackingIds, string clientId,
      string appName, string appVersion,
      IDictionary<IMetric, uint> metricIndexes = null,
      IDictionary<IDimension, uint> dimensionIndexes = null,
      string url = GAClient.DEFAULT_URL
    ) {
      appName.checkLength("appName", 100);
      appVersion.checkLength("appVersion", 100);

      this.trackingIds = trackingIds;
      this.clientId = clientId;
      this.appName = appName;
      this.appVersion = appVersion;
      this.metricIndexes = metricIndexes;
      this.dimensionIndexes = dimensionIndexes;
      this.url = url;

      headers = new Dictionary<string, string>();
      screenResolution = $"{Screen.width}x{Screen.height}";

      if (Application.platform == RuntimePlatform.IPhonePlayer) {
        var os = SystemInfo.operatingSystem;
        var lowos = os.Replace(".", "_");
        var model = SystemInfo.deviceModel;

        if (model.StartsWith("iPhone")) model = "iPhone";
        else if (model.StartsWith("iPad")) model = "iPad";
        else if (model.StartsWith("iPod")) model = "iPod";

        var userAgent = $"Mozilla/5.0 ({model}; U; cpu {lowos} like Mac OS X) AppleWebKit/534.46.0 (KHTML, like Gecko) CriOS/19.0.1084.60 Mobile/9B206 Safari/7534.48.3";
        headers.Add("User-Agent", userAgent);
      }
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
      form.add("ec", category, GAClient.MAX_CATEGORY_LENGTH);
      form.add("ea", action, GAClient.MAX_ACTION_LENGTH);
      form.add("el", label, GAClient.MAX_LABEL_LENGTH);
      if (value != null) form.add("ev", value.ToString());
      addMetrics(form, metricValues);
      addDimensions(form, dimensionValues);

      post(form);
    }

    public void AppView(
      string screenName,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      this.screenName = screenName;
      var form = createForm();
      form.Add("t", "appview"); // Hit type
      addMetrics(form, metricValues);
      addDimensions(form, dimensionValues);
      post(form);
    }

    public void Item(
      string name, float? price=null, int? quantity=null, string code=null,
      string category=null, string currencyCode=null,
      IDictionary<IMetric, uint> metricValues = null,
      IDictionary<IDimension, string> dimensionValues = null
    ) {
      if (name == null) throw new ArgumentNullException(nameof(name));

      var form = createForm();
      form.add("in", name, 500);
      if (price != null) form.add("ip", price.ToString());
      if (quantity != null) form.add("iq", quantity.ToString());
      form.add("ic", code, 500);
      form.add("iv", category, 500);
      form.add("cu", currencyCode, 10);
      addMetrics(form, metricValues);
      addDimensions(form, dimensionValues);
      post(form);
    }

    void addMetrics(IDictionary<string, string> form, IDictionary<IMetric, uint> values) {
      addCustom("metric", "cm", form, metricIndexes, values);
    }

    void addDimensions(IDictionary<string, string> form, IDictionary<IDimension, string> values) {
      if (values == null) return;
      foreach (var pair in values)
        pair.Value.checkLength("dimension " + pair.Key + " value", 150);

      addCustom("dimension", "cd", form, dimensionIndexes, values);
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
        new WWW(url, wwwForm.data, headers);
#else
        // This is used because of il2cpp bug which crashes the runtime
        // if several wwws are running at the same time.
        ASync.oneAtATimeWWW(() => new WWW(url, wwwForm.data, headers));
#endif
      }
    }
  }
}
