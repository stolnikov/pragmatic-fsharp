# Working with functions

### What is currying? What is partial application?

[Currying](https://docs.microsoft.com/en-us/dotnet/fsharp/introduction-to-functional-programming/first-class-functions#curried-functions) is a technique of transforming a function that has more than one parameter into a series of embedded functions, each of which has a single parameter. Currying allows to turn any function of n arguments into a function of n-1 arguments which returns a function that expects the remaining nth argument.

<!-- up -->

### What are `>>` and `<<` operators?

These are [function composition operators](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/#function-composition-and-pipelining) ([📙 source](https://github.com/fsharp/fsharp/blob/master/src/fsharp/FSharp.Core/prim-types.fs)).

[Forward composition operator](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/) reads as: given two functions, `f` and `g`, and a value, `x`, compute the result of `f` of `x` and pass that result to `g`. 

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
x |> (g >> f) = x |> g |> f = f (g x)
```

[Backward composition operator](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/symbol-and-operator-reference/) `<<` composes two functions in reverse order; the second one is executed first.

```f#
// backward composition operator
// val ( << ): g: 'a -> 'b -> f: 'c -> 'a -> x: 'c -> 'b
let inline (<<) g f x = g (f x)
```
```
(f << g) x = f (g x)
```

<!-- up -->