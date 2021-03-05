namespace MarkdownHelpers

open System
open System.IO

open System.Text.RegularExpressions

module TableOfContents =

    type TocHeading = { Title: string; Level: int }

    /// slugify a heading while merging consecutive spaces into one
    let githubSlugify (s: string): string =
        
        ([], s)
        ||> Seq.fold
            (fun acc e ->
                match acc, e with
                | _, ch when ch >= 'a' && ch <= 'z' || ch = ' ' || ch = '-' || Char.IsDigit ch -> ch :: acc
                | _, ch when ch >= 'A' && ch <= 'Z' -> Char.ToLowerInvariant ch :: acc
                | x :: xs, _ when x = e && x = ' ' -> acc
                | _ -> acc)      
        |> Array.ofList
        |> Array.rev
        |> String
        |> fun res -> res.Replace(' ', '-')

    /// make a proper indented TOC heading
    let makeTocHeadingWithLink (slugifier: string -> string) (heading: TocHeading): string =
        let padding =
            sprintf "%-*s" ((heading.Level - 1) * 2 + 1) " "

        sprintf "%s- [%s](#%s)" padding heading.Title (slugifier heading.Title)

    /// compile TOC with Github-style anchors
    let compileToc (tocPattern: string) (lines: string []): string =
        lines
        |> Array.choose
            (fun line ->
                let m = Regex.Match(line, tocPattern)

                match m.Success, m.Groups.["ShouldOmit"].Success with
                | true, false -> 
                    Some { Title = m.Groups.["Title"].Value
                           Level = m.Groups.["Level"].Value.Length }
                | _ -> None)
        |> Array.map (makeTocHeadingWithLink githubSlugify)
        |> String.concat "\n"

module MarkdownMerge =

    let trimFileContents (files: string list): unit =
        files
        |> List.iter
            (fun file ->
                let contents = File.ReadAllText file
                let trimmed = contents.Trim()

                if contents <> trimmed then
                    printfn "%s: Leading and trailing whitespaces have been removed." file
                    File.WriteAllText(file, trimmed))

    let bodyLines (files: string list): string [] =
        files
        |> List.map File.ReadAllLines
        |> fun module2dArray ->
            module2dArray
            |> List.mapi
                (fun i arr ->
                    match i with
                    | _ when i < module2dArray.Length - 1 -> Array.append arr [| "\n" |]
                    | _ -> arr)
        |> Array.concat

    let toBodyString (lines: string []) = lines |> String.concat "\n"
    