using System.Collections.Generic;
using UnityEngine;

#if JSON_ENABLED
using Newtonsoft.Json;
#endif
public static class Initialize
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitializeLib()
    {
        Debug.Log("Loading OtterLib, please standby.");

#if JSON_ENABLED
        Debug.Log("Newtonsoft detected, setting default converters");
        JsonConvert.DefaultSettings = () =>
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new Vector3Converter() }
            };
#endif

        Debug.Log("Finished loading OtterLib.");
    }
}
