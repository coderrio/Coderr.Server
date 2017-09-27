namespace codeRR.Server.App.Tests
{
    internal static class ObjectExtensions
    {
        public static void SetId(this object instance, object id)
        {
            instance.GetType().GetProperty("Id").SetValue(instance, id);
        }

        public static void SetProperty(this object instance, string name, object value)
        {
            instance.GetType().GetProperty(name).SetValue(instance, value);
        }
    }
}