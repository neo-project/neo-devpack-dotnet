using Neo.VM;
using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractAuthor : Attribute
    {
        public ContractAuthor(string author)
        {
            Author = author;
        }

        public string Author { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractDescription : Attribute
    {
        public ContractDescription(string description)
        {
            Description = description;
        }

        public string Description { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractEmail : Attribute
    {
        public ContractEmail(string email)
        {
            Email = email;
        }

        public string Email { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractHasDynamicInvoke : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractHasStorage : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractIsPayable : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractTitle : Attribute
    {
        public ContractTitle(string title)
        {
            Title = title;
        }

        public string Title { get; }
    }

    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class ContractVersion : Attribute
    {
        public ContractVersion(string version)
        {
            Version = version;
        }

        public string Version { get; }
    }
}
