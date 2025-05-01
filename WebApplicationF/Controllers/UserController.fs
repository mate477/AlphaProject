namespace WebApplicationF.Controllers

open Microsoft.AspNetCore.Mvc
open WebApplicationF.Models
open System.Collections.Generic
open System.Security.Cryptography
open System.Text

[<ApiController>]
[<Route("user")>]
type UserController() =
    inherit ControllerBase()

    static let mutable users : List<User> = List<User>()

    let hashPassword (password: string) =
        using (SHA256.Create()) (fun sha ->
            password
            |> Encoding.UTF8.GetBytes
            |> sha.ComputeHash
            |> Array.map (fun b -> b.ToString("x2"))
            |> String.concat "")

    [<HttpPost("register")>]
    member _.Register([<FromBody>] data: RegisterDto) : IActionResult =
        if data.Password <> data.ConfirmPassword then
            base.BadRequest("Passwords do not match")
        elif data.Email <> data.ConfirmEmail then
            base.BadRequest("Emails do not match")
        elif users |> Seq.exists (fun u -> u.Email = data.Email) then
            base.Conflict("User already exists")
        else
            let hashedPassword = hashPassword data.Password
            let newUser = {
                Email = data.Email
                PasswordHash = hashedPassword
                DateOfBirth = data.DateOfBirth
            }
            users.Add(newUser)
            base.Ok("Registration successful")

    [<HttpPost("login")>]
    member _.Login([<FromBody>] login: LoginDto) : IActionResult =
        let hashed = hashPassword login.Password
        match users |> Seq.tryFind (fun u -> u.Email = login.Email && u.PasswordHash = hashed) with
        | Some _ -> base.Ok("Login successful")
        | None -> base.Unauthorized("Invalid credentials")

