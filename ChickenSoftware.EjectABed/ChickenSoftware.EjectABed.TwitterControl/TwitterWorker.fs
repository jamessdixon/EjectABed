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

    let storageConnectionString = RoleEnvironment.GetConfigurationSettingValue("storageConnectionString")

    let createQueue(queueName) =
        let storageAccount = CloudStorageAccount.Parse(storageConnectionString)
        let client = storageAccount.CreateCloudQueueClient()
        let queue = client.GetQueueReference(queueName);
        queue.CreateIfNotExists() |> ignore

    let writeToQueue(queueName) =
        let storageAccount = CloudStorageAccount.Parse(storageConnectionString)
        let client = storageAccount.CreateCloudQueueClient()
        let queue = client.GetQueueReference(queueName)
        let message = new CloudQueueMessage("Eject!")
        queue.AddMessage(message) |> ignore

    let writeTweetToQueue(queueName) =
        createQueue(queueName)
        writeToQueue(queueName)

    let getKeywordFromTweet(tweet: ITweet) = 
        let keyword = "sloan"
        let hasKeyword = tweet.Text.Contains(keyword)
        let isFavourited = tweet.FavouriteCount > 0
        match hasKeyword, isFavourited  with
        | true,false -> Some (keyword,tweet)
        | _,_ -> None
        

    override this.Run() =
        while(true) do
            let consumerKey = RoleEnvironment.GetConfigurationSettingValue("consumerKey")
            let consumerSecret = RoleEnvironment.GetConfigurationSettingValue("consumerSecret")
            let accessToken = RoleEnvironment.GetConfigurationSettingValue("accessToken")
            let accessTokenSecret = RoleEnvironment.GetConfigurationSettingValue("accessTokenSecret")

            let creds = Credentials.TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret)
            Tweetinvi.Auth.SetCredentials(creds)
            let matchingTweets = Tweetinvi.Search.SearchTweets("@ejectabed")
            let matchingTweets' =  matchingTweets |> Seq.map(fun t -> getKeywordFromTweet(t))
                                                  |> Seq.filter(fun t -> t.IsSome)
                                                  |> Seq.map (fun t -> t.Value)
            matchingTweets' |> Seq.iter(fun (k,t) -> writeTweetToQueue(k))        
            matchingTweets' |> Seq.iter(fun (k,t) -> t.Favourite())        

            Thread.Sleep(15000)

    override this.OnStart() = 
        ServicePointManager.DefaultConnectionLimit <- 12
        base.OnStart()
