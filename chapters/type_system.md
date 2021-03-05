## Type System
### What is the difference between variables in F# and C#?

In contrast to imperative programming languages, functional languages emphasize the use of immutable values over mutable variables. In F#, values  are immutable by default and there is a clear distinction between the concepts of assigning a value to a variable (`<-`) and binding a value to an identifier (`=`). A variable can only be reassigned when marked as `mutable` at its declaration.

**[⬆ Back to Top](#table-of-contents)**

### What are units of measure in F#? Is there an extra runtime overhead?

[Units of measure](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/units-of-measure) are a type of metadata that can be associated with floating point or signed integer values. This metadata is then used by the compiler to check whether arithmetic relationships of annotated values are valid. It gets erased during compilation and therefore does not incur a performance hit at runtime.

**[⬆ Back to Top](#table-of-contents)**

### How can F# assist us in "making illegal states unrepresentable" when designing our domain model?

With F# type system, we can encode simple domain rules directly into types.

**[⬆ Back to Top](#table-of-contents)**