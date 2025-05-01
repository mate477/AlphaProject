namespace WebApplicationF.Controllers

open Microsoft.AspNetCore.Mvc
open WebApplicationF.Models
open WebApplicationF.Storage
open System

[<ApiController>]
[<Route("[controller]")>]
type CartController() =
    inherit ControllerBase()

    // File path to store the cart content
    static let cartFilePath = "cart.json"
    static let mutable cart: CartItem list =
        try
            let result = loadFromFile<CartItem list> cartFilePath
            if box result = null then [] else result
        with _ -> []

    // Save the current cart to file
    let save() = saveToFile cartFilePath cart

    // Normalize the cart by combining items with the same ProductId
    let normalizeCart (items: CartItem list) =
        items
        |> List.groupBy (fun item -> item.ProductId)
        |> List.map (fun (productId, group) ->
            let totalQty = group |> List.sumBy (fun i -> i.Quantity)
            { ProductId = productId; Quantity = totalQty })
        |> List.filter (fun i -> i.Quantity > 0)

    // Returns the normalized cart
    [<HttpGet>]
    member _.GetCart() : IActionResult =
        cart <- normalizeCart cart
        base.Ok(cart)

    // Adds or updates an item in the cart
    [<HttpPost>]
    member _.AddToCart([<FromBody>] item: CartItem) : IActionResult =
        if item.Quantity = 0 then
            base.BadRequest("Quantity must not be zero.")
        else
            let existing = cart |> List.tryFind (fun i -> i.ProductId = item.ProductId)

            if existing.IsSome then
                let existingItem = existing.Value
                let newQty = existingItem.Quantity + item.Quantity

                if newQty <= 0 then
                    // Remove the item entirely
                    cart <- cart |> List.filter (fun i -> i.ProductId <> item.ProductId)
                else
                    // Update the quantity
                    cart <- cart |> List.map (fun i ->
                        if i.ProductId = item.ProductId then { i with Quantity = newQty } else i)

                cart <- normalizeCart cart
                save()
                base.Ok()

            elif item.Quantity > 0 then
                // Add new item with positive quantity
                cart <- { ProductId = item.ProductId; Quantity = item.Quantity } :: cart
                cart <- normalizeCart cart
                save()
                base.Ok()

            else
                // Invalid: adding negative quantity to non-existent item
                base.BadRequest("Cannot add negative quantity to a new item.")

    // Removes an item completely from the cart
    [<HttpDelete("{productId}")>]
    member _.RemoveFromCart(productId: int) : IActionResult =
        if cart |> List.exists (fun i -> i.ProductId = productId) then
            cart <- cart |> List.filter (fun i -> i.ProductId <> productId)
            save()
            base.Ok()
        else
            base.NotFound()

    // Clears the entire cart
    [<HttpDelete("clear")>]
    member _.ClearCart() : IActionResult =
        cart <- []
        save()
        base.Ok()