namespace BioEngine.Admin.Helpers
{
    public static class FontAwesomeHelper
    {
        public static string IconClass(string icon, string style = "fas")
        {
            return $"fa {style} fa-{icon}";
        }
    }
}
