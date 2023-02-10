using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DurableTask.Core;

namespace DurableFunctionHook
{
    public static class Hook
    {
        static string _sitePoolJobInstanceId = "SitePoolJobInstance";
        const string _uat_us_uniprov_2_taskhub = "useueyimydazf01uniprov2";

        //[FunctionName("Hook")]
        //public static async Task<List<string>> RunOrchestrator(
        //    [OrchestrationTrigger] IDurableOrchestrationContext context)
        //{
        //    var outputs = new List<string>();

        //    // Replace "hello" with the name of your Durable Activity Function.
        //    outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
        //    outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
        //    outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

        //    // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        //    return outputs;
        //}

        //[FunctionName(nameof(SayHello))]
        //public static string SayHello([ActivityTrigger] string name, ILogger log)
        //{
        //    log.LogInformation($"Saying hello to {name}.");
        //    return $"Hello {name}!";
        //}

        //[FunctionName("Hook_HttpStart")]
        //public static async Task<HttpResponseMessage> HttpStart(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
        //    [DurableClient(TaskHub = "useueyimydazf01uniprov2")] IDurableOrchestrationClient starter,
        //    ILogger log)
        //{
        //    // Function input comes from the request content.
        //    string instanceId = await starter.StartNewAsync("Hook", null);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    return starter.CreateCheckStatusResponse(req, instanceId);
        //}

        //[FunctionName(nameof(TerminateOrchestrator))]
        //public static async Task<List<string>> TerminateOrchestrator(
        //    [OrchestrationTrigger] IDurableOrchestrationContext context)
        //{
        //    var outputs = new List<string>();

        //    // Replace "hello" with the name of your Durable Activity Function.
        //    outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
        //    outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
        //    outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

        //    // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        //    return outputs;
        //}

        //[FunctionName(nameof(TerminateQuery))]
        //public static async Task TerminateQuery([ActivityTrigger] [DurableClient(TaskHub = "euwueyimydazf01uniprov2")] IDurableOrchestrationClient client, ILogger log)
        //{
        //    var ids = await SqlClient.GetInstanceIdsToTerminate();
        //    foreach (var id in ids)
        //    { 

        //    }
        //}

        [FunctionName(nameof(GetStatus))]
        public static async Task<HttpResponseMessage> GetStatus(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient(TaskHub = _uat_us_uniprov_2_taskhub)] IDurableOrchestrationClient client,
            ILogger log)
        {
            var instanceId = await req.Content.ReadAsStringAsync();
            var status = await client.GetStatusAsync(instanceId, true, true, true);
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(status));
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return response;
        }

        [FunctionName(nameof(Terminate))]
        public static async Task<HttpResponseMessage> Terminate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient(TaskHub = _uat_us_uniprov_2_taskhub)] IDurableOrchestrationClient client,
            ILogger log)
        {
            var instanceId = await req.Content.ReadAsStringAsync();
            await client.TerminateAsync(instanceId, "manual terminiation");
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            return response;
        }

        [FunctionName(nameof(Restart))]
        public static async Task<HttpResponseMessage> Restart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient(TaskHub = _uat_us_uniprov_2_taskhub)] IDurableOrchestrationClient client,
            ILogger log)
        {
            var instanceId = await req.Content.ReadAsStringAsync();
            await client.RestartAsync(instanceId, false);
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            return response;
        }

        //[FunctionName(nameof(TerminateOverQuery))]
        //public static async Task<HttpResponseMessage> TerminateOverQuery(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
        //    [DurableClient(TaskHub = "euwueyimydazf01uniprov2")] IDurableOrchestrationClient client,
        //    ILogger log)
        //{
        //    var ids = await SqlClient.GetInstanceIdsToTerminate();
        //    List<Task> threads = new();
        //    foreach (var id in ids)
        //    {
        //        //threads.Add(client.TerminateAsync(id, "SDK requests over limit"));
        //        await client.TerminateAsync(id, "SDK requests over limit batch 2");
        //    }
        //    //await Task.WhenAll(threads);
        //    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        //    return response;
        //}

        //[FunctionName(nameof(LocalPurgeInstanceHistoryJob))]
        //public static async Task LocalPurgeInstanceHistoryJob([DurableClient(TaskHub = "usepeyimydazf01uniprov2")] IDurableOrchestrationClient client, [TimerTrigger("0 */1 * * * *")] TimerInfo timer, ILogger log)
        //{
        //    var purgeHistoryState = new PurgeHistoryState();
        //    log.LogInformation("Starting purge job.");
        //    double minutesBack = 60 * 24 * 2;
        //    double intervalMinutes = .1;
        //    while (true)
        //    {
        //        var to = DateTime.UtcNow.AddMinutes(minutesBack * -1);
        //        var from = to.AddMinutes(intervalMinutes * -1);
        //        log.LogInformation("Purging from {from} to {to}.", from, to);
        //        var purgeHistoryResult = await client.PurgeInstanceHistoryAsync(
        //            from,
        //            to,
        //            new List<OrchestrationStatus>
        //            {
        //                    OrchestrationStatus.Completed,
        //                    OrchestrationStatus.Failed
        //            });
        //        log.LogInformation("Purged {purgeHistoryResult.InstancesDeleted}.", purgeHistoryResult.InstancesDeleted);
        //        if (purgeHistoryResult.InstancesDeleted > 0)
        //        {
        //            minutesBack += intervalMinutes;
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //}

        [FunctionName(nameof(Hook_HttpStart))]
        public static async Task<HttpResponseMessage> Hook_HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient(TaskHub = "SiteProvisioningTaskHub")] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string name = req.RequestUri.ParseQueryString()["name"];
            var jsonInput = System.IO.File.ReadAllText($"json\\{name}.json");
            var input = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(jsonInput);

            string instanceId = await starter.StartNewAsync(name, input);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}