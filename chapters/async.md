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

<!-- up -->

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

<!-- up -->