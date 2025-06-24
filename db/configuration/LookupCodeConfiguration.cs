using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.Configuration;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.lookupcodes;

namespace CAS.DB.configuration
{
    public class LookupCodeOrderConfiguration : BaseEntityConfiguration<LookupCode>
    {
        public override void Configure(EntityTypeBuilder<LookupCode> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 1000);

            builder.HasIndex(lc => new {lc.Type, lc.Code, lc.LocationId}).IsUnique();

            builder.HasOne(b => b.Location).WithMany().HasForeignKey(lc => lc.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(b => b.SortOrder).WithOne(a=> a.LookupCode).HasForeignKey(lc => lc.LookupCodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new LookupCode { CreatedById = User.SystemUser, Id = 1, Code = "Criminal Registry", Description = "Criminal Registry", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 2, Code = "Court Clerk", Description = "Court Clerk", Type = (int)LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 3, Code = "JP", Description = "JP", Type = (int)LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 4, Code = "Traffic Registry", Description = "Traffic Registry", Type = (int)LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 5, Code = "Family Registry", Description = "Family Registry", Type = (int)LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 6, Code = "Small Claims", Description = "Small Claims", Type = (int)LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 7, Code = "DDR", Description = "DDR", Type = (int)LookupTypes.CourtAdminRank },
              
                new LookupCode { CreatedById = User.SystemUser, Id = 8, Code = "CEW (Taser)", Description = "CEW (Taser)", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 9, Code = "DNA", Description = "DNA", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 10, Code = "FRO", Description = "FRO", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 11, Code = "Fire Arm", Description = "Fire Arm", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 12, Code = "First Aid", Description = "First Aid", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 13, Code = "Advanced Escort SPC (AESOC)", Description = "Advanced Escort SPC (AESOC)", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 14, Code = "Extenuating Circumstances SPC (EXSPC)", Description = "Extenuating Circumstances SPC (EXSPC)", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 15, Code = "Search Gate", Description = "Search Gate", Type = (int)LookupTypes.TrainingType },
                new LookupCode { CreatedById = User.SystemUser, Id = 16, Code = "Other", Description = "Other", Type = (int)LookupTypes.TrainingType },
                
                new LookupCode { CreatedById = User.SystemUser, Id = 17, Code = "STIP", Description = "STIP", Type = (int) LookupTypes.LeaveType},
                new LookupCode { CreatedById = User.SystemUser, Id = 18, Code = "Annual", Description = "Annual", Type = (int) LookupTypes.LeaveType },
                new LookupCode { CreatedById = User.SystemUser, Id = 19, Code = "Illness", Description = "Illness", Type = (int) LookupTypes.LeaveType },
                new LookupCode { CreatedById = User.SystemUser, Id = 20, Code = "Special", Description = "Special", Type = (int) LookupTypes.LeaveType },

                new LookupCode { CreatedById = User.SystemUser, Id = 21, Code = "Civil Registry", Description = "Civil Registry", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 22, Code = "Transcripts", Description = "Transcripts", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 23, Code = "Exhibits", Description = "Exhibits", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 24, Code = "Accounting", Description = "Accounting", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 25, Code = "Interpreters", Description = "Interpreters", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 26, Code = "Records or File Search", Description = "Records or File Search", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 27, Code = "Supervisor", Description = "Supervisor", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 28, Code = "Manager", Description = "Manager", Type = (int) LookupTypes.CourtAdminRank },
                new LookupCode { CreatedById = User.SystemUser, Id = 29, Code = "Senior Manager", Description = "Senior Manager", Type = (int) LookupTypes.CourtAdminRank }
            );
            base.Configure(builder);
        }
    }
}
