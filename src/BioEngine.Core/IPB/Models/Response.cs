using System;

namespace BioEngine.Core.IPB.Models
{
    public class Response<T>
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }
        public T[] Results { get; set; } = Array.Empty<T>();
    }
}
