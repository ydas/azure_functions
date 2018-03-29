#r "Newtonsoft.Json"
 
using System;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

// The string caller was added to the function parameters to get the caller from the URL.
// The ICollector<string> outQueue was added to the function parameters to get access to the output queue.
public static async Task<object> Run(HttpRequestMessage req, string caller, ICollector<string> outQueue, TraceWriter log)
{

    log.Info($"Webhook was triggered!");

    // The JSON payload is found in the request
    string jsonContent = await req.Content.ReadAsStringAsync();

    // uncomment this to log incoming message under section "Monitor"
    //log.Info(jsonContent);

    // ----------- function registration 
    // rem out the following code after the function has been validated by grid event
    // check if webhook registration request and return validation code
    dynamic jsonToken = JToken.Parse(jsonContent);
    if (jsonToken.Type==JTokenType.Array)
    {
        JArray jsonArray = JArray.Parse(jsonContent);
        dynamic data2 = JObject.Parse(jsonArray[0].ToString());        
        if(data2.data.validationCode!=null)
        {
            // return validation code
            return req.CreateResponse(HttpStatusCode.OK, new
                {
                    validationResponse = data2.data.validationCode
                });
        }
    }
    // -------------------
    
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