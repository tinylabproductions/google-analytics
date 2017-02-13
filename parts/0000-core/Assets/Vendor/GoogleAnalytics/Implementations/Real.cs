using System.Collections.Generic;
using System.Text;
using com.tinylabproductions.TLPLib.Extensions;
using UnityEngine;
#if UNITY_IOS
using com.tinylabproductions.TLPLib.Concurrent;
#endif

namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  public class GAClientImpl : IGAClient {
    readonly ICollection<string> trackingIds;
    readonly string clientId, appName, appVersion, url, screenResolution;
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
      string url = GAClient.DEFAULT_URL
    ) {
      appName.checkLength("appName", 100);
      appVersion.checkLength("appVersion", 100);

      this.trackingIds = trackingIds;
      this.clientId = clientId;
      this.appName = appName;
      this.appVersion = appVersion;
      this.url = url;

      headers = new Dictionary<string, string>();
      screenResolution = $"{Screen.width}x{Screen.height}";

      if (Application.platform == RuntimePlatform.IPhonePlayer) {
        var os = SystemInfo.operatingSystem;
        var lowos = os.Replace(".", "_");
        var model = SystemInfo.deviceModel;

        if (model.StartsWithFast("iPhone")) model = "iPhone";
        else if (model.StartsWithFast("iPad")) model = "iPad";
        else if (model.StartsWithFast("iPod")) model = "iPod";

        var userAgent = $"Mozilla/5.0 ({model}; U; cpu {lowos} like Mac OS X) AppleWebKit/534.46.0 (KHTML, like Gecko) CriOS/19.0.1084.60 Mobile/9B206 Safari/7534.48.3";
        headers.Add("User-Agent", userAgent);
      }
    }

    public void Event(GAEvent d) {
      var form = createForm();
      form.Add("t", "event"); // Hit type
      form.add("ec", d.category, GAClient.MAX_CATEGORY_LENGTH);
      form.add("ea", d.action, GAClient.MAX_ACTION_LENGTH);
      form.add("el", d.label, GAClient.MAX_LABEL_LENGTH);
      if (d.value != null) form.add("ev", d.value.ToString());
      addMetrics(form, d.metricValues);
      addDimensions(form, d.dimensionValues);
      foreach (var referrer in d.referrer) addReferrer(form, referrer);

      post(form);
    }

    public void AppView(GAAppView d) {
      screenName = d.screenName;
      var form = createForm();
      form.Add("t", "screenview"); // Hit type
      addMetrics(form, d.metricValues);
      addDimensions(form, d.dimensionValues);
      foreach (var referrer in d.referrer) addReferrer(form, referrer);
      post(form);
    }

    public void Item(GAItem d) {
      var form = createForm();
      form.add("in", d.name, 500);
      if (d.price != null) form.add("ip", d.price.ToString());
      if (d.quantity != null) form.add("iq", d.quantity.ToString());
      form.add("ic", d.code, 500);
      form.add("iv", d.category, 500);
      form.add("cu", d.currencyCode, 10);
      addMetrics(form, d.metricValues);
      addDimensions(form, d.dimensionValues);
      foreach (var referrer in d.referrer) addReferrer(form, referrer);
      post(form);
    }

    public void Timing(GATiming d) {
      var form = createForm();
      form.Add("t", "timing"); // Hit type
      form.add("utc", d.category, 150);
      form.add("utv", d.name, 500);
      form.add("utt", d.timeMs.ToString());
      if (d.label != null) form.add("utl", d.label, 500);
      addMetrics(form, d.metricValues);
      addDimensions(form, d.dimensionValues);
      foreach (var referrer in d.referrer) addReferrer(form, referrer);
      post(form);
    }

    static void addMetrics(IDictionary<string, string> form, IDictionary<IMetric, uint> values) {
      addCustom("cm", form, values);
    }

    static void addDimensions(IDictionary<string, string> form, IDictionary<IDimension, string> values) {
      if (values == null) return;
      foreach (var pair in values)
        pair.Value.checkLength("dimension " + pair.Key + " value", 150);

      addCustom("cd", form, values);
    }

    static void addReferrer(IDictionary<string, string> form, GAReferrer referrer) {
      form.add("dr", referrer.documentReferrer, 2048);
      form.add("cn", referrer.campaignName, 100);
      form.add("cs", referrer.campaignSource, 100);
      form.add("cm", referrer.campaignMedium, 50);
      form.add("ck", referrer.campaignKeyword, 500);
      form.add("cc", referrer.campaignContent, 500);
      form.add("ci", referrer.campaignId, 100);
    }

    static void addCustom<Key, Value>(
      string googleName, IDictionary<string, string> form,
      IDictionary<Key, Value> values
    ) where Key : IIndexed {
      if (values == null) return;

      foreach (var pair in values) {
        form.Add(googleName + pair.Key.index, pair.Value.ToString());
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

#if !UNITY_IOS
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
