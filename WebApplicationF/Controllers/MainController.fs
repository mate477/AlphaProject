namespace WebApplicationF.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open WebApplicationF
open WebApplicationF.Models

[<ApiController>]
[<Route("[controller]")>]
type MainController (logger : ILogger<MainController>) =
     inherit ControllerBase()



     [<HttpPost>]
     member _.Post([<FromBody>] item: string) =
        ActionResult<string>(item)