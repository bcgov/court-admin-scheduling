using System;
using System.ComponentModel.DataAnnotations;
using CAS.COMMON.attributes.mapping;
using db.models;
using Mapster;

namespace CAS.DB.models.lookupcodes
{
    [AdaptTo("[name]Dto")]
    [GenerateUpdateDto, GenerateAddDto]
    public class LookupType : BaseEntity
    {
        [Key]
        [ExcludeFromAddDto]
        public int Id { get; set; }
        public LookupTypeCategory Category { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int? SortOrder { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public string DisplayColor { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public bool? IsSystem { get; set; }
    }
}