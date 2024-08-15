namespace OMG
{
    public interface ISaveService
    {
        void Save<T>(string key, T item);
        T Get<T>(string key, T defaultValue);
    }
}