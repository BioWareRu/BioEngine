using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BioEngine.Admin.Shared.Blocks
{
    public abstract class BaseBlockForm : ComponentBase
    {
        [CascadingParameter] public EditContext CurrentEditContext { get; set; }
        protected FieldIdentifier FieldIdentifier { get; private set; }

        protected override void OnInitialized()
        {
            FieldIdentifier = CreateFieldIdentifier();
            base.OnInitialized();
        }

        protected abstract FieldIdentifier CreateFieldIdentifier();

        protected void NotifyChange()
        {
            CurrentEditContext.NotifyFieldChanged(FieldIdentifier);
            StateHasChanged();
        }
    }
}
