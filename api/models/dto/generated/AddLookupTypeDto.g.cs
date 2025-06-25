using System;
using CAS.DB.models.lookupcodes;

namespace CAS.API.models.dto.generated
{
    public partial class AddLookupTypeDto
    {
        public LookupTypeCategory Category { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int? SortOrder { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string DisplayColor { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public bool? IsSystem { get; set; }
        public uint ConcurrencyToken { get; set; }
    }
}