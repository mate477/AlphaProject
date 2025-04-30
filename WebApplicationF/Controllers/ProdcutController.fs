namespace WebApplicationF.Controllers

open Microsoft.AspNetCore.Mvc
open WebApplicationF.Models

[<ApiController>]
[<Route("[controller]")>]
type ProductController() =
    inherit ControllerBase()

    static let mutable products = [
        // Mechanical Keyboards
        { Id = 1; Name = "Logitech Keyboard"; Price = 129.99M; Category = "Mechanical Keyboards"; ImageUrl = "images/kb1.png" }
        { Id = 2; Name = "Aula Gaming Keyboard"; Price = 119.99M; Category = "Mechanical Keyboards"; ImageUrl = "images/kb3.png" }
        { Id = 3; Name = "HyperX Mechanical Keyboard"; Price = 139.99M; Category = "Mechanical Keyboards"; ImageUrl = "images/kb2.png" }
        { Id = 4; Name = "Razer Mechanical Keyboard"; Price = 159.99M; Category = "Mechanical Keyboards"; ImageUrl = "images/kb4.png" }
    
        // Bluetooth Mice
        { Id = 5; Name = "Razer Mouse"; Price = 89.99M; Category = "Bluetooth Mice"; ImageUrl = "images/bm2.png" }
        { Id = 6; Name = "Lenovo Bluetooth Mouse"; Price = 79.99M; Category = "Bluetooth Mice"; ImageUrl = "images/bm4.png" }
        { Id = 7; Name = "Logitech Bluetooth Mouse"; Price = 99.99M; Category = "Bluetooth Mice"; ImageUrl = "images/bm1.png" }
        { Id = 8; Name = "Microsoft Arc Mouse"; Price = 109.99M; Category = "Bluetooth Mice"; ImageUrl = "images/bm3.png" }
    
        // Monitors
        { Id = 9; Name = "Dell Monitor"; Price = 199.99M; Category = "Monitors"; ImageUrl = "images/m1.png" }
        { Id = 10; Name = "Samsung Curved Monitor"; Price = 249.99M; Category = "Monitors"; ImageUrl = "images/m2.png" }
        { Id = 11; Name = "LG UltraWide Monitor"; Price = 299.99M; Category = "Monitors"; ImageUrl = "images/m3.png" }
        { Id = 12; Name = "Asus Gaming Monitor"; Price = 279.99M; Category = "Monitors"; ImageUrl = "images/m4.png" }
    ]

    [<HttpGet>]
    member _.GetAll() : IActionResult =
        base.Ok(products)

    [<HttpGet("{id}")>]
    member _.GetById(id: int) : IActionResult =
        match products |> List.tryFind (fun p -> p.Id = id) with
        | Some p -> base.Ok(p)
        | None   -> base.NotFound()

    [<HttpPost>]
    member _.Create([<FromBody>] product: Product) : IActionResult =
        products <- product :: products
        base.Ok(product)

    [<HttpDelete("{id}")>]
    member _.Delete(id: int) : IActionResult =
        if products |> List.exists (fun p -> p.Id = id) then
            products <- products |> List.filter (fun p -> p.Id <> id)
            base.Ok()
        else
            base.NotFound()