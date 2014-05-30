using System;
using UnityEngine;

namespace com.tinylabproductions.GoogleAnalytics {
  internal static class FormExts {
    internal static void add(
      this WWWForm form, string argName, string value, string googleName,
      int maxLength = 0
    ) {
      if (value == null) return;
      value.checkLength(argName, maxLength);
      form.AddField(googleName, value);
    }

    internal static void checkLength(
      this string value, string argName, int maxLength
    ) {
      if (maxLength != 0 && value.Length > maxLength)
        throw new ArgumentException(string.Format(
          "length cannot exceed {0} bytes!", maxLength
        ), argName);
    }
  }
}
