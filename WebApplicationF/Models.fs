namespace WebApplicationF.Models

type Product = {
    Id: int
    Name: string
    Price: decimal
    Category: string
    ImageUrl: string
}

type CartItem = {
    ProductId: int
    Quantity: int
}

type Order = {
    OrderId: int
    Items: CartItem list
    TotalQuantity: int
}