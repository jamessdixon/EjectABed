open System
open System.Collections.Generic
open System.Diagnostics
open System.Linq
open System.Net
open System.Threading
open Microsoft.WindowsAzure
open Tweetinvi.Core
open Tweetinvi.Core.Enum
open Tweetinvi.Core.Extensions
open Microsoft.WindowsAzure.Storage;
open Microsoft.WindowsAzure.Storage.Queue;

let storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ejectabed;AccountKey=HZMPnsiL0fzqJunxRswtw5DwQYaa2HRXePkFNg66y0TQanAIkLYGYW5TDoP/CClM1u2UDrp192dlcDoWcxdVbA=="
let consumerKey = "uMBReqftahU5gd9fukUeebWQn"
let consumerSecret = "FbbFsNcIAcMT6MnnkDBQoCp4V2XYj8YDpLAB20IE5TcDRWnsDs"
let accessToken = "165481117-Ukrou2BteS9MlJTPGVbrT8DKriw3QvofYB03wlG7"
let accessTokenSecret = "FvSX72o9tzqMJZjyUOFYic08UQFBUFCQEyn7UjE8fb6rg"

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

let writeTweet(queueName) =
    createQueue(queueName)
    writeToQueue(queueName)

let getKeywordFromTweet(tweet:string) =
    let keyword = "sloan"
    let hasKeyword = tweet.Contains(keyword)
    match hasKeyword with
    | true -> Some keyword
    | false -> None

[<EntryPoint>]
let main argv = 
    Console.WriteLine("...Getting Tweets...")
    let creds = Credentials.TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret)
    Tweetinvi.Auth.SetCredentials(creds)
    let matchingTweets = Tweetinvi.Search.SearchTweets("@ejectabed")
    matchingTweets |> Seq.iter(fun t -> Console.WriteLine(t.Text))
    Console.WriteLine("...Parsing...")
    matchingTweets |> Seq.map(fun t -> getKeywordFromTweet(t.Text))
                   |> Seq.filter(fun t -> t.IsSome)
                   |> Seq.iter(fun w -> writeTweet(w.Value))        
                   //|> Seq.iter(fun t -> Console.WriteLine(t.Value))
    Console.WriteLine("Wrote To Queue")
    Console.ReadKey() |> ignore
    0



