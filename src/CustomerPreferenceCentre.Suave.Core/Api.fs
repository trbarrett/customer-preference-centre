﻿module CustomerPreferenceCentre.Suave.Core.Api

open System
open Suave
open Suave.Successful
open CustomerPreferenceCentre.Domain

let getJsonStringFromReq (req : HttpRequest) =
  req.rawForm
  |> System.Text.Encoding.UTF8.GetString

let getReportRequestFromJson req =
  getJsonStringFromReq req
  |> JsonSerializer.deserialize
  |> RequestDtoToDomain.convertReportRequest

let getNintyDayPreferenceReport dateStr =
    (fun (ctx: HttpContext) ->
        let reportDate = (DateTime.ParseExact(dateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))
        let reportRequest = getReportRequestFromJson ctx.request
        let part =
            Report.nintyDayPreferenceReport reportDate reportRequest
            |> DomainToResponseDto.convertReportToResponseDto
            |> JsonSerializer.serialize
            |> OK
        (Async.bind part) (async.Return ctx)
    )
