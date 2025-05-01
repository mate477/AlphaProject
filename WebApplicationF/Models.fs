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

type User = {
    Email: string
    PasswordHash: string
    DateOfBirth: System.DateTime
}

type RegisterDto = {
    Email: string
    ConfirmEmail: string
    Password: string
    ConfirmPassword: string
    DateOfBirth: System.DateTime
}

type LoginDto = {
    Email: string
    Password: string
}