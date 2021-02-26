## Introduction
### What is F#?

F# is a functional-first, strongly typed, [multi-paradigm](https://en.wikipedia.org/wiki/Comparison_of_programming_paradigms), [general-purpose](https://en.wikipedia.org/wiki/General-purpose_programming_language) programming language that can be used to develop software in many different application domains.

* **Functional-first**: Today, simply referring to a programming language as "functional" does not adequately convey what it actually represents because most modern mainstream languages are functional in part. F# offers a [powerful type system](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/fsharp-types) and [concise syntax](https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/) which encourage a developer to write beautiful functional code by making it easy to do so ([see](https://blog.codinghorror.com/falling-into-the-pit-of-success/) "Falling into the Pit of Success" by Jeff Atwood) while also supporting object-oriented and imperative programming styles and thus being a hybrid language per se.

### Is F# a purely functional language?

Even though F# is heavily geared towards functional paradigm, it's not a [purely functional language](https://en.wikipedia.org/wiki/Purely_functional_programming) because it provides a fair number of escape hatches, i.e.:

* Using [reference cells](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/reference-cells) circumvents [value/function bindings](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/let-bindings)
* An identifier that is bound to a value with `mutable` keyword [can then be reassigned](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/values/#mutable-variables) with `<-` operator.
* F# supports procedural loops which is an imperative concept. In a purely functional language, such a construct makes no sense because loops are only useful in combination with mutable state. If you look at [the source code](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/list.fs) for `List.fold` function, you'll find that behind the scenes it's a "for loop" with a mutable variable to store intermediary results. In a purely functional language, this would be a left fold – a tail-recursive function in which each consecutive iteration gets the intermediary result as an explicit function argument so that no mutable `acc` variable is needed.

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
**[⬆ Back to Top](#table-of-contents)**

### What is purity and referential transparency as applied to expressions?

[Referential transparency](https://docs.microsoft.com/en-us/dotnet/fsharp/introduction-to-functional-programming/) is a property of expressions such that they can be replaced with their output without affecting a program's behavior.

In a purely functional programming language, this [requires the expression to be pure](https://en.wikipedia.org/wiki/Referential_transparency). That is to say the expression value must be the same for the same inputs and its evaluation must have no side effects.

In the context of F#, an expression with side effects is impure, but it still can be referentially transparent, provided its result does not depend on those effects.

**[⬆ Back to Top](#table-of-contents)**

### What are the reasons for using mutation-based function implementations in `FSharp.Core`?

[`Array.contains` function](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/array.fs):

```f#
[<CompiledName("Contains")>]
let inline contains value (array:'T[]) =
    checkNonNull "array" array
    let mutable state = false
    let mutable i = 0
    while not state && i < array.Length do
        state <- value = array.[i]
        i <- i + 1
    state
```
There are many algorithms that can be made more performant and memory efficient if implemented on top of mutation. With referential transparency as a goal, it is best to [wrap mutable code in immutable interfaces](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#wrap-mutable-code-in-immutable-interfaces) so that mutable state isn't exposed to the consuming code.

**[⬆ Back to Top](#table-of-contents)**