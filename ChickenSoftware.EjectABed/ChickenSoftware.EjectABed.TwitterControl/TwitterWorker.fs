namespace ChickenSoftware.EjectABed.TwitterControl

open System
open System.Collections.Generic
open System.Diagnostics
open System.Linq
open System.Net
open System.Threading
open Microsoft.WindowsAzure
open Microsoft.WindowsAzure.Diagnostics
open Microsoft.WindowsAzure.ServiceRuntime
open Tweetinvi.Core
open Tweetinvi.Core.Enum
open Tweetinvi.Core.Extensions
open Tweetinvi.Core.Interfaces
open Tweetinvi.Core.Interfaces.Controllers
open Tweetinvi.Core.Interfaces.DTO
open Tweetinvi.Core.Interfaces.Models
open Tweetinvi.Core.Interfaces.Models.Entities
open Tweetinvi.Core.Interfaces.Streaminvi
open Tweetinvi.Core.Interfaces.WebLogic
open Tweetinvi.Json
open Microsoft.WindowsAzure.Storage;
open Microsoft.WindowsAzure.Storage.Queue;

type TwitterWorker() =
    inherit RoleEntryPoint() 

    let storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=HZMPnsiL0fzqJunxRswtw5DwQYaa2HRXePkFNg66y0TQanAIkLYGYW5TDoP/CClM1u2UDrp192dlcDoWcxdVbA==";

    let CreateQueue(queueName) =
        let storageAccount = CloudStorageAccount.Parse(storageConnectionString)
        let client = storageAccount.CreateCloudQueueClient()
        let queue = client.GetQueueReference(queueName);
        queue.CreateIfNotExists() |> ignore

    let WriteToQueue(queueName) =
        let storageAccount = CloudStorageAccount.Parse(storageConnectionString)
        let client = storageAccount.CreateCloudQueueClient()
        let queue = client.GetQueueReference(queueName)
        let message = new CloudQueueMessage("Eject!")
        queue.AddMessage(message) |> ignore

    let WriteTweet(queueName) =
        CreateQueue(queueName)
        WriteToQueue(queueName)

    let GetFirstWordFromTweet(tweet:string) =
        let firstSpace = tweet.IndexOf(" ")
        match firstSpace with
        | -1 -> tweet.Trim()
        | _ -> tweet.Substring(0,firstSpace).Trim()

    override this.Run() =
        while(true) do
            let consumerKey = "uMBReqftahU5gd9fukUeebWQn"
            let consumerSecret = "FbbFsNcIAcMT6MnnkDBQoCp4V2XYj8YDpLAB20IE5TcDRWnsDs" 
            let accessToken = "165481117-Ukrou2BteS9MlJTPGVbrT8DKriw3QvofYB03wlG7"
            let accessTokenSecret = "FvSX72o9tzqMJZjyUOFYic08UQFBUFCQEyn7UjE8fb6rg"
            let creds = Credentials.TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret)
            Tweetinvi.Auth.SetCredentials(creds)
            let matchingTweets = Tweetinvi.Search.SearchTweets("@ejectabed")
            matchingTweets |> Seq.map(fun t -> GetFirstWordFromTweet(t.Text))
                           |> Seq.iter(fun w -> WriteTweet(w))
            Thread.Sleep(15000)

    override this.OnStart() = 
        ServicePointManager.DefaultConnectionLimit <- 12
        base.OnStart()
