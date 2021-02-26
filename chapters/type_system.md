## Type System
### What is the difference between variables in F# and C#?

In contrast to imperative programming languages, functional languages emphasize the use of immutable values over mutable variables. In F#, values  are immutable by default and there is a clear distinction between the concepts of assigning a value to a variable (`<-`) and binding a value to an identifier (`=`). A variable can only be reassigned when marked as `mutable` at its declaration.

**[⬆ Back to Top](#table-of-contents)**