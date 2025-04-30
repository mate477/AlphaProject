namespace WebApplicationF.Controllers

open Microsoft.AspNetCore.Mvc
open WebApplicationF.Models

[<ApiController>]
[<Route("[controller]")>]
type OrderController() =
    inherit ControllerBase()

    static let mutable orders: Order list = []
    static let mutable lastOrderId = 0

    [<HttpGet>]
    member _.GetOrders() : IActionResult =
        base.Ok(orders)

    [<HttpPost>]
    member _.PlaceOrder([<FromBody>] items: CartItem list) : IActionResult =
        let totalQuantity = items |> List.sumBy (fun i -> i.Quantity)
        let newOrder = {
            OrderId = lastOrderId + 1
            Items = items
            TotalQuantity = totalQuantity
        }
        lastOrderId <- newOrder.OrderId
        orders <- newOrder :: orders
        base.Ok(newOrder)