module PreferencesTest

open Expecto
open Expecto.Flip
open System
open CustomerPreferenceCentre.Domain
open CustomerPreferenceCentre.Domain.Report

// Note for Reviewer:
// Time permitting I would have liked to have used fscheck here
[<Tests>]
let tests =
    testList "MarketingPreferences contactCustomer" [
        testList "when Never" [
            testCase "should never contact customer" <| fun _ ->
                let customer = { Name = CustomerName "A"; MarketingPreference = Never}
                canContactCustomerOnDate (DateTime(2018,12,01)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2018,01,16)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2018,02,23)) customer |> Expect.isFalse ""
        ]
        testList "when EveryDay" [
            testCase "should always contact customer" <| fun _ ->
                let customer = { Name = CustomerName "A"; MarketingPreference = EveryDay}
                canContactCustomerOnDate (DateTime(2018,12,01)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2018,01,16)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2018,02,23)) customer |> Expect.isTrue ""
        ]
        testList "when DayOfTheWeek" [
            testCase "should contact customer on a single day" <| fun _ ->
                let customer =
                    { Name = CustomerName "A"
                      MarketingPreference = DayOfTheWeek (Set [ DayOfWeek.Monday ]) }
                canContactCustomerOnDate (DateTime(2019,02,11)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,12)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,13)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,14)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,15)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,16)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,17)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,18)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,19)) customer |> Expect.isFalse ""

            testCase "should contact customer on multiple days" <| fun _ ->
                let customer =
                    { Name = CustomerName "A"
                      MarketingPreference = DayOfTheWeek (Set [ DayOfWeek.Monday
                                                                DayOfWeek.Tuesday
                                                                DayOfWeek.Wednesday ]) }
                canContactCustomerOnDate (DateTime(2019,02,11)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,12)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,13)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,14)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,15)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,16)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,17)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,18)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,19)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,20)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,21)) customer |> Expect.isFalse ""

            testCase "should contact customer on all days" <| fun _ ->
                let customer =
                    { Name = CustomerName "A"
                      MarketingPreference = DayOfTheWeek (Set [ DayOfWeek.Monday
                                                                DayOfWeek.Tuesday
                                                                DayOfWeek.Wednesday
                                                                DayOfWeek.Thursday
                                                                DayOfWeek.Friday
                                                                DayOfWeek.Saturday
                                                                DayOfWeek.Sunday ]) }
                canContactCustomerOnDate (DateTime(2019,02,11)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,12)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,13)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,14)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,15)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,16)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,02,17)) customer |> Expect.isTrue ""
        ]
        testList "when DayOfTheMonth" [
            testCase "should contact customer on a specified day of the month" <| fun _ ->
                let customer =
                    { Name = CustomerName "A"
                      MarketingPreference = DayOfTheMonth (DayOfMonth.create 28)  }
                canContactCustomerOnDate (DateTime(2019,01,27)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,01,28)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,01,29)) customer |> Expect.isFalse ""

                canContactCustomerOnDate (DateTime(2019,02,27)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,02,28)) customer |> Expect.isTrue ""

                canContactCustomerOnDate (DateTime(2019,01,27)) customer |> Expect.isFalse ""
                canContactCustomerOnDate (DateTime(2019,01,28)) customer |> Expect.isTrue ""
                canContactCustomerOnDate (DateTime(2019,01,29)) customer |> Expect.isFalse ""
        ]
    ]
