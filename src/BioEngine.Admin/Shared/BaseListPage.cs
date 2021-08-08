namespace BioEngine.Admin.Shared
{
    public abstract class BaseListPage : BasePage
    {
        protected void ToFormPage() => NavigationManager.NavigateTo(CreatePageUrl);

        protected abstract string CreatePageUrl { get; }
    }
}
