namespace CustomerPreferenceCentre.Domain

module Library =
    open System

    type MarketingPreference =
    | Never
    | EveryDay
    | DayOfTheMonth of int
    | DayOfTheWeek of DayOfWeek list

    type CustomerName = | CustomerName of string

    type Customer =
        { Name: CustomerName
          MarketingPreference: MarketingPreference }

    let nextNintyDays (date: DateTime) =
      let days = { 0.0 .. 89.0 }
      days |> Seq.map (fun d -> (date.AddDays(d)))

    let contactCustomerOnDate (date: DateTime) customer =
        match customer.MarketingPreference with
        | Never -> false
        | EveryDay -> true
        | DayOfTheWeek dowToContact -> dowToContact |> List.contains date.DayOfWeek
        | DayOfTheMonth x -> x = date.Day

    let customersToContactInNextNintyDays (date: DateTime) customers =
      let days = nextNintyDays date
      let customersToContact =
        days |> Seq.map (fun x -> customers |> Set.filter (contactCustomerOnDate x) )
      Seq.zip days customersToContact
