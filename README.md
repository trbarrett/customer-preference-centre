# Customer Preference Centre
#### Technical Exercise

## Development Environment:
Developed for Dotnet Core 2.1 in Visual Studio Community 2017 (Version 15.9.5)

## Design
The guidence said that the "The input/output format is yours to decide.". I have chosen to use Json over HTTP for the input/output, with Suave specifically for the webserver. I had similar code I used for another project which made it fairly easy to do the same thing here.

## Running the application:
Call "`>dotnet run`" from `src/CustomerPreferenceCentre.Suave.Server/` to start a webserver on port `8080`.

You can then perform a POST request via postman with a date and customer preference details as the following example demonstrates:
```
POST /ninty-day-preference-report/2019-02-16 HTTP/1.1
Host: 127.0.0.1:8080
Content-Type: application/json
cache-control: no-cache
[
    {
        "customerName": "A",
        "marketingPreference": "EveryDay"
    },
    {
        "customerName": "B",
        "marketingPreference": {
            "DayOfTheMonth": 10
        }
    },
    {
        "customerName": "C",
        "marketingPreference": {
            "DayOfTheWeek": [
                "Tuesday",
                "Friday"
            ]
        }
    },
    {
        "customerName": "D",
        "marketingPreference": "Never"
    }
]
```

Which should give you the following response (I've abbreviated it):
```json
[{"date":"2019-02-16","customers":["A"]},{"date":"2019-02-17","customers":["A"]},{"date":"2019-02-18","customers":["A"]},{"date":"2019-02-19","customers":["A","C"]},{"date":"2019-02-20","customers":["A"]},{"date":"2019-02-21","customers":["A"]},{"date":"2019-02-22","customers":["A","C"]}, ....
```

## Domain Model and Logic:

The main models and domain logic for generating the report can all be found in `src/CustomerPreferenceCentre.Domain/Report.fs`
Unit tests for that can be found in `src/CustomerPreferenceCentre.Test.Unit/PreferencesTest.fs and ScenarioTest.fs`

## Serialization:

I've used `Newtonsoft.Json` and `Microsoft.FSharpLu.Json` to do the basic serialization. DTOs and conversion can be found in `src/CustomerPreferenceCentre.Domain/Serialization.fs` with tests in `src/CustomerPreferenceCentre.Test.Unit/SerializationTests.fs`

## Web Server

The rest of code is the suave webserver and other tests. I've split the suave code into two parts, the core code and the server/runner. I find that makes it easy to do functional tests as I can start the server however I chose separatley in the tests.

## Tests

Both:
`src/CustomerPreferenceCentre.Test.Unit` and
`src/CustomerPreferenceCentre.Test.Functional`
are written with Expecto.
They can be run from the command line by calling "`>dotnet run`".

## Other Notes:
* I've avoided doing much validation to keep things simple. I'd be happy to add more if you'd like.
* Similarly I've kept the tests straightforward and avoided testing much in the way of failure cases.
* The serialization method I've chosen is simply one I have some experience with.
* While I quite like expecto in general, I've rarely found a good use for the `message` parameter. You'll notice I always set it to a blank string. Perhaps it's due to the way I'm used to structuring tests in node?
