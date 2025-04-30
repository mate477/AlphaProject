namespace WebApplicationF.Controllers

open Microsoft.AspNetCore.Mvc
open WebApplicationF.Models
open WebApplicationF.Storage

[<ApiController>]
[<Route("[controller]")>]
type CartController() =
    inherit ControllerBase()

    static let cartFilePath = "cart.json"
    static let mutable cart: CartItem list =
        try
            let result = loadFromFile<CartItem list> cartFilePath
            if box result = null then [] else result
        with _ -> []

    let save() = saveToFile cartFilePath cart

    [<HttpGet>]
    member _.GetCart() : IActionResult =
        base.Ok(cart)

    [<HttpPost>]
    member _.AddToCart([<FromBody>] item: CartItem) : IActionResult =
        let existing = cart |> List.tryFind (fun i -> i.ProductId = item.ProductId)

        match existing with
        | Some existingItem when item.Quantity > 0 ->
            cart <- cart |> List.map (fun i -> if i.ProductId = item.ProductId then { i with Quantity = i.Quantity + item.Quantity } else i)
            save()
            base.Ok()
        | Some existingItem when item.Quantity < 0 ->
            let newQty = existingItem.Quantity + item.Quantity
            if newQty > 0 then
                cart <- cart |> List.map (fun i -> if i.ProductId = item.ProductId then { i with Quantity = newQty } else i)
            else
                cart <- cart |> List.filter (fun i -> i.ProductId <> item.ProductId)
            save()
            base.Ok()
        | None when item.Quantity > 0 ->
            cart <- { ProductId = item.ProductId; Quantity = item.Quantity } :: cart
            save()
            base.Ok()
        | _ ->
            base.BadRequest("Invalid operation in cart.")

    [<HttpDelete("{productId}")>]
    member _.RemoveFromCart(productId: int) : IActionResult =
        if cart |> List.exists (fun i -> i.ProductId = productId) then
            cart <- cart |> List.filter (fun i -> i.ProductId <> productId)
            save()
            base.Ok()
        else
            base.NotFound()

    [<HttpDelete("clear")>]
    member _.ClearCart() : IActionResult =
        cart <- []
        save()
        base.Ok()