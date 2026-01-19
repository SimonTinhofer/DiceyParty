using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DiceyParty
{
    public static class BackendAPI
    {
        private static readonly string BackendBaseUrl = "https://diceypartyapi.onrender.com";
        
        public static async Awaitable<Session> PostSessions(string name)
        {
            var data = new { sessionName = $"{name}" };
            string json = JsonConvert.SerializeObject(data);
            byte[] jsonToSend = Encoding.UTF8.GetBytes(json);
            
            using var request = new UnityWebRequest($"{BackendBaseUrl}/sessions", "POST");
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Network Error: {request.error} | Response: {request.downloadHandler.text}"); 
            }
            
            return JsonConvert.DeserializeObject<Session>(request.downloadHandler.text);
        }

        public static async Awaitable<List<Session>> GetSessions()
        {
            using var request = UnityWebRequest.Get($"{BackendBaseUrl}/sessions");
            await request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Network Error: {request.error} | Response: {request.downloadHandler.text}");
            }
            
            return JsonConvert.DeserializeObject<List<Session>>(request.downloadHandler.text);
        }
        
        public static async Awaitable<bool> DeleteSessions(Session session)
        {
            using var request = UnityWebRequest.Delete($"{BackendBaseUrl}/sessions?sessionName={session.Name}&deploymentId={session.DeploymentId}");
            await request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Network Error: {request.error} | Response: {request.downloadHandler.text}");
            }
            return true;
        }
    }
}