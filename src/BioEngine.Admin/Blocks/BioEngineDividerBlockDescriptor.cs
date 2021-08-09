using System;
using AntDesign;
using BioEngine.Admin.Blocks.Forms;
using Microsoft.AspNetCore.Components;
using Sitko.Blockly;
using Sitko.Blockly.Blazor;
using Sitko.Core.App.Localization;

namespace BioEngine.Admin.Blocks
{
    public record DividerBlock : ContentBlock
    {
        public string? Text { get; set; }
    }

    public record BioEngineDividerBlockDescriptor : BlockDescriptor<DividerBlock>, IBlazorBlockDescriptor<DividerBlock>
    {
        public BioEngineDividerBlockDescriptor(ILocalizationProvider<DividerBlock> localizationProvider) : base(
            localizationProvider)
        {
        }

        public virtual RenderFragment Icon => builder =>
        {
            builder.OpenComponent(1, typeof(Icon));
            builder.AddAttribute(1, nameof(AntDesign.Icon.Type), "line");
            builder.CloseComponent();
        };

        public virtual Type FormComponent => typeof(DividerBlockForm);
        public Type DisplayComponent => typeof(DividerBlockForm);
        public override string Title => "Разделитель";
    }
}
