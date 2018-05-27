open canopy.classic
open System.IO
open FSharp.Data
open System

type UnisinosHistory = HtmlProvider<"history.html">


let getHistoryPageSource username password =

    url "https://portal.asav.org.br"

    "#txtUser" << username

    "#txtPass" << password

    click "#btnLogin"

    let historyLink = "#ctl13_ctl00_tvAccordionContents_ctl00_ctl02__Node"

    waitForElement historyLink

    click historyLink

    check (last "input[name=rdContexto]")

    let historyTable = "#ctl28_ctl03_fvHistorico_gvHistorico"

    waitForElement historyTable

    browser.PageSource

let saveText path content =
    if File.Exists(path) then File.Delete path
    File.WriteAllText(path, content)

let saveHistoryAsHtml username password =
    getHistoryPageSource username password
        |> fun (source) -> saveText "history.html" source

let headerAsString (table:UnisinosHistory.Ctl28Ctl03FvHistoricoGvHistorico) = 
    match table.Headers with
    | Some items -> String.concat ";" items 
    | None -> String.Empty

let rowToString (x:UnisinosHistory.Ctl28Ctl03FvHistoricoGvHistorico.Row) = 
   String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}", 
        x.Column1, x.Column2, x.Column3, x.Column4, x.Column5, x.Column6, x.Column7, x.Column8, x.Column9)

let rowsAsString (table:UnisinosHistory.Ctl28Ctl03FvHistoricoGvHistorico) = 
    table.Rows |> Seq.map rowToString
               |> Seq.append [headerAsString table]
               |> Seq.toList
 
let saveHistoryAsCsv username password = 
    getHistoryPageSource username password 
        |> UnisinosHistory.Parse
        |> fun (page) -> rowsAsString(page.Tables.Ctl28_ctl03_fvHistorico_gvHistorico)
        |> fun (history) -> File.WriteAllLines("history.csv", history)

[<EntryPoint>]
let main argv = 
    start chrome

    Console.WriteLine("Digite seu login:")
    let username = Console.ReadLine()

    Console.WriteLine("Digite sua senha:")
    let password = Console.ReadLine()

    saveHistoryAsCsv username password

    quit()
    0
