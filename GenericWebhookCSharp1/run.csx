#r "Newtonsoft.Json"
 
using System;
using System.Net;
using Newtonsoft.Json;
 
// The string caller was added to the function parameters to get the caller from the URL.
// The ICollector<string> outQueue was added to the function parameters to get access to the output queue.
public static async Task<object> Run(HttpRequestMessage req, string caller, ICollector<string> outQueue, TraceWriter log)
{
    log.Info($"Webhook was triggered!");
 
    // The JSON payload is found in the request
    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
 
    // Create a dynamic JSON output, enveloping the payload with 
    // The caller, a timestamp, and the payload itself
    dynamic outData = new Newtonsoft.Json.Linq.JObject();
    outData.caller = caller;
    outData.timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    outData.payload = data;
     
    // Add the JSON as a string to the output queue
    outQueue.Add(JsonConvert.SerializeObject(outData));     
     
    // Return status 200 OK to the calling system.
    return req.CreateResponse(HttpStatusCode.OK, new
    {
        caller = $"{caller}",
        status = "OK"
    });
}