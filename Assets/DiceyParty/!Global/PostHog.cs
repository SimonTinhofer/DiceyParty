using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;

namespace DiceyParty
{
    [System.Serializable]
    public class PostHogEvent
    {
        public string api_key;
        public string @event;
        public string distinct_id;
        public Dictionary<string, object> properties;
    }

    public static class PostHog
    {
        private static readonly string Url = "https://eu.i.posthog.com/i/v0/e/";
        private static readonly string ApiKey = "phc_MtFHDyw82zQ46NYtTgFDIz99dSvDmlNtbeaZKPphCuU";

        private static readonly HttpClient Client = new HttpClient();

        public static async void Capture(
            string eventName,
            string distinctId,
            Dictionary<string, object> eventData = null
        )
        {
            var payload = new PostHogEvent
            {
                api_key = ApiKey,
                @event = eventName,
                distinct_id = distinctId,
                properties = eventData ?? new Dictionary<string, object>()
            };

            var json = JsonConvert.SerializeObject(payload);
            Debug.Log("POSTHOG JSON: " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await Client.PostAsync(Url, content);
        }
    }
}