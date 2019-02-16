namespace CustomerPreferenceCentre.Domain

open System

//*************
// Domain Model
//*************

// Restrict `DayOfMonth` so it can only be accessed via functions in
// the module. That way we can ensure the day is valid for the domain
type DayOfMonth = private | DayOfMonth of int
module DayOfMonth =
    // Note for Reviewer: For simplicity sake in this technical exercise I just
    // throw an exception if the date is invalid rather than deal with Option
    // types. In real world code there would be a stricter validation phase
    let create d =
        // Requirement given in the technical exercise: "On a specified date of the month [1-28]"
        if d < 1 || d > 28 then
            failwithf "Day of Month must be specified between the 1st and the 28th"
        else DayOfMonth d
    let get (DayOfMonth d) = d

type MarketingPreference =
    | Never
    | EveryDay
    | DayOfTheMonth of DayOfMonth
    | DayOfTheWeek of Set<DayOfWeek>

type CustomerName = | CustomerName of string

type Customer =
    { Name: CustomerName
      MarketingPreference: MarketingPreference }

//***************************
// Customer Preference Logic
//***************************

module Report =

    let nextNintyDays (date: DateTime) =
      let days = { 0.0 .. 89.0 }
      days |> Seq.map (fun d -> (date.AddDays(d)))

    let canContactCustomerOnDate (date: DateTime) customer =
        match customer.MarketingPreference with
        | Never -> false
        | EveryDay -> true
        | DayOfTheWeek dowToContact -> dowToContact |> Set.contains date.DayOfWeek
        | DayOfTheMonth x -> DayOfMonth.get x = date.Day

    let getContactableForDate date customers =
        customers
        |> Set.filter (canContactCustomerOnDate date)

    let nintyDayPreferenceReport (date: DateTime) customers =
        let days = nextNintyDays date
        days
        |> Seq.map (fun d -> getContactableForDate d customers)
        |> Seq.zip days
