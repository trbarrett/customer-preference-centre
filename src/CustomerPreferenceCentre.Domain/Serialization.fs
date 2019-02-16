namespace CustomerPreferenceCentre.Domain

// Note for reviewer:
// Generally I prefer to put serialization in a separate project when it becomes
// complex enough, but to keep things consise I'm putting it all with the domain.
//
// I've gone for a fairly simple and dumb serialization process:
// I let Netonsoft.Json & Microsoft.FSharpLu.Json do the heavy lifting, and have
// a small bit of code to do the conversion to and from simplified dtos

open Newtonsoft.Json
open Microsoft.FSharpLu.Json

type JsonSettings =
    static member settings =
        let s =
            JsonSerializerSettings(
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Error)
        s.Converters.Add(CompactUnionJsonConverter())
        s
    static member formatting = Formatting.None

type JsonSerializer = With<JsonSettings>

module ResponseDto =

    type DatePreferenceDto =
        { date: string
          customers: string list }

    type ReportDto = DatePreferenceDto list

module DomainToResponseDto =
    open System
    open ResponseDto

    let convertCustomersToNames (customers: Set<Customer>) =
        customers
        |> Set.map (fun c ->
            let (CustomerName name) = c.Name
            name )
        |> Set.toList
        |> List.sort // ensure the customer list is consistently ordered

    let convertReportToResponseDto (preferenceList: (DateTime * Set<Customer>) seq) =
        preferenceList
        |> Seq.map (fun (d, customers) ->
            { date = d.ToString("yyyy-MM-dd")
              customers = convertCustomersToNames customers } )

module RequestDto =

    type MarketingPreferenceDto =
        | Never
        | EveryDay
        | DayOfTheMonth of int
        | DayOfTheWeek of string list

    type CustomerDto =
        { name: string
          marketingPreference:  MarketingPreferenceDto }

    type ReportRequestDto = CustomerDto list

module RequestDtoToDomain =
    open RequestDto
    open System

    let convertMarketingPreference (marketingPreference: MarketingPreferenceDto) =
        match marketingPreference with
        | Never -> MarketingPreference.Never
        | EveryDay -> MarketingPreference.EveryDay
        | DayOfTheMonth d ->
            // Note for reviewer: I've skipped validation here for brevity, in a
            // real-world system we would want to ensure the input is valid
            DayOfMonth.create d
            |> MarketingPreference.DayOfTheMonth
        | DayOfTheWeek days ->
            // Note for reviewer: I've skipped validation here for brevity, in a
            // real-world system we would want to ensure the input is valid
            days
            |> List.map (DayOfWeek.Parse)
            |> Set
            |> MarketingPreference.DayOfTheWeek

    let convertCustomerDto (customer: CustomerDto) =
        { Name = CustomerName customer.name
          MarketingPreference =
            convertMarketingPreference customer.marketingPreference }

    let convertReportRequest (requestDto: ReportRequestDto) =
        requestDto |> List.map convertCustomerDto
