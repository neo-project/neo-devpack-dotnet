# NEO Smart Contract - DAO Governance

This template provides a complete Decentralized Autonomous Organization (DAO) implementation with proposal creation, voting, and execution mechanisms.

## Features

- **Membership Management**: Add/remove members with voting power
- **Proposal System**: Create and manage governance proposals
- **Voting Mechanisms**: Weighted voting based on member power
- **Automatic Execution**: Execute proposals after successful voting
- **Configurable Parameters**: Customizable voting periods and quorum
- **Treasury Management**: Handle DAO funds and assets
- **Event Tracking**: Monitor all governance activities

## Getting Started

### Building the Contract

```bash
dotnet build
```

### Compiling to NEO Bytecode

```bash
dotnet run --project path/to/Neo.Compiler.CSharp -- YourContract.csproj
```

## Contract Methods

### Member Management

- `IsMember(address)` - Check if address is a member
- `GetVotingPower(member)` - Get member's voting power
- `AddMember(member, votingPower)` - Add new member (admin only)
- `RemoveMember(member)` - Remove member (admin only)
- `UpdateVotingPower(member, newPower)` - Update voting power

### Proposal Management

- `CreateProposal(title, description, callData, target)` - Create new proposal
- `GetProposal(proposalId)` - Get proposal details
- `GetProposalCount()` - Get total proposal count

### Voting

- `Vote(proposalId, support)` - Cast vote on proposal
- `HasVoted(proposalId, voter)` - Check if member has voted
- `ExecuteProposal(proposalId)` - Execute successful proposal

### Settings

- `GetSettings()` - Get DAO configuration
- `UpdateSettings(key, value)` - Update DAO parameters (admin only)

### Administrative

- `GetAdmin()` - Get current admin
- `SetAdmin(newAdmin)` - Change admin (admin only)
- `Update(nefFile, manifest)` - Upgrade contract
- `Destroy()` - Destroy contract

## Usage Examples

### Creating a Proposal

```csharp
// Member creates a proposal to transfer funds
var callData = StdLib.Serialize(new object[] { 
    "transfer", 
    recipientAddress, 
    1000_00000000 // 1000 GAS
});

var proposalId = CreateProposal(
    "Treasury Transfer",
    "Transfer 1000 GAS to development team",
    callData,
    GAS.Hash
);
```

### Voting on Proposals

```csharp
// Members vote on the proposal
Vote(proposalId, true);  // Vote yes
Vote(proposalId, false); // Vote no
```

### Executing Proposals

```csharp
// After voting period ends and proposal passes
ExecuteProposal(proposalId);
```

### Managing Members

```csharp
// Admin adds new member with voting power
AddMember(newMemberAddress, 500);

// Update existing member's voting power
UpdateVotingPower(memberAddress, 750);
```

## DAO Configuration

### Default Settings

- **Voting Period**: 7 days
- **Quorum**: 51% of voting power
- **Proposal Threshold**: 100 voting power minimum
- **Execution Delay**: 2 days after voting ends

### Customizable Parameters

```csharp
// Update voting period to 5 days
UpdateSettings("votingPeriod", 5 * 24 * 60 * 60 * 1000);

// Change quorum to 60%
UpdateSettings("quorum", 60);

// Set proposal threshold to 200
UpdateSettings("proposalThreshold", 200);
```

## Governance Process

### 1. Proposal Creation
- Member with sufficient voting power creates proposal
- Proposal includes title, description, and execution data
- Voting period begins immediately

### 2. Voting Phase
- All members can vote once per proposal
- Votes are weighted by member's voting power
- Voting continues for the configured period

### 3. Execution Delay
- After voting ends, execution delay begins
- Allows time for review and potential appeals
- Prevents immediate execution of controversial proposals

### 4. Proposal Execution
- Anyone can trigger execution after delay period
- Proposal must have passed quorum and majority vote
- Execution calls the specified target contract with data

## Security Features

### Access Control
- Admin controls membership and settings
- Members control proposal creation and voting
- Execution requires successful democratic process

### Voting Security
- One vote per member per proposal
- Voting power prevents Sybil attacks
- Time-locked voting periods prevent manipulation

### Execution Safety
- Execution delay provides review period
- Atomic execution prevents partial failures
- Event logging for full transparency

## Events

The contract emits these governance events:

- `ProposalCreated` - New proposal submitted
- `VoteCast` - Member vote recorded
- `ProposalExecuted` - Proposal successfully executed
- `MemberAdded` - New member joined
- `MemberRemoved` - Member removed from DAO

## Best Practices

### Member Management
- Start with trusted founding members
- Gradually decentralize membership
- Regular review of voting power distribution
- Clear membership criteria

### Proposal Guidelines
- Clear and detailed descriptions
- Proper technical specifications
- Consider community impact
- Allow adequate discussion time

### Voting Strategy
- Encourage active participation
- Provide education on proposals
- Consider delegation mechanisms
- Monitor voter turnout

### Security Considerations
- Test all proposal types
- Monitor for governance attacks
- Have emergency procedures
- Regular security audits

## Deployment Checklist

- [ ] Set initial admin address
- [ ] Configure voting parameters
- [ ] Add founding members
- [ ] Test proposal flow
- [ ] Verify execution mechanisms
- [ ] Deploy to testnet first
- [ ] Conduct governance simulation
- [ ] Document procedures

## Use Cases

### Treasury Management
- Fund allocation decisions
- Investment approvals
- Budget planning
- Emergency funding

### Protocol Governance
- Parameter updates
- Feature additions
- Security upgrades
- Partnership decisions

### Community Decisions
- Event planning
- Marketing initiatives
- Community rewards
- Strategic direction

## License

MIT License - see LICENSE file for details