
<p align="center"><img src="img/fsharp.png" width="256px" alt="Pragmatic FSharp"></p>
<h1 align="center">Pragmatic guide to F#</h1>
<p align="center">Explore functional programming with F#</p>
<p align="center">⚠️ This is a work in constant progress</p>
<br>

📙 source code link

📝 author's personal opinion on controversial topics

The document has numerous references to [useful resources](https://fsharp.org/testimonials/) to save a reader some googling time.

### Table of contents
 - [Introduction](#introduction)
     - [What is F#?](#what-is-f)
     - [Is F# a purely functional language?](#is-f-a-purely-functional-language)
 - [Working with functions](#working-with-functions)
   - [Basic terminology](#basic-terminology)
     - [What formal system lies at the core of almost all functional programming?](#what-formal-system-lies-at-the-core-of-almost-all-functional-programming)
     - [What is arity?](#what-is-arity)
     - [What is currying? What is partial application?](#what-is-currying-what-is-partial-application)
     - [What is function composition?](#what-is-function-composition)
     - [What is an expression in computer science?](#what-is-an-expression-in-computer-science)
     - [What is purity and referential transparency as applied to expressions?](#what-is-purity-and-referential-transparency-as-applied-to-expressions)
       - [What are the reasons for using mutation-based function implementations in `FSharp.Core`?](#what-are-the-reasons-for-using-mutation-based-function-implementations-in-fsharpcore)
   - [Operators](#operators)
     - [What are `>>` and `<<` operators?](#what-are--and--operators)
 - [Type System](#type-system)
     - [What is the difference between variables in F# and C#?](#what-is-the-difference-between-variables-in-f-and-c)
     - [What are units of measure in F#? Is there an extra runtime overhead?](#what-are-units-of-measure-in-f-is-there-an-extra-runtime-overhead)
     - [How can F# assist us in "making illegal states unrepresentable" when designing our domain model?](#how-can-f-assist-us-in-making-illegal-states-unrepresentable-when-designing-our-domain-model)
 - [Asynchronous programming](#asynchronous-programming)
     - [What is `Async<'T>` type in F#? What is `async`?](#what-is-asynct-type-in-f-what-is-async)
     - [What are the differences between `Task<T>` in C# and `Async<'T>` in F#?](#what-are-the-differences-between-taskt-in-c-and-asynct-in-f)

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

**[⬆ Back to Top](#table-of-contents)**


# Working with functions

## Basic terminology
### What formal system lies at the core of almost all functional programming?

[Functional programming](https://en.wikipedia.org/wiki/Functional_programming) has its roots in academia, evolving from the [lambda calculus](https://en.wikipedia.org/wiki/Lambda_calculus), a formal system of computation based only on functions.

<details>
<summary><b>Watch about Lambda Calculus on Computerphile</b></summary>
<p>
<a href="https://www.youtube.com/watch?v=eis11j_iGMs" target=”_blank” rel="noreferrer noopener">
    <img src="https://camo.githubusercontent.com/20dfccb8426fde130c2fb275879fd9cf84bf6106544f6e00750290912d30852d/687474703a2f2f69332e7974696d672e636f6d2f76692f65697331316a5f69474d732f687164656661756c742e6a7067" alt="Lambda Calculus - Computerphile" data-canonical-src="http://i3.ytimg.com/vi/eis11j_iGMs/hqdefault.jpg" style="max-width:100%;">
</a>
</p>
</p>
<a href="https://www.youtube.com/watch?v=eis11j_iGMs" target=”_blank” rel="noreferrer noopener">https://www.youtube.com/watch?v=eis11j_iGMs</a>
</p>
</details>

### What is arity?

[Arity](https://en.wikipedia.org/wiki/Arity) is the number of arguments or operands taken by a function or operation in logic, mathematics, and computer science.

### What is currying? What is partial application?

[Currying](https://docs.microsoft.com/en-us/dotnet/fsharp/introduction-to-functional-programming/first-class-functions#curried-functions) is a technique of transforming a function that has more than one parameter into a chain of nested functions, each of which takes a single parameter.

[Partial application](https://en.wikipedia.org/wiki/Partial_application) is a process of fixing a number of arguments to a function that produces another function of smaller arity bigger than one. It's an irreversible operation; you can't unapply arguments.

### What is function composition?

In mathematics, [function composition](https://en.wikipedia.org/wiki/Function_composition) is an operation that takes two functions `f` and `g` and produces a function `h` such that `h(x) = g(f(x))`.

### What is an expression in computer science?

In computer science, an [expression](https://en.wikipedia.org/wiki/Expression_(computer_science)) is a syntactic entity in a programming language that may be evaluated to determine its value. It is a combination of one or more constants, variables, functions, and operators that the programming language interprets (according to its particular rules of precedence and of association) and computes to produce ("to return", in a stateful environment) another value. This process, for mathematical expressions, is called evaluation.

### What is purity and referential transparency as applied to expressions?

[Referential transparency](https://en.wikipedia.org/wiki/Referential_transparency) is a [property of expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/introduction-to-functional-programming/) such that they can be replaced with their output without affecting a program's behavior.

An expression is [pure](https://en.wikipedia.org/wiki/Pure_function) when it evaluates to the same value when provided with the same explicit arguments, its evaluation produces no side effects and it does not access and rely on any data outside its scope (implicit arguments). Referential transparency of a pure expression is the logical corollary of it being pure.

📝 In the context of F#, [which is an impure language](#is-f-a-purely-functional-language), an impure expression can arguably have the property of referential transparency when it includes impure functions and its resulting value does not depend on their side effects. For instance, condider a function with a side effect of logging its computation progress to the standard output. You can replace it with a similar function that does not log anything, which in effect makes it referentially transparent.

#### What are the reasons for using mutation-based function implementations in `FSharp.Core`?

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

## Operators
### What are `>>` and `<<` operators?

These are [function composition operators](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/#function-composition-and-pipelining) ([📙 source](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/prim-types.fs)) which are used to [compose two functions into one](#what-is-function-composition).

[Forward composition operator](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/) `>>` reads as: given two functions, `f` and `g`, and a value, `x`, compute the result of `f` of `x` and pass that result to `g`. 

```f#
// forward composition operator
// val ( >> ) : f: ('a -> 'b) -> g: ('b -> 'c) -> x: 'a -> 'c
//              ~~~~~~~~~~~~~    ~~~~~~~~~~~~~    ~~~~~    ~~~
//                    ^                ^            ^       ^
//                    |                |            |       |
//                    |                |            |       |
//              1st parameter    2nd parameter  3rd param   /
//                                                         |
//                                                       result
let inline (>>) f g x = g (f x)
```
```
(f >> g) x = x |> (f >> g) = x |> f |> g = g (f x)
```

[Backward composition operator](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/) `<<` composes two functions in reverse order; the second one is executed first.

```f#
// backward composition operator
// val ( << ): g: 'a -> 'b -> f: 'c -> 'a -> x: 'c -> 'b
let inline (<<) g f x = g (f x)
```
```
(g << f) x = g (f x)
```

**[⬆ Back to Top](#table-of-contents)**


# Type System
### What is the difference between variables in F# and C#?

In contrast to imperative programming languages, functional languages emphasize the use of immutable values over mutable variables. In F#, values  are immutable by default and there is a clear distinction between the concepts of assigning a value to a variable (`<-`) and [binding](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/let-bindings) a value to an identifier (`=`). A variable can only be reassigned when marked as `mutable` at its declaration.

### What are units of measure in F#? Is there an extra runtime overhead?

[Units of measure](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/units-of-measure) are a type of metadata that can be associated with floating point or signed integer values. This metadata is then used by the compiler to check whether arithmetic relationships of annotated values are valid. It gets erased during compilation and therefore [does not incur a performance hit at runtime](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/units-of-measure#units-at-runtime).

### How can F# assist us in "making illegal states unrepresentable" when designing our domain model?

With F# type system, we can encode simple domain rules directly into types.

**[⬆ Back to Top](#table-of-contents)**


# Asynchronous programming

### What is `Async<'T>` type in F#? What is `async`?

 `Async<'T>` type is the core concept of F# asynchronous programming and represents a **composable** asynchronous computation. A value of type `Async<_>` is best thought of as a [“task specification” or “task generator”](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/async-padl-revised-v2.pdf) ([📙 source](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/async.fs)).

```f#
/// Represents an asynchronous computation
[<NoEquality; NoComparison; CompiledName("FSharpAsync`1")>]
type Async<'T> =
    { Invoke : (AsyncActivation<'T> -> AsyncReturn) }
```
`async` is [not a keyword](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/keyword-reference) in F# as it may seem at first glance. Rather, it is an identifier, an alias for `AsyncBuilder()` object ([📙 source](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/fslib-extra-pervasives.fs)).

```f#
[<CompiledName("DefaultAsyncBuilder")>]
let async = AsyncBuilder()
```
 The `async { }` [computation expression](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions) provides a convenient syntax for building and controlling asynchronous computations. The `expression` placed within the `async` block (`async { expression }`) is set up to run asynchronously. The `async { }` computation expression itself is of type `Async<'T>`. It will produce a value of type `'T`, when executed, and deliver it to [continuation](https://en.wikipedia.org/wiki/Continuation).

### What are the differences between `Task<T>` in C# and `Async<'T>` in F#?

In C# asynchronous methods return tasks of type `Task<T>` (or `Task` if a method returns `void`) that run immediately after the method is called and eventually produce a value. Consider this [top-level](https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/exploration/top-level-statements) C# 9 console application.

```c#
using System;
using System.Threading;
using System.Threading.Tasks;

static void ShowThreadInfo()
{
    var currentThread = Thread.CurrentThread;
    Console.WriteLine(
        "Thread ID: {0}, IsThreadPoolThead: {1}", 
        currentThread.ManagedThreadId, 
        currentThread.IsThreadPoolThread);
}

async Task AsyncTest()
{
    Console.WriteLine("start");
    await Task.Run(ShowThreadInfo);
    Console.WriteLine("end");
}

AsyncTest();
```

<details>
<summary><b>Output</b></summary>
<p>

```sh
$ dotnet run
warning CS4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
start
Thread ID: 4, IsThreadPoolThead: True
end
```

</p>
</details>

Now let's take a look at the equivalent code in F# with the only difference that `Async.Start` is not awaited [by design](https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/asynchronous-and-concurrent-programming/async#asyncstart).

```f#
open System.Threading
let showThreadInfo =
    async {
        let currentThread = Thread.CurrentThread
        System.Console.WriteLine
            ("Thread ID: {0}, IsThreadPoolThead: {1}",
             currentThread.ManagedThreadId,
             currentThread.IsThreadPoolThread)
    }

let asyncTest =
    async {
        printfn "start"
        showThreadInfo |> Async.Start
        printfn "end"
    }

[<EntryPoint>]
let main argv =
    asyncTest
    0
```

In this case, there will be no output. Calling the `asyncTest` function doesn't start a task until you provide a continuation function to be called when the operations within the `async { }` block complete:

 ```f#
[<EntryPoint>]
let main argv =
    asyncTest |> Async.RunSynchronously
    0
 ```

<details>
<summary><b>Output</b></summary>
<p>

```sh
$ dotnet run
start
end
Thread ID: 4, IsThreadPoolThead: True
```

</p>
</details>

**[⬆ Back to Top](#table-of-contents)**