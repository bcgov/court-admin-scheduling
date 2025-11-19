using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CAS.DB.models.lookupcodes;
using CAS.DB.Configuration;
using CAS.DB.models.auth;
using System;

namespace CAS.DB.configuration
{
    public class LookupTypeConfiguration : BaseEntityConfiguration<LookupType>
    {
        public override void Configure(EntityTypeBuilder<LookupType> builder)
        {
            builder.Property(b => b.Id).HasIdentityOptions(startValue: 100);
            builder.Property(x => x.Name).IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();

            // Sequence for Code, starting at 100
            builder.HasIndex(x => x.Code).IsUnique();
            builder.Property(x => x.Code)
                .IsRequired()
                .HasDefaultValueSql("nextval('\"LookupType_Code_seq\"')");

            // Seed initial values with explicit Code values
            builder.HasData(
                new LookupType { Id = 1, Category = LookupTypeCategory.Assignment, Code = 0, Name = "CourtRoom", Abbreviation = "CR", SortOrder = 1, Description = "Court Room", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8930), new TimeSpan(0, 0, 0, 0, 0)), DisplayColor = "#6C3BAA", IsSystem = true },
                new LookupType { Id = 2, Category = LookupTypeCategory.Assignment, Code = 1, Name = "CourtRole", Abbreviation = "CA", SortOrder = 2, Description = "Court Assignment", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8933), new TimeSpan(0, 0, 0, 0, 0)), DisplayColor = "#189fd4" },
                new LookupType { Id = 3, Category = LookupTypeCategory.Assignment, Code = 2, Name = "JailRole", Abbreviation = "J", SortOrder = 3, Description = "Jail Assignment", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8935), new TimeSpan(0, 0, 0, 0, 0)), DisplayColor = "#A22BB9" },
                new LookupType { Id = 4, Category = LookupTypeCategory.Assignment, Code = 3, Name = "EscortRun", Abbreviation = "T", SortOrder = 4, Description = "Transport Assignment", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8936), new TimeSpan(0, 0, 0, 0, 0)), DisplayColor = "#ffb007" },
                new LookupType { Id = 5, Category = LookupTypeCategory.Assignment, Code = 4, Name = "OtherAssignment", Abbreviation = "O", SortOrder = 5, Description = "Other Assignment", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8938), new TimeSpan(0, 0, 0, 0, 0)), DisplayColor = "#7a4528" },
                new LookupType { Id = 6, Category = LookupTypeCategory.Leave, Code = 5, Name = "LeaveType", SortOrder = 1, Description = "Leave Type", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8939), new TimeSpan(0, 0, 0, 0, 0)) },
                new LookupType { Id = 7, Category = LookupTypeCategory.Training, Code = 6, Name = "TrainingType", SortOrder = 1, Description = "Training Type", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8941), new TimeSpan(0, 0, 0, 0, 0)) },
                new LookupType { Id = 8, Category = LookupTypeCategory.Rank, Code = 7, Name = "CourtAdminRank", SortOrder = 1, Description = "Court Admin Rank", CreatedById = User.SystemUser, CreatedOn = new DateTimeOffset(new DateTime(2025, 6, 23, 20, 33, 55, 877, DateTimeKind.Unspecified).AddTicks(8942), new TimeSpan(0, 0, 0, 0, 0)) }
            );
        }
    }
}
