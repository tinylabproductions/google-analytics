using System;
using UnityEngine;

namespace com.tinylabproductions.GoogleAnalytics {
  internal static class FormExts {
    internal static void add(
      this WWWForm form, string argName, string value, string googleName,
      int maxLength = 0
    ) {
      if (value == null) return;
      form.AddField(googleName, fixLength(value, maxLength));
    }

    internal static void checkLength(
      this string value, string argName, int maxLength
    ) {
      if (maxLength != 0 && value.Length > maxLength)
        throw new ArgumentException(string.Format(
          $"length cannot exceed {maxLength} bytes, but it was {value.Length} bytes: {value}"
        ), argName);
    }

    static string fixLength(string value, int maxLength) {
      if (value.Length <= maxLength) return value;
      return value.Substring(0, maxLength);
    }
  }
}
