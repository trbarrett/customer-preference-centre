module SerializationTest

open Expecto
open Expecto.Flip
open System
open CustomerPreferenceCentre.Domain

let customerA = { Name = CustomerName "A"; MarketingPreference = EveryDay }
let customerB = { Name = CustomerName "B"; MarketingPreference = DayOfTheMonth (DayOfMonth.create 10) }
let customerC =
    { Name = CustomerName "C"
      MarketingPreference = DayOfTheWeek (Set [ DayOfWeek.Tuesday; DayOfWeek.Friday ]) }
let customerD = { Name = CustomerName "D"; MarketingPreference = Never }

// Note for Reviewer:
// Time permitting I would have liked to have used fscheck here
[<Tests>]
let tests =
    testList "serialization" [
        testList "DomainToResponseDto" [
            testCase "should convert date and customers into nice json output" <| fun _ ->
                seq { yield DateTime(2018, 04, 09), Set [ customerA ]
                      yield DateTime(2018, 04, 10), Set [ customerA; customerB; customerC ]
                      yield DateTime(2018, 04, 11), Set [ customerA ]
                      yield DateTime(2018, 04, 12), Set [ customerA ]
                      yield DateTime(2018, 04, 13), Set [ customerA; customerC ]
                }
                |> DomainToResponseDto.convertReportToResponseDto
                |> JsonSerializer.serialize
                |> Expect.equal "" (
                    """[{"date":"2018-04-09","customers":["A"]},""" +
                    """{"date":"2018-04-10","customers":["A","B","C"]},""" +
                    """{"date":"2018-04-11","customers":["A"]},""" +
                    """{"date":"2018-04-12","customers":["A"]},""" +
                    """{"date":"2018-04-13","customers":["A","C"]}]""")
        ]

        testList "RequestDtoToDomain" [
            testCase "should convert request json to customers for report" <| fun _ ->
                """[{"customerName":"A","marketingPreference":"EveryDay"},""" +
                """{"customerName":"B","marketingPreference":{"DayOfTheMonth":10}},""" +
                """{"customerName":"C","marketingPreference":{"DayOfTheWeek":["Tuesday","Friday"]}},""" +
                """{"customerName":"D","marketingPreference":"Never"}]"""
                |> JsonSerializer.deserialize
                |> RequestDtoToDomain.convertReportRequest
                |> Expect.equal "" (Set [ customerA; customerB; customerC; customerD])
        ]
    ]
