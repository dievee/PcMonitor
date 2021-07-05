using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMonitor.Shared
{
    public static class Deserializer
    {
        public static T DeserializeJson<T>(string json)
        {
            var desetializedObject = JsonConvert.DeserializeObject<T>(json);

            return desetializedObject;
        }
    }
}
