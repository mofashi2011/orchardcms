namespace Orchard.Notify {
    public static class NotifierExtensions {
        public static void Information(this INotifier notifier, string message) {
            notifier.Add(NotifyType.Information, message);
        }
        public static void Warning(this INotifier notifier, string message) {
            notifier.Add(NotifyType.Warning, message);
        }
        public static void Error(this INotifier notifier, string message) {
            notifier.Add(NotifyType.Error, message);
        }
    }
}