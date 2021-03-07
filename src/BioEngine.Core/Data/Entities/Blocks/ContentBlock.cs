using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using FluentValidation;

namespace BioEngine.Core.Data.Entities.Blocks
{
    public abstract record ContentBlock
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Position { get; set; }

        public override string ToString()
        {
            return GetType().Name;
        }

        public static ContentBlock? CreateBlock(ContentBlockType type)
        {
            if (BlockDescriptors.ContainsKey(type))
            {
                return Activator.CreateInstance(BlockDescriptors[type].Type) as ContentBlock;
            }

            return null;
        }

        public string? GetTitle()
        {
            var descriptor = GetDescriptor();
            return descriptor != null ? descriptor.Title : null;
        }

        public string? GetIcon()
        {
            var descriptor = GetDescriptor();
            return descriptor != null ? descriptor.Icon : null;
        }

        public ContentBlockType? GetContentBlockType()
        {
            return BlockDescriptors.Where(d => d.Value.Type == GetType()).Select(d => d.Key).FirstOrDefault();
        }

        public string GetContentBlockTypeTitle()
        {
            return GetContentBlockType()?.ToString() ?? string.Empty;
        }

        public ContentBlockDescriptor? GetDescriptor()
        {
            return BlockDescriptors.Values.FirstOrDefault(d => d.Type == GetType());
        }

        public static readonly ImmutableDictionary<ContentBlockType, ContentBlockDescriptor> BlockDescriptors = new
            Dictionary<ContentBlockType, ContentBlockDescriptor>
            {
                {ContentBlockType.Text, new ContentBlockDescriptor(typeof(TextBlock), "Текст", "fas fa-pen")},
                {ContentBlockType.Cut, new ContentBlockDescriptor(typeof(CutBlock), "Кат", "fas fa-cut")},
                {
                    ContentBlockType.Quote,
                    new ContentBlockDescriptor(typeof(QuoteBlock), "Цитата", "fas fa-quote-right")
                },
                {
                    ContentBlockType.Picture,
                    new ContentBlockDescriptor(typeof(PictureBlock), "Картинка", "fas fa-image")
                },
                {
                    ContentBlockType.Gallery,
                    new ContentBlockDescriptor(typeof(GalleryBlock), "Галерея", "fas fa-images")
                },
                {ContentBlockType.File, new ContentBlockDescriptor(typeof(FileBlock), "Файл", "fas fa-paperclip")},
                {
                    ContentBlockType.Youtube,
                    new ContentBlockDescriptor(typeof(YoutubeBlock), "YouTube", "fab fa-youtube")
                },
                {
                    ContentBlockType.Twitter,
                    new ContentBlockDescriptor(typeof(TwitterBlock), "Twitter", "fab fa-twitter")
                },
                {ContentBlockType.Twitch, new ContentBlockDescriptor(typeof(TwitchBlock), "Twitch", "fab fa-twitch")},
                {ContentBlockType.IFrame, new ContentBlockDescriptor(typeof(IframeBlock), "IFrame", "fas fa-crop")},
            }.ToImmutableDictionary();
    }

    public record ContentBlockDescriptor(Type Type, string Title, string Icon);

    public abstract record ContentBlock<T> : ContentBlock where T : ContentBlockData, new()
    {
        public T Data { get; set; } = new();
    }

    public abstract record ContentBlockData
    {
    }

    public enum ContentBlockType
    {
        None,
        Text = 1,
        Cut = 2,
        Quote = 3,
        Picture = 4,
        Gallery = 5,
        File = 6,
        Youtube = 7,
        Twitter = 8,
        Twitch = 9,
        IFrame = 10
    }

    public static class ContentBlockExtensions
    {
        public static IRuleBuilderOptions<TModel, ContentBlock> AddBlockValidators<TModel>(
            this IRuleBuilderInitialCollection<TModel, ContentBlock> options)
        {
            return options
                .SetInheritanceValidator(validator =>
                    validator
                        .Add(new TextBlockValidator())
                        .Add(new CutBlockValidator())
                        .Add(new FileBlockValidator())
                        .Add(new GalleryBlockValidator())
                        .Add(new IframeBlockValidator())
                        .Add(new PictureBlockValidator())
                        .Add(new QuoteBlockValidator())
                        .Add(new TwitchBlockValidator())
                        .Add(new TwitterBlockValidator())
                        .Add(new YoutubeBlockValidator())
                );
        }
    }

    public sealed class ValueCollection<T> : Collection<T>, IEquatable<ValueCollection<T>>, IFormattable
    {
        private readonly IEqualityComparer<T>? _equalityComparer;

        public ValueCollection() : this(new List<T>()) { }

        public ValueCollection(IEqualityComparer<T>? equalityComparer = null) : this(new List<T>(), equalityComparer)
        {
        }

        public ValueCollection(IList<T> list, IEqualityComparer<T>? equalityComparer = null) : base(list) =>
            _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;

        public bool Equals(ValueCollection<T>? other)
        {
            Debug.Assert(_equalityComparer != null, "_equalityComparer != null");

            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            using var enumerator1 = this.GetEnumerator();
            using var enumerator2 = other.GetEnumerator();

            while (enumerator1.MoveNext())
                if (!enumerator2.MoveNext() || !(_equalityComparer!).Equals(enumerator1.Current, enumerator2.Current))
                    return false;

            return !enumerator2.MoveNext(); //both enumerations reached the end
        }

        public override bool Equals(object? obj) =>
            obj is { } && (ReferenceEquals(this, obj) || obj is ValueCollection<T> coll && Equals(coll));

        public override int GetHashCode() =>
            unchecked(Items.Aggregate(0,
                (current, element) => (current * 397) ^ (element is null ? 0 : _equalityComparer!.GetHashCode(element))
            ));

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            //TODO propose a meaningful format i.e. [,] or {;}

            return $"[{FormatValue(this, formatProvider)}]";
        }

        public override string ToString() => ToString(null, CultureInfo.CurrentCulture);

        private static string? FormatValue(object? value, IFormatProvider? formatProvider) =>
            value switch
            {
                null => "∅",
                bool b => b ? "true" : "false",
                string s => $"\"{s}\"",
                char c => $"\'{c}\'",
                DateTime dt => dt.ToString("o", formatProvider),
                IFormattable @if => @if.ToString(null, formatProvider),
                IEnumerable ie => "[" +
                                  string.Join(", ", ie.Cast<object>().Select(e => FormatValue(e, formatProvider))) +
                                  "]",
                _ => value.ToString()
            };
    }
}
