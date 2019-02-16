module CustomerPreferenceCenter.Test.Functional.WebServerReportTest

open Expecto
open Expecto.Flip
open System
open FSharp.Data
open CustomerPreferenceCentre.Domain
open CustomerPreferenceCentre.Domain.RequestDto

[<Tests>]
let tests =
    testList "Scenario - nintyDayPreferenceReport" (
        TestServer.ensureRunning |> ignore
        [
        testCase "should return abridged output" <| fun _ ->
            // For example, Customer A chooses 'Every day'. Customer B chooses 'On the 10th of the
            // month'. Customer C chooses ‘On Tuesday and Friday’.
            let customerA = { name = "A"; marketingPreference = EveryDay }
            let customerB = { name = "B"; marketingPreference = DayOfTheMonth 10 }
            let customerC = { name = "C"; marketingPreference = DayOfTheWeek [ "Tuesday"; "Friday" ] }

            let json = [ customerA; customerB; customerC ] |> JsonSerializer.serialize
            let resultJson =
              Http.RequestString(
                  sprintf "http://localhost:18181/ninty-day-preference-report/%s" "2018-04-01",
                  httpMethod = "POST",
                  headers = [
                    HttpRequestHeaders.ContentType HttpContentTypes.Json
                    HttpRequestHeaders.Accept HttpContentTypes.Json
                  ],
                  body = TextRequest json
                )
            let resultDto : ResponseDto.ReportDto = resultJson |> JsonSerializer.deserialize

            // After providing this input the abridged output beginning in April would be:
            resultDto
            |> List.take 14 // to match the Backend Role Technical Exercise
            |> Expect.equal "" [ { date = "2018-04-01"; customers = [ "A" ] }
                                 { date = "2018-04-02"; customers = [ "A" ] }
                                 { date = "2018-04-03"; customers = [ "A"; "C" ] }
                                 { date = "2018-04-04"; customers = [ "A" ] }
                                 { date = "2018-04-05"; customers = [ "A" ] }
                                 { date = "2018-04-06"; customers = [ "A"; "C" ] }
                                 { date = "2018-04-07"; customers = [ "A" ] }
                                 { date = "2018-04-08"; customers = [ "A" ] }
                                 { date = "2018-04-09"; customers = [ "A" ] }
                                 { date = "2018-04-10"; customers = [ "A"; "B"; "C" ] }
                                 { date = "2018-04-11"; customers = [ "A" ] }
                                 { date = "2018-04-12"; customers = [ "A" ] }
                                 { date = "2018-04-13"; customers = [ "A"; "C" ] }
                                 { date = "2018-04-14"; customers = [ "A" ] } ]
        ])
