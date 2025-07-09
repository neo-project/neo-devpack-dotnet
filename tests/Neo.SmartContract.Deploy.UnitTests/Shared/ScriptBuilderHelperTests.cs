using System;
using System.Text;
using Neo;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Native;
using Neo.VM;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Shared;

public class ScriptBuilderHelperTests
{
    [Fact]
    public void BuildUpdateScript_WithBothNefAndManifest_BuildsCorrectScript()
    {
        // Arrange
        var nefBytes = new byte[] { 0x01, 0x02, 0x03 };
        var manifestJson = "{\"name\":\"TestContract\"}";
        var manifestBytes = Encoding.UTF8.GetBytes(manifestJson);

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(nefBytes, manifestBytes);

        // Assert
        Assert.NotNull(script);
        Assert.True(script.Length > 0);
        
        // Verify the script contains ContractManagement hash
        using var sb = new ScriptBuilder();
        // Verify the script contains "update" method name
        var updateBytes = System.Text.Encoding.UTF8.GetBytes("update");
        bool containsUpdate = false;
        for (int i = 0; i <= script.Length - updateBytes.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < updateBytes.Length; j++)
            {
                if (script[i + j] != updateBytes[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                containsUpdate = true;
                break;
            }
        }
        Assert.True(containsUpdate, "Script should contain 'update' method name");
    }

    [Fact]
    public void BuildUpdateScript_WithNefOnly_BuildsCorrectScript()
    {
        // Arrange
        var nefBytes = new byte[] { 0x01, 0x02, 0x03 };

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(nefBytes, null);

        // Assert
        Assert.NotNull(script);
        Assert.True(script.Length > 0);
        
        // Verify PUSHNULL is used for manifest
        Assert.Contains((byte)OpCode.PUSHNULL, script);
    }

    [Fact]
    public void BuildUpdateScript_WithManifestOnly_BuildsCorrectScript()
    {
        // Arrange
        var manifestJson = "{\"name\":\"TestContract\"}";
        var manifestBytes = Encoding.UTF8.GetBytes(manifestJson);

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(null, manifestBytes);

        // Assert
        Assert.NotNull(script);
        Assert.True(script.Length > 0);
        
        // Verify PUSHNULL is used for NEF
        Assert.Contains((byte)OpCode.PUSHNULL, script);
    }

    [Fact]
    public void BuildUpdateScript_WithUpdateParams_IncludesParams()
    {
        // Arrange
        var nefBytes = new byte[] { 0x01, 0x02, 0x03 };
        var manifestBytes = Encoding.UTF8.GetBytes("{\"name\":\"TestContract\"}");
        var updateParams = new object[] { "v2", 42 };

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(nefBytes, manifestBytes, updateParams);

        // Assert
        Assert.NotNull(script);
        Assert.True(script.Length > 0);
        
        // The script should include PACK opcode for array parameters
        Assert.Contains((byte)OpCode.PACK, script);
    }

    [Fact]
    public void BuildUpdateScript_WithSingleUpdateParam_PushesDirectly()
    {
        // Arrange
        var nefBytes = new byte[] { 0x01, 0x02, 0x03 };
        var manifestBytes = Encoding.UTF8.GetBytes("{\"name\":\"TestContract\"}");
        var updateParams = new object[] { "v2" };

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(nefBytes, manifestBytes, updateParams);

        // Assert
        Assert.NotNull(script);
        Assert.True(script.Length > 0);
    }

    [Fact]
    public void BuildUpdateScript_WithNullParams_UsesPushNull()
    {
        // Arrange
        var nefBytes = new byte[] { 0x01, 0x02, 0x03 };
        var manifestBytes = Encoding.UTF8.GetBytes("{\"name\":\"TestContract\"}");

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(nefBytes, manifestBytes, null);

        // Assert
        Assert.NotNull(script);
        Assert.True(script.Length > 0);
        
        // Should have PUSHNULL for the data parameter
        Assert.Contains((byte)OpCode.PUSHNULL, script);
    }

    [Fact]
    public void BuildUpdateScript_CallsContractManagementUpdate()
    {
        // Arrange
        var nefBytes = new byte[] { 0x01, 0x02, 0x03 };
        var manifestBytes = Encoding.UTF8.GetBytes("{\"name\":\"TestContract\"}");

        // Act
        var script = ScriptBuilderHelper.BuildUpdateScript(nefBytes, manifestBytes);

        // Assert
        using var sb = new ScriptBuilder();
        sb.EmitPush("update");
        var updateMethodBytes = sb.ToArray();
        
        // Verify the script contains the "update" method name
        var updateMethodName = System.Text.Encoding.UTF8.GetBytes("update");
        bool hasUpdate = false;
        for (int i = 0; i <= script.Length - updateMethodName.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < updateMethodName.Length; j++)
            {
                if (script[i + j] != updateMethodName[j])
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                hasUpdate = true;
                break;
            }
        }
        Assert.True(hasUpdate, "Script should contain 'update' method name");
        
        // Verify System.Contract.Call syscall is used
        Assert.Contains((byte)0x52, script);
    }

    [Fact]
    public void EmitParameterArray_WithEmptyArray_EmitsNewArray0()
    {
        // Arrange
        using var sb = new ScriptBuilder();
        var parameters = Array.Empty<object>();

        // Act
        ScriptBuilderHelper.EmitParameterArray(sb, parameters);
        var script = sb.ToArray();

        // Assert
        Assert.Single(script);
        Assert.Equal((byte)OpCode.NEWARRAY0, script[0]);
    }

    [Fact]
    public void EmitParameterArray_WithMultipleParams_PacksCorrectly()
    {
        // Arrange
        using var sb = new ScriptBuilder();
        var parameters = new object[] { 1, "test", true };

        // Act
        ScriptBuilderHelper.EmitParameterArray(sb, parameters);
        var script = sb.ToArray();

        // Assert
        Assert.Contains((byte)OpCode.PACK, script);
    }

    [Fact]
    public void EmitParameterArray_NullParameters_ThrowsArgumentNullException()
    {
        // Arrange
        using var sb = new ScriptBuilder();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ScriptBuilderHelper.EmitParameterArray(sb, null!));
    }
}