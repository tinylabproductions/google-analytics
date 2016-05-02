namespace com.tinylabproductions.GoogleAnalytics {
  /**
   * Evil hack due to mono compiler bug which fails with random errors
   * if Client is defined with TMetric generic type (which would allow us
   * to use custom enums as type parameters).
   * 
   * Use this as follows:
   * 
   *    public static class Metric {
   *      class Score : IMetric {}
   *      class Coins : IMetric {}
   *      class Time : IMetric {}
   *
   *      public static readonly IMetric score = new Score();
   *      public static readonly IMetric coins = new Coins();
   *      public static readonly IMetric time = new Time();
   *    }
   **/
  public interface IMetric {}
}
