# Primeiros passos com F# e Web Scraping

## Introdução

Sempre fiquei muito curioso quando escutava falar sobre linguagens funcionais e nas vantagens que elas trazem ao desenvolvimento. A promessa de dar adeus ao "null" e outros erros de runtime que tanto nos incomodam, faz brilhar meus olhos até hoje. Então, como desenvolvedor .NET, decidi começar no mundo funcional através do F#.

Já fazia um tempo que gostaria de estudar melhor a linguagem e escrever um artigo sobre, assim resolvi começar pela prática estudando como utiliza-la para aplicar Web Scraping. Para quem desconhece o termo, Web Scraping nada mais é do que um método de coletar dados de página web progamaticamente e F# é uma ferramenta muito poderosa para isso.

## Formas de usar

Existem basicamente 3 formas de se aplicar Web Scraping com F#:

1. Utilizar alguma biblioteca feita para isso em C#
2. Utilizar um wrapper de Selenium para F#
3. Ou então, utilizar a biblioteca `FSharp.Data`

Essa última é a que irei abordar nessa artigo, trata-se de uma biblioteca que permite trabalhar com os formatos CSV, XML, JSON e até, não se surpreenda, HTML mais facilmente.

## Exemplos

Através do `HtmlProvider` é possível definir um tipo para a página que você deseja fazer o scraping. Ele espera receber um html de exemplo que pode ser um arquivo local ou uma url, e vai servir de base para a criação do tipo F#, dessa forma:

``` f#
type DilbertSearch = HtmlProvider<"https://www.pinterest.pt/search/pins/?q=dilbert%20comic%20strip">
```

Assim é possível, por exemplo, pegar as primeiras imagens disponíveis da pesquisa por "dilbert comic strip" no `Pinterest`:

``` f#
DilbertSearch().Html.CssSelect("._0._25._2s img")
    |> List.iter (fun n -> printfn "%s" (n.AttributeValue("src")))
```

Os tipos gerados a partir do `HtmlProvider` fornecem facilitadores para tabelas e listas encontradas na página html, assim é possível, por exemplo, pegar a lista de personagens da tirinha do Hagar em sua página no `Wikipedia`, assim:

``` f#
type HagarWiki = HtmlProvider<"https://en.wikipedia.org/wiki/H%C3%A4gar_the_Horrible">

HagarWiki().Lists.``Cast of characters``.Values
    |> List.ofArray
    |> List.map getCharacterName
    |> List.iter (printf "%s\n")
```

E o melhor é que as tabelas e listas identificadas ficam disponíveis em tempo de desenvolvimento através do IntelliSense do Visual Studio ou Visual Studio Code.

![Exemplo IntelliSense com personagens do Hagar](https://user-images.githubusercontent.com/16840260/40587521-62d056f2-61a6-11e8-990b-53301248b71f.gif "Exemplo IntelliSense com personagens do Hagar")

Nesse outro exemplo, é possível buscar os filme da franquia `Star Wars` e a sua arrecadação em dólares:

``` f#
type StarWarsWiki = HtmlProvider<"https://en.wikipedia.org/wiki/List_of_Star_Wars_films_and_television_series">

let filmsByRevenue = StarWarsWiki().Tables.``Box office performance``.Rows
                        |> Seq.filter (fun r -> isReleaseDate r.``Release date``)
                        |> Seq.sortBy (fun x -> convertReleaseDate x.``Release date``)
                        |> Seq.map (fun r -> r.Film, convertRevenue r.``Box office revenue - Worldwide``)
                        |> Seq.toArray
```

E então, é possível plotar um gráfico utilizando a biblioteca `FSharp.Charting`, asssim:

``` f#
Chart.Column filmsByRevenue
    |> Chart.WithYAxis(Title = "Billions")
    |> Chart.WithXAxis(Title = "Films")
    |> Chart.Show
```

![Gráfico da arrecadação dos filmes da franquia Star Wars](https://user-images.githubusercontent.com/16840260/40587523-6414d826-61a6-11e8-940e-d1f60f38c1f9.PNG "Exemplo de arrecadações")

## Referências

Seguem alguns links de referência:

* [Why F# is the best langauge for Web Scraping](https://biarity.gitlab.io/2016/11/23/why-f-is-the-best-langauge-for-web-scraping/)
* [FSharp.Data](http://fsharp.github.io/FSharp.Data/index.html)