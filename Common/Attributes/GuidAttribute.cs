namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class GuidAttribute : Attribute
{
    public string EntityGuid { get; }

    public GuidAttribute(string guid)
    {
        EntityGuid = guid;
    }
}