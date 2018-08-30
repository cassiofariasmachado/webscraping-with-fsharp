# Primeiros passos com F# e Web Scraping

## Introdução

Sempre fiquei muito curioso quando ouvia sobre as linguagens funcionais e as vantagens que elas trazem ao desenvolvimento. A promessa de dar adeus ao `null` e outros erros de runtime que tanto nos incomodam, faz brilhar meus olhos até hoje. Então, como desenvolvedor .NET, decidi começar no mundo funcional através do F#.

Já fazia um tempo que gostaria de estudar melhor a linguagem e escrever um artigo sobre, assim resolvi começar pela prática estudando como utiliza-lá para aplicar _Web Scraping_. Para quem desconhece o termo, [Web Scraping](https://en.wikipedia.org/wiki/Web_scraping) nada mais é do que um método de coletar dados de páginas web e F# é uma ferramenta muito poderosa para isso.

## Formas de usar

Existem basicamente 3 formas de se aplicar _Web Scraping_ com F#:

1. Utilizar alguma biblioteca feita para isso em C#
2. Utilizar um wrapper de Selenium para F#
3. Ou então, utilizar a biblioteca `FSharp.Data`

Essa última é a que irei abordar nessa artigo, trata-se de uma biblioteca que permite trabalhar mais facilmente com os formatos CSV, XML, JSON e até, não se surpreenda, HTML.

Ela também fornece helpers para realizar requisições HTTP, conversão para os tipos já mencionados e acesso ao [WorldBank](http://www.worldbank.org/), mas isso não será abordado nesse artigo.

## HtmlProvider

Através do `HtmlProvider` é possível definir um tipo para a página que você deseja fazer o _scraping_. Ele espera receber um HTML de exemplo que pode ser um arquivo ou uma URL, e vai servir de base para a criação do tipo F#, dessa forma:

``` f#
type DilbertSearch = HtmlProvider<"https://www.pinterest.pt/search/pins/?q=dilbert%20comic%20strip">
```

Assim é possível, por exemplo, pegar as primeiras imagens disponíveis da pesquisa por "dilbert comic strip" no `Pinterest`:

``` f#
DilbertSearch().Html.CssSelect(".mainContainer img")
    |> List.map (fun d -> getUrlOfLargestImage(d.AttributeValue("srcset")))
    |> List.iter (printfn "%s")

// → https://i.pinimg.com/736x/90/38/bb/9038bbcabd5b31d6faa6705230df3a78--peanuts-comics-peanuts-gang.jpg
// → https://i.pinimg.com/736x/b4/f5/ba/b4f5bac902a421a8b2eb00f232a227e4--human-resources-online-comics.jpg
// ...
```

Os tipos gerados a partir do `HtmlProvider` indentificam automaticamente as tabelas e listas (literalmente `<table>`, `<ul>` ou `<ol>`) encontradas na página HTML, assim é possível, por exemplo, pegar a lista de personagens da tirinha do Hagar em sua página no `Wikipedia`:

``` f#
type HagarWiki = HtmlProvider<"https://en.wikipedia.org/wiki/H%C3%A4gar_the_Horrible">

HagarWiki().Lists.``Cast of characters``.Values
    |> List.ofArray
    |> List.map getCharacterName
    |> List.iter (printf "%s\n")

// → Hägar the Horrible
// → Helga
// ...
```

O nome dado a lista ou tabela identificada é retirado dos atributos/tags HTML `id`, `title`, `name`, `summary` ou `caption`, se nenhum deles é encontrado então o nome dado será `TableXX` ou `ListXX`, em que o `XX` é um número sequencial de onde o elemento foi encontrado na página.

Mas o melhor disso é que essas tabelas e listas ficam disponíveis em tempo de desenvolvimento através do IntelliSense do Visual Studio ou Visual Studio Code:

![Exemplo IntelliSense com personagens do Hagar](https://user-images.githubusercontent.com/16840260/40587521-62d056f2-61a6-11e8-990b-53301248b71f.gif "Exemplo IntelliSense com personagens do Hagar")

Nesse outro exemplo, é possível buscar os filmes da franquia `Star Wars` e a sua arrecadação em dólares:

``` f#
type StarWarsWiki = HtmlProvider<"https://en.wikipedia.org/wiki/List_of_Star_Wars_films_and_television_series">

let filmsByRevenue = StarWarsWiki().Tables.``Box office performance``.Rows
                        |> Seq.filter (fun r -> isReleaseDate r.``Release date``)
                        |> Seq.sortBy (fun x -> convertReleaseDate x.``Release date``)
                        |> Seq.map (fun r -> r.Film, convertRevenue r.``Box office revenue - Worldwide``)
                        |> Seq.toArray

filmsByRevenue
    |> Seq.iter (fun elem -> elem ||> printf "%s - %f Billions \n")

// → Star Wars - 0.775398 Billions
// → The Empire Strikes Back - 0.547969 Billions
// ...
```

E então, é possível também plotar um gráfico utilizando a biblioteca `FSharp.Charting`, assim:

``` f#
Chart.Column filmsByRevenue
    |> Chart.WithYAxis(Title = "Billions")
    |> Chart.WithXAxis(Title = "Films")
    |> Chart.Show
```

![Gráfico da arrecadação dos filmes da franquia Star Wars](https://user-images.githubusercontent.com/16840260/40587523-6414d826-61a6-11e8-940e-d1f60f38c1f9.PNG "Exemplo de arrecadações")

## Repositório com os exemplos

Os exemplos utilizados no artigo estão disponíveis nesse [repositório do GitHub](https://github.com/cassiofariasmachado/webscraping-with-fsharp).

Nele existem dois projetos de exemplo um utilizando [.NET Framework](https://github.com/cassiofariasmachado/webscraping-with-fsharp/tree/master/src/samples) e outro utilizando o [.NET Core](https://github.com/cassiofariasmachado/webscraping-with-fsharp/tree/master/src/samples-core). As únicas diferenças entre eles, além da versão do .NET, é que nesse último:

* A biblioteca `FSharp.Data` só é compatível com o .NET Core, se utilizada a partir da sua versão `3.0.0-beta` que está em beta
* E a biblioteca `FSharp.Charting` foi removida, pois tem dependência do framework

## Conclusão

Assim, a biblioteca `Fsharp.Data` torna o F# uma ferramenta muito poderosa para fazer _scraping_ de páginas web. Entretando, nem tudo são flores e se a página possui conteúdo muito dinâmico (utilização de javascript para renderização), existem dificuldades de se utilizar a biblioteca, mas que podem ser contornadas utilizando-a em conjunto da segunda opção apresentada no início do artigo, um wrapper de Selenium para F#.

Enfim, a primeira impressão com a linguagem e com o paradigma foi muito positiva, trata-se de uma forma diferente de desenvolvimento que torna o código muito mais claro, mas que necessita de certo aprofundamento teorico, pois exige uma mudança de _mindset_ para quem esta acostumado com o mundo orientado a objetos. Como próximo passo, devo continuar me aprofundando na linguagem pra quem sabe trazer mais algum artigo sobre o assunto.

## Referências

Seguem algumas referências utilizadas:

* [FSharp Official Website](https://fsharp.org/)
* [FSharp.Data](http://fsharp.github.io/FSharp.Data/index.html)
* [Why F# is the best langauge for Web Scraping](https://biarity.gitlab.io/2016/11/23/why-f-is-the-best-langauge-for-web-scraping/)