using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM.Types;
using Xunit;

namespace GovernanceContract.Tests;

public class GovernanceContractTests : SmartContractTestBase<DeploymentExample.Contract.GovernanceContract>
{
    [Fact]
    public void TestInitialize()
    {
        var admin = Engine.GetAccount("admin");
        var tokenContract = UInt160.Parse("0x1234567890123456789012345678901234567890");
        
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        
        // Verify initialization
        Assert.True(Contract.IsCouncilMember(admin.ScriptHash));
    }

    [Fact]
    public void TestCreateProposal()
    {
        var admin = Engine.GetAccount("admin");
        var alice = Engine.GetAccount("alice");
        var tokenContract = UInt160.Zero;
        
        // Initialize
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        
        // Add alice as council member
        Contract.AddCouncilMember(alice.ScriptHash);
        
        // Create proposal
        Engine.SetTransaction(alice);
        var proposalData = new BigInteger(1000_00000000); // Mint amount
        var proposalId = Contract.CreateProposal(1, proposalData, "Mint 1000 tokens");
        
        Assert.Equal(BigInteger.One, proposalId);
    }

    [Fact]
    public void TestVoting()
    {
        var admin = Engine.GetAccount("admin");
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        var tokenContract = UInt160.Zero;
        
        // Initialize and setup
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        Contract.AddCouncilMember(alice.ScriptHash);
        Contract.AddCouncilMember(bob.ScriptHash);
        
        // Create proposal
        Engine.SetTransaction(alice);
        var proposalId = Contract.CreateProposal(1, BigInteger.One, "Test proposal");
        
        // Vote on proposal
        Assert.True(Contract.Vote(proposalId, true)); // Alice votes yes
        
        Engine.SetTransaction(bob);
        Assert.True(Contract.Vote(proposalId, true)); // Bob votes yes
        
        Engine.SetTransaction(admin);
        Assert.True(Contract.Vote(proposalId, false)); // Admin votes no
        
        // Check vote counts (2 yes, 1 no out of 3 members)
        var proposal = Contract.GetProposal(proposalId) as Map;
        Assert.NotNull(proposal);
        Assert.Equal(new BigInteger(2), proposal["yesVotes"].GetInteger());
        Assert.Equal(new BigInteger(1), proposal["noVotes"].GetInteger());
    }

    [Fact]
    public void TestExecuteProposal()
    {
        var admin = Engine.GetAccount("admin");
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        var tokenContract = UInt160.Zero;
        
        // Initialize
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        Contract.AddCouncilMember(alice.ScriptHash);
        Contract.AddCouncilMember(bob.ScriptHash);
        
        // Create and vote on proposal
        Engine.SetTransaction(alice);
        var proposalId = Contract.CreateProposal(3, alice.ScriptHash, "Add new council member");
        Contract.Vote(proposalId, true);
        
        Engine.SetTransaction(bob);
        Contract.Vote(proposalId, true);
        
        // Fast forward time (simulate voting period end)
        Engine.SetTransactionAttribute(new OracleResponseAttribute { Timestamp = 1000000 });
        
        // Execute proposal
        Engine.SetTransaction(admin);
        var result = Contract.ExecuteProposal(proposalId);
        Assert.True(result);
        
        // Verify proposal was executed
        var proposal = Contract.GetProposal(proposalId) as Map;
        Assert.True((bool)proposal["executed"]);
    }

    [Fact]
    public void TestRemoveCouncilMember()
    {
        var admin = Engine.GetAccount("admin");
        var alice = Engine.GetAccount("alice");
        var tokenContract = UInt160.Zero;
        
        // Initialize and add member
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        Contract.AddCouncilMember(alice.ScriptHash);
        
        Assert.True(Contract.IsCouncilMember(alice.ScriptHash));
        
        // Remove member
        Contract.RemoveCouncilMember(alice.ScriptHash);
        
        Assert.False(Contract.IsCouncilMember(alice.ScriptHash));
    }

    [Fact]
    public void TestNonCouncilMemberCannotCreateProposal()
    {
        var admin = Engine.GetAccount("admin");
        var alice = Engine.GetAccount("alice");
        var tokenContract = UInt160.Zero;
        
        // Initialize (alice is not a council member)
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        
        // Try to create proposal as non-member
        Engine.SetTransaction(alice);
        Assert.Throws<TestException>(() => 
            Contract.CreateProposal(1, BigInteger.One, "Invalid proposal"));
    }

    [Fact]
    public void TestCannotVoteTwice()
    {
        var admin = Engine.GetAccount("admin");
        var alice = Engine.GetAccount("alice");
        var tokenContract = UInt160.Zero;
        
        // Initialize and setup
        Engine.SetTransaction(admin);
        Contract.Initialize(admin.ScriptHash, tokenContract);
        Contract.AddCouncilMember(alice.ScriptHash);
        
        // Create proposal and vote
        Engine.SetTransaction(alice);
        var proposalId = Contract.CreateProposal(1, BigInteger.One, "Test");
        Assert.True(Contract.Vote(proposalId, true));
        
        // Try to vote again
        Assert.Throws<TestException>(() => Contract.Vote(proposalId, false));
    }
}