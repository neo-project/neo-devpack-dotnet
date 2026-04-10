// Copyright (C) 2015-2026 The Neo Project.
//
// RiscVDirectRunner.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Neo.SmartContract.Testing;

/// <summary>
/// Minimal P/Invoke wrapper for directly executing RISC-V contracts via
/// the native neo_riscv_host library. Avoids the full adapter dependency
/// which requires a separate Neo.csproj that conflicts with the NuGet
/// Neo package used by the test framework.
///
/// Struct layouts and function signatures match the Rust FFI in
/// crates/neo-riscv-host/src/ffi.rs exactly.
/// </summary>
public static class RiscVExecutionBridge
{
    // ---------------------------------------------------------------
    //  Native struct layouts (must match repr(C) structs in Rust)
    // ---------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeStackItem
    {
        public uint Kind;           // 0=Integer, 1=ByteString, 2=Null, 3=Boolean, 4=Array, 5=BigInteger, 6=Iterator, 7=Struct, 8=Map, 9=Interop
        public long IntegerValue;
        public IntPtr BytesPtr;
        public nuint BytesLen;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeExecutionResult
    {
        public long FeeConsumedPico;
        public uint State;           // 0=HALT, 1=FAULT
        public IntPtr StackPtr;      // *mut NativeStackItem
        public nuint StackLen;
        public IntPtr ErrorPtr;      // *mut u8 (UTF-8)
        public nuint ErrorLen;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeHostResult
    {
        public IntPtr StackPtr;      // *mut NativeStackItem
        public nuint StackLen;
        public IntPtr ErrorPtr;      // *mut u8 (UTF-8)
        public nuint ErrorLen;
    }

    // ---------------------------------------------------------------
    //  Callback delegate types
    // ---------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool HostCallbackDelegate(
        IntPtr userData,
        uint api,
        nuint instructionPointer,
        byte trigger,
        uint networkMagic,
        byte addressVersion,
        ulong persistingTimestamp,
        long gasLeft,
        IntPtr inputStackPtr,    // *const NativeStackItem
        nuint inputStackLen,
        IntPtr output);          // *mut NativeHostResult

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void HostFreeCallbackDelegate(
        IntPtr userData,
        IntPtr result);          // *mut NativeHostResult

    // ---------------------------------------------------------------
    //  Native function delegate types
    // ---------------------------------------------------------------

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool ExecuteNativeContractDelegate(
        IntPtr binaryPtr,
        nuint binaryLen,
        IntPtr methodPtr,
        nuint methodLen,
        IntPtr initialStackPtr,  // *const NativeStackItem
        nuint initialStackLen,
        byte trigger,
        uint network,
        byte addressVersion,
        ulong timestamp,
        long gasLeft,
        long execFeeFactorPico,
        IntPtr userData,
        IntPtr hostCallback,
        IntPtr hostFree,
        IntPtr output);          // *mut NativeExecutionResult

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FreeExecutionResultDelegate(
        IntPtr result);          // *mut NativeExecutionResult

    // ---------------------------------------------------------------
    //  State
    // ---------------------------------------------------------------

    private static IntPtr s_libraryHandle;
    private static ExecuteNativeContractDelegate? s_executeNativeContract;
    private static FreeExecutionResultDelegate? s_freeExecutionResult;

    // Prevent GC of delegates whose function pointers are passed to native code.
    private static HostCallbackDelegate? s_hostCallbackDelegate;
    private static HostFreeCallbackDelegate? s_hostFreeDelegate;
    private static IntPtr s_hostCallbackPtr;
    private static IntPtr s_hostFreePtr;

    private static bool s_initialized;

    // ---------------------------------------------------------------
    //  Public API
    // ---------------------------------------------------------------

    /// <summary>
    /// True when the native library was loaded and all symbols resolved.
    /// </summary>
    public static bool IsAvailable => s_initialized;

    /// <summary>
    /// Load the native RISC-V host library and resolve symbols.
    /// Safe to call multiple times -- subsequent calls are no-ops.
    /// </summary>
    public static void Initialize(string? libraryPath = null)
    {
        if (s_initialized) return;

        libraryPath ??= FindNativeLibrary();
        if (libraryPath == null || !File.Exists(libraryPath))
            throw new FileNotFoundException(
                "libneo_riscv_host.so not found. Build the Rust host library first.",
                libraryPath ?? "libneo_riscv_host.so");

        s_libraryHandle = NativeLibrary.Load(libraryPath);

        s_executeNativeContract = Marshal.GetDelegateForFunctionPointer<ExecuteNativeContractDelegate>(
            NativeLibrary.GetExport(s_libraryHandle, "neo_riscv_execute_native_contract"));
        s_freeExecutionResult = Marshal.GetDelegateForFunctionPointer<FreeExecutionResultDelegate>(
            NativeLibrary.GetExport(s_libraryHandle, "neo_riscv_free_execution_result"));

        // Pin callback delegates for the lifetime of the process.
        s_hostCallbackDelegate = DummyHostCallback;
        s_hostFreeDelegate = DummyHostFree;
        s_hostCallbackPtr = Marshal.GetFunctionPointerForDelegate(s_hostCallbackDelegate);
        s_hostFreePtr = Marshal.GetFunctionPointerForDelegate(s_hostFreeDelegate);

        s_initialized = true;
    }

    /// <summary>
    /// Result of a RISC-V contract execution.
    /// </summary>
    public sealed class ExecutionResult
    {
        /// <summary>0 = HALT, 1 = FAULT</summary>
        public uint State { get; init; }
        public long FeeConsumedPico { get; init; }
        public string? Error { get; init; }
        /// <summary>Decoded result stack items (kind, raw bytes or integer).</summary>
        public ResultStackItem[] Stack { get; init; } = Array.Empty<ResultStackItem>();

        public bool IsHalt => State == 0;
        public bool IsFault => State != 0;
    }

    /// <summary>
    /// A decoded stack item from the native result.
    /// </summary>
    public sealed class ResultStackItem
    {
        public uint Kind { get; init; }
        public long IntegerValue { get; init; }
        public byte[]? Bytes { get; init; }
        public ResultStackItem[]? Children { get; init; }
    }

    /// <summary>
    /// Execute a native RISC-V contract method with no input arguments.
    /// </summary>
    public static ExecutionResult Execute(byte[] binary, string method)
    {
        return Execute(binary, method, ReadOnlySpan<byte>.Empty, 0);
    }

    /// <summary>
    /// Execute a native RISC-V contract method with a pre-serialized initial stack.
    /// </summary>
    public static ExecutionResult Execute(
        byte[] binary,
        string method,
        ReadOnlySpan<byte> serializedInitialStack,
        int initialStackItemCount)
    {
        if (!s_initialized)
            throw new InvalidOperationException("RiscVDirectRunner.Initialize() has not been called.");

        var binaryHandle = GCHandle.Alloc(binary, GCHandleType.Pinned);
        var methodBytes = Encoding.UTF8.GetBytes(method);
        var methodHandle = GCHandle.Alloc(methodBytes, GCHandleType.Pinned);

        // Allocate the output struct on unmanaged heap so we can pass a stable pointer.
        var resultSize = Marshal.SizeOf<NativeExecutionResult>();
        var resultPtr = Marshal.AllocHGlobal(resultSize);
        // Zero-init so the Rust side sees null pointers if it bails out early.
        {
            var zero = new byte[resultSize];
            Marshal.Copy(zero, 0, resultPtr, resultSize);
        }

        try
        {
            var ok = s_executeNativeContract!(
                binaryHandle.AddrOfPinnedObject(),
                (nuint)binary.Length,
                methodHandle.AddrOfPinnedObject(),
                (nuint)methodBytes.Length,
                IntPtr.Zero,           // no initial stack for now
                0,
                0x40,                  // TriggerType.Application
                860833102u,            // Neo N3 MainNet magic
                53,                    // address version
                0,                     // timestamp (0 = none)
                10_000_000_000_000L,   // 10k GAS in pico
                30_000L,               // exec fee factor pico
                IntPtr.Zero,           // userData (unused in dummy callback)
                s_hostCallbackPtr,
                s_hostFreePtr,
                resultPtr);

            var nativeResult = Marshal.PtrToStructure<NativeExecutionResult>(resultPtr);

            string? error = null;
            if (nativeResult.ErrorPtr != IntPtr.Zero && nativeResult.ErrorLen > 0)
            {
                error = Marshal.PtrToStringUTF8(nativeResult.ErrorPtr, checked((int)nativeResult.ErrorLen));
            }

            var stack = ReadResultStack(nativeResult.StackPtr, nativeResult.StackLen);

            return new ExecutionResult
            {
                State = nativeResult.State,
                FeeConsumedPico = nativeResult.FeeConsumedPico,
                Error = error,
                Stack = stack,
            };
        }
        finally
        {
            // Free the native result's heap allocations via the Rust free function.
            var nativeResult = Marshal.PtrToStructure<NativeExecutionResult>(resultPtr);
            if (nativeResult.StackPtr != IntPtr.Zero || nativeResult.ErrorPtr != IntPtr.Zero)
            {
                s_freeExecutionResult!(resultPtr);
            }
            Marshal.FreeHGlobal(resultPtr);

            methodHandle.Free();
            binaryHandle.Free();
        }
    }

    // ---------------------------------------------------------------
    //  Dummy host callbacks
    // ---------------------------------------------------------------

    /// <summary>
    /// Dummy host callback that returns an empty stack for any syscall.
    /// This is sufficient for contracts that do not use syscalls (pure
    /// computation contracts). Contracts that need storage, notifications,
    /// etc. will fault with "Unsupported syscall".
    /// </summary>
    private static bool DummyHostCallback(
        IntPtr userData,
        uint api,
        nuint instructionPointer,
        byte trigger,
        uint networkMagic,
        byte addressVersion,
        ulong persistingTimestamp,
        long gasLeft,
        IntPtr inputStackPtr,
        nuint inputStackLen,
        IntPtr output)
    {
        // Write a NativeHostResult with empty stack to the output pointer.
        if (output != IntPtr.Zero)
        {
            var result = new NativeHostResult
            {
                StackPtr = IntPtr.Zero,
                StackLen = 0,
                ErrorPtr = IntPtr.Zero,
                ErrorLen = 0,
            };
            Marshal.StructureToPtr(result, output, false);
        }
        return true;
    }

    private static void DummyHostFree(IntPtr userData, IntPtr result)
    {
        // The dummy callback allocates nothing, so nothing to free.
    }

    // ---------------------------------------------------------------
    //  TestHostCallback: in-memory syscall handler for testing
    // ---------------------------------------------------------------

    /// <summary>
    /// API ID constants matching neo-riscv-devpack/src/api_ids.rs (FNV-1a hashes).
    /// </summary>
    private static class ApiIds
    {
        // System.Storage
        public const uint StorageGetContext = 0xCE67F69B;
        public const uint StorageGetReadOnlyContext = 0xE26BB4F6;
        public const uint StorageAsReadOnly = 0xE9BF4C76;
        public const uint StorageGet = 0x31E85D92;
        public const uint StoragePut = 0x84183FE6;
        public const uint StorageDelete = 0xEDC5582F;
        public const uint StorageFind = 0x9AB830DF;

        // System.Runtime
        public const uint RuntimeCheckWitness = 0x8CEC27F8;
        public const uint RuntimeNotify = 0x616F0195;
        public const uint RuntimeLog = 0x9647E7CF;
        public const uint RuntimeGetTrigger = 0xA0387DE9;
        public const uint RuntimeGetNetwork = 0xE0A0FBC5;
        public const uint RuntimeGetAddressVersion = 0xDC92494C;
        public const uint RuntimeGetTime = 0x0388C3B7;
        public const uint RuntimeGasLeft = 0xCED88814;
        public const uint RuntimePlatform = 0xF6FC79B2;
        public const uint RuntimeGetExecutingScriptHash = 0x74A8FEDB;
        public const uint RuntimeGetCallingScriptHash = 0x3C6E5339;
        public const uint RuntimeGetEntryScriptHash = 0x38E2B4F9;

        // System.Iterator
        public const uint IteratorNext = 0x9CED089C;
        public const uint IteratorValue = 0x1DBF54F3;

        // System.Storage.Local
        public const uint StorageLocalGet = 0xE85E8DD5;
        public const uint StorageLocalPut = 0x0AE30C39;
        public const uint StorageLocalDelete = 0x94F55475;
        public const uint StorageLocalFind = 0xF3527607;

        // CALLT marker
        public const uint CalltMarker = 0x43540000;
    }

    /// <summary>
    /// A test-friendly host callback that handles common syscalls with
    /// in-memory state. Enables testing contracts that use storage,
    /// logging, and notifications without a full Neo blockchain.
    /// </summary>
    public sealed class TestHostCallback
    {
        /// <summary>In-memory storage (key -> value).</summary>
        public Dictionary<byte[], byte[]> Storage { get; } = new(ByteArrayEqualityComparer.Instance);

        /// <summary>Captured log messages.</summary>
        public List<string> Logs { get; } = new();

        /// <summary>Debug: tracks which API IDs were called via callback.</summary>
        public List<uint> CalledApis { get; } = new();

        /// <summary>Captured notifications (script hash, event name, args).</summary>
        public List<NotificationRecord> Notifications { get; } = new();

        /// <summary>Delegate to handle Contract.Call. Return null for default (empty).</summary>
        public Func<byte[], string, ResultStackItem[], ResultStackItem[]?>? OnContractCall { get; set; }

        /// <summary>
        /// Execute a RISC-V contract with this host callback providing syscall support.
        /// </summary>
        public ExecutionResult Execute(byte[] binary, string method)
        {
            return ExecuteWithHost(binary, method, this);
        }

        /// <summary>
        /// Execute a RISC-V contract with initial stack arguments and this host callback.
        /// </summary>
        public ExecutionResult Execute(byte[] binary, string method, ResultStackItem[] initialArgs)
        {
            return ExecuteWithHost(binary, method, initialArgs, this);
        }
    }

    /// <summary>
    /// A recorded notification event.
    /// </summary>
    public sealed class NotificationRecord
    {
        public required byte[] ScriptHash { get; init; }
        public required string EventName { get; init; }
        public required ResultStackItem[] Args { get; init; }
    }

    /// <summary>
    /// Execute a RISC-V contract with an optional TestHostCallback.
    /// Falls back to DummyHostCallback if host is null.
    /// </summary>
    public static ExecutionResult ExecuteWithHost(byte[] binary, string method, TestHostCallback? host)
    {
        if (!s_initialized)
            throw new InvalidOperationException("RiscVExecutionBridge.Initialize() has not been called.");

        HostCallbackDelegate callback;
        HostFreeCallbackDelegate freeCallback;
        GCHandle hostHandle;

        if (host != null)
        {
            hostHandle = GCHandle.Alloc(host);
            callback = TestHostCallbackImpl;
            freeCallback = TestHostFreeImpl;
        }
        else
        {
            hostHandle = default;
            callback = DummyHostCallback;
            freeCallback = DummyHostFree;
        }

        var callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
        var freePtr = Marshal.GetFunctionPointerForDelegate(freeCallback);

        var binaryHandle = GCHandle.Alloc(binary, GCHandleType.Pinned);
        var methodBytes = Encoding.UTF8.GetBytes(method);
        var methodHandle = GCHandle.Alloc(methodBytes, GCHandleType.Pinned);

        var resultSize = Marshal.SizeOf<NativeExecutionResult>();
        var resultPtr = Marshal.AllocHGlobal(resultSize);
        {
            var zero = new byte[resultSize];
            Marshal.Copy(zero, 0, resultPtr, resultSize);
        }

        try
        {
            var userData = host != null ? GCHandle.ToIntPtr(hostHandle) : IntPtr.Zero;

            s_executeNativeContract!(
                binaryHandle.AddrOfPinnedObject(),
                (nuint)binary.Length,
                methodHandle.AddrOfPinnedObject(),
                (nuint)methodBytes.Length,
                IntPtr.Zero,
                0,
                0x40,
                860833102u,
                53,
                0,
                10_000_000_000_000L,
                30_000L,
                userData,
                callbackPtr,
                freePtr,
                resultPtr);

            var nativeResult = Marshal.PtrToStructure<NativeExecutionResult>(resultPtr);

            string? error = null;
            if (nativeResult.ErrorPtr != IntPtr.Zero && nativeResult.ErrorLen > 0)
            {
                error = Marshal.PtrToStringUTF8(nativeResult.ErrorPtr, checked((int)nativeResult.ErrorLen));
            }

            var stack = ReadResultStack(nativeResult.StackPtr, nativeResult.StackLen);

            return new ExecutionResult
            {
                State = nativeResult.State,
                FeeConsumedPico = nativeResult.FeeConsumedPico,
                Error = error,
                Stack = stack,
            };
        }
        finally
        {
            var nativeResult = Marshal.PtrToStructure<NativeExecutionResult>(resultPtr);
            if (nativeResult.StackPtr != IntPtr.Zero || nativeResult.ErrorPtr != IntPtr.Zero)
            {
                s_freeExecutionResult!(resultPtr);
            }
            Marshal.FreeHGlobal(resultPtr);

            methodHandle.Free();
            binaryHandle.Free();
            if (hostHandle.IsAllocated)
                hostHandle.Free();
        }
    }

    /// <summary>
    /// Execute a RISC-V contract with initial stack arguments and an optional TestHostCallback.
    /// </summary>
    public static ExecutionResult ExecuteWithHost(
        byte[] binary,
        string method,
        ResultStackItem[] initialArgs,
        TestHostCallback? host)
    {
        if (!s_initialized)
            throw new InvalidOperationException("RiscVDirectRunner.Initialize() has not been called.");

        HostCallbackDelegate callback;
        HostFreeCallbackDelegate freeCallback;
        GCHandle hostHandle;

        if (host != null)
        {
            hostHandle = GCHandle.Alloc(host);
            callback = TestHostCallbackImpl;
            freeCallback = TestHostFreeImpl;
        }
        else
        {
            hostHandle = default;
            callback = DummyHostCallback;
            freeCallback = DummyHostFree;
        }

        var callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
        var freePtr = Marshal.GetFunctionPointerForDelegate(freeCallback);

        // Build the initial stack as a pinned NativeStackItem array
        var itemSize = Marshal.SizeOf<NativeStackItem>();
        var stackItems = new NativeStackItem[initialArgs.Length];
        var allocatedByteArrays = new List<IntPtr>();

        for (var i = 0; i < initialArgs.Length; i++)
        {
            var arg = initialArgs[i];
            var item = new NativeStackItem
            {
                Kind = arg.Kind,
                IntegerValue = arg.IntegerValue,
            };

            if (arg.Bytes != null && arg.Bytes.Length > 0)
            {
                var bytesPtr = Marshal.AllocHGlobal(arg.Bytes.Length);
                Marshal.Copy(arg.Bytes, 0, bytesPtr, arg.Bytes.Length);
                item.BytesPtr = bytesPtr;
                item.BytesLen = (nuint)arg.Bytes.Length;
                allocatedByteArrays.Add(bytesPtr);
            }

            stackItems[i] = item;
        }

        // Marshal the array to unmanaged memory
        var stackPtr = Marshal.AllocHGlobal(itemSize * stackItems.Length);
        for (var i = 0; i < stackItems.Length; i++)
        {
            Marshal.StructureToPtr(stackItems[i], IntPtr.Add(stackPtr, i * itemSize), false);
        }

        var binaryHandle = GCHandle.Alloc(binary, GCHandleType.Pinned);
        var methodBytes = Encoding.UTF8.GetBytes(method);
        var methodHandle = GCHandle.Alloc(methodBytes, GCHandleType.Pinned);

        var resultSize = Marshal.SizeOf<NativeExecutionResult>();
        var resultPtr = Marshal.AllocHGlobal(resultSize);
        {
            var zero = new byte[resultSize];
            Marshal.Copy(zero, 0, resultPtr, resultSize);
        }

        try
        {
            var userData = host != null ? GCHandle.ToIntPtr(hostHandle) : IntPtr.Zero;

            s_executeNativeContract!(
                binaryHandle.AddrOfPinnedObject(),
                (nuint)binary.Length,
                methodHandle.AddrOfPinnedObject(),
                (nuint)methodBytes.Length,
                stackPtr,
                (nuint)initialArgs.Length,
                0x40,
                860833102u,
                53,
                0,
                10_000_000_000_000L,
                30_000L,
                userData,
                callbackPtr,
                freePtr,
                resultPtr);

            var nativeResult = Marshal.PtrToStructure<NativeExecutionResult>(resultPtr);

            string? error = null;
            if (nativeResult.ErrorPtr != IntPtr.Zero && nativeResult.ErrorLen > 0)
            {
                error = Marshal.PtrToStringUTF8(nativeResult.ErrorPtr, checked((int)nativeResult.ErrorLen));
            }

            var stack = ReadResultStack(nativeResult.StackPtr, nativeResult.StackLen);

            return new ExecutionResult
            {
                State = nativeResult.State,
                FeeConsumedPico = nativeResult.FeeConsumedPico,
                Error = error,
                Stack = stack,
            };
        }
        finally
        {
            var nativeResult = Marshal.PtrToStructure<NativeExecutionResult>(resultPtr);
            if (nativeResult.StackPtr != IntPtr.Zero || nativeResult.ErrorPtr != IntPtr.Zero)
            {
                s_freeExecutionResult!(resultPtr);
            }
            Marshal.FreeHGlobal(resultPtr);
            Marshal.FreeHGlobal(stackPtr);

            foreach (var ptr in allocatedByteArrays)
                Marshal.FreeHGlobal(ptr);

            methodHandle.Free();
            binaryHandle.Free();
            if (hostHandle.IsAllocated)
                hostHandle.Free();
        }
    }

    /// <summary>
    /// Reads a NativeStackItem array from unmanaged memory into a managed array.
    /// </summary>
    private static NativeStackItem[] ReadInputStack(IntPtr ptr, nuint len)
    {
        if (ptr == IntPtr.Zero || len == 0)
            return Array.Empty<NativeStackItem>();

        var count = (int)len;
        var items = new NativeStackItem[count];
        var itemSize = Marshal.SizeOf<NativeStackItem>();

        for (var i = 0; i < count; i++)
        {
            items[i] = Marshal.PtrToStructure<NativeStackItem>(IntPtr.Add(ptr, i * itemSize));
        }

        return items;
    }

    /// <summary>
    /// Reads bytes from a NativeStackItem's bytes_ptr/bytes_len.
    /// </summary>
    private static byte[] ReadItemBytes(NativeStackItem item)
    {
        if (item.BytesPtr == IntPtr.Zero || item.BytesLen == 0)
            return Array.Empty<byte>();
        var bytes = new byte[(int)item.BytesLen];
        Marshal.Copy(item.BytesPtr, bytes, 0, bytes.Length);
        return bytes;
    }

    /// <summary>
    /// Allocates a NativeStackItem on unmanaged heap and writes the result.
    /// Caller must free via TestHostFreeImpl.
    /// </summary>
    private static IntPtr AllocateResultItem(uint kind, long integerValue, byte[]? bytes)
    {
        var itemSize = Marshal.SizeOf<NativeStackItem>();
        var ptr = Marshal.AllocHGlobal(itemSize);
        var item = new NativeStackItem { Kind = kind, IntegerValue = integerValue };

        if (bytes != null && bytes.Length > 0)
        {
            var bytesPtr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, bytesPtr, bytes.Length);
            item.BytesPtr = bytesPtr;
            item.BytesLen = (nuint)bytes.Length;
        }

        Marshal.StructureToPtr(item, ptr, false);
        return ptr;
    }

    /// <summary>
    /// Writes a single NativeStackItem result to the output NativeHostResult.
    /// </summary>
    private static void WriteOutputSingle(IntPtr output, uint kind, long integerValue, byte[]? bytes)
    {
        if (output == IntPtr.Zero) return;

        var itemPtr = AllocateResultItem(kind, integerValue, bytes);
        var itemSize = Marshal.SizeOf<NativeStackItem>();
        var arrayPtr = Marshal.AllocHGlobal(itemSize);
        // Copy the item into the array
        var item = Marshal.PtrToStructure<NativeStackItem>(itemPtr);
        Marshal.StructureToPtr(item, arrayPtr, false);
        Marshal.FreeHGlobal(itemPtr);

        var result = new NativeHostResult
        {
            StackPtr = arrayPtr,
            StackLen = 1,
            ErrorPtr = IntPtr.Zero,
            ErrorLen = 0,
        };
        Marshal.StructureToPtr(result, output, false);
    }

    /// <summary>
    /// Writes an empty result to the output NativeHostResult.
    /// </summary>
    private static void WriteOutputEmpty(IntPtr output)
    {
        if (output == IntPtr.Zero) return;
        var result = new NativeHostResult
        {
            StackPtr = IntPtr.Zero,
            StackLen = 0,
            ErrorPtr = IntPtr.Zero,
            ErrorLen = 0,
        };
        Marshal.StructureToPtr(result, output, false);
    }

    /// <summary>
    /// Writes an error message to the output NativeHostResult.
    /// </summary>
    private static void WriteOutputError(IntPtr output, string message)
    {
        if (output == IntPtr.Zero) return;
        var errorBytes = Encoding.UTF8.GetBytes(message);
        var errorPtr = Marshal.AllocHGlobal(errorBytes.Length);
        Marshal.Copy(errorBytes, 0, errorPtr, errorBytes.Length);

        var result = new NativeHostResult
        {
            StackPtr = IntPtr.Zero,
            StackLen = 0,
            ErrorPtr = errorPtr,
            ErrorLen = (nuint)errorBytes.Length,
        };
        Marshal.StructureToPtr(result, output, false);
    }

    /// <summary>
    /// Tracks allocated NativeStackItem arrays for cleanup.
    /// </summary>
    private static readonly List<IntPtr> s_allocatedArrays = new();
    private static readonly object s_allocLock = new();

    /// <summary>
    /// Native callback implementation that dispatches to TestHostCallback.
    /// </summary>
    private static bool TestHostCallbackImpl(
        IntPtr userData,
        uint api,
        nuint instructionPointer,
        byte trigger,
        uint networkMagic,
        byte addressVersion,
        ulong persistingTimestamp,
        long gasLeft,
        IntPtr inputStackPtr,
        nuint inputStackLen,
        IntPtr output)
    {
        var host = (TestHostCallback)GCHandle.FromIntPtr(userData).Target!;
        var input = ReadInputStack(inputStackPtr, inputStackLen);
        host.CalledApis.Add(api);

        switch (api)
        {
            // --- Storage syscalls ---
            case ApiIds.StorageGetContext:
            case ApiIds.StorageGetReadOnlyContext:
                // Return a dummy context (integer 0)
                WriteOutputSingle(output, 0, 0, null);
                return true;

            case ApiIds.StorageAsReadOnly:
                // Return the same context (no-op)
                WriteOutputSingle(output, 0, 0, null);
                return true;

            case ApiIds.StorageGet:
                // input: [ByteString(key)] -> returns ByteString(value) or null
                if (input.Length >= 1)
                {
                    var key = ReadItemBytes(input[0]);
                    if (host.Storage.TryGetValue(key, out var value))
                    {
                        WriteOutputSingle(output, 1, 0, value);
                    }
                    else
                    {
                        // Key not found — return null
                        WriteOutputSingle(output, 2, 0, null);
                    }
                }
                else
                {
                    WriteOutputSingle(output, 2, 0, null);
                }
                return true;

            case ApiIds.StoragePut:
                // input: [context, ByteString(key), ByteString(value)]
                if (input.Length >= 3)
                {
                    var key = ReadItemBytes(input[1]);
                    var value = ReadItemBytes(input[2]);
                    host.Storage[key] = value;
                }
                else if (input.Length >= 2)
                {
                    // Some contracts pass [key, value] without context
                    var key = ReadItemBytes(input[0]);
                    var value = ReadItemBytes(input[1]);
                    host.Storage[key] = value;
                }
                WriteOutputEmpty(output);
                return true;

            case ApiIds.StorageDelete:
                // input: [context, ByteString(key)]
                if (input.Length >= 2)
                {
                    var key = ReadItemBytes(input[1]);
                    host.Storage.Remove(key);
                }
                else if (input.Length >= 1)
                {
                    var key = ReadItemBytes(input[0]);
                    host.Storage.Remove(key);
                }
                WriteOutputEmpty(output);
                return true;

            case ApiIds.StorageFind:
                // Return an empty iterator handle
                WriteOutputSingle(output, 6, 0, null);
                return true;

            // --- Storage.Local syscalls ---
            case ApiIds.StorageLocalGet:
                // input: [id, ByteString(key)]
                if (input.Length >= 2)
                {
                    var key = ReadItemBytes(input[1]);
                    if (host.Storage.TryGetValue(key, out var val))
                        WriteOutputSingle(output, 1, 0, val);
                    else
                        WriteOutputEmpty(output);
                }
                else
                {
                    WriteOutputEmpty(output);
                }
                return true;

            case ApiIds.StorageLocalPut:
                // input: [id, ByteString(key), ByteString(value)]
                if (input.Length >= 3)
                {
                    var key = ReadItemBytes(input[1]);
                    var value = ReadItemBytes(input[2]);
                    host.Storage[key] = value;
                }
                WriteOutputEmpty(output);
                return true;

            case ApiIds.StorageLocalDelete:
                // input: [id, ByteString(key)]
                if (input.Length >= 2)
                {
                    var key = ReadItemBytes(input[1]);
                    host.Storage.Remove(key);
                }
                WriteOutputEmpty(output);
                return true;

            case ApiIds.StorageLocalFind:
                // Return an empty iterator handle
                WriteOutputSingle(output, 6, 0, null);
                return true;

            // --- Runtime syscalls ---
            case ApiIds.RuntimeCheckWitness:
                // Always return true in tests
                WriteOutputSingle(output, 3, 1, null);
                return true;

            case ApiIds.RuntimeLog:
                if (input.Length >= 1)
                {
                    var msgBytes = ReadItemBytes(input[0]);
                    host.Logs.Add(Encoding.UTF8.GetString(msgBytes));
                }
                WriteOutputEmpty(output);
                return true;

            case ApiIds.RuntimeNotify:
                // input can be [scriptHash, eventName, Array(args)] or [eventName, Array(args)]
                if (input.Length >= 3)
                {
                    var hashBytes = ReadItemBytes(input[0]);
                    var eventName = Encoding.UTF8.GetString(ReadItemBytes(input[1]));
                    var args = new ResultStackItem[input.Length - 2];
                    for (var i = 2; i < input.Length; i++)
                    {
                        args[i - 2] = new ResultStackItem
                        {
                            Kind = input[i].Kind,
                            IntegerValue = input[i].IntegerValue,
                            Bytes = input[i].Kind == 1 || input[i].Kind == 5 ? ReadItemBytes(input[i]) : null,
                        };
                    }
                    host.Notifications.Add(new NotificationRecord
                    {
                        ScriptHash = hashBytes,
                        EventName = eventName,
                        Args = args,
                    });
                }
                else if (input.Length >= 2)
                {
                    var eventName = Encoding.UTF8.GetString(ReadItemBytes(input[0]));
                    var args = new ResultStackItem[input.Length - 1];
                    for (var i = 1; i < input.Length; i++)
                    {
                        args[i - 1] = new ResultStackItem
                        {
                            Kind = input[i].Kind,
                            IntegerValue = input[i].IntegerValue,
                            Bytes = input[i].Kind == 1 || input[i].Kind == 5 ? ReadItemBytes(input[i]) : null,
                        };
                    }
                    host.Notifications.Add(new NotificationRecord
                    {
                        ScriptHash = Array.Empty<byte>(),
                        EventName = eventName,
                        Args = args,
                    });
                }
                WriteOutputEmpty(output);
                return true;

            case ApiIds.RuntimeGetTrigger:
                // TriggerType.Application = 0x40
                WriteOutputSingle(output, 0, 0x40, null);
                return true;

            case ApiIds.RuntimeGetNetwork:
                WriteOutputSingle(output, 0, 860833102, null);
                return true;

            case ApiIds.RuntimeGetAddressVersion:
                WriteOutputSingle(output, 0, 53, null);
                return true;

            case ApiIds.RuntimeGetTime:
                WriteOutputSingle(output, 0, 0, null);
                return true;

            case ApiIds.RuntimeGasLeft:
                WriteOutputSingle(output, 0, 10_000_000_000_000L, null);
                return true;

            case ApiIds.RuntimePlatform:
                var platBytes = Encoding.UTF8.GetBytes("NEO");
                WriteOutputSingle(output, 1, 0, platBytes);
                return true;

            case ApiIds.RuntimeGetExecutingScriptHash:
            case ApiIds.RuntimeGetCallingScriptHash:
            case ApiIds.RuntimeGetEntryScriptHash:
                // Return 20 zero bytes as a script hash
                WriteOutputSingle(output, 1, 0, new byte[20]);
                return true;

            // --- Iterator syscalls ---
            case ApiIds.IteratorNext:
                // Return false (no more items)
                WriteOutputSingle(output, 3, 0, null);
                return true;

            case ApiIds.IteratorValue:
                WriteOutputSingle(output, 2, 0, null);
                return true;

            default:
                // Unknown syscall — return empty to avoid fault
                WriteOutputEmpty(output);
                return true;
        }
    }

    /// <summary>
    /// Free callback for memory allocated by TestHostCallbackImpl.
    /// </summary>
    private static void TestHostFreeImpl(IntPtr userData, IntPtr result)
    {
        if (result == IntPtr.Zero) return;

        var nativeResult = Marshal.PtrToStructure<NativeHostResult>(result);

        // Free the stack items and their byte arrays
        if (nativeResult.StackPtr != IntPtr.Zero && nativeResult.StackLen > 0)
        {
            var itemSize = Marshal.SizeOf<NativeStackItem>();
            for (var i = 0; i < (int)nativeResult.StackLen; i++)
            {
                var itemPtr = IntPtr.Add(nativeResult.StackPtr, i * itemSize);
                var item = Marshal.PtrToStructure<NativeStackItem>(itemPtr);
                if (item.BytesPtr != IntPtr.Zero)
                    Marshal.FreeHGlobal(item.BytesPtr);
            }
            Marshal.FreeHGlobal(nativeResult.StackPtr);
        }

        if (nativeResult.ErrorPtr != IntPtr.Zero)
            Marshal.FreeHGlobal(nativeResult.ErrorPtr);
    }

    /// <summary>
    /// Equality comparer for byte[] keys in the storage dictionary.
    /// </summary>
    private sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public static readonly ByteArrayEqualityComparer Instance = new();

        public bool Equals(byte[]? x, byte[]? y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            if (x.Length != y.Length) return false;
            for (var i = 0; i < x.Length; i++)
                if (x[i] != y[i]) return false;
            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            var hash = 17;
            foreach (var b in obj)
                hash = hash * 31 + b;
            return hash;
        }
    }

    // ---------------------------------------------------------------
    //  Stack reading helpers
    // ---------------------------------------------------------------

    private static ResultStackItem[] ReadResultStack(IntPtr stackPtr, nuint stackLen)
    {
        if (stackPtr == IntPtr.Zero || stackLen == 0)
            return Array.Empty<ResultStackItem>();

        var count = (int)stackLen;
        var items = new ResultStackItem[count];
        var itemSize = Marshal.SizeOf<NativeStackItem>();

        for (var i = 0; i < count; i++)
        {
            var itemPtr = IntPtr.Add(stackPtr, i * itemSize);
            var native = Marshal.PtrToStructure<NativeStackItem>(itemPtr);
            items[i] = DecodeStackItem(native);
        }

        return items;
    }

    private static ResultStackItem DecodeStackItem(NativeStackItem native)
    {
        byte[]? bytes = null;
        ResultStackItem[]? children = null;

        switch (native.Kind)
        {
            case 4: // Array
            case 7: // Struct
            case 8: // Map
                // BytesPtr points to nested NativeStackItem array, BytesLen is the count.
                children = ReadResultStack(native.BytesPtr, native.BytesLen);
                break;

            case 1: // ByteString
            case 5: // BigInteger (stored as little-endian bytes)
                if (native.BytesPtr != IntPtr.Zero && native.BytesLen > 0)
                {
                    bytes = new byte[(int)native.BytesLen];
                    Marshal.Copy(native.BytesPtr, bytes, 0, bytes.Length);
                }
                else
                {
                    bytes = Array.Empty<byte>();
                }
                break;

                // 0=Integer, 2=Null, 3=Boolean, 6=Iterator, 9=Interop
                // These use IntegerValue only; no BytesPtr payload.
        }

        return new ResultStackItem
        {
            Kind = native.Kind,
            IntegerValue = native.IntegerValue,
            Bytes = bytes,
            Children = children,
        };
    }

    // ---------------------------------------------------------------
    //  Library path resolution
    // ---------------------------------------------------------------

    private static string? FindNativeLibrary()
    {
        var candidates = new[]
        {
            // Next to test binary (copied by MSBuild Content item).
            Path.Combine(AppContext.BaseDirectory, "libneo_riscv_host.so"),
            // Direct path from the repo root.
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "target", "release", "libneo_riscv_host.so"),
        };

        foreach (var path in candidates)
        {
            var full = Path.GetFullPath(path);
            if (File.Exists(full))
                return full;
        }

        return null;
    }
}
