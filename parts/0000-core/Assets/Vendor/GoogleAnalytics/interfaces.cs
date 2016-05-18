namespace com.tinylabproductions.GoogleAnalytics {
  public interface IIndexed {
    uint index { get; }
  }

  /**
   * Evil hack due to mono compiler bug which fails with random errors
   * if Client is defined with TMetric generic type (which would allow us
   * to use custom enums as type parameters).
   *
   * Use this as follows:
   *
   *    public static class Metric {
   *      class Score : IMetric { uint index { get; } = 1; }
   *      class Coins : IMetric { uint index { get; } = 2; }
   *      class Time : IMetric { uint index { get; } = 3; }
   *
   *      public static readonly IMetric score = new Score();
   *      public static readonly IMetric coins = new Coins();
   *      public static readonly IMetric time = new Time();
   *    }
   **/
  public interface IMetric : IIndexed {}

  // See IMetric for motivation.
  public interface IDimension : IIndexed {}
}
