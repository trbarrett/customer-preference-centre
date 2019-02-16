module CustomerPreferenceCentre.Suave.Core.Routing

open CustomerPreferenceCentre.Suave.Core
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.Writers

let app =
    choose
        [ GET >=> choose
            [ path "/hello" >=> OK "Hello GET"
            ]
          POST >=> choose
              // Reviewers Note:
              // A Pure calucation method doesn't always fit neatly within REST.
              // In the HTTP RFC for POST it allows for "providing a block of data ... to
              // a data-handling process" which seems to best describe this. The RFC also
              // allows it to return "an entity that describes the result" without creating
              // a resource.

              // The caller should provide the date the report is to run in the url in the
              // form: "yyyy-MM-dd", and the cusomer details in the body
            [ pathScan "/ninty-day-preference-report/%s"
                       Api.getNintyDayPreferenceReport
                       >=> setMimeType "application/json"
            ]
          Suave.RequestErrors.NOT_FOUND "Resource not found."
        ]
