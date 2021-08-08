using System.Text.RegularExpressions;
using AntDesign;

namespace BioEngine.Admin.Shared.Ant
{
    public partial class BioGridCol
    {
        private string _hostFlexStyle = null;

        protected override string GenerateStyle()
        {
            return $"{base.GenerateStyle()}; flex: {_hostFlexStyle}";
        }

        private void SetHostFlexStyle()
        {
            if (this.Flex.Value == null)
                return;

            this._hostFlexStyle = this.Flex.Match(str =>
                {
                    if (Regex.Match(str, "^\\d+(\\.\\d+)?(px|em|rem|%)$").Success)
                    {
                        return $"0 0 {Flex}";
                    }

                    return Flex.AsT0;
                },
                num => $"{Flex} {Flex} auto");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetHostFlexStyle();
        }
    }
}
