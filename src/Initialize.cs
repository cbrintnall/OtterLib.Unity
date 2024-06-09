using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class Initialize
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitializeLib()
    {
        Debug.Log("Loading OtterLib, please standby.");

        JsonConvert.DefaultSettings = () =>
            new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new Vector3Converter() }
            };

        Debug.Log("Finished loading OtterLib.");
    }
}
