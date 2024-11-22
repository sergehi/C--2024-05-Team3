namespace Common.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DescriptionAttribute : Attribute
{
    public string Text { get; }

    public DescriptionAttribute(string text)
    {
        Text = text;
    }
}

