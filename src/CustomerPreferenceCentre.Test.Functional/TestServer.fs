namespace CustomerPreferenceCenter.Test.Functional

open Suave
open System.Threading
open System
open CustomerPreferenceCentre.Suave.Core

// Start a test server to test with

// I'm using this unusual pattern to ensure the server is always runnning at the start of
// the tests. This gets around an issue I found with running tests in the visual studio
// test runner with "Expecto.VisualStudio.TestAdapter". I initially had the test server
// started in the console main method, which works fine from the CLI. But the test runner
// wasn't calling that when running inside VS. So I created this work around.
type TestServer private() =
    static let mutable Server = None
    static let locker = Object()
    static let testPort = 18181us

    static member ensureRunning =
        match Server with
        | Some x -> x
        | None ->
            // Need a lock so we don't start multiple servers when running tests in parallel
            lock locker (fun () ->
                let cts = new CancellationTokenSource()
                let conf =
                    { defaultConfig
                      with
                        cancellationToken = cts.Token
                        bindings = [ HttpBinding.create HTTP Net.IPAddress.Loopback testPort ]
                    }
                let listening, server = startWebServerAsync conf Routing.app

                Async.Start(server, cts.Token)

                async {
                    let! started = listening
                    started |> ignore
                } |> Async.RunSynchronously

                Server <- Some cts
                cts
            )
