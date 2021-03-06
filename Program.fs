open MarkdownHelpers.TableOfContents
open MarkdownHelpers.MarkdownMerge
open Chapters
open System.IO

[<EntryPoint>]
let main argv =
    let chaptersDirectory = "./chapters"
    let extension = "md"

    let validChapterPaths =
        chapters
        |> List.map (toRelativePath chaptersDirectory extension)
        |> List.filter File.Exists

    trimFileContents validChapterPaths

    let allTextLines = 
        validChapterPaths
        |> bodyLines
        |> addBackToTopLinks "<!-- up -->" "**[⬆ Back to Top](#table-of-contents)**"

    let header = """
<p align="center"><img src="img/fsharp.png" width="256px" alt="Pragmatic FSharp"></p>
<h1 align="center">Pragmatic guide to F#</h1>
<p align="center">Explore F# in the form of questions and answers</p>

<br>

📝 author's personal opinion on controversial topics

📙 source code link

The document has numerous references to [useful resources](https://fsharp.org/testimonials/) to save a reader some googling time.

### Table of contents
"""

    let tocPattern =
        @"^(?<Level>[#]{1,4}) (?<Title>.{4,128}?)(?<ShouldOmit><!-- omit in toc -->)?$"

    let toc =
        header + compileToc tocPattern allTextLines

    let body = toBodyString allTextLines

    System.IO.File.WriteAllText("README.md", toc + "\n\n" + body)

    0
