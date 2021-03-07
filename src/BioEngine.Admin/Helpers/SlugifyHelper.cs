using System.Collections.Generic;
using NickBuhro.Translit;
using Slugify;

namespace BioEngine.Admin.Helpers
{
    public static class SlugifyHelper
    {
        public static string Slugify(string text, string oldText, string currentSlug)
        {
            string slug = currentSlug;
            if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(oldText))
            {
                var helper = GetSlugHelper();

                if (string.IsNullOrEmpty(currentSlug) && string.IsNullOrEmpty(oldText))
                {
                    slug = helper.GenerateSlug(Transliteration.CyrillicToLatin(text));
                }
                else
                {
                    var oldSlug = helper.GenerateSlug(Transliteration.CyrillicToLatin(oldText));
                    if (string.IsNullOrEmpty(currentSlug) || oldSlug == currentSlug)
                    {
                        slug = helper.GenerateSlug(Transliteration.CyrillicToLatin(text));
                    }
                }
            }

            return slug;
        }

        public static SlugHelper GetSlugHelper()
        {
            var config = new SlugHelperConfiguration
            {
                StringReplacements = new Dictionary<string, string> {{" ", "_"}},
                ForceLowerCase = true,
                CollapseDashes = true,
                TrimWhitespace = true,
                CollapseWhiteSpace = true
            };
            return new SlugHelper(config);
        }
    }
}
