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
              path "/ninty-day-preference-report"
                    >=> (Api.getNintyDayPreferenceReport ())
                    >=> setMimeType "application/json"
            ]
          Suave.RequestErrors.NOT_FOUND "Resource not found."
        ]
