using Sitko.Core.App.Blazor.Components;

namespace BioEngine.Admin.Shared
{
    public abstract class BasePage : BaseComponent
    {
        protected abstract string Title { get; }
        protected string PageTitle => $"{Title} / BRC Admin";
    }
}
