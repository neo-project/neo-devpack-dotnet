# 13 Statements

## 13.1 General

C# provides a variety of statements.

> *Note*: Most of these statements will be familiar to developers who have programmed in C and C++. *end note*

```ANTLR
statement
    : labeled_statement
    | declaration_statement
    | embedded_statement
    ;

embedded_statement
    : block
    | empty_statement
    | expression_statement
    | selection_statement
    | iteration_statement
    | jump_statement
    | try_statement
    | checked_statement
    | unchecked_statement
    | lock_statement
    | using_statement
    | yield_statement
    ;
```

The *embedded_statement* nonterminal is used for statements that appear within other statements. The use of *embedded_statement* rather than *statement* excludes the use of declaration statements and labeled statements in these contexts.

> *Example*: The code
>
> <!-- Example: {template:"standalone-console-without-using", name:"Statements", expectedErrors:["CS1023"], ignoredWarnings:["CS8321","CS0219"]} -->
> ```csharp
> void F(bool b)
> {
>    if (b)
>       int i = 44;
> }
> ```
>
> results in a compile-time error because an `if` statement requires an *embedded_statement* rather than a *statement* for its `if` branch. If this code were permitted, then the variable `i` would be declared, but it could never be used. Note, however, that by placing `i`’s declaration in a block, the example is valid.
>
> *end example*

## 13.2 End points and reachability

Every statement has an ***end point***. In intuitive terms, the end point of a statement is the location that immediately follows the statement. The execution rules for composite statements (statements that contain embedded statements) specify the action that is taken when control reaches the end point of an embedded statement.

> *Example*: When control reaches the end point of a statement in a block, control is transferred to the next statement in the block. *end example*

If a statement can possibly be reached by execution, the statement is said to be ***reachable***. Conversely, if there is no possibility that a statement will be executed, the statement is said to be ***unreachable***.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"Reachability1", expectedWarnings:["CS8321","CS0162"]} -->
> ```csharp
> void F()
> {
>     Runtime.Log("reachable");
>     goto Label;
>     Runtime.Log("unreachable");
>   Label:
>     Runtime.Log("reachable");
> }
> ```
>
> the second invocation of Runtime.Log is unreachable because there is no possibility that the statement will be executed.
>
> *end example*

A warning is reported if a statement other than *throw_statement*, *block*, or *empty_statement* is unreachable. It is specifically not an error for a statement to be unreachable.

> *Note*: To determine whether a particular statement or end point is reachable, the compiler performs flow analysis according to the reachability rules defined for each statement. The flow analysis takes into account the values of constant expressions ([§12.23](12-expressions.md#1223-constant-expressions)) that control the behavior of statements, but the possible values of non-constant expressions are not considered. In other words, for purposes of control flow analysis, a non-constant expression of a given type is considered to have any possible value of that type.
>
> In the example
>
> <!-- Example: {template:"standalone-console", name:"Reachability2", expectedWarnings:["CS8321","CS0162"]} -->
> ```csharp
> void F()
> {
>     const int i = 1;
>     if (i == 2)
>         Runtime.Log("unreachable");
> }
> ```
>
> the Boolean expression of the `if` statement is a constant expression because both operands of the `==` operator are constants. As the constant expression is evaluated at compile-time, producing the value `false`, the `Runtime.Log` invocation is considered unreachable. However, if `i` is changed to be a local variable
>
> <!-- Example: {template:"standalone-console", name:"Reachability3", expectedWarnings:["CS8321"]} -->
> ```csharp
> void F()
> {
>     int i = 1;
>     if (i == 2)
>         Runtime.Log("reachable");
> }
> ```
>
> the `Runtime.Log` invocation is considered reachable, even though, in reality, it will never be executed.
>
> *end note*

The *block* of a function member or an anonymous function is always considered reachable. By successively evaluating the reachability rules of each statement in a block, the reachability of any given statement can be determined.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"Reachability4", expectedWarnings:["CS8321"]} -->
> ```csharp
> void F(int x)
> {
>     Runtime.Log("start");
>     if (x < 0)
>         Runtime.Log("negative");
> }
> ```
>
> the reachability of the second `Runtime.Log` is determined as follows:
>
> - The first `Runtime.Log` expression statement is reachable because the block of the `F` method is reachable ([§13.3](13-statements.md#133-blocks)).
> - The end point of the first `Runtime.Log` expression statement is reachable because that statement is reachable ([§13.7](13-statements.md#137-expression-statements) and [§13.3](13-statements.md#133-blocks)).
> - The `if` statement is reachable because the end point of the first `Runtime.Log` expression statement is reachable ([§13.7](13-statements.md#137-expression-statements) and [§13.3](13-statements.md#133-blocks)).
> - The second `Runtime.Log` expression statement is reachable because the Boolean expression of the `if` statement does not have the constant value `false`.
>
> *end example*

There are two situations in which it is a compile-time error for the end point of a statement to be reachable:

- Because the `switch` statement does not permit a switch section to “fall through” to the next switch section, it is a compile-time error for the end point of the statement list of a switch section to be reachable. If this error occurs, it is typically an indication that a `break` statement is missing.

- It is a compile-time error for the end point of the block of a function member or an anonymous function that computes a value to be reachable. If this error occurs, it typically is an indication that a `return` statement is missing ([§13.10.5](13-statements.md#13105-the-return-statement)).

## 13.3 Blocks

### 13.3.1 General

A *block* permits multiple statements to be written in contexts where a single statement is allowed.

```ANTLR
block
    : '{' statement_list? '}'
    ;
```

A *block* consists of an optional *statement_list* ([§13.3.2](13-statements.md#1332-statement-lists)), enclosed in braces. If the statement list is omitted, the block is said to be empty.

A block may contain declaration statements ([§13.6](13-statements.md#136-declaration-statements)). The scope of a local variable or constant declared in a block is the block.

A block is executed as follows:

- If the block is empty, control is transferred to the end point of the block.
- If the block is not empty, control is transferred to the statement list. When and if control reaches the end point of the statement list, control is transferred to the end point of the block.

The statement list of a block is reachable if the block itself is reachable.

The end point of a block is reachable if the block is empty or if the end point of the statement list is reachable.

A *block* that contains one or more `yield` statements ([§13.15](13-statements.md#1315-the-yield-statement)) is called an iterator block. Iterator blocks are used to implement function members as iterators ([§15.14](15-classes.md#1514-iterators)). Some additional restrictions apply to iterator blocks:

- It is a compile-time error for a `return` statement to appear in an iterator block (but `yield return` statements are permitted).

### 13.3.2 Statement lists

A ***statement list*** consists of one or more statements written in sequence. Statement lists occur in *block*s ([§13.3](13-statements.md#133-blocks)) and in *switch_block*s ([§13.8.3](13-statements.md#1383-the-switch-statement)).

```ANTLR
statement_list
    : statement+
    ;
```

A statement list is executed by transferring control to the first statement. When and if control reaches the end point of a statement, control is transferred to the next statement. When and if control reaches the end point of the last statement, control is transferred to the end point of the statement list.

A statement in a statement list is reachable if at least one of the following is true:

- The statement is the first statement and the statement list itself is reachable.
- The end point of the preceding statement is reachable.
- The statement is a labeled statement and the label is referenced by a reachable `goto` statement.

The end point of a statement list is reachable if the end point of the last statement in the list is reachable.

## 13.4 The empty statement

An *empty_statement* does nothing.

```ANTLR
empty_statement
    : ';'
    ;
```

An empty statement is used when there are no operations to perform in a context where a statement is required.

Execution of an empty statement simply transfers control to the end point of the statement. Thus, the end point of an empty statement is reachable if the empty statement is reachable.

> *Example*: An empty statement can be used when writing a `while` statement with a null body:
>
> <!-- Example: {template:"standalone-console-without-using", name:"EmptyStatement1", replaceEllipsis:true, customEllipsisReplacements:["return true;"], expectedWarnings:["CS8321"]} -->
> ```csharp
> bool ProcessMessage() {...}
> void ProcessMessages()
> {
>     while (ProcessMessage())
>         ;
> }
> ```
>
> Also, an empty statement can be used to declare a label just before the closing “`}`” of a block:
>
> <!-- Example: {template:"standalone-console-without-using", name:"EmptyStatement2", replaceEllipsis:true, expectedWarnings:["CS8321"]} -->
> ```csharp
> void F(bool done)
> {
>     ...
>     if (done)
>     {
>         goto exit;
>     }
>     ...
>   exit:
>     ;
> }
> ```
>
> *end example*

## 13.5 Labeled statements

A *labeled_statement* permits a statement to be prefixed by a label. Labeled statements are permitted in blocks, but are not permitted as embedded statements.

```ANTLR
labeled_statement
    : identifier ':' statement
    ;
```

A labeled statement declares a label with the name given by the *identifier*. The scope of a label is the whole block in which the label is declared, including any nested blocks. It is a compile-time error for two labels with the same name to have overlapping scopes.

A label can be referenced from `goto` statements ([§13.10.4](13-statements.md#13104-the-goto-statement)) within the scope of the label.

> *Note*: This means that `goto` statements can transfer control within blocks and out of blocks, but never into blocks. *end note*

Labels have their own declaration space and do not interfere with other identifiers.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console-without-using", name:"LabeledStatements", expectedWarnings:["CS8321"]} -->
> ```csharp
> int F(int x)
> {
>     if (x >= 0)
>     {
>         goto x;
>     }
>     x = -x;
>   x:
>     return x;
> }
> ```
>
> is valid and uses the name x as both a parameter and a label.
>
> *end example*

Execution of a labeled statement corresponds exactly to execution of the statement following the label.

In addition to the reachability provided by normal flow of control, a labeled statement is reachable if the label is referenced by a reachable `goto` statement, unless the `goto` statement is inside the `try` block or a `catch` block of a *try_statement* that includes a `finally` block whose end point is unreachable, and the labeled statement is outside the *try_statement*.

## 13.6 Declaration statements

### 13.6.1 General

A *declaration_statement* declares one or more local variables, one or more local constants, or a local function. Declaration statements are permitted in blocks and switch blocks, but are not permitted as embedded statements.

```ANTLR
declaration_statement
    : local_variable_declaration ';'
    | local_constant_declaration ';'
    | local_function_declaration
    ;
```

A local variable is declared using a *local_variable_declaration* ([§13.6.2](13-statements.md#1362-local-variable-declarations)). A local constant is declared using a *local_constant_declaration* ([§13.6.3](13-statements.md#1363-local-constant-declarations)). A local function is declared using a *local_function_declaration* ([§13.6.4](13-statements.md#1364-local-function-declarations)).

The declared names are introduced into the nearest enclosing declaration space ([§7.3](7-basic-concepts.md#73-declarations)).

### 13.6.2 Local variable declarations

#### 13.6.2.1 General

A *local_variable_declaration* declares one or more local variables.

```ANTLR
local_variable_declaration
    : implicitly_typed_local_variable_declaration
    | explicitly_typed_local_variable_declaration
    | ref_local_variable_declaration
    ;
```

Local variable declarations fall into one of the three categories: implicitly typed, explicitly typed, and ref local.

Implicitly typed declarations contain the contextual keyword ([§6.4.4](6-lexical-structure.md#644-keywords)) `var` resulting in a syntactic ambiguity between the three categories which is resolved as follows:

- If there is no type named `var` in scope and the input matches *implicitly_typed_local_variable_declaration* then it is chosen;
- Otherwise if a type named `var` is in scope then *implicitly_typed_local_variable_declaration* is not considered as a possible match.

Within a *local_variable_declaration* each variable is introduced by a ***declarator***, which is one of *implicitly_typed_local_variable_declarator*, *explicitly_typed_local_variable_declarator* or *ref_local_variable_declarator* for implicitly typed, explicitly typed and ref local variables respectively. The declarator defines the name (*identifier*) and initial value, if any, of the introduced variable.

If there are multiple declarators in a declaration then they are processed, including any initializing expressions, in order left to right ([§9.4.4.5](9-variables.md#9445-declaration-statements)).

> *Note*: For a *local_variable_declaration* not occuring as a *for_initializer* ([§13.9.4](13-statements.md#1394-the-for-statement)) or *resource_acquisition* ([§13.14](13-statements.md#1314-the-using-statement)) this left to right order is equivalent to each declarator being within a separate *local_variable_declaration*. For example:
>
> <!-- Example: {template:"standalone-console-without-using", name:"LocalVariableDecls2", ignoredWarnings:["CS0168","CS8321"]} -->
> ```csharp
> void F()
> {
>     int x = 1, y, z = x * 2;
> }
> ```
>
> is equivalent to:
>
> <!-- Example: {template:"standalone-console-without-using", name:"LocalVariableDecls3", ignoredWarnings:["CS0168","CS8321"]} -->
> ```csharp
> void F()
> {
>     int x = 1;
>     int y;
>     int z = x * 2;
> }
> ```
>
> *end note*

The value of a local variable is obtained in an expression using a *simple_name* ([§12.8.4](12-expressions.md#1284-simple-names)). A local variable shall be definitely assigned ([§9.4](9-variables.md#94-definite-assignment)) at each location where its value is obtained. Each local variable introduced by a *local_variable_declaration* is *initially unassigned* ([§9.4.3](9-variables.md#943-initially-unassigned-variables)). If a declarator has an initializing expression then the introduced local variable is classified as *assigned* at the end of the declarator ([§9.4.4.5](9-variables.md#9445-declaration-statements)).

The scope of a local variable introduced by a *local_variable_declaration* is defined as follows ([§7.7](7-basic-concepts.md#77-scopes)):

- If the declaration occurs as a *for_initializer* then the scope is the *for_initializer*, *for_condition*, *for_iterator*, and *embedded_statement* ([§13.9.4](13-statements.md#1394-the-for-statement));
- If the declaration occurs as a *resource_acquisition* then the scope is the outermost block of the semantically equivalent expansion of the *using_statement* ([§13.14](13-statements.md#1314-the-using-statement));
- Otherwise the scope is the block in which the declaration occurs.

It is an error to refer to a local variable by name in a textual position that precedes its declarator, or within any initializing expression within its declarator. Within the scope of a local variable, it is a compile-time error to declare another local variable, local function or constant with the same name.

The ref-safe-context ([§9.7.2](9-variables.md#972-ref-safe-contexts)) of a ref local variable is the ref-safe-context of its initializing *variable_reference*. The ref-safe-context of non-ref local variables is *declaration-block*.

#### 13.6.2.2 Implicitly typed local variable declarations

```ANTLR
implicitly_typed_local_variable_declaration
    : 'var' implicitly_typed_local_variable_declarator
    | ref_kind 'var' ref_local_variable_declarator
    ;

implicitly_typed_local_variable_declarator
    : identifier '=' expression
    ;
```

An *implicity_typed_local_variable_declaration* introduces a single local variable, *identifier*. The *expression* or *variable_reference* shall have a compile-time type, `T`. The first alternative declares a variable with type `T` and an initial value of *expression*. The second alternative declares a ref variable with type `ref T` and an initial value of `ref` *variable_reference*.

> *Example*:
>
> <!-- Example: {template:"code-in-main", name:"LocalVariableDecls4", expectedWarnings:["CS0219","CS0219"], additionalFiles:["Order.cs"]} -->
> ```csharp
> var i = 5;
> var s = "Hello";
> var numbers = new int[] {1, 2, 3};
> var orders = new Dictionary<int,Order>();
> ref var j = ref i;
> ref readonly var k = ref i;
> ```
>
> The implicitly typed local variable declarations above are precisely equivalent to the following explicitly typed declarations:
>
> <!-- Example: {template:"code-in-main", name:"LocalVariableDecls5", expectedWarnings:["CS0219","CS0219"], additionalFiles:["Order.cs"]} -->
> ```csharp
> int i = 5;
> string s = "Hello";
> int[] numbers = new int[] {1, 2, 3};
> Dictionary<int,Order> orders = new Dictionary<int,Order>();
> ref int j = ref i;
> ref readonly int k = ref i;
> ```
>
> The following are incorrect implicitly typed local variable declarations:
>
> <!-- Example: {template:"standalone-console-without-using", name:"LocalVariableDecls1", expectedErrors:["CS0818","CS0820","CS0815","CS8917","CS0841"], ignoredWarnings:["CS0168"]} -->
> ```csharp
> var x;                  // Error, no initializer to infer type from
> var y = {1, 2, 3};      // Error, array initializer not permitted
> var z = null;           // Error, null does not have a type
> var u = x => x + 1;     // Error, anonymous functions do not have a type
> var v = v++;            // Error, initializer cannot refer to v itself
> ```
>
> *end example*

#### 13.6.2.3 Explicitly typed local variable declarations

```ANTLR
explicitly_typed_local_variable_declaration
    : type explicitly_typed_local_variable_declarators
    ;

explicitly_typed_local_variable_declarators
    : explicitly_typed_local_variable_declarator
      (',' explicitly_typed_local_variable_declarator)*
    ;

explicitly_typed_local_variable_declarator
    : identifier ('=' local_variable_initializer)?
    ;

local_variable_initializer
    : expression
    | array_initializer
    ;
```

An *explicity_typed_local_variable_declaration* introduces one or more local variables with the specified *type*.

If a *local_variable_initializer* is present then its type shall be appropriate according to the rules of simple assignment ([§12.21.2](12-expressions.md#12212-simple-assignment)) or array initialization ([§17.7](17-arrays.md#177-array-initializers)) and its value is assigned as the initial value of the variable.

#### 13.6.2.4 Ref local variable declarations

```ANTLR
ref_local_variable_declaration
    : ref_kind type ref_local_variable_declarators
    ;

ref_local_variable_declarators
    : ref_local_variable_declarator (',' ref_local_variable_declarator)*
    ;

ref_local_variable_declarator
    : identifier '=' 'ref' variable_reference
    ;
```

The initializing *variable_reference* shall have type *type* and meet the same requirements as for a *ref assignment* ([§12.21.3](12-expressions.md#12213-ref-assignment)).

If *ref_kind* is `ref readonly`, the *identifier*(s) being declared are references to variables that are treated as read-only. Otherwise, if *ref_kind* is `ref`, the *identifier*(s) being declared are references to variables that shall be writable.

It is a compile-time error to declare a ref local variable, or a variable of a `ref struct` type, within an iterator ([§15.14](15-classes.md#1514-iterators)).

### 13.6.3 Local constant declarations

A *local_constant_declaration* declares one or more local constants.

```ANTLR
local_constant_declaration
    : 'const' type constant_declarators
    ;

constant_declarators
    : constant_declarator (',' constant_declarator)*
    ;

constant_declarator
    : identifier '=' constant_expression
    ;
```

The *type* of a *local_constant_declaration* specifies the type of the constants introduced by the declaration. The type is followed by a list of *constant_declarator*s, each of which introduces a new constant. A *constant_declarator* consists of an *identifier* that names the constant, followed by an “`=`” token, followed by a *constant_expression* ([§12.23](12-expressions.md#1223-constant-expressions)) that gives the value of the constant.

The *type* and *constant_expression* of a local constant declaration shall follow the same rules as those of a constant member declaration ([§15.4](15-classes.md#154-constants)).

The value of a local constant is obtained in an expression using a *simple_name* ([§12.8.4](12-expressions.md#1284-simple-names)).

The scope of a local constant is the block in which the declaration occurs. It is an error to refer to a local constant in a textual position that precedes the end of its *constant_declarator*.

A local constant declaration that declares multiple constants is equivalent to multiple declarations of single constants with the same type.

### 13.6.4 Local function declarations

A *local_function_declaration* declares a local function.

```ANTLR
local_function_declaration
    : local_function_modifier* return_type local_function_header
      local_function_body
    | ref_local_function_modifier* ref_kind ref_return_type
      local_function_header ref_local_function_body
    ;

local_function_header
    : identifier '(' formal_parameter_list? ')'
    | identifier type_parameter_list '(' formal_parameter_list? ')'
      type_parameter_constraints_clause*
    ;

local_function_modifier
    : ref_local_function_modifier
    ;

ref_local_function_modifier
    : 'static'
    ;

local_function_body
    : block
    | '=>' null_conditional_invocation_expression ';'
    | '=>' expression ';'
    ;

ref_local_function_body
    : block
    | '=>' 'ref' variable_reference ';'
    ;
```

Grammar note: When recognising a *local_function_body* if both the *null_conditional_invocation_expression* and *expression* alternatives are applicable then the former shall be chosen. ([§15.6.1](15-classes.md#1561-general))

> *Example*: There is a common use cases for local functions: iterator methods. In iterator methods, any exceptions are observed only when calling code that enumerates the returned sequence. The following example demonstrates separating parameter validation from the iterator implementation using a local function:
>
> <!-- Example: {template:"code-in-class-lib", name:"LocalFunctionDeclarations1"} -->
> ```csharp
> public static IEnumerable<char> AlphabetSubset(char start, char end)
> {
>     if (start < 'a' || start > 'z')
>     {
>         throw new ArgumentOutOfRangeException(paramName: nameof(start),
>             message: "start must be a letter");
>     }
>     if (end < 'a' || end > 'z')
>     {
>         throw new ArgumentOutOfRangeException(paramName: nameof(end),
>             message: "end must be a letter");
>     }
>     if (end <= start)
>     {
>         throw new ArgumentException(
>             $"{nameof(end)} must be greater than {nameof(start)}");
>     }
>     return AlphabetSubsetImplementation();
>
>     IEnumerable<char> AlphabetSubsetImplementation()
>     {
>         for (var c = start; c < end; c++)
>         {
>             yield return c;
>         }
>     }
> }
> ```
>
> *end example*

Unless specified otherwise below, the semantics of all grammar elements is the same as for *method_declaration* ([§15.6.1](15-classes.md#1561-general)), read in the context of a local function instead of a method.

The *identifier* of a *local_function_declaration* shall be unique in its declared block scope, including any enclosing local variable declaration spaces. One consequence of this is that overloaded *local_function_declaration*s are not allowed.

If the declaration includes the `static` modifier, the function is a ***static local function***; otherwise, it is a ***non-static local function***. It is a compile-time error for *type_parameter_list* or *formal_parameter_list* to contain *attributes*. If the local function is declared in an unsafe context ([§23.2](unsafe-code.md#232-unsafe-contexts)), the local function may include unsafe code, even if the local function declaration doesn’t include the `unsafe` modifier.

A local function is declared at block scope. A non-static local function may capture variables from the enclosing scope while a static local function shall not (so it has no access to enclosing locals, parameters, non-static local functions, or `this`). It is a compile-time error if a captured variable is read by the body of a non-static local function but is not definitely assigned before each call to the function. The compiler shall determine which variables are definitely assigned on return ([§9.4.4.33](9-variables.md#94433-rules-for-variables-in-local-functions)).

When the type of `this` is a struct type, it is a compile-time error for the body of a local function to access `this`. This is true whether the access is explicit (as in `this.x`) or implicit (as in `x` where `x` is an instance member of the struct). This rule only prohibits such access and does not affect whether member lookup results in a member of the struct.

It is a compile-time error for the body of the local function to contain a `goto` statement, a `break` statement, or a `continue` statement whose target is outside the body of the local function.

> *Note*: the above rules for `this` and `goto` mirror the rules for anonymous functions in [§12.19.3](12-expressions.md#12193-anonymous-function-bodies). *end note*

A local function may be called from a lexical point prior to its declaration. However, it is a compile-time error for the function to be declared lexically prior to the declaration of a variable used in the local function ([§7.7](7-basic-concepts.md#77-scopes)).

It is a compile-time error for a local function to declare a parameter, type parameter or local variable with the same name as one declared in any enclosing local variable declaration space.

Local function bodies are always reachable. The endpoint of a local function declaration is reachable if the beginning point of the local function declaration is reachable.

> *Example*: In the following example, the body of `L` is reachable even though the beginning point of `L` is not reachable. Because the beginning point of `L` isn’t reachable, the statement following the endpoint of `L` is not reachable:
>
> <!-- Example: {template:"standalone-lib-without-using", name:"LocalFunctionDeclarations2", expectedWarnings:["CS0162"]} -->
> ```csharp
> class C
> {
>     int M()
>     {
>         L();
>         return 1;
>
>         // Beginning of L is not reachable
>         int L()
>         {
>             // The body of L is reachable
>             return 2;
>         }
>         // Not reachable, because beginning point of L is not reachable
>         return 3;
>     }
> }
> ```
>
> In other words, the location of a local function declaration doesn’t affect the reachability of any statements in the containing function. *end example*

If the type of the argument to a local function is `dynamic`, the function to be called shall be resolved at compile time, not runtime.

A local function shall not be used in an expression tree.

A static local function

- May reference static members, type parameters, constant definitions and static local functions from the enclosing scope.
- Shall not reference `this` or `base` nor instance members from an implicit `this` reference, nor local variables, parameters, or non-static local functions from the enclosing scope. However, all these are permitted in a `nameof()` expression.

## 13.7 Expression statements

An *expression_statement* evaluates a given expression. The value computed by the expression, if any, is discarded.

```ANTLR
expression_statement
    : statement_expression ';'
    ;

statement_expression
    : null_conditional_invocation_expression
    | invocation_expression
    | object_creation_expression
    | assignment
    | post_increment_expression
    | post_decrement_expression
    | pre_increment_expression
    | pre_decrement_expression
    | await_expression
    ;
```

Not all expressions are permitted as statements.

> *Note*: In particular, expressions such as `x + y` and `x == 1`, that merely compute a value (which will be discarded), are not permitted as statements. *end note*

Execution of an *expression_statement* evaluates the contained expression and then transfers control to the end point of the *expression_statement*. The end point of an *expression_statement* is reachable if that *expression_statement* is reachable.

## 13.8 Selection statements

### 13.8.1 General

Selection statements select one of a number of possible statements for execution based on the value of some expression.

```ANTLR
selection_statement
    : if_statement
    | switch_statement
    ;
```

### 13.8.2 The if statement

The `if` statement selects a statement for execution based on the value of a Boolean expression.

```ANTLR
if_statement
    : 'if' '(' boolean_expression ')' embedded_statement
    | 'if' '(' boolean_expression ')' embedded_statement
      'else' embedded_statement
    ;
```

An `else` part is associated with the lexically nearest preceding `if` that is allowed by the syntax.

> *Example*: Thus, an `if` statement of the form
>
> <!-- Example: {template:"code-in-main-without-using", name:"IfStatement1", additionalFiles:["PartialProgramWithFGxy.cs"]} -->
> ```csharp
> if (x) if (y) F(); else G();
> ```
>
> is equivalent to
>
> <!-- Example: {template:"code-in-main-without-using", name:"IfStatement2", additionalFiles:["PartialProgramWithFGxy.cs"]} -->
> ```csharp
> if (x)
> {
>     if (y)
>     {
>         F();
>     }
>     else
>     {
>         G();
>     }
> }
> ```
>
> *end example*

An `if` statement is executed as follows:

- The *boolean_expression* ([§12.24](12-expressions.md#1224-boolean-expressions)) is evaluated.
- If the Boolean expression yields `true`, control is transferred to the first embedded statement. When and if control reaches the end point of that statement, control is transferred to the end point of the `if` statement.
- If the Boolean expression yields `false` and if an `else` part is present, control is transferred to the second embedded statement. When and if control reaches the end point of that statement, control is transferred to the end point of the `if` statement.
- If the Boolean expression yields `false` and if an `else` part is not present, control is transferred to the end point of the `if` statement.

The first embedded statement of an `if` statement is reachable if the `if` statement is reachable and the Boolean expression does not have the constant value `false`.

The second embedded statement of an `if` statement, if present, is reachable if the `if` statement is reachable and the Boolean expression does not have the constant value `true`.

The end point of an `if` statement is reachable if the end point of at least one of its embedded statements is reachable. In addition, the end point of an `if` statement with no `else` part is reachable if the `if` statement is reachable and the Boolean expression does not have the constant value `true`.

### 13.8.3 The switch statement

The `switch` statement selects for execution a statement list having an associated switch label that corresponds to the value of the switch expression.

```ANTLR
switch_statement
    : 'switch' '(' expression ')' switch_block
    ;

switch_block
    : '{' switch_section* '}'
    ;

switch_section
    : switch_label+ statement_list
    ;

switch_label
    : 'case' pattern case_guard?  ':'
    | 'default' ':'
    ;

case_guard
    : 'when' expression
    ;
```

A *switch_statement* consists of the keyword `switch`, followed by a parenthesized expression (called the ***switch expression***), followed by a *switch_block*. The *switch_block* consists of zero or more *switch_section*s, enclosed in braces. Each *switch_section* consists of one or more *switch_label*s followed by a *statement_list* ([§13.3.2](13-statements.md#1332-statement-lists)). Each *switch_label* containing `case` has an associated pattern ([§11](11-patterns.md#11-patterns-and-pattern-matching)) against which the value of the switch expression is tested. If *case_guard* is present, its expression shall be implicitly convertible to the type `bool` and that expression is evaluated as an additional condition for the case to be considered satisfied.

The ***governing type*** of a `switch` statement is established by the switch expression.

- If the type of the switch expression is `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `bool`, `string`, or an *enum_type*, or if it is the nullable value type corresponding to one of these types, then that is the governing type of the `switch` statement.
- Otherwise, if exactly one user-defined implicit conversion exists from the type of the switch expression to one of the following possible governing types: `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `char`, `string`, or, a nullable value type corresponding to one of those types, then the converted type is the governing type of the `switch` statement.
- Otherwise, the governing type of the `switch` statement is the type of the switch expression. It is an error if no such type exists.

There can be at most one `default` label in a `switch` statement.

It is an error if the pattern of any switch label is not *applicable* ([§11.2.1](11-patterns.md#1121-general)) to the type of the input expression.

It is an error if the pattern of any switch label is *subsumed* by ([§11.3](11-patterns.md#113-pattern-subsumption)) the set of patterns of earlier switch labels of the switch statement that do not have a case guard or whose case guard is a constant expression with the value true.

> *Example*:
>
> ```csharp
> switch (shape)
> {
>     case var x:
>         break;
>     case var _: // error: pattern subsumed, as previous case always matches
>         break;
>     default:
>         break;  // warning: unreachable, all possible values already handled.
> }
> ```
>
> *end example*

A `switch` statement is executed as follows:

- The switch expression is evaluated and converted to the governing type.
- Control is transferred according to the value of the converted switch expression:
  - The lexically first pattern in the set of `case` labels in the same `switch` statement that matches the value of the switch expression, and for which the guard expression is either absent or evaluates to true, causes control to be transferred to the statement list following the matched `case` label.
  - Otherwise, if a `default` label is present, control is transferred to the statement list following the `default` label.
  - Otherwise, control is transferred to the end point of the `switch` statement.

> *Note*: The order in which patterns are matched at runtime is not defined. A compiler is permitted (but not required) to match patterns out of order, and to reuse the results of already matched patterns to compute the result of matching of other patterns. Nevertheless, the compiler is required to determine the lexically first pattern that matches the expression and for which the guard clause is either absent or evaluates to `true`. *end note*

If the end point of the statement list of a switch section is reachable, a compile-time error occurs. This is known as the “no fall through” rule.

> *Example*: The example
>
> <!-- Example: {template:"code-in-main-without-using", name:"SwitchStatement1", additionalFiles:["PartialProgramForSwitch.cs"]} -->
> ```csharp
> switch (i)
> {
>     case 0:
>         CaseZero();
>         break;
>     case 1:
>         CaseOne();
>         break;
>     default:
>         CaseOthers();
>         break;
> }
> ```
>
> is valid because no switch section has a reachable end point. Unlike C and C++, execution of a switch section is not permitted to “fall through” to the next switch section, and the example
>
> <!-- Example: {template:"code-in-main-without-using", name:"SwitchStatement2", additionalFiles:["PartialProgramForSwitch.cs"], expectedErrors:["CS0163","CS0163","CS8070"]} -->
> ```csharp
> switch (i)
> {
>     case 0:
>         CaseZero();
>     case 1:
>         CaseZeroOrOne();
>     default:
>         CaseAny();
> }
> ```
>
> results in a compile-time error. When execution of a switch section is to be followed by execution of another switch section, an explicit `goto case` or `goto default` statement shall be used:
>
> <!-- Example: {template:"code-in-main-without-using", name:"SwitchStatement3", additionalFiles:["PartialProgramForSwitch.cs"]} -->
> ```csharp
> switch (i)
> {
>     case 0:
>         CaseZero();
>         goto case 1;
>     case 1:
>         CaseZeroOrOne();
>         goto default;
>     default:
>         CaseAny();
>         break;
> }
> ```
>
> *end example*

Multiple labels are permitted in a *switch_section*.

> *Example*: The example
>
> <!-- Example: {template:"standalone-console", name:"SwitchStatement4", additionalFiles:["PartialProgramForSwitch.cs"]} -->
> ```csharp
> switch (i)
> {
>     case 0:
>         CaseZero();
>         break;
>     case 1:
>         CaseOne();
>         break;
>     case 2:
>     default:
>         CaseTwo();
>         break;
> }
> ```
>
> is valid. The example does not violate the “no fall through” rule because the labels `case 2:` and `default:` are part of the same *switch_section*.
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The “no fall through” rule prevents a common class of bugs that occur in C and C++ when `break` statements are accidentally omitted. For example, the sections of the `switch` statement above can be reversed without affecting the behavior of the statement:
>
> <!-- Example: {template:"standalone-console", name:"SwitchStatement5", additionalFiles:["PartialProgramForSwitch.cs"]} -->
> ```csharp
> switch (i)
> {
>     default:
>         CaseAny();
>         break;
>     case 1:
>         CaseZeroOrOne();
>         goto default;
>     case 0:
>         CaseZero();
>         goto case 1;
> }
> ```
>
> *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: The statement list of a switch section typically ends in a `break`, `goto case`, or `goto default` statement, but any construct that renders the end point of the statement list unreachable is permitted. For example, a `while` statement controlled by the Boolean expression `true` is known to never reach its end point. Likewise, a `throw` or `return` statement always transfers control elsewhere and never reaches its end point. Thus, the following example is valid:
>
> <!-- Example: {template:"code-in-main", name:"SwitchStatement6", additionalFiles:["PartialProgramForSwitch.cs"]} -->
> ```csharp
> switch (i)
> {
>      case 0:
>          while (true)
>          {
>              F();
>          }
>      case 1:
>          throw new ArgumentException();
>      case 2:
>          return;
> }
> ```
>
> *end note*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: The governing type of a `switch` statement can be the type `string`. For example:
>
> <!-- Example: {template:"code-in-class-lib", name:"SwitchStatement7", additionalFiles:["PartialClass1ForSwitch.cs"]} -->
> ```csharp
> void DoCommand(string command)
> {
>     switch (command.ToLower())
>     {
>         case "run":
>             DoRun();
>             break;
>         case "save":
>             DoSave();
>             break;
>         case "quit":
>             DoQuit();
>             break;
>         default:
>             InvalidCommand(command);
>             break;
>     }
> }
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Note*: Like the string equality operators ([§12.12.8](12-expressions.md#12128-string-equality-operators)), the `switch` statement is case sensitive and will execute a given switch section only if the switch expression string exactly matches a `case` label constant. *end note*
When the governing type of a `switch` statement is `string` or a nullable value type, the value `null` is permitted as a `case` label constant.

The *statement_list*s of a *switch_block* may contain declaration statements ([§13.6](13-statements.md#136-declaration-statements)). The scope of a local variable or constant declared in a switch block is the switch block.

A switch label is reachable if at least one of the following is true:

- The switch expression is a constant value and either
  - the label is a `case` whose pattern *would match* ([§11.2.1](11-patterns.md#1121-general)) that value, and label’s guard is either absent or not a constant expression with the value false; or
  - it is a `default` label, and no switch section contains a case label whose pattern would match that value, and whose guard is either absent or a constant expression with the value true.
- The switch expression is not a constant value and either
  - the label is a `case` without a guard or with a guard whose value is not the constant false; or
  - it is a `default` label and
    - the set of patterns appearing among the cases of the switch statement that do not have guards or have guards whose value is the constant true, is not *exhaustive* ([§11.4](11-patterns.md#114-pattern-exhaustiveness)) for the switch governing type; or
    - the switch governing type is a nullable type and the set of patterns appearing among the cases of the switch statement that do not have guards or have guards whose value is the constant true does not contain a pattern that would match the value `null`.
- The switch label is referenced by a reachable `goto case` or `goto default` statement.

The statement list of a given switch section is reachable if the `switch` statement is reachable and the switch section contains a reachable switch label.

The end point of a `switch` statement is reachable if the switch statement is reachable and at least one of the following is true:

- The `switch` statement contains a reachable `break` statement that exits the `switch` statement.
- No `default` label is present and either
  - The switch expression is a non-constant value, and the set of patterns appearing among the cases of the switch statement that do not have guards or have guards whose value is the constant true, is not *exhaustive* ([§11.4](11-patterns.md#114-pattern-exhaustiveness)) for the switch governing type.
  - The switch expression is a non-constant value of a nullable type, and no pattern appearing among the cases of the switch statement that do not have guards or have guards whose value is the constant true would match the value `null`.
  - The switch expression is a constant value and no `case` label without a guard or whose guard is the constant true would match that value.

> *Example*: The following code shows a succinct use of the `when` clause:
>
> ```csharp
> static object CreateShape(string shapeDescription)
> {
>    switch (shapeDescription)
>    {
>         case "circle":
>             return new Circle(2);
>         …
>         case var o when string.IsNullOrWhiteSpace(o):
>             return null;
>         default:
>             return "invalid shape description";
>     }
> }
> ```
>
> The var case matches `null`, the empty string, or any string that contains only white space. *end example*

## 13.9 Iteration statements

### 13.9.1 General

Iteration statements repeatedly execute an embedded statement.

```ANTLR
iteration_statement
    : while_statement
    | do_statement
    | for_statement
    | foreach_statement
    ;
```

### 13.9.2 The while statement

The `while` statement conditionally executes an embedded statement zero or more times.

```ANTLR
while_statement
    : 'while' '(' boolean_expression ')' embedded_statement
    ;
```

A `while` statement is executed as follows:

- The *boolean_expression* ([§12.24](12-expressions.md#1224-boolean-expressions)) is evaluated.
- If the Boolean expression yields `true`, control is transferred to the embedded statement. When and if control reaches the end point of the embedded statement (possibly from execution of a `continue` statement), control is transferred to the beginning of the `while` statement.
- If the Boolean expression yields `false`, control is transferred to the end point of the `while` statement.

Within the embedded statement of a `while` statement, a `break` statement ([§13.10.2](13-statements.md#13102-the-break-statement)) may be used to transfer control to the end point of the `while` statement (thus ending iteration of the embedded statement), and a `continue` statement ([§13.10.3](13-statements.md#13103-the-continue-statement)) may be used to transfer control to the end point of the embedded statement (thus performing another iteration of the `while` statement).

The embedded statement of a `while` statement is reachable if the `while` statement is reachable and the Boolean expression does not have the constant value `false`.

The end point of a `while` statement is reachable if at least one of the following is true:

- The `while` statement contains a reachable `break` statement that exits the `while` statement.
- The `while` statement is reachable and the Boolean expression does not have the constant value `true`.

### 13.9.3 The do statement

The `do` statement conditionally executes an embedded statement one or more times.

```ANTLR
do_statement
    : 'do' embedded_statement 'while' '(' boolean_expression ')' ';'
    ;
```

A `do` statement is executed as follows:

- Control is transferred to the embedded statement.
- When and if control reaches the end point of the embedded statement (possibly from execution of a `continue` statement), the *boolean_expression* ([§12.24](12-expressions.md#1224-boolean-expressions)) is evaluated. If the Boolean expression yields `true`, control is transferred to the beginning of the `do` statement. Otherwise, control is transferred to the end point of the `do` statement.

Within the embedded statement of a `do` statement, a `break` statement ([§13.10.2](13-statements.md#13102-the-break-statement)) may be used to transfer control to the end point of the `do` statement (thus ending iteration of the embedded statement), and a `continue` statement ([§13.10.3](13-statements.md#13103-the-continue-statement)) may be used to transfer control to the end point of the embedded statement (thus performing another iteration of the `do` statement).

The embedded statement of a `do` statement is reachable if the `do` statement is reachable.

The end point of a `do` statement is reachable if at least one of the following is true:

- The `do` statement contains a reachable `break` statement that exits the `do` statement.
- The end point of the embedded statement is reachable and the Boolean expression does not have the constant value `true`.

### 13.9.4 The for statement

The `for` statement evaluates a sequence of initialization expressions and then, while a condition is true, repeatedly executes an embedded statement and evaluates a sequence of iteration expressions.

```ANTLR
for_statement
    : 'for' '(' for_initializer? ';' for_condition? ';' for_iterator? ')'
      embedded_statement
    ;

for_initializer
    : local_variable_declaration
    | statement_expression_list
    ;

for_condition
    : boolean_expression
    ;

for_iterator
    : statement_expression_list
    ;

statement_expression_list
    : statement_expression (',' statement_expression)*
    ;
```

The *for_initializer*, if present, consists of either a *local_variable_declaration* ([§13.6.2](13-statements.md#1362-local-variable-declarations)) or a list of *statement_expression*s ([§13.7](13-statements.md#137-expression-statements)) separated by commas. The scope of a local variable declared by a *for_initializer* is the *for_initializer*, *for_condition*, *for_iterator*, and *embedded_statement*.

The *for_condition*, if present, shall be a *boolean_expression* ([§12.24](12-expressions.md#1224-boolean-expressions)).

The *for_iterator*, if present, consists of a list of *statement_expression*s ([§13.7](13-statements.md#137-expression-statements)) separated by commas.

A `for` statement is executed as follows:

- If a *for_initializer* is present, the variable initializers or statement expressions are executed in the order they are written. This step is only performed once.
- If a *for_condition* is present, it is evaluated.
- If the *for_condition* is not present or if the evaluation yields `true`, control is transferred to the embedded statement. When and if control reaches the end point of the embedded statement (possibly from execution of a `continue` statement), the expressions of the *for_iterator*, if any, are evaluated in sequence, and then another iteration is performed, starting with evaluation of the *for_condition* in the step above.
- If the *for_condition* is present and the evaluation yields `false`, control is transferred to the end point of the `for` statement.

Within the embedded statement of a `for` statement, a `break` statement ([§13.10.2](13-statements.md#13102-the-break-statement)) may be used to transfer control to the end point of the `for` statement (thus ending iteration of the embedded statement), and a `continue` statement ([§13.10.3](13-statements.md#13103-the-continue-statement)) may be used to transfer control to the end point of the embedded statement (thus executing the *for_iterator* and performing another iteration of the `for` statement, starting with the *for_condition*).

The embedded statement of a `for` statement is reachable if one of the following is true:

- The `for` statement is reachable and no *for_condition* is present.
- The `for` statement is reachable and a *for_condition* is present and does not have the constant value `false`.

The end point of a `for` statement is reachable if at least one of the following is true:

- The `for` statement contains a reachable `break` statement that exits the `for` statement.
- The `for` statement is reachable and a *for_condition* is present and does not have the constant value `true`.

### 13.9.5 The foreach statement

The `foreach` statement enumerates the elements of a collection, executing an embedded statement for each element of the collection.

```ANTLR
foreach_statement
    : 'foreach' '(' ref_kind? local_variable_type identifier 'in' 
      expression ')' embedded_statement
    ;
```

The *local_variable_type* and *identifier* of a foreach statement declare the ***iteration variable*** of the statement. If the `var` identifier is given as the *local_variable_type*, and no type named `var` is in scope, the iteration variable is said to be an ***implicitly typed iteration variable***, and its type is taken to be the element type of the `foreach` statement, as specified below.

If the *foreach_statement* contains both or neither `ref` and `readonly`, the iteration variable denotes a variable that is treated as read-only. Otherwise, if *foreach_statement* contains `ref` without `readonly`, the iteration variable denotes a variable that shall be writable.

The iteration variable corresponds to a local variable with a scope that extends over the embedded statement. During execution of a `foreach` statement, the iteration variable represents the collection element for which an iteration is currently being performed. If the iteration variable denotes a read-only variable, a compile-time error occurs if the embedded statement attempts to modify it (via assignment or the `++` and `--` operators) or pass it as a `ref` or `out` parameter.

In the following, for brevity, `IEnumerable`, `IEnumerator`, `IEnumerable<T>` and `IEnumerator<T>` refer to the corresponding types in the namespaces `System.Collections` and `System.Collections.Generic`.

The compile-time processing of a `foreach` statement first determines the ***collection type***, ***enumerator type*** and ***iteration type*** of the expression. This determination proceeds as follows:

- If the type `X` of *expression* is an array type then there is an implicit reference conversion from X to the `IEnumerable` interface (since `System.Array` implements this interface). The collection type is the `IEnumerable` interface, the enumerator type is the `IEnumerator` interface and the iteration type is the element type of the array type `X`.
- If the type `X` of *expression* is `dynamic` then there is an implicit conversion from *expression* to the `IEnumerable` interface ([§10.2.10](10-conversions.md#10210-implicit-dynamic-conversions)). The collection type is the `IEnumerable` interface and the enumerator type is the `IEnumerator` interface. If the `var` identifier is given as the *local_variable_type* then the iteration type is `dynamic`, otherwise it is `object`.
- Otherwise, determine whether the type `X` has an appropriate `GetEnumerator` method:
  - Perform member lookup on the type `X` with identifier `GetEnumerator` and no type arguments. If the member lookup does not produce a match, or it produces an ambiguity, or produces a match that is not a method group, check for an enumerable interface as described below. It is recommended that a warning be issued if member lookup produces anything except a method group or no match.
  - Perform overload resolution using the resulting method group and an empty argument list. If overload resolution results in no applicable methods, results in an ambiguity, or results in a single best method but that method is either static or not public, check for an enumerable interface as described below. It is recommended that a warning be issued if overload resolution produces anything except an unambiguous public instance method or no applicable methods.
  - If the return type `E` of the `GetEnumerator` method is not a class, struct or interface type, an error is produced and no further steps are taken.
  - Member lookup is performed on `E` with the identifier `Current` and no type arguments. If the member lookup produces no match, the result is an error, or the result is anything except a public instance property that permits reading, an error is produced and no further steps are taken.
  - Member lookup is performed on `E` with the identifier `MoveNext` and no type arguments. If the member lookup produces no match, the result is an error, or the result is anything except a method group, an error is produced and no further steps are taken.
  - Overload resolution is performed on the method group with an empty argument list. If overload resolution results in no applicable methods, results in an ambiguity, or results in a single best method but that method is either static or not public, or its return type is not `bool`, an error is produced and no further steps are taken.
  - The collection type is `X`, the enumerator type is `E`, and the iteration type is the type of the `Current` property. The `Current` property may include the `ref` modifier, in which case, the expression returned is a *variable_reference* ([§9.5](9-variables.md#95-variable-references)) that is optionally read-only.
- Otherwise, check for an enumerable interface:
  - If among all the types `Tᵢ` for which there is an implicit conversion from `X` to `IEnumerable<Tᵢ>`, there is a unique type `T` such that `T` is not `dynamic` and for all the other `Tᵢ` there is an implicit conversion from `IEnumerable<T>` to `IEnumerable<Tᵢ>`, then the collection type is the interface `IEnumerable<T>`, the enumerator type is the interface `IEnumerator<T>`, and the iteration type is `T`.
  - Otherwise, if there is more than one such type `T`, then an error is produced and no further steps are taken.
  - Otherwise, if there is an implicit conversion from `X` to the `System.Collections.IEnumerable` interface, then the collection type is this interface, the enumerator type is the interface `System.Collections.IEnumerator`, and the iteration type is `object`.
  - Otherwise, an error is produced and no further steps are taken.

The above steps, if successful, unambiguously produce a collection type `C`, enumerator type `E` and iteration type `T`, `ref T`, or `ref readonly T`. A `foreach` statement of the form

```csharp
foreach (V v in x) «embedded_statement»
```

is then equivalent to:

```csharp
{
    E e = ((C)(x)).GetEnumerator();
    try
    {
        while (e.MoveNext())
        {
            V v = (V)(T)e.Current;
            «embedded_statement»
        }
    }
    finally
    {
        ... // Dispose e
    }
}
```

The variable `e` is not visible to or accessible to the expression `x` or the embedded statement or any other source code of the program. The variable `v` is read-only in the embedded statement. If there is not an explicit conversion ([§10.3](10-conversions.md#103-explicit-conversions)) from `T` (the iteration type) to `V` (the *local_variable_type* in the `foreach` statement), an error is produced and no further steps are taken.

When the iteration variable is a reference variable ([§9.7](9-variables.md#97-reference-variables-and-returns)), a `foreach` statement of the form

```csharp
foreach (ref V v in x) «embedded_statement»
```

is then equivalent to:

```csharp
{
    E e = ((C)(x)).GetEnumerator();
    try
    {
        while (e.MoveNext())
        {
            ref V v = ref e.Current;
            «embedded_statement»
        }
    }
    finally
    {
        ... // Dispose e
    }
}
```

The variable `e` is not visible or accessible to the expression `x` or the embedded statement or any other source code of the program. The reference variable `v` is read-write in the embedded statement, but `v` shall not be ref-reassigned ([§12.21.3](12-expressions.md#12213-ref-assignment)). If there is not an identity conversion ([§10.2.2](10-conversions.md#1022-identity-conversion)) from `T` (the iteration type) to `V` (the *local_variable_type* in the `foreach` statement), an error is produced and no further steps are taken.

A `foreach` statement of the form `foreach (ref readonly V v in x) «embedded_statement»` has a similar equivalent form, but the reference variable `v` is `ref readonly` in the embedded statement, and therefore cannot be ref-reassigned or reassigned.

> *Note*: If `x` has the value `null`, a `System.NullReferenceException` is thrown at run-time. *end note*

An implementation is permitted to implement a given *foreach_statement* differently; e.g., for performance reasons, as long as the behavior is consistent with the above expansion.

The placement of `v` inside the `while` loop is important for how it is captured ([§12.19.6.2](12-expressions.md#121962-captured-outer-variables)) by any anonymous function occurring in the *embedded_statement*.

> *Example*:
>
> <!-- Example: {template:"code-in-main", name:"ForeachStatement1", expectedOutput:["First value: 7"]} -->
> ```csharp
> int[] values = { 7, 9, 13 };
> Action f = null;
> foreach (var value in values)
> {
>     if (f == null)
>     {
>         f = () => Runtime.Log("First value: " + value);
>     }
> }
> f();
> ```
>
> If `v` in the expanded form were declared outside of the `while` loop, it would be shared among all iterations, and its value after the `for` loop would be the final value, `13`, which is what the invocation of `f` would print. Instead, because each iteration has its own variable `v`, the one captured by `f` in the first iteration will continue to hold the value `7`, which is what will be printed. (Note that earlier versions of C# declared `v` outside of the `while` loop.)
>
> *end example*

The body of the `finally` block is constructed according to the following steps:

- If there is an implicit conversion from `E` to the `System.IDisposable` interface, then
  - If `E` is a non-nullable value type then the `finally` clause is expanded to the semantic equivalent of:

    ```csharp
    finally
    {
        ((System.IDisposable)e).Dispose();
    }
    ```

  - Otherwise the `finally` clause is expanded to the semantic equivalent of:

    ```csharp
    finally
    {
        System.IDisposable d = e as System.IDisposable;
        if (d != null)
        {
            d.Dispose();
        }
    }
    ```

    except that if `E` is a value type, or a type parameter instantiated to a value type, then the conversion of `e` to `System.IDisposable` shall not cause boxing to occur.
- Otherwise, if `E` is a sealed type, the `finally` clause is expanded to an empty block:

  ```csharp
  finally {}
  ```

- Otherwise, the `finally` clause is expanded to:
  
  ```csharp
  finally
  {
      System.IDisposable d = e as System.IDisposable;
      if (d != null)
      {
          d.Dispose();
      }
  }
  ```

The local variable `d` is not visible to or accessible to any user code. In particular, it does not conflict with any other variable whose scope includes the `finally` block.

The order in which `foreach` traverses the elements of an array, is as follows: For single-dimensional arrays elements are traversed in increasing index order, starting with index 0 and ending with index `Length – 1`. For multi-dimensional arrays, elements are traversed such that the indices of the rightmost dimension are increased first, then the next left dimension, and so on to the left.

> *Example*: The following example prints out each value in a two-dimensional array, in element order:
>
> <!-- Example: {template:"standalone-console", name:"ForeachStatement2", replaceEllipsis:true, inferOutput:true} -->
> ```csharp
> class Test : SmartContract.Framework.SmartContract
> {
>     public static void Test()
>     {
>         double[,] values =
>         {
>             {1.2, 2.3, 3.4, 4.5},
>             {5.6, 6.7, 7.8, 8.9}
>         };
>         foreach (double elementValue in values)
>         {
>             Console.Write($"{elementValue} ");
>         }
>         Runtime.Log();
>     }
> }
> ```
>
> The output produced is as follows:
>
> ```console
> 1.2 2.3 3.4 4.5 5.6 6.7 7.8 8.9
> ```
>
> *end example*
<!-- markdownlint-disable MD028 -->

<!-- markdownlint-enable MD028 -->
> *Example*: In the following example
>
> <!-- Example: {template:"standalone-console", name:"ForeachStatement3", expectedOutput:["1","3","5","7","9"]} -->
> ```csharp
> int[] numbers = { 1, 3, 5, 7, 9 };
> foreach (var n in numbers)
> {
>     Runtime.Log(n);
> }
> ```
>
> the type of `n` is inferred to be `int`, the iteration type of `numbers`.
>
> *end example*

## 13.10 Jump statements

### 13.10.1 General

Jump statements unconditionally transfer control.

```ANTLR
jump_statement
    : break_statement
    | continue_statement
    | goto_statement
    | return_statement
    | throw_statement
    ;
```

The location to which a jump statement transfers control is called the ***target*** of the jump statement.

When a jump statement occurs within a block, and the target of that jump statement is outside that block, the jump statement is said to ***exit*** the block. While a jump statement can transfer control out of a block, it can never transfer control into a block.

Execution of jump statements is complicated by the presence of intervening `try` statements. In the absence of such `try` statements, a jump statement unconditionally transfers control from the jump statement to its target. In the presence of such intervening `try` statements, execution is more complex. If the jump statement exits one or more `try` blocks with associated `finally` blocks, control is initially transferred to the `finally` block of the innermost `try` statement. When and if control reaches the end point of a `finally` block, control is transferred to the `finally` block of the next enclosing `try` statement. This process is repeated until the `finally` blocks of all intervening `try` statements have been executed.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"JumpStatements", inferOutput:true} -->
> ```csharp
> class Test : SmartContract.Framework.SmartContract
> {
>     public static void Test()
>     {
>         while (true)
>         {
>             try
>             {
>                 try
>                 {
>                     Runtime.Log("Before break");
>                     break;
>                 }
>                 finally
>                 {
>                     Runtime.Log("Innermost finally block");
>                 }
>             }
>             finally
>             {
>                 Runtime.Log("Outermost finally block");
>             }
>         }
>         Runtime.Log("After break");
>     }
> }
> ```
>
> the `finally` blocks associated with two `try` statements are executed before control is transferred to the target of the jump statement.
> The output produced is as follows:
>
> ```console
> Before break
> Innermost finally block
> Outermost finally block
> After break
> ```
>
> *end example*

### 13.10.2 The break statement

The `break` statement exits the nearest enclosing `switch`, `while`, `do`, `for`, or `foreach` statement.

```ANTLR
break_statement
    : 'break' ';'
    ;
```

The target of a `break` statement is the end point of the nearest enclosing `switch`, `while`, `do`, `for`, or `foreach` statement. If a `break` statement is not enclosed by a `switch`, `while`, `do`, `for`, or `foreach` statement, a compile-time error occurs.

When multiple `switch`, `while`, `do`, `for`, or `foreach` statements are nested within each other, a `break` statement applies only to the innermost statement. To transfer control across multiple nesting levels, a `goto` statement ([§13.10.4](13-statements.md#13104-the-goto-statement)) shall be used.

A `break` statement cannot exit a `finally` block ([§13.11](13-statements.md#1311-the-try-statement)). When a `break` statement occurs within a `finally` block, the target of the `break` statement shall be within the same `finally` block; otherwise a compile-time error occurs.

A `break` statement is executed as follows:

- If the `break` statement exits one or more `try` blocks with associated `finally` blocks, control is initially transferred to the `finally` block of the innermost `try` statement. When and if control reaches the end point of a `finally` block, control is transferred to the `finally` block of the next enclosing `try` statement. This process is repeated until the `finally` blocks of all intervening `try` statements have been executed.
- Control is transferred to the target of the `break` statement.

Because a `break` statement unconditionally transfers control elsewhere, the end point of a `break` statement is never reachable.

### 13.10.3 The continue statement

The `continue` statement starts a new iteration of the nearest enclosing `while`, `do`, `for`, or `foreach` statement.

```ANTLR
continue_statement
    : 'continue' ';'
    ;
```

The target of a `continue` statement is the end point of the embedded statement of the nearest enclosing `while`, `do`, `for`, or `foreach` statement. If a `continue` statement is not enclosed by a `while`, `do`, `for`, or `foreach` statement, a compile-time error occurs.

When multiple `while`, `do`, `for`, or `foreach` statements are nested within each other, a `continue` statement applies only to the innermost statement. To transfer control across multiple nesting levels, a `goto` statement ([§13.10.4](13-statements.md#13104-the-goto-statement)) shall be used.

A `continue` statement cannot exit a `finally` block ([§13.11](13-statements.md#1311-the-try-statement)). When a `continue` statement occurs within a `finally` block, the target of the `continue` statement shall be within the same `finally` block; otherwise a compile-time error occurs.

A `continue` statement is executed as follows:

- If the `continue` statement exits one or more `try` blocks with associated `finally` blocks, control is initially transferred to the `finally` block of the innermost `try` statement. When and if control reaches the end point of a `finally` block, control is transferred to the `finally` block of the next enclosing `try` statement. This process is repeated until the `finally` blocks of all intervening `try` statements have been executed.
- Control is transferred to the target of the `continue` statement.

Because a `continue` statement unconditionally transfers control elsewhere, the end point of a `continue` statement is never reachable.

### 13.10.4 The goto statement

The `goto` statement transfers control to a statement that is marked by a label.

```ANTLR
goto_statement
    : 'goto' identifier ';'
    | 'goto' 'case' constant_expression ';'
    | 'goto' 'default' ';'
    ;
```

The target of a `goto` *identifier* statement is the labeled statement with the given label. If a label with the given name does not exist in the current function member, or if the `goto` statement is not within the scope of the label, a compile-time error occurs.

> *Note*: This rule permits the use of a `goto` statement to transfer control *out of* a nested scope, but not *into* a nested scope. In the example
>
> <!-- Example: {template:"standalone-console", name:"GotoStatement"} -->
> ```csharp
> class Test : SmartContract.Framework.SmartContract
> {
>     static void Main(string[] args)
>     {
>         string[,] table =
>         {
>             {"Red", "Blue", "Green"},
>             {"Monday", "Wednesday", "Friday"}
>         };
>         foreach (string str in args)
>         {
>             int row, colm;
>             for (row = 0; row <= 1; ++row)
>             {
>                 for (colm = 0; colm <= 2; ++colm)
>                 {
>                     if (str == table[row,colm])
>                     {
>                         goto done;
>                     }
>                 }
>             }
>             Runtime.Log($"{str} not found");
>             continue;
>           done:
>             Runtime.Log($"Found {str} at [{row}][{colm}]");
>         }
>     }
> }
> ```
>
> a `goto` statement is used to transfer control out of a nested scope.
>
> *end note*

The target of a `goto case` statement is the statement list in the immediately enclosing `switch` statement ([§13.8.3](13-statements.md#1383-the-switch-statement)) which contains a `case` label with a constant pattern of the given constant value and no guard. If the `goto case` statement is not enclosed by a `switch` statement, if the nearest enclosing `switch` statement does not contain such a `case`, or if the *constant_expression* is not implicitly convertible ([§10.2](10-conversions.md#102-implicit-conversions)) to the governing type of the nearest enclosing `switch` statement, a compile-time error occurs.

The target of a `goto default` statement is the statement list in the immediately enclosing `switch` statement ([§13.8.3](13-statements.md#1383-the-switch-statement)), which contains a `default` label. If the `goto default` statement is not enclosed by a `switch` statement, or if the nearest enclosing `switch` statement does not contain a `default` label, a compile-time error occurs.

A `goto` statement cannot exit a `finally` block ([§13.11](13-statements.md#1311-the-try-statement)). When a `goto` statement occurs within a `finally` block, the target of the `goto` statement shall be within the same `finally` block, or otherwise a compile-time error occurs.

A `goto` statement is executed as follows:

- If the `goto` statement exits one or more `try` blocks with associated `finally` blocks, control is initially transferred to the `finally` block of the innermost `try` statement. When and if control reaches the end point of a `finally` block, control is transferred to the `finally` block of the next enclosing `try` statement. This process is repeated until the `finally` blocks of all intervening `try` statements have been executed.
- Control is transferred to the target of the `goto` statement.

Because a `goto` statement unconditionally transfers control elsewhere, the end point of a `goto` statement is never reachable.

### 13.10.5 The return statement

The `return` statement returns control to the current caller of the function member in which the return statement appears, optionally returning a value or a *variable_reference* ([§9.5](9-variables.md#95-variable-references)).

```ANTLR
return_statement
    : 'return' ';'
    | 'return' expression ';'
    | 'return' 'ref' variable_reference ';'
    ;
```

A *return_statement* without *expression* is called a ***return-no-value***; one containing `ref` *expression* is called a ***return-by-ref***; and one containing only *expression* is called a ***return-by-value***.

It is a compile-time error to use a return-no-value from a method declared as being returns-by-value or returns-by-ref ([§15.6.1](15-classes.md#1561-general)).

It is a compile-time error to use a return-by-ref from a method declared as being returns-no-value or returns-by-value.

It is a compile-time error to use a return-by-value from a method declared as being returns-no-value or returns-by-ref.

It is a compile-time error to use a return-by-ref if *expression* is not a *variable_reference* or is a reference to a variable whose ref-safe-context is not caller-context ([§9.7.2](9-variables.md#972-ref-safe-contexts)).

A function member is said to ***compute a value*** if it is a method with a returns-by-value method ([§15.6.11](15-classes.md#15611-method-body)), a returns-by-value get accessor of a property or indexer, or a user-defined operator. Function members that are returns-no-value do not compute a value and are methods with the effective return type `void`, set accessors of properties and indexers, add and remove accessors of events, instance constructors, static constructors and finalizers. Function members that are returns-by-ref do not compute a value.

For a return-by-value, an implicit conversion ([§10.2](10-conversions.md#102-implicit-conversions)) shall exist from the type of *expression* to the effective return type ([§15.6.11](15-classes.md#15611-method-body)) of the containing function member. For a return-by-ref, an identity conversion ([§10.2.2](10-conversions.md#1022-identity-conversion)) shall exist between the type of *expression* and the effective return type of the containing function member.

`return` statements can also be used in the body of anonymous function expressions ([§12.19](12-expressions.md#1219-anonymous-function-expressions)), and participate in determining which conversions exist for those functions ([§10.7.1](10-conversions.md#1071-general)).

It is a compile-time error for a `return` statement to appear in a `finally` block ([§13.11](13-statements.md#1311-the-try-statement)).

A `return` statement is executed as follows:

- For a return-by-value, *expression* is evaluated and its value is converted to the effective return type of the containing function by an implicit conversion. The result of the conversion becomes the result value produced by the function. For a return-by-ref, the *expression* is evaluated, and the result shall be classified as a variable. If the enclosing method’s return-by-ref includes `readonly`, the resulting variable is read-only.
- If the `return` statement is enclosed by one or more `try` or `catch` blocks with associated `finally` blocks, control is initially transferred to the `finally` block of the innermost `try` statement. When and if control reaches the end point of a `finally` block, control is transferred to the `finally` block of the next enclosing `try` statement. This process is repeated until the `finally` blocks of all enclosing `try` statements have been executed.

Because a `return` statement unconditionally transfers control elsewhere, the end point of a `return` statement is never reachable.

### 13.10.6 The throw statement

The `throw` statement throws an exception.

```ANTLR
throw_statement
    : 'throw' expression? ';'
    ;
```

A `throw` statement with an expression throws an exception produced by evaluating the expression. The expression shall be implicitly convertible to `System.Exception`, and the result of evaluating the expression is converted to `System.Exception` before being thrown. If the result of the conversion is `null`, a `System.NullReferenceException` is thrown instead.

A `throw` statement with no expression can be used only in a `catch` block, in which case, that statement re-throws the exception that is currently being handled by that `catch` block.

Because a `throw` statement unconditionally transfers control elsewhere, the end point of a `throw` statement is never reachable.

When an exception is thrown, control is transferred to the first `catch` clause in an enclosing `try` statement that can handle the exception. The process that takes place from the point of the exception being thrown to the point of transferring control to a suitable exception handler is known as ***exception propagation***. Propagation of an exception consists of repeatedly evaluating the following steps until a `catch` clause that matches the exception is found. In this description, the ***throw point*** is initially the location at which the exception is thrown. This behavior is specified in ([§21.4](21-exceptions.md#214-how-exceptions-are-handled)).

- In the current function member, each `try` statement that encloses the throw point is examined. For each statement `S`, starting with the innermost `try` statement and ending with the outermost `try` statement, the following steps are evaluated:

  - If the `try` block of `S` encloses the throw point and if `S` has one or more `catch` clauses, the `catch` clauses are examined in order of appearance to locate a suitable handler for the exception. The first `catch` clause that specifies an exception type `T` (or a type parameter that at run-time denotes an exception type `T`) such that the run-time type of `E` derives from `T` is considered a match. If the clause contains an exception filter, the exception object is assigned to the exception variable, and the exception filter is evaluated. When a `catch` clause contains an exception filter, that `catch` clause is considered a match if the exception filter evaluates to `true`. A general `catch` ([§13.11](13-statements.md#1311-the-try-statement)) clause is considered a match for any exception type. If a matching `catch` clause is located, the exception propagation is completed by transferring control to the block of that `catch` clause.
  - Otherwise, if the `try` block or a `catch` block of `S` encloses the throw point and if `S` has a `finally` block, control is transferred to the `finally` block. If the `finally` block throws another exception, processing of the current exception is terminated. Otherwise, when control reaches the end point of the `finally` block, processing of the current exception is continued.
- If an exception handler was not located in the current function invocation, the function invocation is terminated, and one of the following occurs:
- If the exception processing terminates all function member invocations. The impact of such termination is implementation-defined.

## 13.11 The try statement

The `try` statement provides a mechanism for catching exceptions that occur during execution of a block. Furthermore, the `try` statement provides the ability to specify a block of code that is always executed when control leaves the `try` statement.

```ANTLR
try_statement
    : 'try' block catch_clauses
    | 'try' block catch_clauses? finally_clause
    ;

catch_clauses
    : specific_catch_clause+
    | specific_catch_clause* general_catch_clause
    ;

specific_catch_clause
    : 'catch' exception_specifier exception_filter? block
    | 'catch' exception_filter block
    ;

exception_specifier
    : '(' type identifier? ')'
    ;

exception_filter
    : 'when' '(' boolean_expression ')'
    ;

general_catch_clause
    : 'catch' block
    ;

finally_clause
    : 'finally' block
    ;
```

A *try_statement* consists of the keyword `try` followed by a *block*, then zero or more *catch_clauses*, then an optional *finally_clause*. There shall be at least one *catch_clause* or a *finally_clause*.

In an *exception_specifier* the *type*, or its effective base class if it is a *type_parameter*, shall be `System.Exception` or a type that derives from it.

When a `catch` clause specifies both a *class_type* and an *identifier*, an ***exception variable*** of the given name and type is declared. The exception variable is introduced into the declaration space of the *specific_catch_clause* ([§7.3](7-basic-concepts.md#73-declarations)). During execution of the *exception_filter* and `catch` block, the exception variable represents the exception currently being handled. For purposes of definite assignment checking, the exception variable is considered definitely assigned in its entire scope.

Unless a `catch` clause includes an exception variable name, it is impossible to access the exception object in the filter and `catch` block.

A `catch` clause that specifies neither an exception type nor an exception variable name is called a general `catch` clause. A `try` statement can only have one general `catch` clause, and, if one is present, it shall be the last `catch` clause.

> *Note*: Some programming languages might support exceptions that are not representable as an object derived from `System.Exception`, although such exceptions could never be generated by C# code. A general `catch` clause might be used to catch such exceptions. Thus, a general `catch` clause is semantically different from one that specifies the type `System.Exception`, in that the former might also catch exceptions from other languages. *end note*

In order to locate a handler for an exception, `catch` clauses are examined in lexical order. If a `catch` clause specifies a type but no exception filter, it is a compile-time error for a later `catch` clause of the same `try` statement to specify a type that is the same as, or is derived from, that type.

> *Note*: Without this restriction, it would be possible to write unreachable `catch` clauses. *end note*

Within a `catch` block, a `throw` statement ([§13.10.6](13-statements.md#13106-the-throw-statement)) with no expression can be used to re-throw the exception that was caught by the `catch` block. Assignments to an exception variable do not alter the exception that is re-thrown.

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"TryStatement1", inferOutput:true} -->
> ```csharp
> class Test : SmartContract.Framework.SmartContract
> {
>     static void F()
>     {
>         try
>         {
>             G();
>         }
>         catch (Exception e)
>         {
>             Runtime.Log("Exception in F: " + e.Message);
>             e = new Exception("F");
>             throw; // re-throw
>         }
>     }
>
>     static void G() => throw new Exception("G");
>
>     public static void Test()
>     {
>         try
>         {
>             F();
>         }
>         catch (Exception e)
>         {
>             Runtime.Log("Exception in Main: " + e.Message);
>         }
>     }
> }
> ```
>
> the method `F` catches an exception, writes some diagnostic information to the console, alters the exception variable, and re-throws the exception. The exception that is re-thrown is the original exception, so the output produced is:
>
> ```console
> Exception in F: G
> Exception in Main: G
> ```
>
> If the first `catch` block had thrown `e` instead of rethrowing the current exception, the output produced would be as follows:
>
> ```console
> Exception in F: G
> Exception in Main: F
> ```
>
> *end example*

It is a compile-time error for a `break`, `continue`, or `goto` statement to transfer control out of a `finally` block. When a `break`, `continue`, or `goto` statement occurs in a `finally` block, the target of the statement shall be within the same `finally` block, or otherwise a compile-time error occurs.

It is a compile-time error for a `return` statement to occur in a `finally` block.

When execution reaches a `try` statement, control is transferred to the `try` block. If control reaches the end point of the `try` block without an exception being propagated, control is transferred to the `finally` block if one exists. If no `finally` block exists, control is transferred to the end point of the `try` statement.

If an exception has been propagated, the `catch` clauses, if any, are examined in lexical order seeking the first match for the exception. The search for a matching `catch` clause continues with all enclosing blocks as described in [§13.10.6](13-statements.md#13106-the-throw-statement). A `catch` clause is a match if the exception type matches any *exception_specifier* and any *exception_filter* is true. A `catch` clause without an *exception_specifier* matches any exception type. The exception type matches the *exception_specifier* when the *exception_specifier* specifies the exception type or a base type of the exception type. If the clause contains an exception filter, the exception object is assigned to the exception variable, and the exception filter is evaluated.

If an exception has been propagated and a matching `catch` clause is found, control is transferred to the first matching `catch` block. If control reaches the end point of the `catch` block without an exception being propagated, control is transferred to the `finally` block if one exists. If no `finally` block exists, control is transferred to the end point of the `try` statement. If an exception has been propagated from the `catch` block, control transfers to the `finally` block if one exists. The exception is propagated to the next enclosing `try` statement.

If an exception has been propagated, and no matching `catch` clause is found, control transfers to the `finally` block, if it exists. The exception is propagated to the next enclosing `try` statement.

The statements of a `finally` block are always executed when control leaves a `try` statement. This is true whether the control transfer occurs as a result of normal execution, as a result of executing a `break`, `continue`, `goto`, or `return` statement, or as a result of propagating an exception out of the `try` statement. If control reaches the end point of the `finally` block without an exception being propagated, control is transferred to the end point of the `try` statement.

If an exception is thrown during execution of a `finally` block, and is not caught within the same `finally` block, the exception is propagated to the next enclosing `try` statement. If another exception was in the process of being propagated, that exception is lost. The process of propagating an exception is discussed further in the description of the `throw` statement ([§13.10.6](13-statements.md#13106-the-throw-statement)).

> *Example*: In the following code
>
> <!-- Example: {template:"standalone-console", name:"TryStatement2", inferOutput:true} -->
> ```csharp
> public class Test
> {
>     public static void Test()
>     {
>         try
>         {
>             Method();
>         }
>         catch (Exception ex) when (ExceptionFilter(ex))
>         {
>             Runtime.Log("Catch");
>         }
> 
>         bool ExceptionFilter(Exception ex)
>         {
>             Runtime.Log("Filter");
>             return true;
>         }
>     }
> 
>     static void Method()
>     {
>         try
>         {
>             throw new ArgumentException();
>         }
>         finally
>         {
>             Runtime.Log("Finally");
>         }
>     }
> }
> ```
>
> the method `Method` throws an exception. The first action is to examine the enclosing `catch` clauses, executing any *exception filters*. Then, the `finally` clause in `Method` executes before control transfers to the enclosing matching `catch` clause. The resulting output is:
>
> ```console
> Filter
> Finally
> Catch
> ```
>
> *end example*

The `try` block of a `try` statement is reachable if the `try` statement is reachable.

A `catch` block of a `try` statement is reachable if the `try` statement is reachable.

The `finally` block of a `try` statement is reachable if the `try` statement is reachable.

The end point of a `try` statement is reachable if both of the following are true:

- The end point of the `try` block is reachable or the end point of at least one `catch` block is reachable.
- If a `finally` block is present, the end point of the `finally` block is reachable.

## 13.12 The checked and unchecked statements

The `checked` and `unchecked` statements are used to control the ***overflow-checking context*** for integral-type arithmetic operations and conversions.

```ANTLR
checked_statement
    : 'checked' block
    ;

unchecked_statement
    : 'unchecked' block
    ;
```

The `checked` statement causes all expressions in the *block* to be evaluated in a checked context, and the `unchecked` statement causes all expressions in the *block* to be evaluated in an unchecked context.

The `checked` and `unchecked` statements are precisely equivalent to the `checked` and `unchecked` operators ([§12.8.19](12-expressions.md#12819-the-checked-and-unchecked-operators)), except that they operate on blocks instead of expressions.


## 13.15 The yield statement

The `yield` statement is used in an iterator block ([§13.3](13-statements.md#133-blocks)) to yield a value to the enumerator object ([§15.14.5](15-classes.md#15145-enumerator-objects)) or enumerable object ([§15.14.6](15-classes.md#15146-enumerable-objects)) of an iterator or to signal the end of the iteration.

```ANTLR
yield_statement
    : 'yield' 'return' expression ';'
    | 'yield' 'break' ';'
    ;
```

`yield` is a contextual keyword ([§6.4.4](6-lexical-structure.md#644-keywords)) and has special meaning only when used immediately before a `return` or `break` keyword.

There are several restrictions on where a `yield` statement can appear, as described in the following.

- It is a compile-time error for a `yield` statement (of either form) to appear outside a *method_body*, *operator_body*, or *accessor_body*.
- It is a compile-time error for a `yield` statement (of either form) to appear inside an anonymous function.
- It is a compile-time error for a `yield` statement (of either form) to appear in the `finally` clause of a `try` statement.
- It is a compile-time error for a `yield return` statement to appear anywhere in a `try` statement that contains any *catch_clauses*.

> *Example*: The following example shows some valid and invalid uses of `yield` statements.
>
> <!-- Example: {template:"code-in-class-lib-without-using", name:"YieldStatement", expectedErrors:["CS1625","CS1625","CS1626","CS1631","CS1643","CS1621","CS1624"], expectedWarnings:["CS0162"]} -->
> ```csharp
> delegate IEnumerable<int> D();
>
> IEnumerator<int> GetEnumerator()
> {
>     try
>     {
>         yield return 1; // Ok
>         yield break;    // Ok
>     }
>     finally
>     {
>         yield return 2; // Error, yield in finally
>         yield break;    // Error, yield in finally
>     }
>     try
>     {
>         yield return 3; // Error, yield return in try/catch
>         yield break;    // Ok
>     }
>     catch
>     {
>         yield return 4; // Error, yield return in try/catch
>         yield break;    // Ok
>     }
>     D d = delegate
>     {
>         yield return 5; // Error, yield in an anonymous function
>     };
> }
>
> int MyMethod()
> {
>     yield return 1;     // Error, wrong return type for an iterator block
> }
> ```
>
> *end example*

An implicit conversion ([§10.2](10-conversions.md#102-implicit-conversions)) shall exist from the type of the expression in the `yield return` statement to the yield type ([§15.14.4](15-classes.md#15144-yield-type)) of the iterator.

A `yield return` statement is executed as follows:

- The expression given in the statement is evaluated, implicitly converted to the yield type, and assigned to the `Current` property of the enumerator object.
- Execution of the iterator block is suspended. If the `yield return` statement is within one or more `try` blocks, the associated `finally` blocks are *not* executed at this time.
- The `MoveNext` method of the enumerator object returns `true` to its caller, indicating that the enumerator object successfully advanced to the next item.

The next call to the enumerator object’s `MoveNext` method resumes execution of the iterator block from where it was last suspended.

A `yield break` statement is executed as follows:

- If the `yield break` statement is enclosed by one or more `try` blocks with associated `finally` blocks, control is initially transferred to the `finally` block of the innermost `try` statement. When and if control reaches the end point of a `finally` block, control is transferred to the `finally` block of the next enclosing `try` statement. This process is repeated until the `finally` blocks of all enclosing `try` statements have been executed.
- Control is returned to the caller of the iterator block. This is either the `MoveNext` method or `Dispose` method of the enumerator object.

Because a `yield break` statement unconditionally transfers control elsewhere, the end point of a `yield break` statement is never reachable.