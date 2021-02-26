# Pragmatic guide to F#
### Table of contents
 - [Introduction](#introduction)
   - [What is F#?](#what-is-f)
   - [Is F# a purely functional language?](#is-f-a-purely-functional-language)
   - [What is purity and referential transparency as applied to expressions?](#what-is-purity-and-referential-transparency-as-applied-to-expressions)
   - [What are the reasons for using mutation-based function implementations in `FSharp.Core`?](#what-are-the-reasons-for-using-mutation-based-function-implementations-in-fsharpcore)
 - [Type System](#type-system)
   - [What is the difference between variables in F# and C#?](#what-is-the-difference-between-variables-in-f-and-c)
 - [Asynchronous programming](#asynchronous-programming)
   - [What is `Async<'T>` type in F#? What is `async` keyword?](#what-is-asynct-type-in-f-what-is-async-keyword)
   - [What are the differences between `Task<T>` in C# and `Async<'T>` in F#?](#what-are-the-differences-between-taskt-in-c-and-asynct-in-f)

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


## Type System
### What is the difference between variables in F# and C#?

In contrast to imperative programming languages, functional languages emphasize the use of immutable values over mutable variables. In F#, values  are immutable by default and there is a clear distinction between the concepts of assigning a value to a variable (`<-`) and binding a value to an identifier (`=`). A variable can only be reassigned when marked as `mutable` at its declaration.

**[⬆ Back to Top](#table-of-contents)**


## Asynchronous programming

### What is `Async<'T>` type in F#? What is `async` keyword?

 `Async<'T>` type is the core concept of F# asynchronous programming and represents a **composable** asynchronous computation. A value of type `Async<_>` is best thought of as a [“task specification” or “task generator”](https://www.microsoft.com/en-us/research/wp-content/uploads/2016/02/async-padl-revised-v2.pdf) ([source code link](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/async.fs)).

```f#
/// Represents an asynchronous computation
[<NoEquality; NoComparison; CompiledName("FSharpAsync`1")>]
type Async<'T> =
    { Invoke : (AsyncActivation<'T> -> AsyncReturn) }
```
The `async` keyword is an alias for `AsyncBuilder()` object ([source code link](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/fslib-extra-pervasives.fs)).

```f#
[<CompiledName("DefaultAsyncBuilder")>]
let async = AsyncBuilder()
```
 The `async { }` [computation expression](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions) provides a convenient syntax for building and controlling asynchronous computations. The `expression` placed within the `async` block (`async { expression }`) is set up to run asynchronously. The `async { }` computation expression itself is of type `Async<'T>`. It will produce a value of type `'T`, when executed, and deliver it to [continuation](https://en.wikipedia.org/wiki/Continuation).

**[⬆ Back to Top](#table-of-contents)**

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
warning CS4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call. [/media/veracrypt1/UpskillTube/CsharpConsole/CsharpConsole.csproj]
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