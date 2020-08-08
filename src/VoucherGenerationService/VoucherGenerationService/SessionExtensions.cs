using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace WomPlatform.Web.Generator {

    public static class SessionExtensions {

        public static void SetObject<T>(this ISession session, string key, T value) {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key) where T : class {
            var v = session.GetString(key);
            if(v == null) {
                return null;
            }

            return JsonSerializer.Deserialize<T>(v);
        }

    }

}
