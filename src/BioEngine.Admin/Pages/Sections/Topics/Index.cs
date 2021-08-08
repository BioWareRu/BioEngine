using System;
using BioEngine.Core.Data.Entities;
using ImgProxy;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Topics
{
    [Route("/Sections/Topics")]
    public class Index : BaseSectionsListPage<Topic, TopicData>
    {
        protected override string GetUrl(Topic item) => $"/Sections/Topics/{item.Id}";

        protected override string Title => "Темы";

        protected override string CreatePageUrl => "/Sections/Topics/Add";
    }

    public class ResizeOption : ImgProxyOption
    {
        public string Type { get; }
        public int Width { get; }
        public int Height { get; }
        public bool Enlarge { get; }
        public bool Extend { get; }

        public ResizeOption(string type, int width, int height, bool enlarge = false, bool extend = false)
        {
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));

            Type = type;
            Width = width;
            Height = height;
            Enlarge = enlarge;
            Extend = extend;
        }

        public override string ToString()
        {
            var enlarge = Enlarge ? "1" : "0";
            var extend = Extend ? "1" : "0";

            return $"resize:{Type}:{Width}:{Height}:{enlarge}:{extend}";
        }
    }
}
