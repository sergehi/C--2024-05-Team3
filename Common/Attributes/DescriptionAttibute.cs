namespace Common.Attributes;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DescriptionAttibute : Attribute
    {
        public string Text { get; }

        public DescriptionAttibute(string text)
        {
            Text = text;
        }
    }

