module ScenarioTest

open Expecto
open Expecto.Flip
open System
open CustomerPreferenceCentre.Domain.Library

[<Tests>]
let tests =
    testList "Scenario" (
        // For example, Customer A chooses 'Every day'. Customer B chooses 'On the 10th of the
        // month'. Customer C chooses ‘On Tuesday and Friday’.
        let customerA = { Name = CustomerName "A"; MarketingPreference = EveryDay }
        let customerB = { Name = CustomerName "B"; MarketingPreference = DayOfTheMonth (DayOfMonth.create 10) }
        let customerC =
            { Name = CustomerName "C"
              MarketingPreference = DayOfTheWeek (Set [ DayOfWeek.Tuesday; DayOfWeek.Friday ]) }

        let result = customersToContactInNextNintyDays (DateTime(2018, 04, 01))
                                                        (Set [customerA; customerB; customerC])
        [
        testCase "should return abridged output" <| fun _ ->
            // After providing this input the abridged output beginning in April would be:
            result
            |> List.ofSeq
            |> List.take 14
            |> Expect.equal "" [ (DateTime(2018, 04, 01)), Set [ customerA ]
                                 (DateTime(2018, 04, 02)), Set [ customerA ]
                                 (DateTime(2018, 04, 03)), Set [ customerA; customerC ]
                                 (DateTime(2018, 04, 04)), Set [ customerA ]
                                 (DateTime(2018, 04, 05)), Set [ customerA ]
                                 (DateTime(2018, 04, 06)), Set [ customerA; customerC ]
                                 (DateTime(2018, 04, 07)), Set [ customerA ]
                                 (DateTime(2018, 04, 08)), Set [ customerA ]
                                 (DateTime(2018, 04, 09)), Set [ customerA ]
                                 (DateTime(2018, 04, 10)), Set [ customerA; customerB; customerC ]
                                 (DateTime(2018, 04, 11)), Set [ customerA ]
                                 (DateTime(2018, 04, 12)), Set [ customerA ]
                                 (DateTime(2018, 04, 13)), Set [ customerA; customerC ]
                                 (DateTime(2018, 04, 14)), Set [ customerA ] ]

        testCase "should include customer A for each day" <| fun _ ->
            result |> Expect.all "" (fun (_, custs) -> custs |> Set.contains customerA)

        testCase "should include customer B on the 10th of each month" <| fun _ ->
            result
            |> Seq.filter (fun (d, _) -> d.Day = 10)
            |> Seq.map (fun (_, custs) -> custs |> Seq.tryFind (fun x -> x = customerB))
            |> Expect.all "" (fun x -> x = Some customerB)

        testCase "should include customer B exactly 3 times" <| fun _ ->
            result
            |> Seq.filter (fun (_, custs) -> custs |> Set.contains customerB)
            |> Seq.length
            |> Expect.equal "" 3

        testCase "should include customer C on Tuesdays and Thursdays" <| fun _ ->
            result
            |> Seq.filter (fun (d, _) -> d.DayOfWeek = DayOfWeek.Tuesday || d.DayOfWeek = DayOfWeek.Friday)
            |> Seq.map (fun (_, custs) -> custs |> Seq.tryFind (fun x -> x = customerC))
            |> Expect.all "" (fun x -> x = Some customerC)

        testCase "should NOT include customer C on other days" <| fun _ ->
            result
            |> Seq.filter (fun (d, _) -> d.DayOfWeek <> DayOfWeek.Tuesday && d.DayOfWeek <> DayOfWeek.Friday)
            |> Seq.map (fun (_, custs) -> custs |> Seq.tryFind (fun x -> x = customerC))
            |> Expect.all "" (fun x -> x = None)
    ])
