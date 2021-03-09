# Introduction
### What is F#?

F# is a [functional-first](https://dotnet.microsoft.com/languages/fsharp), strongly typed, [multi-paradigm](https://en.wikipedia.org/wiki/Comparison_of_programming_paradigms), [general-purpose](https://en.wikipedia.org/wiki/General-purpose_programming_language) programming language that can be used to develop software in many different application domains.

* **Functional-first**: Today, simply referring to a programming language as "functional" does not adequately convey what it actually represents because most modern mainstream languages are functional in part. F# offers a [powerful type system](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/fsharp-types) and [concise syntax](https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/) which encourage a developer to write code in a functional way while also supporting object-oriented and imperative programming styles and thus being a hybrid language.

### Is F# a purely functional language?

Even though F# is heavily geared towards functional paradigm, it's not a [purely functional language](https://en.wikipedia.org/wiki/Purely_functional_programming) because it provides a fair number of escape hatches, i.e.:

* Using [reference cells](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/reference-cells) circumvents [value and function bindings](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/let-bindings).
* An identifier that is bound to a value with `mutable` keyword [can then be reassigned](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/values/#mutable-variables) with `<-` operator.
* F# supports procedural loops which is an imperative concept. In a purely functional language, such a construct makes no sense because loops are only useful in combination with mutable state.
 
  [📙 `List.fold` function](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/list.fs) uses a "for loop" with a mutable variable to store intermediary results. In a purely functional language, this can be implemented using a tail-recursive function ([left fold](https://en.wikipedia.org/wiki/Fold_(higher-order_function)#On_lists)) where each consecutive iteration gets the intermediary result as an explicit function argument thus eliminating the need for a mutable `acc` variable.

```f#
[<CompiledName("Fold")>]
let fold<'T,'State> folder (state:'State) (list: 'T list) = 
    match list with 
    | [] -> state
    | _ -> 
        let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(folder)
        let mutable acc = state
        for x in list do
            acc <- f.Invoke(acc, x)
        acc
```

<!-- up -->