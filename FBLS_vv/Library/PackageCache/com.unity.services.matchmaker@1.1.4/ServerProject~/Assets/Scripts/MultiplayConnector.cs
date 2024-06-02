using CommandLine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Ucg.Usqp;
using Newtonsoft.Json;
using System.Net;

/// <summary>
/// Simple Multiplay Server
/// N.B. Parameters should be configured with '--' for long-form on Multiplay build configuration.
/// </summary>
public class MultiplayConnector : MonoBehaviour
{
    const string multiplayServerIp = "0.0.0.0";
    const string localServerIp = "127.0.0.1";
    const int localPort = 46997;
    const int portDelta = 1;

    string logDirectory;
    Options parameters;
    LogHandler logHandler;
    TestServer game;
    Config config;
    UsqpServer queryServer;
    ServerInfo.Data sqpServerData;

    class Options 
    {
        [Option("port", Required = true, HelpText = "Bindable port value.")]
        public uint Port { get; set; }

        [Option("log", Required = true, HelpText = "Relative logs directory path.")]
        public string LogDirectory { get; set; }

        [Option("queryport", Required = false, HelpText = "Multiplay analytics query port.")]
        public uint QueryPort { get; set; }

        [Option("config", Required = false, HelpText = "Path to the config file to use.")]
        public string Config { get; set; }

        [Option("local", Required = false, HelpText = "If true, ignore requirement for allocationId and call allocate on init.")]
        public bool IsLocalServer { get; set; }
    }

    async void Start() 
    {
        await UnityServices.InitializeAsync();

        //Adding in authentication call as it will otherwise get stripped and cause runtime dependency errors.
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //var sdkConfiguration = (IMatchmakerSdkConfiguration)MatchmakerService.Instance;
        //sdkConfiguration.SetBasePath("https://matchmaker-stg.services.api.unity.com");

        //Set to a low value for low CPU usage on servers
        //https://support.unity.com/hc/en-us/articles/360000283043-Target-frame-rate-or-V-Blank-not-set-properly
        Application.targetFrameRate = 30;

        //Parse args
        List<string> args;
        ParseArgs(out args);
        if (parameters == null) 
        {
            throw new InvalidOperationException("Arguments to test server should not be empty!");
        }

        //Set logging directory
        logDirectory = Path.Combine(new[] { Directory.GetCurrentDirectory(), parameters.LogDirectory });
        Directory.CreateDirectory(logDirectory);
        logHandler = new LogHandler(logDirectory);

        //Print args
        string result = $"Args: {string.Join(", ", args)}";
        Debug.Log(result);

        //Messaging port declaration
        //N.B. Multiplay keeps 100 ports open by default, use delta to reserve within the range.
        uint messagingPort = parameters.IsLocalServer ? localPort : parameters.Port + portDelta;
        string serverIp = parameters.IsLocalServer ? localServerIp : multiplayServerIp;

        //Initalize and start SQP
        InitSqpData();
        StartSqp(serverIp, (int)parameters.Port, (int)parameters.QueryPort);

        //Start server w/ agent
        var testServer = GetComponent<TestServer>();
        testServer.Init(serverIp, messagingPort);
        testServer.StartAgent();
    }

    void Update()
    {
        // Update server
        queryServer?.Update();
    }

    void ParseArgs(out List<string> args) 
    {
#if UNITY_EDITOR
        args = new List<string>
        {
            "--port=9000",
            "--queryport=20000",
            "--log=logs",
            "--config=server.json",
            "--local=true",
        };
#else
        args = Environment.GetCommandLineArgs().ToList();
#endif

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                parameters = options;
            })
            .WithNotParsed(errors =>
            {
                Debug.Log("Example argument list: '--port=9000 --messagingport=9010 --queryport=20000 --log=../logs/' --config=config.json");
                Application.Quit(1);
            });
    }

    void InitSqpData() 
    {
        //Setup SQP from config
        config = Config.NewConfigFromFile(parameters.Config);
        sqpServerData = new ServerInfo.Data()
        {
            BuildId = "1",
            CurrentPlayers = 0,
            GameType = config.GameType,
            Map = config.Map,
            MaxPlayers = (ushort)config.MaxPlayers,
            Port = (ushort)parameters.Port,
            ServerName = "Multiplay-Unity Sample Server",
        };

        //Logging
        var jsonInitServerData = JsonConvert.SerializeObject(sqpServerData, Formatting.Indented);
        Debug.Log("SQP server initialized with:");
        Debug.Log(jsonInitServerData);
        var jsonConfig = JsonConvert.SerializeObject(config, Formatting.Indented);
        Debug.Log("Initial config:");
        Debug.Log(jsonConfig);
    }

    void StartSqp(string ip, int port, int sqpPort)
    {
        Debug.Log("SQP started");
        var parsedIP = IPAddress.Parse(ip);
        var endpoint = new IPEndPoint(parsedIP, sqpPort);
        queryServer = new UsqpServer(endpoint)
        {
            ServerInfoData = sqpServerData
        };
    }

    void OnDestroy()
    {
        queryServer?.Dispose();
    }
}
