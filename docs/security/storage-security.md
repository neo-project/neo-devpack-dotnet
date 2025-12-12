# Storage Security for Neo Smart Contracts

Comprehensive guide for implementing secure data storage patterns in Neo N3 smart contracts with advanced key management, encryption, and access control.

> **Foundation**: Review [Common Vulnerabilities](common-vulnerabilities.md#storage-manipulation) for basic storage security concepts.

## Table of Contents

- [Storage Fundamentals](#storage-fundamentals)
- [Key Management Patterns](#key-management-patterns)
- [Data Isolation and Namespacing](#data-isolation-and-namespacing)
- [Encryption and Privacy](#encryption-and-privacy)
- [Access Control for Storage](#access-control-for-storage)
- [Data Integrity Protection](#data-integrity-protection)
- [Storage Optimization](#storage-optimization)
- [Audit and Monitoring](#audit-and-monitoring)
- [Testing Storage Security](#testing-storage-security)

## Storage Fundamentals

### Neo Storage Context

Neo smart contracts use a key-value storage system. Understanding storage contexts and proper key management is essential for security.

```csharp
using System.ComponentModel;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

[DisplayName("SecureStorageContract")]
[ContractDescription("Demonstrates secure storage patterns")]
public class SecureStorageContract : SmartContract
{
    // Storage prefixes for different data types
    private enum StoragePrefix : byte
    {
        UserData = 0x01,
        AdminData = 0x02,
        PublicConfig = 0x03,
        PrivateConfig = 0x04,
        AuditLog = 0x05,
        Temporary = 0x06
    }
    
    // All storage lives under the same context. Prefixes are applied via StorageMap
    // or manual key construction to keep data isolated.
    private static StorageContext UserContext => Storage.CurrentContext;
    private static StorageContext AdminContext => Storage.CurrentContext;
    private static StorageContext AuditContext => Storage.CurrentContext;
}
```

### Advanced Storage Patterns

This guide focuses on production-ready storage security implementations including encryption, data isolation, integrity protection, and comprehensive audit systems.

## Key Management Patterns

### 1. Hierarchical Key Structure

```csharp
public class HierarchicalKeyStorage : SmartContract
{
    /// <summary>
    /// Create hierarchical keys to prevent collisions and organize data
    /// </summary>
    private static ByteString CreateUserKey(UInt160 user, string category, string identifier)
    {
        ExecutionEngine.Assert(user != null && user.IsValid, "Invalid user address");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(category), "Category required");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(identifier), "Identifier required");
        
        // Create hierarchical key: prefix + user + category + identifier
        return ((ByteString)((byte)StoragePrefix.UserData).ToByteArray())
            .Concat(user)
            .Concat(category)
            .Concat(identifier);
    }
    
    /// <summary>
    /// Store user data with hierarchical key structure
    /// </summary>
    public static bool StoreUserData(UInt160 user, string category, string identifier, 
                                   ByteString data)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied: Invalid user signature");
        ExecutionEngine.Assert(data.Length <= 65536, "Data too large"); // 64KB limit
        
        ByteString key = CreateUserKey(user, category, identifier);
        Storage.Put(Storage.CurrentContext, key, data);
        
        // Log storage operation for audit
        LogStorageOperation(user, "store", category, identifier, data.Length);
        OnDataStored(user, category, identifier);
        
        return true;
    }
    
    /// <summary>
    /// Retrieve user data with access control
    /// </summary>
    [Safe]
    public static ByteString GetUserData(UInt160 user, UInt160 requester, 
                                       string category, string identifier)
    {
        // Access control: user can access own data, or requester needs permission
        ExecutionEngine.Assert(user == requester || HasDataAccessPermission(requester, user), 
               "Access denied: No permission to read user data");
        
        ByteString key = CreateUserKey(user, category, identifier);
        ByteString data = Storage.Get(Storage.CurrentContext, key);
        
        if (data != null)
        {
            LogStorageOperation(requester, "read", category, identifier, data.Length);
        }
        
        return data;
    }
    
    /// <summary>
    /// Check if requester has permission to access user's data
    /// </summary>
    private static bool HasDataAccessPermission(UInt160 requester, UInt160 dataOwner)
    {
        // Check if requester is authorized to read this user's data
        StorageMap permissions = new(AdminContext, "data_permissions");
        return permissions.Get(requester + dataOwner) != null;
    }
}
```

### 2. Secure Key Generation

```csharp
public class SecureKeyGeneration : SmartContract
{
    /// <summary>
    /// Generate secure storage keys using cryptographic hashing
    /// </summary>
    private static ByteString GenerateSecureKey(UInt160 user, string dataType, 
                                              string identifier, bool includeTimestamp = false)
    {
        ExecutionEngine.Assert(user != null && user.IsValid, "Invalid user");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(dataType), "Data type required");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(identifier), "Identifier required");
        
        // Build key components
        ByteString keyData = user
            .Concat(dataType)
            .Concat(identifier);
        
        if (includeTimestamp)
        {
            keyData = keyData.Concat((ByteString)(System.Numerics.BigInteger)Runtime.Time);
        }
        
        // Hash for consistent length and security
        ByteString hashedKey = CryptoLib.Sha256(keyData);
        
        // Prefix with data type for organization
        return (ByteString)((byte)StoragePrefix.UserData).ToByteArray()
            .Concat(dataType.ToByteArray().Take(4)) // First 4 bytes of type
            .Concat(((byte[])hashedKey).Take(28)); // First 28 bytes of hash
    }
    
    /// <summary>
    /// Store data with secure key generation
    /// </summary>
    public static bool SecureStore(UInt160 user, string dataType, string identifier, 
                                 ByteString data, bool isTemporary = false)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        
        ByteString key = GenerateSecureKey(user, dataType, identifier, isTemporary);
        
        // Store the data
        Storage.Put(Storage.CurrentContext, key, data);
        
        // Store key mapping for retrieval (if not temporary)
        if (!isTemporary)
        {
            ByteString mappingKey = user.Concat(dataType + identifier);
            Storage.Put(Storage.CurrentContext, "keymap_" + mappingKey, key);
        }
        
        OnSecureDataStored(user, dataType, identifier, isTemporary);
        return true;
    }
    
    /// <summary>
    /// Retrieve data using secure key lookup
    /// </summary>
    [Safe]
    public static ByteString SecureRetrieve(UInt160 user, string dataType, string identifier)
    {
        ByteString mappingKey = user.Concat(dataType + identifier);
        ByteString actualKey = Storage.Get(Storage.CurrentContext, "keymap_" + mappingKey);
        
        if (actualKey == null) return null;
        
        return Storage.Get(Storage.CurrentContext, actualKey);
    }
}
```

## Data Isolation and Namespacing

### 1. Multi-Tenant Data Isolation

```csharp
public class MultiTenantStorage : SmartContract
{
    /// <summary>
    /// Tenant-isolated storage with strict separation
    /// </summary>
    public static bool StoreTenantData(UInt160 tenant, string namespace_, string key, 
                                     ByteString data)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(tenant), "Access denied: Invalid tenant signature");
        ExecutionEngine.Assert(IsTenantAuthorized(tenant), "Access denied: Tenant not authorized");
        ExecutionEngine.Assert(IsValidNamespace(namespace_), "Invalid namespace");
        ExecutionEngine.Assert(IsValidKey(key), "Invalid key format");
        ExecutionEngine.Assert(data.Length <= GetTenantStorageLimit(tenant), "Data exceeds tenant limit");
        
        // Create isolated tenant key
        ByteString tenantKey = CreateTenantKey(tenant, namespace_, key);
        
        // Verify tenant hasn't exceeded storage quota
        ExecutionEngine.Assert(CheckTenantQuota(tenant, data.Length), "Tenant quota exceeded");
        
        Storage.Put(Storage.CurrentContext, tenantKey, data);
        UpdateTenantUsage(tenant, data.Length);
        
        OnTenantDataStored(tenant, namespace_, key);
        return true;
    }
    
    /// <summary>
    /// Create isolated tenant key with multiple isolation layers
    /// </summary>
    private static ByteString CreateTenantKey(UInt160 tenant, string namespace_, string key)
    {
        // Layer 1: Tenant prefix
        byte[] tenantPrefix = ((byte)StoragePrefix.UserData).ToByteArray()
            .Concat("tenant_".ToByteArray())
            .Concat(tenant);
        
        // Layer 2: Namespace separation
        ByteString namespaceHash = CryptoLib.Sha256(namespace_);
        
        // Layer 3: Key hashing for security
        ByteString keyHash = CryptoLib.Sha256(key);
        
        return (ByteString)tenantPrefix
            .Concat(((byte[])namespaceHash).Take(8))
            .Concat(((byte[])keyHash).Take(16));
    }
    
    /// <summary>
    /// Retrieve tenant data with strict access control
    /// </summary>
    [Safe]
    public static ByteString GetTenantData(UInt160 tenant, UInt160 requester, 
                                         string namespace_, string key)
    {
        // Strict access control - only tenant or authorized admins
        ExecutionEngine.Assert(tenant == requester || IsAuthorizedAdmin(requester), 
               "Access denied: Insufficient permissions");
        
        if (tenant != requester)
        {
            // Log admin access for audit
            LogAdminAccess(requester, tenant, namespace_, key);
        }
        
        ByteString tenantKey = CreateTenantKey(tenant, namespace_, key);
        return Storage.Get(Storage.CurrentContext, tenantKey);
    }
    
    /// <summary>
    /// Bulk tenant data operations with atomicity
    /// </summary>
    public static bool BulkStoreTenantData(UInt160 tenant, 
                                         (string namespace_, string key, ByteString data)[] items)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(tenant), "Access denied");
        ExecutionEngine.Assert(items.Length <= 50, "Bulk operation too large");
        
        // Pre-validate all items
        int totalSize = 0;
        foreach (var item in items)
        {
            ExecutionEngine.Assert(IsValidNamespace(item.namespace_), $"Invalid namespace: {item.namespace_}");
            ExecutionEngine.Assert(IsValidKey(item.key), $"Invalid key: {item.key}");
            totalSize += item.data.Length;
        }
        
        ExecutionEngine.Assert(CheckTenantQuota(tenant, totalSize), "Bulk operation exceeds quota");
        
        // Perform atomic bulk storage
        foreach (var item in items)
        {
            ByteString tenantKey = CreateTenantKey(tenant, item.namespace_, item.key);
            Storage.Put(Storage.CurrentContext, tenantKey, item.data);
        }
        
        UpdateTenantUsage(tenant, totalSize);
        OnBulkTenantDataStored(tenant, items.Length, totalSize);
        
        return true;
    }
    
    // Helper methods
    private static bool IsTenantAuthorized(UInt160 tenant)
    {
        StorageMap authorizedTenants = new(AdminContext, "tenants");
        return authorizedTenants.Get(tenant) != null;
    }
    
    private static bool IsValidNamespace(string namespace_)
    {
        return !string.IsNullOrEmpty(namespace_) && 
               namespace_.Length <= 32 && 
               namespace_.All(c => char.IsLetterOrDigit(c) || c == '_');
    }
    
    private static bool IsValidKey(string key)
    {
        return !string.IsNullOrEmpty(key) && 
               key.Length <= 64 && 
               key.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }
    
    private static int GetTenantStorageLimit(UInt160 tenant)
    {
        StorageMap limits = new(AdminContext, "tenant_limits");
        ByteString limit = limits.Get(tenant);
        return limit != null ? (int)limit : 1048576; // Default 1MB
    }
    
    private static bool CheckTenantQuota(UInt160 tenant, int additionalSize)
    {
        StorageMap usage = new(AdminContext, "tenant_usage");
        int currentUsage = (int)(usage.Get(tenant) ?? 0);
        int limit = GetTenantStorageLimit(tenant);
        
        return currentUsage + additionalSize <= limit;
    }
    
    private static void UpdateTenantUsage(UInt160 tenant, int additionalSize)
    {
        StorageMap usage = new(AdminContext, "tenant_usage");
        int currentUsage = (int)(usage.Get(tenant) ?? 0);
        usage.Put(tenant, currentUsage + additionalSize);
    }
}
```

## Encryption and Privacy

### 1. Client-Side Encryption Pattern

```csharp
public class EncryptedStorage : SmartContract
{
    /// <summary>
    /// Store encrypted data (encryption performed client-side)
    /// </summary>
    public static bool StoreEncryptedData(UInt160 user, string identifier, 
                                        ByteString encryptedData, ByteString keyHash)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(identifier), "Identifier required");
        ExecutionEngine.Assert(encryptedData.Length > 0, "No data provided");
        ExecutionEngine.Assert(keyHash.Length == 32, "Invalid key hash length"); // SHA256 hash
        
        // Store encrypted data
        ByteString dataKey = CreateUserKey(user, "encrypted", identifier);
        Storage.Put(Storage.CurrentContext, dataKey, encryptedData);
        
        // Store key hash for verification (not the actual key!)
        ByteString hashKey = CreateUserKey(user, "keyhash", identifier);
        Storage.Put(Storage.CurrentContext, hashKey, keyHash);
        
        // Store metadata
        ByteString metaKey = CreateUserKey(user, "meta", identifier);
        ByteString metadata = StdLib.Serialize(new {
            Timestamp = Runtime.Time,
            Size = encryptedData.Length,
            Owner = user
        });
        Storage.Put(Storage.CurrentContext, metaKey, metadata);
        
        OnEncryptedDataStored(user, identifier, encryptedData.Length);
        return true;
    }
    
    /// <summary>
    /// Retrieve encrypted data with key verification
    /// </summary>
    [Safe]
    public static ByteString GetEncryptedData(UInt160 user, string identifier, 
                                            ByteString providedKeyHash)
    {
        // Verify key hash before returning data
        ByteString hashKey = CreateUserKey(user, "keyhash", identifier);
        ByteString storedKeyHash = Storage.Get(Storage.CurrentContext, hashKey);
        
        ExecutionEngine.Assert(storedKeyHash != null, "Data not found");
        ExecutionEngine.Assert(storedKeyHash == providedKeyHash, "Invalid decryption key");
        
        // Return encrypted data
        ByteString dataKey = CreateUserKey(user, "encrypted", identifier);
        return Storage.Get(Storage.CurrentContext, dataKey);
    }
}
```

### 2. On-Chain Privacy Protection

```csharp
public class PrivacyProtectedStorage : SmartContract
{
    /// <summary>
    /// Store data with privacy protection using commitment schemes
    /// </summary>
    public static bool StorePrivateData(UInt160 user, ByteString commitment, 
                                      ByteString encryptedData)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        ExecutionEngine.Assert(commitment.Length == 32, "Invalid commitment length");
        ExecutionEngine.Assert(encryptedData.Length > 0, "No data provided");
        
        // Store commitment instead of raw identifier
        ByteString key = ((ByteString)((byte)StoragePrefix.PrivateConfig).ToByteArray())
            .Concat(user)
            .Concat(commitment);
        
        Storage.Put(Storage.CurrentContext, key, encryptedData);
        
        // Store only hash of the data for verification
        ByteString dataHash = CryptoLib.Sha256(encryptedData);
        ByteString verifyKey = key.Concat("_verify");
        Storage.Put(Storage.CurrentContext, verifyKey, dataHash);
        
        OnPrivateDataCommitted(user, commitment);
        return true;
    }
    
    /// <summary>
    /// Reveal and retrieve private data using original value and nonce
    /// </summary>
    public static ByteString RevealPrivateData(UInt160 user, string originalValue, 
                                             ByteString nonce)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        
        // Recreate commitment
        ByteString commitment = CryptoLib.Sha256(
            ((ByteString)originalValue).Concat(nonce)
        );
        
        ByteString key = ((ByteString)((byte)StoragePrefix.PrivateConfig).ToByteArray())
            .Concat(user)
            .Concat(commitment);
        
        ByteString encryptedData = Storage.Get(Storage.CurrentContext, key);
        ExecutionEngine.Assert(encryptedData != null, "Data not found or invalid reveal");
        
        // Verify data integrity
        ByteString verifyKey = key.Concat("_verify");
        ByteString storedHash = Storage.Get(Storage.CurrentContext, verifyKey);
        ByteString currentHash = CryptoLib.Sha256(encryptedData);
        ExecutionEngine.Assert(storedHash == currentHash, "Data integrity check failed");
        
        OnPrivateDataRevealed(user, originalValue);
        return encryptedData;
    }
}
```

## Data Integrity Protection

### 1. Checksums and Verification

```csharp
public class IntegrityProtectedStorage : SmartContract
{
    /// <summary>
    /// Store data with integrity protection
    /// </summary>
    public static bool StoreWithIntegrity(UInt160 user, string identifier, 
                                        ByteString data, bool enableVersioning = false)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(identifier), "Identifier required");
        ExecutionEngine.Assert(data.Length > 0, "No data provided");
        
        // Calculate multiple checksums for robust integrity protection
        ByteString sha256Hash = CryptoLib.Sha256(data);
        ByteString ripemd160Hash = CryptoLib.Ripemd160(data);
        
        // Create version number if versioning enabled
        int version = 1;
        if (enableVersioning)
        {
            version = GetNextVersion(user, identifier);
        }
        
        // Store data with version
        ByteString dataKey = CreateVersionedKey(user, identifier, version);
        Storage.Put(Storage.CurrentContext, dataKey, data);
        
        // Store integrity information in a deterministic order.
        // StdLib.Serialize/Deserialize works well with object arrays and avoids dynamic access.
        object[] integrityInfo = new object[]
        {
            sha256Hash,
            ripemd160Hash,
            Runtime.Time,
            data.Length,
            version
        };
        
        ByteString integrityKey = CreateVersionedKey(user, identifier + "_integrity", version);
        Storage.Put(Storage.CurrentContext, integrityKey, StdLib.Serialize(integrityInfo));
        
        // Update latest version pointer
        if (enableVersioning)
        {
            ByteString versionKey = CreateUserKey(user, "version", identifier);
            Storage.Put(Storage.CurrentContext, versionKey, version);
        }
        
        OnDataStoredWithIntegrity(user, identifier, version);
        return true;
    }
    
    /// <summary>
    /// Retrieve and verify data integrity
    /// </summary>
    [Safe]
    public static ByteString GetWithIntegrityCheck(UInt160 user, string identifier, 
                                                 int version = -1)
    {
        // Get version to retrieve
        if (version == -1)
        {
            ByteString versionKey = CreateUserKey(user, "version", identifier);
            ByteString versionData = Storage.Get(Storage.CurrentContext, versionKey);
            version = versionData != null ? (int)versionData : 1;
        }
        
        // Retrieve data and integrity info
        ByteString dataKey = CreateVersionedKey(user, identifier, version);
        ByteString data = Storage.Get(Storage.CurrentContext, dataKey);
        
        if (data == null) return null;
        
        ByteString integrityKey = CreateVersionedKey(user, identifier + "_integrity", version);
        ByteString integrityData = Storage.Get(Storage.CurrentContext, integrityKey);
        
        if (integrityData != null)
        {
            // Verify integrity
            object[] storedInfo = (object[])StdLib.Deserialize(integrityData);

            ByteString storedSHA256 = (ByteString)storedInfo[0];
            ByteString storedRIPEMD160 = (ByteString)storedInfo[1];
            int storedSize = (int)storedInfo[3];
            
            ByteString currentSHA256 = CryptoLib.Sha256(data);
            ByteString currentRIPEMD160 = CryptoLib.Ripemd160(data);
            
            ExecutionEngine.Assert(currentSHA256 == storedSHA256, "SHA256 integrity check failed");
            ExecutionEngine.Assert(currentRIPEMD160 == storedRIPEMD160, "RIPEMD160 integrity check failed");
            ExecutionEngine.Assert(data.Length == storedSize, "Size integrity check failed");
            
            OnIntegrityVerified(user, identifier, version);
        }
        
        return data;
    }
    
    /// <summary>
    /// Get all versions of data for an identifier
    /// </summary>
    [Safe]
    public static int[] GetVersionHistory(UInt160 user, string identifier)
    {
        Neo.SmartContract.Framework.List<int> versions = new Neo.SmartContract.Framework.List<int>();
        
        // Search for all versions
        string versionPrefix = user.ToString() + "_" + identifier + "_v";
        var iterator = (Iterator<ByteString>)Storage.Find(Storage.CurrentContext, versionPrefix, FindOptions.KeysOnly);
        
        while (iterator.Next())
        {
            string key = (string)iterator.Value;
            // Extract version number from key
            string versionStr = key.Substring(key.LastIndexOf("_v") + 2);
            if (int.TryParse(versionStr, out int version))
            {
                versions.Add(version);
            }
        }

        SortIntList(versions);
        return versions;
    }

    private static void SortIntList(Neo.SmartContract.Framework.List<int> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (list[j] > list[j + 1])
                {
                    int temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }
    
    private static ByteString CreateVersionedKey(UInt160 user, string identifier, int version)
    {
        return CreateUserKey(user, identifier, $"v{version}");
    }
    
    private static int GetNextVersion(UInt160 user, string identifier)
    {
        ByteString versionKey = CreateUserKey(user, "version", identifier);
        ByteString currentVersion = Storage.Get(Storage.CurrentContext, versionKey);
        return currentVersion != null ? (int)currentVersion + 1 : 1;
    }
}
```

## Audit and Monitoring

### 1. Comprehensive Audit Logging

```csharp
public class AuditableStorage : SmartContract
{
    /// <summary>
    /// Audit log entry structure
    /// </summary>
    public struct AuditEntry
    {
        public UInt160 User;
        public string Operation;
        public string Target;
        public long Timestamp;
        public int DataSize;
        public ByteString OperationHash;
    }
    
    /// <summary>
    /// Store data with comprehensive audit logging
    /// </summary>
    public static bool StoreWithAudit(UInt160 user, string category, string identifier, 
                                    ByteString data)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        
        // Store the actual data
        ByteString dataKey = CreateUserKey(user, category, identifier);
        Storage.Put(Storage.CurrentContext, dataKey, data);
        
        // Create audit entry
        ByteString operationData = user.Concat("store")
            .Concat(category)
            .Concat(identifier)
            .Concat(data);
        
        AuditEntry auditEntry = new AuditEntry
        {
            User = user,
            Operation = "store",
            Target = category + "/" + identifier,
            Timestamp = Runtime.Time,
            DataSize = data.Length,
            OperationHash = CryptoLib.Sha256(operationData)
        };
        
        // Store audit entry
        LogAuditEntry(auditEntry);
        
        OnAuditedOperation(user, "store", category, identifier);
        return true;
    }
    
    /// <summary>
    /// Delete data with audit trail
    /// </summary>
    public static bool DeleteWithAudit(UInt160 user, string category, string identifier, 
                                     string reason)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Access denied");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(reason), "Deletion reason required");
        
        ByteString dataKey = CreateUserKey(user, category, identifier);
        ByteString existingData = Storage.Get(Storage.CurrentContext, dataKey);
        
        ExecutionEngine.Assert(existingData != null, "Data not found");
        
        // Archive data before deletion
        ByteString archiveKey = dataKey.Concat("_archived_")
            .Concat((ByteString)(System.Numerics.BigInteger)Runtime.Time);
        Storage.Put(Storage.CurrentContext, archiveKey, existingData);
        
        // Delete original data
        Storage.Delete(Storage.CurrentContext, dataKey);
        
        // Create audit entry
        ByteString operationData = user.Concat("delete")
            .Concat(category)
            .Concat(identifier)
            .Concat(reason);
        
        AuditEntry auditEntry = new AuditEntry
        {
            User = user,
            Operation = "delete",
            Target = category + "/" + identifier,
            Timestamp = Runtime.Time,
            DataSize = existingData.Length,
            OperationHash = CryptoLib.Sha256(operationData)
        };
        
        LogAuditEntry(auditEntry);
        
        OnAuditedOperation(user, "delete", category, identifier);
        return true;
    }
    
    /// <summary>
    /// Get audit trail for specific user/target
    /// </summary>
    [Safe]
    public static AuditEntry[] GetAuditTrail(UInt160 requester, UInt160 targetUser, 
                                           string category, int limit = 50)
    {
        ExecutionEngine.Assert(HasAuditAccess(requester, targetUser), "Access denied: No audit permission");
        ExecutionEngine.Assert(limit > 0 && limit <= 100, "Invalid limit");
        
        Neo.SmartContract.Framework.List<AuditEntry> entries = new Neo.SmartContract.Framework.List<AuditEntry>();
        string searchPrefix = targetUser.ToString();
        if (!string.IsNullOrEmpty(category))
        {
            searchPrefix += "_" + category;
        }
        
        var iterator = (Iterator<ByteString>)Storage.Find(AuditContext, searchPrefix, FindOptions.ValuesOnly);
        int count = 0;
        
        while (iterator.Next() && count < limit)
        {
            AuditEntry entry = (AuditEntry)StdLib.Deserialize(iterator.Value);
            entries.Add(entry);
            count++;
        }
        
        return entries;
    }
    
    /// <summary>
    /// Store audit entry with tamper protection
    /// </summary>
    private static void LogAuditEntry(AuditEntry entry)
    {
        // Create unique audit key
        ByteString auditKey = (ByteString)(((byte)StoragePrefix.AuditLog).ToByteArray()
            .Concat(entry.User)
            .Concat(((System.Numerics.BigInteger)entry.Timestamp).ToByteArray())
            .Concat(((byte[])CryptoLib.Sha256(entry.Operation + entry.Target)).Take(8)));
        
        // Store audit entry
        Storage.Put(AuditContext, auditKey, StdLib.Serialize(entry));
        
        // Update audit index for efficient querying
        UpdateAuditIndex(entry);
    }
    
    /// <summary>
    /// Update audit index for efficient searching
    /// </summary>
    private static void UpdateAuditIndex(AuditEntry entry)
    {
        // User-based index
        ByteString userIndexKey = "user_index_".ToByteArray().Concat(entry.User);
        Neo.SmartContract.Framework.List<ByteString> userEntries = GetIndexEntries(userIndexKey);
        userEntries.Add((ByteString)((System.Numerics.BigInteger)entry.Timestamp).ToByteArray());
        
        // Keep only recent entries in index (trim oldest)
        while (userEntries.Count > 1000)
        {
            userEntries.RemoveAt(0);
        }
        
        Storage.Put(AuditContext, userIndexKey, StdLib.Serialize(userEntries));
    }
    
    private static Neo.SmartContract.Framework.List<ByteString> GetIndexEntries(ByteString indexKey)
    {
        ByteString indexData = Storage.Get(AuditContext, indexKey);
        return indexData != null
            ? (Neo.SmartContract.Framework.List<ByteString>)StdLib.Deserialize(indexData)
            : new Neo.SmartContract.Framework.List<ByteString>();
    }
    
    private static bool HasAuditAccess(UInt160 requester, UInt160 targetUser)
    {
        // Users can view their own audit trail, admins can view any
        return requester == targetUser || IsAdmin(requester);
    }
    
    [DisplayName("AuditedOperation")]
    public static event Action<UInt160, string, string, string> OnAuditedOperation;
}
```

## Testing Storage Security

### Comprehensive Storage Security Tests

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

[TestClass]
public class StorageSecurityTests : TestBase<SecureStorageContract>
{
    [TestInitialize]
    public void Setup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
    }
    
    [TestMethod]
    public void TestDataIsolation()
    {
        // Store data for different users
        Engine.SetCallingScriptHash(User1);
        Assert.IsTrue(Contract.StoreUserData(User1, "profile", "name", "User1Data"));
        
        Engine.SetCallingScriptHash(User2);
        Assert.IsTrue(Contract.StoreUserData(User2, "profile", "name", "User2Data"));
        
        // Verify isolation - users can only access their own data
        var user1Data = Contract.GetUserData(User1, User1, "profile", "name");
        var user2Data = Contract.GetUserData(User2, User2, "profile", "name");
        
        Assert.AreEqual("User1Data", user1Data.GetString());
        Assert.AreEqual("User2Data", user2Data.GetString());
        
        // Verify unauthorized access is denied
        Assert.ThrowsException<Exception>(() =>
            Contract.GetUserData(User1, User2, "profile", "name"));
    }
    
    [TestMethod]
    public void TestStorageQuotas()
    {
        // Test storage quota enforcement
        byte[] largeData = new byte[2048]; // 2KB
        byte[] exceedsQuota = new byte[2097152]; // 2MB, exceeds default quota
        
        Engine.SetCallingScriptHash(TenantUser);
        
        // Normal size should work
        Assert.IsTrue(Contract.StoreTenantData(TenantUser, "test", "small", largeData));
        
        // Exceeding quota should fail
        Assert.ThrowsException<Exception>(() =>
            Contract.StoreTenantData(TenantUser, "test", "large", exceedsQuota));
    }
    
    [TestMethod]
    public void TestIntegrityProtection()
    {
        // Store data with integrity protection
        string testData = "Important contract data";
        Engine.SetCallingScriptHash(User1);
        
        Assert.IsTrue(Contract.StoreWithIntegrity(User1, "important", testData.ToByteArray()));
        
        // Retrieve and verify integrity
        var retrievedData = Contract.GetWithIntegrityCheck(User1, "important");
        Assert.AreEqual(testData, retrievedData.GetString());
        
        // Verify integrity check events
        var integrityEvents = Notifications.Where(n => n.EventName == "IntegrityVerified");
        Assert.IsTrue(integrityEvents.Any());
    }
    
    [TestMethod]
    public void TestVersioning()
    {
        Engine.SetCallingScriptHash(User1);
        
        // Store multiple versions
        Assert.IsTrue(Contract.StoreWithIntegrity(User1, "doc", "Version 1".ToByteArray(), true));
        Assert.IsTrue(Contract.StoreWithIntegrity(User1, "doc", "Version 2".ToByteArray(), true));
        Assert.IsTrue(Contract.StoreWithIntegrity(User1, "doc", "Version 3".ToByteArray(), true));
        
        // Retrieve specific versions
        var v1 = Contract.GetWithIntegrityCheck(User1, "doc", 1);
        var v2 = Contract.GetWithIntegrityCheck(User1, "doc", 2);
        var latest = Contract.GetWithIntegrityCheck(User1, "doc", -1);
        
        Assert.AreEqual("Version 1", v1.GetString());
        Assert.AreEqual("Version 2", v2.GetString());
        Assert.AreEqual("Version 3", latest.GetString());
        
        // Check version history
        var versions = Contract.GetVersionHistory(User1, "doc");
        Assert.AreEqual(3, versions.Length);
    }
    
    [TestMethod]
    public void TestAuditTrail()
    {
        Engine.SetCallingScriptHash(User1);
        
        // Perform audited operations
        Assert.IsTrue(Contract.StoreWithAudit(User1, "docs", "file1", "content1".ToByteArray()));
        Assert.IsTrue(Contract.StoreWithAudit(User1, "docs", "file2", "content2".ToByteArray()));
        Assert.IsTrue(Contract.DeleteWithAudit(User1, "docs", "file1", "No longer needed"));
        
        // Check audit trail
        Engine.SetCallingScriptHash(AdminUser);
        var auditTrail = Contract.GetAuditTrail(AdminUser, User1, "docs", 10);
        
        Assert.AreEqual(3, auditTrail.Length);
        Assert.IsTrue(auditTrail.Any(e => e.Operation == "store"));
        Assert.IsTrue(auditTrail.Any(e => e.Operation == "delete"));
    }
    
    [TestMethod]
    public void TestEncryptedStorage()
    {
        // Simulate client-side encryption
        string originalData = "Sensitive information";
        byte[] encryptedData = SimulateEncryption(originalData);
        byte[] keyHash = CryptoLib.Sha256("encryption_key".ToByteArray());
        
        Engine.SetCallingScriptHash(User1);
        
        // Store encrypted data
        Assert.IsTrue(Contract.StoreEncryptedData(User1, "sensitive", encryptedData, keyHash));
        
        // Retrieve with correct key hash
        var retrieved = Contract.GetEncryptedData(User1, "sensitive", keyHash);
        Assert.IsNotNull(retrieved);
        
        // Attempt retrieval with wrong key hash should fail
        byte[] wrongKeyHash = CryptoLib.Sha256("wrong_key".ToByteArray());
        Assert.ThrowsException<Exception>(() =>
            Contract.GetEncryptedData(User1, "sensitive", wrongKeyHash));
    }
    
    private byte[] SimulateEncryption(string data)
    {
        // Simple XOR encryption for testing
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        byte key = 0x42;
        
        for (int i = 0; i < dataBytes.Length; i++)
        {
            dataBytes[i] ^= key;
        }
        
        return dataBytes;
    }
}
```

Storage security is fundamental to protecting user data and maintaining contract integrity. Always implement proper access controls, data isolation, and audit trails to ensure a secure and trustworthy smart contract system.
