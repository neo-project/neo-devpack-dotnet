# Unsupported C# Features in Neo Compiler

The versioned syntax checklists flag every feature the Neo compiler currently rejects. This page is generated from those checklists so the status remains accurate.

## Summary by Version

- **C# 1 Syntax Checklist**  
  - Unsafe code blocks (`unsafe_code`)
  - BitOperations helpers (`numerics_bit_operations`)
  - DateTime helpers (`datetime_methods`)
  - TimeSpan helpers (`timespan_methods`)
  - Convert class helpers (`convert_methods`)

- **C# 2 Syntax Checklist**  
  - Anonymous methods (`anonymous_method`)
  - Iterator blocks (`iterator_block`)

- **C# 3 Syntax Checklist**  
  - LINQ query expressions (`query_expression`)

- **C# 4 Syntax Checklist**  
  - Dynamic binding (`dynamic_binding`)

- **C# 5 Syntax Checklist**  
  - Async method declarations (`async_method`)
  - Await expressions (`await_expression`)

- **C# 6 Syntax Checklist**  
  - Exception filters (`exception_filter`)

- **C# 7 Syntax Checklist**  
  - Pattern matching with `is` (`pattern_matching_is`)
  - Local functions (`local_function`)
  - Ref locals and returns (`ref_local`)

- **C# 8 Syntax Checklist**  
  - Index and range operators (`index_and_range`)
  - Async streams (`async_streams`)

- **C# 9 Syntax Checklist**  
  - Native-sized integers (`native_int`)
  - Top-level statements (`top_level_statements`)
  - Function pointers (`function_pointer`)

- **C# 10 Syntax Checklist**  
  - Global using directives (`global_using`)
  - Extended property patterns (`extended_property_pattern`)

- **C# 11 Syntax Checklist**  
  - List patterns (`list_patterns`)
  - UTF-8 string literals (`utf_8_string_literals`)
  - Default interface member implementations (`default_interface_methods`)
  - File-local types (`file_local_types`)
  - Numeric `nint` and `nuint` (`numeric_intptr_and_uintptr`)

- **C# 12 Syntax Checklist**  
  - `ref readonly` parameters (`ref_readonly_parameters`)
  - Interceptors (`interceptors`)

- **C# 13 Syntax Checklist**  
  - New `lock` type semantics (`new_lock_object`)
  - Implicit indexer access in object initializers (`implicit_index_access`)
  - `ref` locals and unsafe contexts in iterators and async methods (`ref_and_unsafe_in_iterators_and_async_methods`)
  - `field` contextual keyword (`the_field_keyword`)
  - Extension types (`extension_types`)

## Next Actions

1. Confirm with the compiler team which gaps are expected versus candidates for future support.
2. File GitHub issues or backlog items for each unsupported feature that should be implemented.
3. Update the version checklists and rerun this script whenever support status changes.
