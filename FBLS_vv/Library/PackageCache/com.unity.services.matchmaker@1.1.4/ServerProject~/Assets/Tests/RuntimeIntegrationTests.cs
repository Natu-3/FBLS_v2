using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace Testing
{
    /// <summary>
    /// Runtime Test fixture for Matchmaker Backfill APIs
    /// </summary>
    [RemoteTestFixture]
    public class MatchmakerBackfillIntegrationTests
    {
        enum BackfillApi
        {
            ApproveTicket,
            CreateTicket,
            DeleteTicket,
            UpdateTicket
        }

        static readonly TimeSpan k_BackfillApiRateLimit = TimeSpan.FromSeconds(1);
        static readonly Dictionary<BackfillApi, TimeSpan> k_BackfillRateLimits = new Dictionary<BackfillApi, TimeSpan>
        {
            [BackfillApi.ApproveTicket] = k_BackfillApiRateLimit,
            [BackfillApi.CreateTicket] = k_BackfillApiRateLimit,
            [BackfillApi.DeleteTicket] = k_BackfillApiRateLimit,
            [BackfillApi.UpdateTicket] = k_BackfillApiRateLimit
        };

        const string k_TargetQueueWithBackfill = "integration-queue-backfill";
        const string k_PayloadProxyUrl = "http://localhost:8086";

        string backfillTicketId = "";
        string connection = "";
        bool runAllInProgress = false;

        public MatchmakerBackfillIntegrationTests()
        {
            RateLimitUtility<BackfillApi>.SetRateLimits(k_BackfillRateLimits);
        }

        public void Example()
        {
            Debug.Log("Example");
        }

        public async Task ExampleAsync()
        {
            await Task.Delay(1000);
            Debug.Log("ExampleAsync");
        }

        public void ExampleExceptionThrow()
        {
            throw new InvalidOperationException("ExampleExceptionThrow");
        }

        public async Task RunAllTests() 
        {
            runAllInProgress = true;

            //Setup step (as the initial backfillTicketId now needs to be retrieved from payload)
            var payload = await GetMatchmakerAllocationPayloadAsync(Config.ActiveConfig.AllocatedUuid);
            backfillTicketId = payload.BackfillTicketId;

            await ApproveBackfillTicket_ReturnsValidTicket();
            await UpdateBackfillTicket_AddNewPlayer_DoesNotThrow();
            await DeleteBackfillTicket_DoesNotThrow();
            await CreateBackfillTicket_DoesNotThrow();
            runAllInProgress = false;
        }

        /// <summary>
        /// This should be in the SDK but we can use web-requests to get access to the MatchmakerAllocationPayload
        /// </summary>
        /// <param name="allocationID"></param>
        /// <returns></returns>
        private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync(string allocationID)
        {
            Debug.Log($"Getting Allocation Payload with ID: {allocationID}");
            var payloadUrl = k_PayloadProxyUrl + $"/payload/{allocationID}";
            using var webRequest = UnityWebRequest.Get(payloadUrl);
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Delay(50);
            }

            Debug.Log($"Web Request Text:{operation.webRequest.downloadHandler.text}");

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError(nameof(GetMatchmakerAllocationPayloadAsync) + ": ConnectionError: " +
                        webRequest.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(nameof(GetMatchmakerAllocationPayloadAsync) + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(nameof(GetMatchmakerAllocationPayloadAsync) + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(nameof(GetMatchmakerAllocationPayloadAsync) + ":\nReceived: " +
                        webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.InProgress:
                    break;
            }

            try
            {
                return JsonConvert.DeserializeObject<MatchmakingResults>(webRequest.downloadHandler.text);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Something went wrong deserializing the Allocation Payload:\n{exception}");
                return null;
            }
        }

        public async Task ApproveBackfillTicket_ReturnsValidTicket() 
        {
            Debug.Log("Sending approval to backfill endpoint..");
            Debug.Log("Backfill Ticket ID: " + backfillTicketId);
            Assert.IsNotNull(backfillTicketId);
            Assert.IsTrue(backfillTicketId.Length > 0);
            Assert.IsNotNull(MatchmakerService.Instance);

            BackfillTicket backfillTicket = await RateLimitUtility<BackfillApi>.CallApiWithRateLimit(BackfillApi.ApproveTicket, 
                () => MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId));
            var backfillTicketJson = JsonConvert.SerializeObject(backfillTicket, Formatting.Indented);
            connection = backfillTicket.Connection;

            Assert.IsNotNull(backfillTicket);
            Debug.Log("Backfill Ticket Approved. Ticket Json: " + Environment.NewLine + backfillTicketJson);
        }

        public async Task UpdateBackfillTicket_AddNewPlayer_DoesNotThrow() 
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(backfillTicketId), 
                "Backfill Ticket has been deleted prior to running this test!");

            BackfillTicket backfillTicket = null;
            //Approve the ticket first to get the BackfillTicket properties
            backfillTicket = await GetActiveBackfillTicket();
            Assert.IsNotNull(backfillTicket);

            //Edit the recieved ticket with an additional player
            string newPlayerId = $"ExtraPlayer-{new Guid()}";
            var extraPlayer = new Player(newPlayerId, new { });
            var players = new List<Player>();

            Assert.IsNotNull(backfillTicket?.Properties?.MatchProperties?.Players);
            players.AddRange(backfillTicket.Properties.MatchProperties.Players);
            players.Add(extraPlayer);
            var matchProperties = backfillTicket.Properties.MatchProperties;
            Assert.IsTrue(matchProperties?.Teams != null && matchProperties.Teams.Count > 0);

            matchProperties.Teams.First().PlayerIds.Add(extraPlayer.Id);
            backfillTicket.Properties = new BackfillTicketProperties(matchProperties);

            Debug.Log("Updating ticket with additional parameters..");
            await RateLimitUtility<BackfillApi>.CallApiWithRateLimit(BackfillApi.UpdateTicket,
                () => MatchmakerService.Instance.UpdateBackfillTicketAsync(backfillTicketId, backfillTicket));

            Debug.Log($"Sleeping for {5}s..");
            await Task.Delay(TimeSpan.FromSeconds(5));

            //Approve ticket to validate the new player exists
            backfillTicket = await RateLimitUtility<BackfillApi>.CallApiWithRateLimit(BackfillApi.ApproveTicket,
                () => MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId));

            Assert.IsNotNull(backfillTicket?.Properties?.MatchProperties?.Players);
            Assert.IsNotNull(backfillTicket.Properties.MatchProperties.Players.FirstOrDefault(x => x.Id == newPlayerId));
        }

        public async Task DeleteBackfillTicket_DoesNotThrow() 
        {
            Debug.Log("Deleting backfill ticket..");
            await RateLimitUtility<BackfillApi>.CallApiWithRateLimit(BackfillApi.DeleteTicket,
                () => MatchmakerService.Instance.DeleteBackfillTicketAsync(backfillTicketId));
        }

        public async Task CreateBackfillTicket_DoesNotThrow() 
        {
            //We provide a dummy set of parameters here, disregarding the actual number of players
            // as it's not relevant to the passing of this test or any related tests (as of time of writing)
            var teams = new List<Team>{
                new Team( "Red", "9c8e302e-9cf3-4ad6-a005-b2604e6851e3", new List<string>{ "c9e6857b-a810-488f-bacc-08d18d253b0a"  } ),
                new Team( "Blue", "e2d8f4fd-5db8-4153-bca7-72dfc9b2ac09", new List<string>{ "fe1a52cd-535a-4e34-bd24-d6db489eaa19"  } ),
            };
            var players = new List<Player>  
            {
                new Player("c9e6857b-a810-488f-bacc-08d18d253b0a", new { }),
                new Player("fe1a52cd-535a-4e34-bd24-d6db489eaa19", new { })
            };

            var matchProperties = new MatchProperties(teams, players);
            var backfillTicketProperties = new BackfillTicketProperties(matchProperties);
            var options = new CreateBackfillTicketOptions(k_TargetQueueWithBackfill, connection, new Dictionary<string, object>(), backfillTicketProperties);

            backfillTicketId = await RateLimitUtility<BackfillApi>.CallApiWithRateLimit(BackfillApi.CreateTicket,
                () => MatchmakerService.Instance.CreateBackfillTicketAsync(options));

            Assert.IsTrue(!string.IsNullOrWhiteSpace(backfillTicketId));
        }

        //Utility method ensures we have a valid backfill ticket id in cases where the default may have been deleted
        // e.g. by a deletion test
        private async Task<BackfillTicket> GetActiveBackfillTicket() 
        {
            BackfillTicket backfillTicket = await RateLimitUtility<BackfillApi>.CallApiWithRateLimit(BackfillApi.ApproveTicket,
                () => MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId));
            connection = backfillTicket.Connection;
            return backfillTicket;
        }
    }
}
