namespace OMG
{
    public interface ISaveService
    {
        void Save<T>(string key, T item);
        T Get<T>(string key, T defaultValue);

        void SaveInt(string key, int defaultValue);
        int GetInt(string key, int defaultValue);
    }
}