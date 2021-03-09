open MarkdownHelpers.TableOfContents
open MarkdownHelpers.MarkdownMerge
open Chapters
open System.IO

[<EntryPoint>]
let main argv =
    let chaptersDirectory = "./chapters"
    let fileExtension = "md"

    let validChapterPaths =
        chapters
        |> List.map (toRelativePath chaptersDirectory fileExtension)
        |> List.filter File.Exists

    trimFileContents validChapterPaths |> ignore

    let allTextLines = 
        validChapterPaths
        |> bodyLines
        |> addBackToTopLinks "<!-- up -->" "**[⬆ Back to Top](#table-of-contents)**"

    let header = """
<p align="center"><img src="img/fsharp.png" width="256px" alt="Pragmatic FSharp"></p>
<h1 align="center">Pragmatic guide to F#</h1>
<p align="center">Explore functional programming with F#</p>
<p align="center">⚠️ This is a work in constant progress</p>
<br>

📙 source code link

📝 author's personal opinion on controversial topics

The document has numerous references to [useful resources](https://fsharp.org/testimonials/) to save a reader some googling time.

### Table of contents
"""

    let tocRegexPattern =
        @"^(?<Level>[#]{1,4}) (?<Title>.{4,128}?)(?<ShouldOmit><!-- omit in toc -->)?$"

    let toc =
        header + compileToc tocRegexPattern allTextLines

    let body = toBodyString allTextLines

    System.IO.File.WriteAllText("README.md", toc + "\n\n" + body)

    0
