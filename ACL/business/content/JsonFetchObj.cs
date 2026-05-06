namespace ACL.business.content
{
    public class JsonFetchObj : IFetchObj
    {
        public T? Fetch<T>(string content) where T : class, new()
        {
            if (content == null || content.Length == 0) return default;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
        }
    }
}