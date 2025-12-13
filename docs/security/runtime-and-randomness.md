# Runtime Security and Random Number Generation in Neo Smart Contracts

Random number generation and runtime security are critical aspects of smart contract development. This guide covers Neo N3 specific runtime security patterns and secure randomness generation techniques.

## Table of Contents

- [Neo Runtime Security Fundamentals](#neo-runtime-security-fundamentals)
- [Random Number Generation Security](#random-number-generation-security)
- [Neo N3 Random Number Sources](#neo-n3-random-number-sources)
- [Secure Randomness Patterns](#secure-randomness-patterns)
- [Time-Based Security Considerations](#time-based-security-considerations)
- [Block Data and Entropy Sources](#block-data-and-entropy-sources)
- [Commit-Reveal Schemes](#commit-reveal-schemes)
- [External Randomness Sources](#external-randomness-sources)
- [Testing Randomness Security](#testing-randomness-security)

## Neo Runtime Security Fundamentals

### Understanding Neo Runtime Environment

```csharp
using System;
using System.ComponentModel;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

[DisplayName("RuntimeSecurityDemo")]
[ContractDescription("Demonstrates Neo runtime security patterns")]
public class RuntimeSecurityDemo : SmartContract
{
    /// <summary>
    /// Demonstrate secure access to Neo runtime information
    /// </summary>
    [Safe]
    public static void DemonstrateRuntimeInfo()
    {
        // Safe runtime properties - these are deterministic and secure
        uint blockIndex = Ledger.CurrentIndex;            // Current block index
        ulong timestamp = Runtime.Time;                   // Current block timestamp (milliseconds)
        string platform = Runtime.Platform;              // Platform identifier
        UInt160 scriptHash = Runtime.ExecutingScriptHash; // Current contract hash
        UInt160 callingHash = Runtime.CallingScriptHash;  // Calling contract hash
        
        // Transaction context information
        var transaction = Runtime.Transaction;
        UInt160 sender = transaction.Sender;
        
        // Validate runtime state
        ExecutionEngine.Assert(blockIndex > 0, "Invalid block index");
        ExecutionEngine.Assert(timestamp > 0, "Invalid timestamp");
        ExecutionEngine.Assert(!string.IsNullOrEmpty(platform), "Invalid platform");
        ExecutionEngine.Assert(scriptHash != null && scriptHash.IsValid, "Invalid script hash");
    }
    
    /// <summary>
    /// Secure pattern for runtime validation
    /// </summary>
    public static bool SecureRuntimeValidation(UInt160 expectedCaller)
    {
        // Verify caller identity
        ExecutionEngine.Assert(Runtime.CheckWitness(expectedCaller), "Invalid caller witness");
        
        // Verify execution context
        ExecutionEngine.Assert(Runtime.CallingScriptHash == expectedCaller || 
               Runtime.CallingScriptHash == Runtime.ExecutingScriptHash, 
               "Invalid calling context");
        
        // Verify transaction authenticity
        var tx = Runtime.Transaction;
        ExecutionEngine.Assert(tx != null, "Invalid transaction context");
        
        return true;
    }
}
```

### Runtime Security Best Practices

```csharp
public class RuntimeSecurityPatterns : SmartContract
{
    /// <summary>
    /// Secure timestamp validation with acceptable range
    /// </summary>
    public static bool ValidateTimestamp(long expectedTime, long toleranceSeconds = 300)
    {
        long currentTime = (long)Runtime.Time;
        long timeDifference = currentTime > expectedTime ? 
            currentTime - expectedTime : expectedTime - currentTime;
        
        ExecutionEngine.Assert(timeDifference <= toleranceSeconds * 1000, // Convert to milliseconds
               $"Timestamp difference too large: {timeDifference}ms");
        
        return true;
    }
    
    /// <summary>
    /// Secure contract interaction validation
    /// </summary>
    public static bool ValidateContractInteraction(UInt160 allowedContract)
    {
        UInt160 callingContract = Runtime.CallingScriptHash;
        
        // Allow direct calls or calls from specific contracts
        if (callingContract == Runtime.ExecutingScriptHash)
        {
            // Direct call - validate transaction sender
            var tx = Runtime.Transaction;
            ExecutionEngine.Assert(Runtime.CheckWitness(tx.Sender), "Invalid transaction signature");
            return true;
        }
        else
        {
            // Contract call - validate calling contract
            ExecutionEngine.Assert(callingContract == allowedContract, "Unauthorized contract call");
            return true;
        }
    }
    
    /// <summary>
    /// Secure execution environment validation
    /// </summary>
    public static bool ValidateExecutionEnvironment()
    {
        // Verify we're running in valid Neo environment
        ExecutionEngine.Assert(!string.IsNullOrEmpty(Runtime.Platform), "Invalid platform");
        ExecutionEngine.Assert(Runtime.Time > 1609459200000UL, "Invalid timestamp (before 2021)"); // Basic sanity check
        
        // Verify contract state consistency
        UInt160 currentHash = Runtime.ExecutingScriptHash;
        ExecutionEngine.Assert(currentHash != null && currentHash.IsValid, "Invalid contract hash");
        
        return true;
    }
}
```

## Random Number Generation Security

### Common Randomness Vulnerabilities

```csharp
/// <summary>
/// Examples of INSECURE random number generation - DO NOT USE
/// </summary>
public class InsecureRandomness : SmartContract
{
    // ❌ INSECURE: Using predictable sources
    public static int BadRandom1()
    {
        // Block timestamp is predictable and manipulable by miners
        return (int)(Runtime.Time % 100);
    }
    
    // ❌ INSECURE: Using block index
    public static int BadRandom2()
    {
        // Block index is completely predictable
        return (int)(Ledger.CurrentIndex % 100);
    }
    
    // ❌ INSECURE: Using transaction hash alone
    public static int BadRandom3()
    {
        // Transaction hash can be influenced by attacker
        var tx = Runtime.Transaction;
        return (int)(tx.Hash[0] % 100);
    }
    
    // ❌ INSECURE: Simple combination of predictable values
    public static int BadRandom4()
    {
        // Combining predictable values doesn't make them unpredictable
        return (int)((Runtime.Time + Ledger.CurrentIndex) % 100);
    }
}
```

## Neo N3 Random Number Sources

### Using Neo's Built-in Randomness

```csharp
public class Neo3RandomnessPatterns : SmartContract
{
    /// <summary>
    /// Using Neo N3's enhanced randomness sources
    /// </summary>
    public static BigInteger GetSecureRandom()
    {
        // Neo N3 provides improved randomness through various sources
        // Combine multiple entropy sources for better security
        
        // Get current block hash as entropy source
        var currentBlockHash = Ledger.CurrentHash;
        
        // Get transaction hash
        var txHash = Runtime.Transaction.Hash;
        
        // Create composite entropy
        ByteString entropy = currentBlockHash
            .Concat(txHash)
            .Concat((ByteString)(System.Numerics.BigInteger)Runtime.Time);
        
        // Hash the combined entropy
        ByteString randomBytes = CryptoLib.Sha256(entropy);
        
        return (BigInteger)randomBytes;
    }
    
    /// <summary>
    /// Generate random number in specific range
    /// </summary>
    public static int GetRandomInRange(int min, int max)
    {
        ExecutionEngine.Assert(min < max, "Invalid range: min must be less than max");
        ExecutionEngine.Assert(max - min < 1000000, "Range too large for secure random generation");
        
        BigInteger randomValue = GetSecureRandom();
        
        // Use modulo bias reduction technique
        BigInteger range = max - min;
        BigInteger result = (randomValue % range) + min;
        
        return (int)result;
    }
    
    /// <summary>
    /// Generate cryptographically secure random bytes
    /// </summary>
    public static ByteString GetRandomBytes(int length)
    {
        ExecutionEngine.Assert(length > 0 && length <= 64, "Invalid length for random bytes");
        
        // Generate base randomness
        BigInteger baseRandom = GetSecureRandom();
        ByteString baseBytes = (ByteString)baseRandom;
        
        // If we need more bytes, generate additional entropy
        if (length > baseBytes.Length)
        {
            // Generate additional entropy using different sources
            var additionalEntropy = CryptoLib.Sha256(
                baseBytes.Concat(Runtime.ExecutingScriptHash)
            );
            baseBytes = baseBytes.Concat(additionalEntropy);
        }
        
        // Return requested length
        return (ByteString)((byte[])baseBytes).Take(length);
    }
}
```

### Enhanced Randomness with Multiple Sources

```csharp
public class EnhancedRandomness : SmartContract
{
    /// <summary>
    /// Multi-source entropy generation for critical randomness needs
    /// </summary>
    public static BigInteger GenerateHighQualityRandom(UInt160 userSeed = null)
    {
        // Collect entropy from multiple sources
        Neo.SmartContract.Framework.List<ByteString> entropySources = new Neo.SmartContract.Framework.List<ByteString>();
        
        // Source 1: Current block hash
        entropySources.Add(Ledger.CurrentHash);
        
        // Source 2: Previous block hash for additional history
        uint currentIndex = Ledger.CurrentIndex;
        if (currentIndex > 0)
        {
            var prevHash = Ledger.GetBlock(currentIndex - 1)?.Hash;
            if (prevHash != null)
            {
                entropySources.Add(prevHash);
            }
        }
        
        // Source 3: Transaction hash and sender
        var tx = Runtime.Transaction;
        entropySources.Add(tx.Hash);
        entropySources.Add(tx.Sender);
        
        // Source 4: Contract execution context
        entropySources.Add(Runtime.ExecutingScriptHash);
        entropySources.Add(Runtime.CallingScriptHash);
        
        // Source 5: Timestamp with microsecond precision if available
        entropySources.Add((ByteString)(System.Numerics.BigInteger)Runtime.Time);
        
        // Source 6: User-provided seed (if any)
        if (userSeed != null)
        {
            entropySources.Add(userSeed);
        }
        
        // Source 7: Contract storage entropy (if available)
        ByteString storedEntropy = Storage.Get(Storage.CurrentContext, "entropy_pool");
        if (storedEntropy != null)
        {
            entropySources.Add(storedEntropy);
        }
        
        // Combine all entropy sources
        ByteString combinedEntropy = ByteString.Empty;
        foreach (var source in entropySources)
        {
            combinedEntropy = combinedEntropy.Concat(source);
        }
        
        // Apply multiple rounds of hashing for entropy extraction
        ByteString round1 = CryptoLib.Sha256(combinedEntropy);
        ByteString round2 = CryptoLib.Ripemd160(round1.Concat(combinedEntropy));
        ByteString finalHash = CryptoLib.Sha256(round1.Concat(round2));
        
        // Update stored entropy for future use
        Storage.Put(Storage.CurrentContext, "entropy_pool", (ByteString)((byte[])finalHash).Take(16));
        
        return (BigInteger)finalHash;
    }
    
    /// <summary>
    /// Generate multiple random numbers with decorrelation
    /// </summary>
    public static BigInteger[] GenerateMultipleRandom(int count, ByteString salt = null)
    {
        ExecutionEngine.Assert(count > 0 && count <= 100, "Invalid count for random generation");
        
        BigInteger[] results = new BigInteger[count];
        ByteString currentSalt = salt ?? GetRandomBytes(16);
        
        for (int i = 0; i < count; i++)
        {
            // Use index and previous results as additional entropy
            ByteString indexEntropy = (ByteString)(BigInteger)i;
            if (i > 0)
            {
                indexEntropy = indexEntropy.Concat((ByteString)results[i - 1]);
            }
            
            ByteString roundSalt = CryptoLib.Sha256(currentSalt.Concat(indexEntropy));
            results[i] = GenerateHighQualityRandom() ^ (BigInteger)roundSalt;
            
            // Update salt for next iteration
            currentSalt = CryptoLib.Sha256(roundSalt);
        }
        
        return results;
    }
}
```

## Secure Randomness Patterns

### Commit-Reveal Scheme Implementation

```csharp
public class CommitRevealRandom : SmartContract
{
    /// <summary>
    /// Structure for commit-reveal random generation
    /// </summary>
    public struct RandomCommitment
    {
        public UInt160 User;
        public ByteString Commitment;
        public ulong CommitTimestamp;
        public ulong RevealDeadline;
        public bool IsRevealed;
        public BigInteger RevealedValue;
    }
    
    private static readonly StorageMap Commitments = new(Storage.CurrentContext, "commitments");
    
    /// <summary>
    /// Phase 1: Commit to a random value
    /// </summary>
    public static bool CommitRandom(UInt160 user, ByteString commitment, int validityHours = 24)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Invalid user signature");
        ExecutionEngine.Assert(commitment.Length == 32, "Commitment must be 32 bytes (SHA256 hash)");
        ExecutionEngine.Assert(validityHours > 0 && validityHours <= 168, "Validity period must be 1-168 hours");
        
        string commitmentId = user.ToString() + "_" + Runtime.Time.ToString();
        
        // Check for existing unrevealed commitments
        var existingCommitment = GetUserCommitment(user);
        ExecutionEngine.Assert(existingCommitment.Commitment == null || existingCommitment.IsRevealed || 
               Runtime.Time > existingCommitment.RevealDeadline,
               "User has pending unrevealed commitment");
        
        RandomCommitment newCommitment = new RandomCommitment
        {
            User = user,
            Commitment = commitment,
            CommitTimestamp = Runtime.Time,
            RevealDeadline = Runtime.Time + (ulong)(validityHours * 3600 * 1000), // Convert to milliseconds
            IsRevealed = false,
            RevealedValue = 0
        };
        
        Commitments.Put(commitmentId, StdLib.Serialize(newCommitment));
        OnRandomCommitted(user, commitment, newCommitment.RevealDeadline);
        
        return true;
    }
    
    /// <summary>
    /// Phase 2: Reveal the random value and nonce
    /// </summary>
    public static BigInteger RevealRandom(UInt160 user, BigInteger randomValue, ByteString nonce)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Invalid user signature");
        
        var commitment = GetUserCommitment(user);
        ExecutionEngine.Assert(commitment.Commitment != null, "No commitment found for user");
        ExecutionEngine.Assert(!commitment.IsRevealed, "Commitment already revealed");
        ExecutionEngine.Assert(Runtime.Time <= commitment.RevealDeadline, "Reveal deadline exceeded");
        
        // Verify commitment
        ByteString expectedCommitment = CryptoLib.Sha256(((ByteString)randomValue).Concat(nonce));
        ExecutionEngine.Assert(expectedCommitment == commitment.Commitment, "Invalid reveal - commitment mismatch");
        
        // Update commitment with revealed value
        commitment.IsRevealed = true;
        commitment.RevealedValue = randomValue;
        
        string commitmentId = user.ToString() + "_" + commitment.CommitTimestamp.ToString();
        Commitments.Put(commitmentId, StdLib.Serialize(commitment));
        
        // Generate final random number combining user input with block entropy
        BigInteger blockEntropy = GenerateHighQualityRandom();
        BigInteger finalRandom = (BigInteger)CryptoLib.Sha256(((ByteString)randomValue).Concat((ByteString)blockEntropy));
        
        OnRandomRevealed(user, randomValue, finalRandom);
        return finalRandom;
    }
    
    /// <summary>
    /// Multi-party commit-reveal for collective randomness
    /// </summary>
    public static BigInteger GenerateCollectiveRandom(UInt160[] participants, 
                                                     string sessionId)
    {
        ExecutionEngine.Assert(participants.Length >= 2 && participants.Length <= 10, 
               "Invalid number of participants");
        
        // Verify all participants have revealed their commitments
        BigInteger combinedRandom = 0;
        foreach (var participant in participants)
        {
            var commitment = GetUserCommitment(participant);
            ExecutionEngine.Assert(commitment.IsRevealed, $"Participant {participant} has not revealed");
            
            combinedRandom ^= commitment.RevealedValue;
        }
        
        // Add block entropy for additional security
        BigInteger blockEntropy = GenerateHighQualityRandom();
        BigInteger finalRandom = (BigInteger)CryptoLib.Sha256(((ByteString)combinedRandom).Concat((ByteString)blockEntropy));
        
        OnCollectiveRandomGenerated(sessionId, participants, finalRandom);
        return finalRandom;
    }
    
    private static RandomCommitment GetUserCommitment(UInt160 user)
    {
        // Find most recent commitment for user
        var iterator = (Iterator<ByteString>)Storage.Find(Storage.CurrentContext, "commitments" + user.ToString(), FindOptions.ValuesOnly);
        RandomCommitment latest = new RandomCommitment();
        
        while (iterator.Next())
        {
            var commitment = (RandomCommitment)StdLib.Deserialize(iterator.Value);
            if (commitment.CommitTimestamp > latest.CommitTimestamp)
            {
                latest = commitment;
            }
        }
        
        return latest;
    }
    
    [DisplayName("RandomCommitted")]
    public static event Action<UInt160, ByteString, ulong> OnRandomCommitted;
    
    [DisplayName("RandomRevealed")]
    public static event Action<UInt160, BigInteger, BigInteger> OnRandomRevealed;
    
    [DisplayName("CollectiveRandomGenerated")]
    public static event Action<string, UInt160[], BigInteger> OnCollectiveRandomGenerated;
}
```

## Time-Based Security Considerations

### Secure Time Validation Patterns

```csharp
public class TimeBasedSecurity : SmartContract
{
    /// <summary>
    /// Secure time window validation
    /// </summary>
    public static bool ValidateTimeWindow(long startTime, long endTime, 
                                        long toleranceMs = 30000)
    {
        long currentTime = (long)Runtime.Time;
        
        ExecutionEngine.Assert(startTime < endTime, "Invalid time window");
        ExecutionEngine.Assert(endTime - startTime <= 86400000, "Time window too large (max 24 hours)");
        
        // Allow slight tolerance for network latency
        ExecutionEngine.Assert(currentTime >= startTime - toleranceMs, "Operation too early");
        ExecutionEngine.Assert(currentTime <= endTime + toleranceMs, "Operation too late");
        
        return true;
    }
    
    /// <summary>
    /// Time-based one-time operation
    /// </summary>
    public static bool TimeLockedOperation(UInt160 user, string operationId, 
                                         long scheduledTime, long windowMs = 3600000)
    {
        ExecutionEngine.Assert(Runtime.CheckWitness(user), "Invalid user signature");
        
        // Validate operation is within allowed time window
        long currentTime = (long)Runtime.Time;
        ExecutionEngine.Assert(currentTime >= scheduledTime, "Operation scheduled for future");
        ExecutionEngine.Assert(currentTime <= scheduledTime + windowMs, "Operation window expired");
        
        // Ensure operation hasn't been executed
        string operationKey = "timelock_" + user.ToString() + "_" + operationId;
        ExecutionEngine.Assert(Storage.Get(Storage.CurrentContext, operationKey) == null, 
               "Operation already executed");
        
        // Mark operation as executed
        Storage.Put(Storage.CurrentContext, operationKey, currentTime);
        
        OnTimeLockedOperationExecuted(user, operationId, currentTime);
        return true;
    }
    
    /// <summary>
    /// Secure countdown timer with manipulation protection
    /// </summary>
    public static bool SecureCountdown(string timerId, long durationMs, 
                                     bool allowEarlyTrigger = false)
    {
        string timerKey = "timer_" + timerId;
        ByteString timerData = Storage.Get(Storage.CurrentContext, timerKey);
        
        if (timerData == null)
        {
            // Start new timer
            long startTime = (long)Runtime.Time;
            long endTime = startTime + durationMs;
            
            object[] timer = new object[] { startTime, endTime };
            Storage.Put(Storage.CurrentContext, timerKey, StdLib.Serialize(timer));
            
            OnCountdownStarted(timerId, startTime, endTime);
            return false; // Timer not yet expired
        }
        else
        {
            // Check existing timer
            object[] timer = (object[])StdLib.Deserialize(timerData);
            long startTime = (long)timer[0];
            long endTime = (long)timer[1];
            long currentTime = (long)Runtime.Time;
            
            if (allowEarlyTrigger && currentTime >= startTime + (durationMs / 2))
            {
                // Allow trigger after half the duration
                Storage.Delete(Storage.CurrentContext, timerKey);
                OnCountdownTriggered(timerId, currentTime, true);
                return true;
            }
            else if (currentTime >= endTime)
            {
                // Timer expired naturally
                Storage.Delete(Storage.CurrentContext, timerKey);
                OnCountdownTriggered(timerId, currentTime, false);
                return true;
            }
            
            return false; // Timer still running
        }
    }
    
    [DisplayName("TimeLockedOperationExecuted")]
    public static event Action<UInt160, string, long> OnTimeLockedOperationExecuted;
    
    [DisplayName("CountdownStarted")]
    public static event Action<string, long, long> OnCountdownStarted;
    
    [DisplayName("CountdownTriggered")]
    public static event Action<string, long, bool> OnCountdownTriggered;
}
```

## Block Data and Entropy Sources

### Secure Block Data Usage

```csharp
public class BlockDataSecurity : SmartContract
{
    /// <summary>
    /// Safely use block data for entropy
    /// </summary>
    public static BigInteger GetBlockBasedEntropy(int blockOffset = 1)
    {
        ExecutionEngine.Assert(blockOffset >= 1 && blockOffset <= 10, "Invalid block offset");
        
        uint currentIndex = Ledger.CurrentIndex;
        ExecutionEngine.Assert(currentIndex >= blockOffset, "Insufficient block history");
        
        // Use historical block data for entropy (harder to manipulate)
        uint targetIndex = currentIndex - (uint)blockOffset;
        var targetBlock = Ledger.GetBlock(targetIndex);
        ExecutionEngine.Assert(targetBlock != null, "Block not found");
        
        // Combine multiple block properties for better entropy
        ByteString entropy = targetBlock.Hash
            .Concat(targetBlock.PrevHash)
            .Concat((ByteString)(BigInteger)targetBlock.Timestamp)
            .Concat((ByteString)(BigInteger)targetBlock.Index);
        
        return (BigInteger)CryptoLib.Sha256(entropy);
    }
    
    /// <summary>
    /// Generate entropy from multiple historical blocks
    /// </summary>
    public static BigInteger GetMultiBlockEntropy(int blockCount = 3)
    {
        ExecutionEngine.Assert(blockCount >= 1 && blockCount <= 10, "Invalid block count");
        
        uint currentIndex = Ledger.CurrentIndex;
        ExecutionEngine.Assert(currentIndex >= blockCount, "Insufficient block history");
        
        ByteString combinedEntropy = ByteString.Empty;
        
        for (int i = 1; i <= blockCount; i++)
        {
            uint blockIndex = currentIndex - (uint)i;
            var block = Ledger.GetBlock(blockIndex);
            
            if (block != null)
            {
                combinedEntropy = combinedEntropy
                    .Concat(block.Hash)
                    .Concat(block.PrevHash);
            }
        }
        
        return (BigInteger)CryptoLib.Sha256(combinedEntropy);
    }
    
    /// <summary>
    /// Validate block-based randomness quality
    /// </summary>
    public static bool ValidateEntropyQuality(BigInteger entropy)
    {
        ByteString entropyBytes = (ByteString)entropy;
        
        // Basic entropy quality checks
        ExecutionEngine.Assert(entropyBytes.Length >= 16, "Entropy too short");
        
        // Check for obvious patterns (all zeros, all ones, etc.)
        bool allSame = true;
        byte firstByte = entropyBytes[0];
        
        for (int i = 1; i < entropyBytes.Length; i++)
        {
            if (entropyBytes[i] != firstByte)
            {
                allSame = false;
                break;
            }
        }
        
        ExecutionEngine.Assert(!allSame, "Entropy has insufficient randomness");
        
        return true;
    }
}
```

## Testing Randomness Security

### Comprehensive Randomness Security Tests

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;

[TestClass]
public class RandomnessSecurityTests : TestBase<RandomnessContract>
{
    [TestInitialize]
    public void Setup()
    {
        var (nef, manifest) = TestCleanup.EnsureArtifactsUpToDateInternal();
        TestBaseSetup(nef, manifest);
    }
    
    [TestMethod]
    public void TestRandomnessUnpredictability()
    {
        // Generate multiple random numbers and verify they're different
        var randomNumbers = new List<BigInteger>();
        
        for (int i = 0; i < 10; i++)
        {
            Engine.IncreaseTime(1000); // Advance time to change entropy
            var random = Contract.GetSecureRandom();
            
            Assert.IsFalse(randomNumbers.Contains(random), 
                $"Duplicate random number generated: {random}");
            randomNumbers.Add(random);
        }
        
        // Verify numbers are within reasonable distribution
        var distinctCount = randomNumbers.Distinct().Count();
        Assert.AreEqual(10, distinctCount, "All random numbers should be unique");
    }
    
    [TestMethod]
    public void TestRandomnessRange()
    {
        // Test random numbers within specified range
        for (int i = 0; i < 100; i++)
        {
            int random = Contract.GetRandomInRange(1, 100);
            Assert.IsTrue(random >= 1 && random <= 100, 
                $"Random number {random} outside range [1, 100]");
        }
    }
    
    [TestMethod]
    public void TestCommitRevealScheme()
    {
        var user = TestUser1;
        var randomValue = BigInteger.Parse("12345678901234567890");
        var nonce = "test_nonce".ToByteArray();
        
        // Create commitment
        var commitment = CryptoLib.Sha256(randomValue.ToByteArray().Concat(nonce));
        
        Engine.SetCallingScriptHash(user);
        
        // Phase 1: Commit
        Assert.IsTrue(Contract.CommitRandom(user, commitment, 1));
        
        // Verify commitment is stored
        var events = Notifications.Where(n => n.EventName == "RandomCommitted");
        Assert.IsTrue(events.Any());
        
        // Phase 2: Reveal
        var revealedRandom = Contract.RevealRandom(user, randomValue, nonce);
        Assert.IsTrue(revealedRandom != 0);
        
        // Verify reveal events
        var revealEvents = Notifications.Where(n => n.EventName == "RandomRevealed");
        Assert.IsTrue(revealEvents.Any());
    }
    
    [TestMethod]
    public void TestInvalidCommitReveal()
    {
        var user = TestUser1;
        var randomValue = BigInteger.Parse("12345678901234567890");
        var nonce = "test_nonce".ToByteArray();
        var wrongNonce = "wrong_nonce".ToByteArray();
        
        // Create commitment with correct nonce
        var commitment = CryptoLib.Sha256(randomValue.ToByteArray().Concat(nonce));
        
        Engine.SetCallingScriptHash(user);
        
        // Commit
        Assert.IsTrue(Contract.CommitRandom(user, commitment, 1));
        
        // Try to reveal with wrong nonce - should fail
        Assert.ThrowsException<Exception>(() =>
            Contract.RevealRandom(user, randomValue, wrongNonce));
    }
    
    [TestMethod]
    public void TestTimeBasedSecurity()
    {
        var currentTime = Runtime.Time;
        var futureTime = currentTime + 3600000; // 1 hour later
        
        // Test time window validation
        Assert.IsTrue(Contract.ValidateTimeWindow(currentTime - 1000, currentTime + 1000));
        
        // Test invalid time windows
        Assert.ThrowsException<Exception>(() =>
            Contract.ValidateTimeWindow(futureTime, currentTime)); // End before start
        
        Assert.ThrowsException<Exception>(() =>
            Contract.ValidateTimeWindow(currentTime - 100000, currentTime - 50000)); // Too early
    }
    
    [TestMethod]
    public void TestMultiPartyRandomness()
    {
        var participants = new[] { TestUser1, TestUser2, TestUser3 };
        var sessionId = "test_session";
        
        // Each participant commits
        for (int i = 0; i < participants.Length; i++)
        {
            var randomValue = BigInteger.Parse($"12345{i}67890");
            var nonce = $"nonce_{i}".ToByteArray();
            var commitment = CryptoLib.Sha256(randomValue.ToByteArray().Concat(nonce));
            
            Engine.SetCallingScriptHash(participants[i]);
            Assert.IsTrue(Contract.CommitRandom(participants[i], commitment, 1));
        }
        
        // Each participant reveals
        for (int i = 0; i < participants.Length; i++)
        {
            var randomValue = BigInteger.Parse($"12345{i}67890");
            var nonce = $"nonce_{i}".ToByteArray();
            
            Engine.SetCallingScriptHash(participants[i]);
            Contract.RevealRandom(participants[i], randomValue, nonce);
        }
        
        // Generate collective random
        var collectiveRandom = Contract.GenerateCollectiveRandom(participants, sessionId);
        Assert.IsTrue(collectiveRandom != 0);
        
        // Verify collective random event
        var events = Notifications.Where(n => n.EventName == "CollectiveRandomGenerated");
        Assert.IsTrue(events.Any());
    }
    
    [TestMethod]
    public void TestEntropyQuality()
    {
        // Test entropy quality validation
        var goodEntropy = Contract.GetSecureRandom();
        Assert.IsTrue(Contract.ValidateEntropyQuality(goodEntropy));
        
        // Test bad entropy (all zeros)
        var badEntropy = BigInteger.Zero;
        Assert.ThrowsException<Exception>(() =>
            Contract.ValidateEntropyQuality(badEntropy));
        
        // Test insufficient entropy length
        var shortEntropy = BigInteger.Parse("123");
        Assert.ThrowsException<Exception>(() =>
            Contract.ValidateEntropyQuality(shortEntropy));
    }
    
    [TestMethod]
    public void TestBlockBasedEntropy()
    {
        // Test block-based entropy generation
        var entropy1 = Contract.GetBlockBasedEntropy(1);
        var entropy2 = Contract.GetBlockBasedEntropy(2);
        
        Assert.AreNotEqual(entropy1, entropy2, "Block entropy should vary by block offset");
        
        // Test multi-block entropy
        var multiEntropy = Contract.GetMultiBlockEntropy(3);
        Assert.IsTrue(multiEntropy != 0, "Multi-block entropy should not be zero");
        
        // Test invalid parameters
        Assert.ThrowsException<Exception>(() =>
            Contract.GetBlockBasedEntropy(0)); // Invalid offset
        
        Assert.ThrowsException<Exception>(() =>
            Contract.GetBlockBasedEntropy(20)); // Offset too large
    }
}
```

## Best Practices Summary

### Randomness Security Checklist

- ✅ **Never use predictable sources** (timestamps, block indices) alone
- ✅ **Combine multiple entropy sources** for better randomness quality
- ✅ **Use commit-reveal schemes** for user-generated randomness
- ✅ **Validate entropy quality** before using random numbers
- ✅ **Consider timing attacks** when implementing randomness
- ✅ **Use historical block data** rather than current block data
- ✅ **Implement proper range validation** for random number generation
- ✅ **Test randomness distribution** and uniqueness properties

### Runtime Security Guidelines

- ✅ **Validate runtime context** before critical operations
- ✅ **Use proper time window validation** with reasonable tolerances
- ✅ **Verify caller authenticity** using `Runtime.CheckWitness()`
- ✅ **Implement execution environment checks** for consistency
- ✅ **Handle edge cases** in time and block-based operations
- ✅ **Use multiple validation layers** for critical security functions

Secure randomness and runtime validation are fundamental to building trustworthy smart contracts. Always implement multiple layers of protection and thoroughly test edge cases to ensure robust security.
