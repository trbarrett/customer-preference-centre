module CustomerPreferenceCentre.Suave.Core.Api

open System
open Suave.Successful
open CustomerPreferenceCentre.Domain

let getNintyDayPreferenceReport _ =
    let customerA = { Name = CustomerName "A"; MarketingPreference = EveryDay }
    let customerB = { Name = CustomerName "B"; MarketingPreference = DayOfTheMonth (DayOfMonth.create 10) }
    let customerC =
        { Name = CustomerName "C"
          MarketingPreference = DayOfTheWeek (Set [ DayOfWeek.Tuesday; DayOfWeek.Friday ]) }

    let customers = Set [ customerA; customerB; customerC ]

    Report.nintyDayPreferenceReport (DateTime.Today) customers
    |> DomainToResponseDto.convertReportToResponseDto
    |> JsonSerializer.serialize
    |> OK
