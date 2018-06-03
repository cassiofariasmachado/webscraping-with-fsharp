open System
open FSharp.Data
open System.Text.RegularExpressions
open System.Globalization

//Exemplo 1: Imprime os primeiros links da busca de 'dilbert comic strip' no Pinterest
type DilbertSearch = HtmlProvider<"https://www.pinterest.pt/search/pins/?q=dilbert%20comic%20strip">

let formatUrl (url:string) = Regex(" (\dx)").Replace(url, String.Empty)
                            |> fun url -> url.Trim()

let getUrl (url:string) = url.Split ','
                          |> Seq.map formatUrl
                          |> Seq.last

DilbertSearch().Html.CssSelect(".mainContainer img")
    |> List.map (fun d -> getUrl(d.AttributeValue("srcset")))
    |> List.iter (printfn "%s")

//Exemplo 2: Imprime os personagens de Hagar the horrible
type HagarWiki = HtmlProvider<"https://en.wikipedia.org/wiki/H%C3%A4gar_the_Horrible">

let getCharacterName str = Regex("^([\w\W]*?):.*")
                            |> fun x -> x.Match(str).Groups
                            |> fun x -> x.Item(1).Value.Trim()

HagarWiki().Lists.``Cast of characters``.Values
    |> List.ofArray
    |> List.map getCharacterName
    |> List.iter (printf "%s\n")

//Exemplo 3: Imprime e plota um gráfico da arrecadação de cada filme da franquia Star Wars
type StarWarsWiki = HtmlProvider<"https://en.wikipedia.org/wiki/List_of_Star_Wars_films_and_television_series">

let convertRevenue str = Regex("(\$|,)").Replace(str, String.Empty)
                            |> Decimal.Parse
                            |> fun number -> number / 1000000000M

let convertReleaseDate str = DateTime.ParseExact(str, "MMMM dd, yyyy", CultureInfo.InvariantCulture)

let isReleaseDate str = Regex("[a-zA-Z]+ \d{2}, \d{4}", RegexOptions.IgnoreCase).IsMatch(str)

let filmsByRevenue = StarWarsWiki().Tables.``Box office performance``.Rows
                        |> Seq.filter (fun r -> isReleaseDate r.``Release date``)
                        |> Seq.sortBy (fun x -> convertReleaseDate x.``Release date``)
                        |> Seq.map (fun r -> r.Film, convertRevenue r.``Box office revenue - Worldwide``)
                        |> Seq.toArray
filmsByRevenue 
    |> Seq.iter (fun elem -> elem ||> printf "%s - %f Billions \n")