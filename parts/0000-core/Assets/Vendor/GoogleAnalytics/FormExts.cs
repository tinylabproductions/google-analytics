using System;
using System.Collections.Generic;

namespace com.tinylabproductions.GoogleAnalytics {
  internal static class FormExts {
    internal static void add(
      this Dictionary<string, string> form, string key, string value, int maxLength=-1
    ) {
      if (value == null) return;
      form.Add(key, fixLength(value, maxLength));
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
      if (maxLength < 0) return value;
      if (value.Length <= maxLength) return value;
      return value.Substring(0, maxLength);
    }
  }
}
