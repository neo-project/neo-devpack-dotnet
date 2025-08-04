# Access Control Patterns for Neo Smart Contracts

Access control is fundamental to smart contract security. This guide provides comprehensive patterns and implementations for securing your Neo N3 smart contracts with proper authorization mechanisms.

## Table of Contents

- [Overview](#overview)
- [Basic Access Control Patterns](#basic-access-control-patterns)
- [Role-Based Access Control (RBAC)](#role-based-access-control-rbac)
- [Capability-Based Access Control](#capability-based-access-control)
- [Multi-Signature Patterns](#multi-signature-patterns)
- [Time-Based Access Control](#time-based-access-control)
- [Advanced Access Control Patterns](#advanced-access-control-patterns)
- [Best Practices](#best-practices)
- [Testing Access Control](#testing-access-control)

## Overview

This guide provides comprehensive access control implementation patterns for Neo smart contracts, focusing on practical code examples and advanced authorization mechanisms.

> **Foundation**: Review [Security Overview](security-overview.md) for core security principles before implementing these patterns.

## Basic Access Control Patterns

### 1. Owner-Only Pattern

The simplest access control pattern restricts certain functions to a single owner.

```csharp
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

[DisplayName("OwnerOnlyContract")]
[ContractDescription("Demonstrates basic owner-only access control")]
public class OwnerOnlyContract : SmartContract
{
    // Define contract owner at deployment time
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    
    /// <summary>
    /// Modifier to ensure only owner can execute function
    /// </summary>
    private static void OnlyOwner()
    {
        Assert(Runtime.CheckWitness(OWNER), "Access denied: Owner only");
    }
    
    /// <summary>
    /// Administrative function restricted to owner
    /// </summary>
    public static bool AdminFunction(string parameter)
    {
        OnlyOwner();
        
        // Perform admin operation
        Storage.Put(Storage.CurrentContext, "admin_action", parameter);
        OnAdminAction(parameter);
        return true;
    }
    
    /// <summary>
    /// Transfer ownership to new address
    /// </summary>
    public static bool TransferOwnership(UInt160 newOwner)
    {
        OnlyOwner();
        Assert(newOwner != OWNER, "New owner must be different");
        Assert(Runtime.CheckWitness(newOwner), "New owner must authorize the transfer");
        
        // Update owner in storage for future reference
        Storage.Put(Storage.CurrentContext, "owner", newOwner);
        OnOwnershipTransferred(OWNER, newOwner);
        return true;
    }
    
    /// <summary>
    /// Get current owner address
    /// </summary>
    [Safe]
    public static UInt160 GetOwner()
    {
        ByteString storedOwner = Storage.Get(Storage.CurrentContext, "owner");
        return storedOwner != null ? (UInt160)storedOwner : OWNER;
    }
    
    [DisplayName("AdminAction")]
    public static event Action<string> OnAdminAction;
    
    [DisplayName("OwnershipTransferred")]
    public static event Action<UInt160, UInt160> OnOwnershipTransferred;
}
```

### 2. Whitelist Pattern

Control access through a maintained list of authorized addresses.

```csharp
[DisplayName("WhitelistContract")]
public class WhitelistContract : SmartContract
{
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    private static readonly StorageMap Whitelist = new(Storage.CurrentContext, "whitelist");
    
    /// <summary>
    /// Add address to whitelist (owner only)
    /// </summary>
    public static bool AddToWhitelist(UInt160 address)
    {
        Assert(Runtime.CheckWitness(OWNER), "Access denied: Owner only");
        Assert(address != null && address.IsValid, "Invalid address");
        
        Whitelist.Put(address, 1);
        OnWhitelistUpdated(address, true);
        return true;
    }
    
    /// <summary>
    /// Remove address from whitelist (owner only)
    /// </summary>
    public static bool RemoveFromWhitelist(UInt160 address)
    {
        Assert(Runtime.CheckWitness(OWNER), "Access denied: Owner only");
        Assert(address != null && address.IsValid, "Invalid address");
        
        Whitelist.Delete(address);
        OnWhitelistUpdated(address, false);
        return true;
    }
    
    /// <summary>
    /// Check if address is whitelisted
    /// </summary>
    [Safe]
    public static bool IsWhitelisted(UInt160 address)
    {
        if (address == OWNER) return true; // Owner is always whitelisted
        return Whitelist.Get(address) != null;
    }
    
    /// <summary>
    /// Function restricted to whitelisted addresses
    /// </summary>
    public static bool WhitelistedFunction(string data)
    {
        UInt160 caller = (UInt160)Runtime.ScriptContainer.Sender;
        Assert(IsWhitelisted(caller), "Access denied: Not whitelisted");
        Assert(Runtime.CheckWitness(caller), "Access denied: Invalid signature");
        
        // Perform whitelisted operation
        Storage.Put(Storage.CurrentContext, "data_" + caller, data);
        return true;
    }
    
    [DisplayName("WhitelistUpdated")]
    public static event Action<UInt160, bool> OnWhitelistUpdated;
}
```

## Role-Based Access Control (RBAC)

RBAC provides flexible access control through role assignments and hierarchies.

```csharp
[DisplayName("RBACContract")]
[ContractDescription("Role-based access control implementation")]
public class RBACContract : SmartContract
{
    // Role definitions
    private const string ADMIN_ROLE = "admin";
    private const string MODERATOR_ROLE = "moderator";
    private const string USER_ROLE = "user";
    private const string AUDITOR_ROLE = "auditor";
    
    // Storage maps for roles and permissions
    private static readonly StorageMap UserRoles = new(Storage.CurrentContext, "user_roles");
    private static readonly StorageMap RolePermissions = new(Storage.CurrentContext, "role_permissions");
    
    // Contract owner
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    
    /// <summary>
    /// Initialize contract with default roles and permissions
    /// </summary>
    public static void _deploy(object data, bool update)
    {
        if (update) return;
        
        // Set up default role hierarchy
        SetupDefaultRoles();
        
        // Assign owner as admin
        UserRoles.Put(OWNER + ADMIN_ROLE, 1);
        OnRoleGranted(OWNER, ADMIN_ROLE);
    }
    
    /// <summary>
    /// Set up default role permissions
    /// </summary>
    private static void SetupDefaultRoles()
    {
        // Admin permissions
        RolePermissions.Put(ADMIN_ROLE + "grant_role", 1);
        RolePermissions.Put(ADMIN_ROLE + "revoke_role", 1);
        RolePermissions.Put(ADMIN_ROLE + "manage_contract", 1);
        RolePermissions.Put(ADMIN_ROLE + "view_audit", 1);
        
        // Moderator permissions
        RolePermissions.Put(MODERATOR_ROLE + "moderate_content", 1);
        RolePermissions.Put(MODERATOR_ROLE + "view_audit", 1);
        
        // User permissions
        RolePermissions.Put(USER_ROLE + "basic_access", 1);
        
        // Auditor permissions
        RolePermissions.Put(AUDITOR_ROLE + "view_audit", 1);
    }
    
    /// <summary>
    /// Grant role to user
    /// </summary>
    public static bool GrantRole(UInt160 user, string role)
    {
        Assert(HasPermission(Runtime.CallingScriptHash, "grant_role"), "Access denied: No grant permission");
        Assert(user != null && user.IsValid, "Invalid user address");
        Assert(IsValidRole(role), "Invalid role");
        
        UserRoles.Put(user + role, 1);
        OnRoleGranted(user, role);
        return true;
    }
    
    /// <summary>
    /// Revoke role from user
    /// </summary>
    public static bool RevokeRole(UInt160 user, string role)
    {
        Assert(HasPermission(Runtime.CallingScriptHash, "revoke_role"), "Access denied: No revoke permission");
        Assert(user != null && user.IsValid, "Invalid user address");
        Assert(role != ADMIN_ROLE || user != OWNER, "Cannot revoke admin role from owner");
        
        UserRoles.Delete(user + role);
        OnRoleRevoked(user, role);
        return true;
    }
    
    /// <summary>
    /// Check if user has specific role
    /// </summary>
    [Safe]
    public static bool HasRole(UInt160 user, string role)
    {
        if (user == OWNER && role == ADMIN_ROLE) return true;
        return UserRoles.Get(user + role) != null;
    }
    
    /// <summary>
    /// Check if user has specific permission
    /// </summary>
    [Safe]
    public static bool HasPermission(UInt160 user, string permission)
    {
        // Check all roles the user has
        string[] roles = { ADMIN_ROLE, MODERATOR_ROLE, USER_ROLE, AUDITOR_ROLE };
        
        foreach (string role in roles)
        {
            if (HasRole(user, role) && RolePermissions.Get(role + permission) != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Administrative function requiring specific permission
    /// </summary>
    public static bool ManageContract(string action, string parameter)
    {
        UInt160 caller = (UInt160)Runtime.ScriptContainer.Sender;
        Assert(Runtime.CheckWitness(caller), "Access denied: Invalid signature");
        Assert(HasPermission(caller, "manage_contract"), "Access denied: No management permission");
        
        Storage.Put(Storage.CurrentContext, "last_action", action + ":" + parameter);
        OnContractManaged(caller, action, parameter);
        return true;
    }
    
    /// <summary>
    /// Audit function with read-only access
    /// </summary>
    [Safe]
    public static string[] GetAuditLog(UInt160 requester, int limit)
    {
        Assert(HasPermission(requester, "view_audit"), "Access denied: No audit permission");
        Assert(limit > 0 && limit <= 100, "Invalid limit");
        
        // Return audit information (implementation depends on audit log structure)
        List<string> auditEntries = new List<string>();
        
        var iterator = Storage.Find(Storage.CurrentContext, "audit_", FindOptions.None);
        int count = 0;
        
        while (iterator.Next() && count < limit)
        {
            auditEntries.Add((string)iterator.Value);
            count++;
        }
        
        return auditEntries.ToArray();
    }
    
    /// <summary>
    /// Validate role name
    /// </summary>
    private static bool IsValidRole(string role)
    {
        return role == ADMIN_ROLE || role == MODERATOR_ROLE || 
               role == USER_ROLE || role == AUDITOR_ROLE;
    }
    
    [DisplayName("RoleGranted")]
    public static event Action<UInt160, string> OnRoleGranted;
    
    [DisplayName("RoleRevoked")]
    public static event Action<UInt160, string> OnRoleRevoked;
    
    [DisplayName("ContractManaged")]
    public static event Action<UInt160, string, string> OnContractManaged;
}
```

## Multi-Signature Patterns

Implement multi-signature requirements for critical operations.

```csharp
[DisplayName("MultiSigContract")]
[ContractDescription("Multi-signature access control")]
public class MultiSigContract : SmartContract
{
    // Multi-signature configuration
    private const int REQUIRED_SIGNATURES = 3;
    private const int MAX_SIGNERS = 7;
    private const int MIN_SIGNERS = 3;
    
    private static readonly StorageMap AuthorizedSigners = new(Storage.CurrentContext, "signers");
    private static readonly StorageMap PendingOperations = new(Storage.CurrentContext, "pending");
    
    /// <summary>
    /// Structure for multi-sig operations
    /// </summary>
    public struct MultiSigOperation
    {
        public string OperationId;
        public string Action;
        public ByteString Data;
        public UInt160[] Signers;
        public int RequiredSigs;
        public long Timestamp;
        public long ExpiryTime;
    }
    
    /// <summary>
    /// Initialize multi-sig contract with authorized signers
    /// </summary>
    public static bool InitializeSigners(UInt160[] initialSigners)
    {
        Assert(initialSigners.Length >= MIN_SIGNERS && initialSigners.Length <= MAX_SIGNERS, 
               $"Signer count must be between {MIN_SIGNERS} and {MAX_SIGNERS}");
        
        // Verify all signers are valid and unique
        for (int i = 0; i < initialSigners.Length; i++)
        {
            Assert(initialSigners[i] != null && initialSigners[i].IsValid, $"Invalid signer at index {i}");
            Assert(Runtime.CheckWitness(initialSigners[i]), $"Signer {i} must authorize initialization");
            
            // Check for duplicates
            for (int j = i + 1; j < initialSigners.Length; j++)
            {
                Assert(initialSigners[i] != initialSigners[j], "Duplicate signers not allowed");
            }
            
            AuthorizedSigners.Put(initialSigners[i], 1);
        }
        
        Storage.Put(Storage.CurrentContext, "signer_count", initialSigners.Length);
        OnSignersInitialized(initialSigners);
        return true;
    }
    
    /// <summary>
    /// Propose a multi-sig operation
    /// </summary>
    public static bool ProposeOperation(string operationId, string action, ByteString data, 
                                      UInt160[] proposedSigners, long expiryTime)
    {
        UInt160 proposer = (UInt160)Runtime.ScriptContainer.Sender;
        Assert(Runtime.CheckWitness(proposer), "Access denied: Invalid signature");
        Assert(IsAuthorizedSigner(proposer), "Access denied: Not authorized signer");
        
        Assert(!string.IsNullOrEmpty(operationId), "Operation ID required");
        Assert(!string.IsNullOrEmpty(action), "Action required");
        Assert(proposedSigners.Length >= REQUIRED_SIGNATURES, "Insufficient proposed signers");
        Assert(expiryTime > Runtime.Time, "Expiry time must be in future");
        
        // Verify all proposed signers are authorized
        foreach (var signer in proposedSigners)
        {
            Assert(IsAuthorizedSigner(signer), "Unauthorized signer in proposal");
        }
        
        // Check if operation already exists
        ByteString existingOp = PendingOperations.Get(operationId);
        Assert(existingOp == null, "Operation already exists");
        
        // Create operation
        MultiSigOperation operation = new MultiSigOperation
        {
            OperationId = operationId,
            Action = action,
            Data = data,
            Signers = new UInt160[] { proposer }, // Start with proposer
            RequiredSigs = REQUIRED_SIGNATURES,
            Timestamp = Runtime.Time,
            ExpiryTime = expiryTime
        };
        
        // Store operation
        PendingOperations.Put(operationId, StdLib.Serialize(operation));
        OnOperationProposed(operationId, proposer, action);
        return true;
    }
    
    /// <summary>
    /// Sign a pending multi-sig operation
    /// </summary>
    public static bool SignOperation(string operationId)
    {
        UInt160 signer = (UInt160)Runtime.ScriptContainer.Sender;
        Assert(Runtime.CheckWitness(signer), "Access denied: Invalid signature");
        Assert(IsAuthorizedSigner(signer), "Access denied: Not authorized signer");
        
        // Get pending operation
        ByteString operationData = PendingOperations.Get(operationId);
        Assert(operationData != null, "Operation not found");
        
        MultiSigOperation operation = (MultiSigOperation)StdLib.Deserialize(operationData);
        
        // Check expiry
        Assert(Runtime.Time <= operation.ExpiryTime, "Operation expired");
        
        // Check if already signed
        foreach (var existingSigner in operation.Signers)
        {
            Assert(existingSigner != signer, "Already signed by this signer");
        }
        
        // Add signature
        UInt160[] newSigners = new UInt160[operation.Signers.Length + 1];
        for (int i = 0; i < operation.Signers.Length; i++)
        {
            newSigners[i] = operation.Signers[i];
        }
        newSigners[operation.Signers.Length] = signer;
        operation.Signers = newSigners;
        
        // Update stored operation
        PendingOperations.Put(operationId, StdLib.Serialize(operation));
        
        OnOperationSigned(operationId, signer);
        
        // Check if enough signatures collected
        if (operation.Signers.Length >= operation.RequiredSigs)
        {
            return ExecuteOperation(operation);
        }
        
        return true;
    }
    
    /// <summary>
    /// Execute multi-sig operation once enough signatures are collected
    /// </summary>
    private static bool ExecuteOperation(MultiSigOperation operation)
    {
        Assert(operation.Signers.Length >= operation.RequiredSigs, "Insufficient signatures");
        
        // Execute based on action type
        bool success = false;
        switch (operation.Action)
        {
            case "transfer":
                success = ExecuteTransfer(operation.Data);
                break;
            case "update_config":
                success = ExecuteConfigUpdate(operation.Data);
                break;
            case "add_signer":
                success = ExecuteAddSigner(operation.Data);
                break;
            case "remove_signer":
                success = ExecuteRemoveSigner(operation.Data);
                break;
            default:
                Assert(false, "Unknown operation action");
                break;
        }
        
        if (success)
        {
            // Remove from pending operations
            PendingOperations.Delete(operation.OperationId);
            OnOperationExecuted(operation.OperationId, operation.Action);
        }
        
        return success;
    }
    
    /// <summary>
    /// Check if address is authorized signer
    /// </summary>
    [Safe]
    public static bool IsAuthorizedSigner(UInt160 address)
    {
        return AuthorizedSigners.Get(address) != null;
    }
    
    /// <summary>
    /// Get pending operation details
    /// </summary>
    [Safe]
    public static MultiSigOperation GetPendingOperation(string operationId)
    {
        ByteString operationData = PendingOperations.Get(operationId);
        Assert(operationData != null, "Operation not found");
        return (MultiSigOperation)StdLib.Deserialize(operationData);
    }
    
    // Execution methods for different operation types
    private static bool ExecuteTransfer(ByteString data)
    {
        // Implementation for transfer operations
        Storage.Put(Storage.CurrentContext, "last_transfer", data);
        return true;
    }
    
    private static bool ExecuteConfigUpdate(ByteString data)
    {
        // Implementation for config updates
        Storage.Put(Storage.CurrentContext, "config", data);
        return true;
    }
    
    private static bool ExecuteAddSigner(ByteString data)
    {
        UInt160 newSigner = (UInt160)data;
        Assert(newSigner != null && newSigner.IsValid, "Invalid signer address");
        Assert(!IsAuthorizedSigner(newSigner), "Signer already authorized");
        
        int currentCount = (int)Storage.Get(Storage.CurrentContext, "signer_count");
        Assert(currentCount < MAX_SIGNERS, "Maximum signers reached");
        
        AuthorizedSigners.Put(newSigner, 1);
        Storage.Put(Storage.CurrentContext, "signer_count", currentCount + 1);
        
        OnSignerAdded(newSigner);
        return true;
    }
    
    private static bool ExecuteRemoveSigner(ByteString data)
    {
        UInt160 signerToRemove = (UInt160)data;
        Assert(IsAuthorizedSigner(signerToRemove), "Signer not found");
        
        int currentCount = (int)Storage.Get(Storage.CurrentContext, "signer_count");
        Assert(currentCount > MIN_SIGNERS, "Cannot go below minimum signers");
        
        AuthorizedSigners.Delete(signerToRemove);
        Storage.Put(Storage.CurrentContext, "signer_count", currentCount - 1);
        
        OnSignerRemoved(signerToRemove);
        return true;
    }
    
    [DisplayName("SignersInitialized")]
    public static event Action<UInt160[]> OnSignersInitialized;
    
    [DisplayName("OperationProposed")]
    public static event Action<string, UInt160, string> OnOperationProposed;
    
    [DisplayName("OperationSigned")]
    public static event Action<string, UInt160> OnOperationSigned;
    
    [DisplayName("OperationExecuted")]
    public static event Action<string, string> OnOperationExecuted;
    
    [DisplayName("SignerAdded")]
    public static event Action<UInt160> OnSignerAdded;
    
    [DisplayName("SignerRemoved")]
    public static event Action<UInt160> OnSignerRemoved;
}
```

## Time-Based Access Control

Implement access control based on time constraints and schedules.

```csharp
[DisplayName("TimeBasedAccessContract")]
[ContractDescription("Time-based access control patterns")]
public class TimeBasedAccessContract : SmartContract
{
    private static readonly UInt160 OWNER = "NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB".ToScriptHash();
    private static readonly StorageMap TimeWindows = new(Storage.CurrentContext, "time_windows");
    private static readonly StorageMap UserAccess = new(Storage.CurrentContext, "user_access");
    
    /// <summary>
    /// Structure for time-based access windows
    /// </summary>
    public struct AccessWindow
    {
        public UInt160 User;
        public long StartTime;
        public long EndTime;
        public string[] Permissions;
        public bool IsActive;
    }
    
    /// <summary>
    /// Grant time-limited access to user
    /// </summary>
    public static bool GrantTimeLimitedAccess(UInt160 user, long startTime, long endTime, 
                                            string[] permissions)
    {
        Assert(Runtime.CheckWitness(OWNER), "Access denied: Owner only");
        Assert(user != null && user.IsValid, "Invalid user address");
        Assert(startTime >= Runtime.Time, "Start time must be in future");
        Assert(endTime > startTime, "End time must be after start time");
        Assert(permissions.Length > 0, "At least one permission required");
        
        string accessId = user.ToString() + "_" + startTime.ToString();
        
        AccessWindow window = new AccessWindow
        {
            User = user,
            StartTime = startTime,
            EndTime = endTime,
            Permissions = permissions,
            IsActive = true
        };
        
        TimeWindows.Put(accessId, StdLib.Serialize(window));
        OnAccessGranted(user, startTime, endTime, permissions);
        return true;
    }
    
    /// <summary>
    /// Check if user has specific permission at current time
    /// </summary>
    [Safe]
    public static bool HasTimeBasedPermission(UInt160 user, string permission)
    {
        if (user == OWNER) return true; // Owner always has access
        
        long currentTime = Runtime.Time;
        
        // Check all access windows for this user
        var iterator = Storage.Find(TimeWindows.Context, user.ToString(), FindOptions.None);
        while (iterator.Next())
        {
            AccessWindow window = (AccessWindow)StdLib.Deserialize(iterator.Value);
            
            // Check if window is active and current time is within window
            if (window.IsActive && 
                currentTime >= window.StartTime && 
                currentTime <= window.EndTime)
            {
                // Check if permission is granted in this window
                foreach (string perm in window.Permissions)
                {
                    if (perm == permission) return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Emergency access function (limited time window)
    /// </summary>
    public static bool EmergencyAccess(string action, string justification)
    {
        UInt160 caller = (UInt160)Runtime.ScriptContainer.Sender;
        Assert(Runtime.CheckWitness(caller), "Access denied: Invalid signature");
        
        // Check if caller has emergency access permission
        Assert(HasTimeBasedPermission(caller, "emergency"), "Access denied: No emergency permission");
        
        // Log emergency access
        Storage.Put(Storage.CurrentContext, "emergency_" + Runtime.Time, 
                   caller + ":" + action + ":" + justification);
        
        OnEmergencyAccess(caller, action, justification);
        return true;
    }
    
    /// <summary>
    /// Revoke access window before expiry
    /// </summary>
    public static bool RevokeAccess(UInt160 user, long startTime)
    {
        Assert(Runtime.CheckWitness(OWNER), "Access denied: Owner only");
        
        string accessId = user.ToString() + "_" + startTime.ToString();
        ByteString windowData = TimeWindows.Get(accessId);
        Assert(windowData != null, "Access window not found");
        
        AccessWindow window = (AccessWindow)StdLib.Deserialize(windowData);
        window.IsActive = false;
        
        TimeWindows.Put(accessId, StdLib.Serialize(window));
        OnAccessRevoked(user, startTime);
        return true;
    }
    
    /// <summary>
    /// Clean up expired access windows
    /// </summary>
    public static bool CleanupExpiredWindows(int maxCleanup)
    {
        Assert(maxCleanup > 0 && maxCleanup <= 100, "Invalid cleanup count");
        
        long currentTime = Runtime.Time;
        int cleaned = 0;
        List<ByteString> toDelete = new List<ByteString>();
        
        var iterator = Storage.Find(TimeWindows.Context, "", FindOptions.None);
        while (iterator.Next() && cleaned < maxCleanup)
        {
            AccessWindow window = (AccessWindow)StdLib.Deserialize(iterator.Value);
            
            if (currentTime > window.EndTime)
            {
                toDelete.Add(iterator.Key);
                cleaned++;
            }
        }
        
        // Delete expired windows
        foreach (ByteString key in toDelete)
        {
            TimeWindows.Delete(key);
        }
        
        OnExpiredWindowsCleaned(cleaned);
        return true;
    }
    
    [DisplayName("AccessGranted")]
    public static event Action<UInt160, long, long, string[]> OnAccessGranted;
    
    [DisplayName("AccessRevoked")]
    public static event Action<UInt160, long> OnAccessRevoked;
    
    [DisplayName("EmergencyAccess")]
    public static event Action<UInt160, string, string> OnEmergencyAccess;
    
    [DisplayName("ExpiredWindowsCleaned")]
    public static event Action<int> OnExpiredWindowsCleaned;
}
```

## Best Practices

### 1. Access Control Security Checklist

- ✅ **Always use `Runtime.CheckWitness()`** for cryptographic verification
- ✅ **Implement fail-safe defaults** - deny access by default
- ✅ **Use principle of least privilege** - grant minimum necessary permissions
- ✅ **Validate all inputs** before performing authorization checks
- ✅ **Emit events** for all access control changes for audit trails
- ✅ **Implement emergency procedures** for critical situations
- ✅ **Test all access control paths** thoroughly

### 2. Common Pitfalls to Avoid

```csharp
// ❌ WRONG: Only checking sender without witness verification
public static bool WrongAccessControl()
{
    UInt160 sender = (UInt160)Runtime.ScriptContainer.Sender;
    if (sender == OWNER) // This can be spoofed!
    {
        return PerformAdminAction();
    }
    return false;
}

// ✅ CORRECT: Always verify witness
public static bool CorrectAccessControl()
{
    Assert(Runtime.CheckWitness(OWNER), "Access denied: Owner only");
    return PerformAdminAction();
}

// ❌ WRONG: Storing sensitive data without proper access control
public static bool WrongDataAccess(UInt160 user, ByteString sensitiveData)
{
    Storage.Put(Storage.CurrentContext, "sensitive_" + user, sensitiveData);
    return true;
}

// ✅ CORRECT: Proper access control for sensitive operations
public static bool CorrectDataAccess(UInt160 user, ByteString sensitiveData)
{
    Assert(Runtime.CheckWitness(user), "Access denied: Invalid signature");
    Assert(HasPermission(user, "write_sensitive"), "Access denied: No permission");
    
    Storage.Put(Storage.CurrentContext, "sensitive_" + user, sensitiveData);
    return true;
}
```

### 3. Performance Considerations

```csharp
// Optimize frequent access control checks
private static readonly Dictionary<UInt160, bool> _adminCache = new();

[Safe]
public static bool IsAdminCached(UInt160 user)
{
    // Use cache for frequently checked permissions
    if (_adminCache.ContainsKey(user))
        return _adminCache[user];
    
    bool isAdmin = Storage.Get(Storage.CurrentContext, "admin_" + user) != null;
    _adminCache[user] = isAdmin;
    
    return isAdmin;
}
```

## Testing Access Control

### Comprehensive Test Framework

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

[TestClass]
public class AccessControlTests : TestBase<RBACContract>
{
    [TestInitialize]
    public void Setup()
    {
        // Setup test environment
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
    }
    
    [TestMethod]
    public void TestOwnerAccess()
    {
        // Test owner has admin access
        Engine.SetCallingScriptHash(OwnerAddress);
        Assert.IsTrue(Contract.HasRole(OwnerAddress, "admin"));
        
        // Test owner can grant roles
        Assert.IsTrue(Contract.GrantRole(TestUser1, "moderator"));
        Assert.IsTrue(Contract.HasRole(TestUser1, "moderator"));
    }
    
    [TestMethod]
    public void TestUnauthorizedAccess()
    {
        // Test unauthorized user cannot grant roles
        Engine.SetCallingScriptHash(TestUser1);
        Assert.ThrowsException<Exception>(() => 
            Contract.GrantRole(TestUser2, "admin"));
    }
    
    [TestMethod]
    public void TestRoleHierarchy()
    {
        // Setup roles
        Engine.SetCallingScriptHash(OwnerAddress);
        Contract.GrantRole(TestUser1, "admin");
        Contract.GrantRole(TestUser2, "moderator");
        Contract.GrantRole(TestUser3, "user");
        
        // Test permissions
        Assert.IsTrue(Contract.HasPermission(TestUser1, "manage_contract"));
        Assert.IsTrue(Contract.HasPermission(TestUser2, "moderate_content"));
        Assert.IsTrue(Contract.HasPermission(TestUser3, "basic_access"));
        
        // Test permission boundaries
        Assert.IsFalse(Contract.HasPermission(TestUser3, "manage_contract"));
        Assert.IsFalse(Contract.HasPermission(TestUser2, "grant_role"));
    }
    
    [TestMethod]
    public void TestMultiSignature()
    {
        // Test multi-signature operations
        Engine.SetCallingScriptHash(Signer1);
        Assert.IsTrue(Contract.ProposeOperation("op1", "transfer", "data", 
            new[] { Signer1, Signer2, Signer3 }, Runtime.Time + 3600));
        
        // Test signing process
        Engine.SetCallingScriptHash(Signer2);
        Assert.IsTrue(Contract.SignOperation("op1"));
        
        Engine.SetCallingScriptHash(Signer3);
        Assert.IsTrue(Contract.SignOperation("op1"));
        
        // Operation should be executed after sufficient signatures
        var notifications = Notifications;
        Assert.IsTrue(notifications.Any(n => n.EventName == "OperationExecuted"));
    }
}
```

Access control is the foundation of smart contract security. Always implement multiple layers of protection, test thoroughly, and follow the principle of least privilege to maintain a secure contract ecosystem.