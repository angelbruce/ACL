using ACL.dao;

namespace ACL.business.content
{

    interface IFetchObj
    {
        public T? Fetch<T>(string content) where T : class, new();

        public static IFetchObj Create(ContentType contentType)
        {
            switch (contentType)
            {
                case ContentType.Yaml:
                    return new YamlFetchObj();
                case ContentType.Json:
                    return new JsonFetchObj();
                default:
                    return null;
            }
        }
    }
}