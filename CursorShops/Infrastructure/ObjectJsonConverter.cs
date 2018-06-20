using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CursorShops.Infrastructure
{
    public static class ObjectJsonConverter
    {
        public static T ConvertFromJson<T>(string json)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (T)ser.ReadObject(ms);
            }
        }

        public static string ConvertToJson(object obj)
        {
            StringBuilder json_html = new StringBuilder();
            System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                json_html.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                return json_html.ToString();
            }
        }
    }
}
