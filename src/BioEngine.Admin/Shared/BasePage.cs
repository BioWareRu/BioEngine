using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Shared
{
    public class BasePage : OwningComponentBase
    {
        protected bool IsLoading { get; private set; }
        protected bool IsInitialized { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            IsInitialized = true;
            StateHasChanged();
        }

        protected void StartLoading()
        {
            IsLoading = true;
            StateHasChanged();
        }

        protected void StopLoading()
        {
            IsLoading = false;
            StateHasChanged();
        }
    }
}
