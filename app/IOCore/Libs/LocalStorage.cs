using Windows.Storage;

namespace IOCore.Libs
{
    public class LocalStorage
    {
        private readonly static string SCOPE = nameof(LocalStorage);

        public static void Set<T>(string name, T value)
        {
            ApplicationData.Current.LocalSettings.Values[$"{SCOPE}-{name}"] = Utils.ChangeType<string>(value);
        }

        public static T GetValueOrDefault<T>(string name, T defaultValue)
        {
            return IsExist(name) ?
                Utils.ChangeType<T>(ApplicationData.Current.LocalSettings.Values[$"{SCOPE}-{name}"]) :
                defaultValue;
        }

        public static void Remove(string name)
        {
            ApplicationData.Current.LocalSettings.Values.Remove($"{SCOPE}-{name}");
        }

        public static bool IsExist(string name)
        {
            return ApplicationData.Current.LocalSettings.Values[$"{SCOPE}-{name}"] != null;
        }
    }
}
