using System;
using BioEngine.Core.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections
{
    public abstract partial class BaseSectionsListPage<TItem>
        where TItem : Section
    {
        

        protected abstract string GetUrl(TItem item);

        
    }
}
