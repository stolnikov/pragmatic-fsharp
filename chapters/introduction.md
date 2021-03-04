## Introduction
### What is F#?

F# is a functional-first, strongly typed, [multi-paradigm](https://en.wikipedia.org/wiki/Comparison_of_programming_paradigms), [general-purpose](https://en.wikipedia.org/wiki/General-purpose_programming_language) programming language that can be used to develop software in many different application domains.

* **Functional-first**: Today, simply referring to a programming language as "functional" does not adequately convey what it actually represents because most modern mainstream languages are functional in part. F# offers a [powerful type system](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/fsharp-types) and [concise syntax](https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/) which encourage a developer to write code in a functional way while also supporting object-oriented and imperative programming styles and thus being a hybrid language.

### Is F# a purely functional language?

Even though F# is heavily geared towards functional paradigm, it's not a [purely functional language](https://en.wikipedia.org/wiki/Purely_functional_programming) because it provides a fair number of escape hatches, i.e.:

* Using [reference cells](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/reference-cells) circumvents [value and function bindings](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/let-bindings).
* An identifier that is bound to a value with `mutable` keyword [can then be reassigned](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/values/#mutable-variables) with `<-` operator.
* F# supports procedural loops which is an imperative concept. In a purely functional language, such a construct makes no sense because loops are only useful in combination with mutable state.
 
  [📙 `List.fold` function](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/list.fs) uses a "for loop" with a mutable variable to store intermediary results. In a purely functional language, this can be implemented using a tail-recursive function (left fold) where each consecutive iteration gets the intermediary result as an explicit function argument thus eliminating the need for a mutable `acc` variable.

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

[Referential transparency](https://en.wikipedia.org/wiki/Referential_transparency) is a [property of expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/introduction-to-functional-programming/) such that they can be replaced with their output without affecting a program's behavior.

An expression is [pure](https://en.wikipedia.org/wiki/Pure_function) when it evaluates to the same value when provided with the same explicit arguments, its evaluation produces no side effects and it does not access and rely on any data outside its scope (implicit arguments). Referential transparency of a pure expression is the logical corollary of it being pure.

📝 In the context of F#, which is an impure language, an impure expression can arguably have the property of referential transparency when it includes impure functions and its resulting value does not depend on their side effects. For instance, condider a function with a side effect of logging its computation progress to the standard output. You can replace it with a similar function that does not log anything, which in effect makes it referentially transparent.

**[⬆ Back to Top](#table-of-contents)**

### What are the reasons for using mutation-based function implementations in `FSharp.Core`?

For obvious reasons, library functions like [📙 `Array.contains`](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/array.fs) are performance-critical:

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
There are plenty of algorithms that can be made more performant or/and memory efficient if implemented on top of mutation. It is a good practice to [wrap mutable code in immutable interfaces](https://docs.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions#wrap-mutable-code-in-immutable-interfaces) to steer clear of any unwanted effects as well as to achieve referential transparency. This way, mutable state isn't exposed to the consuming code and the caller isn't required to maintain it.

**[⬆ Back to Top](#table-of-contents)**