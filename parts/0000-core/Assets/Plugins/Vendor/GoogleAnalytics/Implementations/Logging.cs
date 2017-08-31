using com.tinylabproductions.TLPLib.Logger;

namespace com.tinylabproductions.GoogleAnalytics.Implementations {
  public class GAClientLoggingImpl : IGAClient {
    public const Log.Level DEFAULT_LEVEL = Log.Level.DEBUG;

    readonly ILog logger;
    readonly Log.Level level;

    public GAClientLoggingImpl(ILog logger, Log.Level level = DEFAULT_LEVEL) {
      this.logger = logger;
      this.level = level;
    }

    public GAClientLoggingImpl(Log.Level level = DEFAULT_LEVEL) 
      : this(Log.defaultLogger, level) {}

    public void Event(GAEvent data) { if (logger.willLog(level)) log(data.ToString()); }
    public void AppView(GAAppView data) { if (logger.willLog(level)) log(data.ToString()); }
    public void Item(GAItem data) { if (logger.willLog(level)) log(data.ToString()); }
    public void Timing(GATiming data) { if (logger.willLog(level)) log(data.ToString()); }

    void log(string s) => logger.debug($"[Google Analytics] {s}");
  }
}
