using UnityEngine;

namespace OMG
{
    public class JSONSaveService : ISaveService
    {
        public T Get<T>(string key, T defaultValue)
        {
            T result = default(T);

            if (!PlayerPrefs.HasKey(key))
            {
                result = defaultValue;
            }
            else
            {
                result = JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
            }

            return result;
        }

        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int GetInt(string key, int defaultValue) 
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void Save<T>(string key, T item)
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(item));
        }
    }
}