using Microsoft.AspNetCore.Components.Forms;

namespace BioEngine.Admin.Blocks.Forms
{
    public partial class DividerBlockForm
    {
        protected override FieldIdentifier CreateFieldIdentifier() => FieldIdentifier.Create(() => Block.Text);
    }
}
