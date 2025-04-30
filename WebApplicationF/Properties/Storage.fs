namespace WebApplicationF

module Storage =

    open System.IO
    open System.Text.Json
    open WebApplicationF.Models

    let private options = JsonSerializerOptions(WriteIndented = true)

    let saveToFile<'T> (path: string) (data: 'T) =
        let json = JsonSerializer.Serialize(data, options)
        File.WriteAllText(path, json)

    let loadFromFile<'T> (path: string) : 'T =
        if File.Exists(path) then
            let json = File.ReadAllText(path)
            JsonSerializer.Deserialize<'T>(json, options)
        else
            Unchecked.defaultof<'T>