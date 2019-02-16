module CustomerPreferenceCentre.Suave.Server

open Suave
open System.Threading
open System
open CustomerPreferenceCentre.Suave.Core

[<EntryPoint>]
let main _ =
    let cts = new CancellationTokenSource()
    let conf = { defaultConfig with cancellationToken = cts.Token }
    let _, server = startWebServerAsync conf Routing.app

    Async.Start(server, cts.Token)
    printfn "Make requests now"
    Console.ReadKey true |> ignore

    cts.Cancel()

    0 // return an integer exit code
