using System;
using System.IO;
using Newtonsoft.Json;

namespace IOCore.Libs
{
    public class JsonUtils
    {
        public static T Load<T>(string filePath, T defaultValue)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
            }
            catch (Exception)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch(Exception)
                {
                }

                return defaultValue;
            }
        }

        public static void Save<T>(string filePath, T value)
        {
            try
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(value));
            }
            catch (Exception)
            {
            }
        }

        public static void Clear(string filePath)
        {
            Utils.DeleteFileOrDirectory(filePath);
        }
    }
}