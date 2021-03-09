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

<!-- up -->

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

<!-- up -->