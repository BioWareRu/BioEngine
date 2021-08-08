using System;
using Microsoft.AspNetCore.Components;
using Sitko.Core.App.Blazor.Components;
using Sitko.Core.App.Blazor.Forms;

namespace BioEngine.Admin.Shared
{
    public abstract class BaseFormPage<TEntity, TForm> : BasePage
        where TForm : BaseForm<TEntity> where TEntity : class, new()
    {
        protected TForm Form { get; set; } = null!;
        [Parameter] public Guid EntityId { get; set; }
    }
}
