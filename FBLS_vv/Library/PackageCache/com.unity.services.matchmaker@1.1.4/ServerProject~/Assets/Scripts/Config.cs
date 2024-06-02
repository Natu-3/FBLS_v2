using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Config represents the game server configuration.
/// </summary>
public class Config 
{
    // AllocatedUUID is the allocation ID provided to an event.
    [JsonProperty("allocatedUUID")]
    public string AllocatedUuid;

    // ReadBuffer is the size of the UDP connection read buffer.
    [JsonProperty("readBuffer")]
    public int ReadBuffer;

    // WriteBuffer is the size of the UDP connection write buffer.
    [JsonProperty("writeBuffer")]
    public int WriteBuffer;

    // MaxPlayers is the default value to report for max players.
    [JsonProperty("maxPlayers")]
    public uint MaxPlayers;

    // Map is the default value to report for map.
    [JsonProperty("map")]
    public string Map;

    // GameType is the default value to report for gametype.
    [JsonProperty("gameType")]
    public string GameType;

    // MatchmakerURL is the public domain name used for approving backfill tickets.
    [JsonProperty("matchmakerUrl")]
    public string MatchmakerURL;

    // PayloadProxyURL is the url for the payload proxy which is used to retrieve the token.
    [JsonProperty("payloadProxyUrl")]
    public string PayloadProxyUrl;

    // EnableBackfill enables backfill during the game loop.
    [JsonProperty("enableBackfill")]
    public bool EnableBackfill;

    // QueryType determines the protocol used for query responses.
    [JsonProperty("queryType")]
    public string QueryType;

    public static Config ActiveConfig = null;

    private Config() 
    {
    }

    public static Config NewConfigFromFile(string configFilePath) 
    {
        Config config;
        Config defaultConfig = GetDefault();

        try
        {
            config = FromJson(configFilePath);

            if (config.ReadBuffer == 0)
            {
                config.ReadBuffer = defaultConfig.ReadBuffer;
            }

            if (config.WriteBuffer == 0)
            {
                config.WriteBuffer = defaultConfig.WriteBuffer;
            }

            if (config.GameType == null) 
            {
                config.GameType = defaultConfig.GameType;
            }

            if (config.Map == null) 
            {
                config.Map = defaultConfig.Map;
            }

            if (config.MaxPlayers == 0) 
            {
                config.MaxPlayers = defaultConfig.MaxPlayers;
            }

            //Force backfill since we're not getting it from the json file.
            config.EnableBackfill = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception encountered when attempting to read config file. Exception type: {e.GetType()}");
            config = new Config();
        }

        ActiveConfig = config;
        return config;
    }

    public static Config GetDefault() 
    {
        return new Config 
        {
            ReadBuffer = 40960,
            WriteBuffer = 40960,
            MaxPlayers = 4,
            Map = "Sample Map",
            GameType = "Sample Game",
            QueryType = "sqp",
            MatchmakerURL = "https://matchmaker.services.api.unity.com",
            PayloadProxyUrl = "http://localhost:8086",
            EnableBackfill = true
        };
    }

    private static Config FromJson(string configFilePath) 
    {
#if UNITY_EDITOR
        configFilePath = Application.dataPath + "/" + configFilePath;
#endif
        using (StreamReader reader = new StreamReader(configFilePath)) 
        {
            string json = reader.ReadToEnd();

            Debug.Log("Loading config file..");
            Debug.Log(json);

            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
