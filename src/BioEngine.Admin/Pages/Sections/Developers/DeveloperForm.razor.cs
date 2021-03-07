using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Helpers;
using BioEngine.Admin.Pages.Sites;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Entities.Blocks;
using BioEngine.Core.Data.Repositories;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Pages.Sections.Developers
{
    public partial class DeveloperForm
    {
        protected override Task OnCreatedAsync(Developer entity)
        {
            NavigationManager.NavigateTo($"/Sections/Developers/{entity.Id}");
            return base.OnCreatedAsync(entity);
        }
    }

    public class DeveloperFormModel : BaseSectionFormModel<Developer, DeveloperData>
    {
        public DeveloperFormModel(Developer section, IEnumerable<Site> sites) : base(section, sites)
        {
        }
    }

    public class DeveloperFormValidator : BaseSectionFormValidator<DeveloperFormModel, Developer, DeveloperData>
    {
    }
}
