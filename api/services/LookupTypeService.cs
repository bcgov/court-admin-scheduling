using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.DB.models.lookupcodes;
using CAS.DB.models;
using System;
using CAS.API.models.dto.generated;
using Mapster;

namespace CAS.API.services
{
    public class LookupTypeService
    {
        private CourtAdminDbContext Db { get; }

        public LookupTypeService(CourtAdminDbContext dbContext)
        {
            Db = dbContext;
        }

        public async Task<string> GetNameById(int id)
            => (await Db.LookupType.FindAsync(id))?.Name;

        public async Task<int?> GetIdByName(string name)
            => (await Db.LookupType.FirstOrDefaultAsync(x => x.Name == name))?.Id;

        /// <summary>
        /// Returns all active (not expired) LookupTypes, sorted by SortOrder.
        /// </summary>
        public async Task<List<LookupType>> GetActiveAsync(LookupTypeCategory category)
        {
            var now = DateTimeOffset.UtcNow;
            return await Db.LookupType
                .Where(x => x.Category == category && !x.ExpiryDate.HasValue || x.ExpiryDate > now)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all LookupTypes: active first (sorted), then expired (unsorted).
        /// </summary>
        public async Task<List<LookupType>> GetAllWithExpiredAsync(LookupTypeCategory category)
        {
            var now = DateTimeOffset.UtcNow;
            var active = await Db.LookupType
                .Where(x => x.Category == category && !x.ExpiryDate.HasValue || x.ExpiryDate > now)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();

            var expired = await Db.LookupType
                .Where(x => x.ExpiryDate.HasValue && x.ExpiryDate <= now)
                .OrderByDescending(x => x.ExpiryDate)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return active.Concat(expired).ToList();
        }

        /// <summary>
        /// Returns all LookupTypes that are not system-created.
        /// </summary>
        public async Task<List<LookupType>> GetAllNonSystemAsync()
        {
            return await Db.LookupType
                .Where(x => x.IsSystem != true)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new LookupType, assigning SortOrder to the end of active items.
        /// </summary>
        public async Task<LookupType> AddAsync(AddLookupTypeDto addDto)
        {
            var lookupTypeCd = addDto.Adapt<LookupType>();
            if (string.IsNullOrWhiteSpace(lookupTypeCd.Name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(lookupTypeCd.Name));
            if (string.IsNullOrWhiteSpace(lookupTypeCd.Description))
                throw new ArgumentException("Name cannot be null or empty.", nameof(lookupTypeCd.Description));
            // Check for duplicate name (case-insensitive)
            if (await Db.LookupType.AnyAsync(x => x.Name.ToLower() == lookupTypeCd.Name.ToLower()))
                throw new InvalidOperationException($"A LookupType with the name '{lookupTypeCd.Name}' already exists.");

            // Find the max SortOrder among active records
            var now = DateTimeOffset.UtcNow;
            int maxSortOrder = await Db.LookupType
                .Where(x => x.Category == addDto.Category && !x.ExpiryDate.HasValue || x.ExpiryDate > now)
                .Select(x => x.SortOrder)
                .MaxAsync() ?? 0;
            
            lookupTypeCd.SortOrder = maxSortOrder + 1;
            await Db.LookupType.AddAsync(lookupTypeCd);
            await Db.SaveChangesAsync();
            return lookupTypeCd;
        }

        /// <summary>
        /// Batch update sort order for active records.
        /// </summary>
        public async Task UpdateSortOrderAsync(List<(int id, int sortOrder)> updates)
        {
            var now = DateTimeOffset.UtcNow;
            var ids = updates.Select(u => u.id).ToList();
            var records = await Db.LookupType
                .Where(x => ids.Contains(x.Id) && (!x.ExpiryDate.HasValue || x.ExpiryDate > now))
                .ToListAsync();

            foreach (var record in records)
            {
                var update = updates.First(u => u.id == record.Id);
                record.SortOrder = update.sortOrder;
            }
            await Db.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await Db.LookupType.FindAsync(id);
            if (entity == null)
                return false;
            if (entity.IsSystem == true)
                throw new InvalidOperationException("Cannot delete system LookupTypes.");
            // Prevent delete if any LookupCode uses this type
            bool inUse = await Db.LookupCode.AnyAsync(lc => lc.Type == entity.Code);
            if (inUse)
                throw new InvalidOperationException("Cannot delete: LookupType is in use by one or more LookupCodes.");


            Db.LookupType.Remove(entity);
            await Db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates Name, DisplayColor, and Description of a LookupType.
        /// </summary>
        public async Task<LookupType> UpdateAsync(UpdateLookupTypeDto updateDto)
        {
            var entity = await Db.LookupType.FindAsync(updateDto.Id);
            if (entity == null)
                throw new InvalidOperationException($"LookupType with Id {updateDto.Id} not found.");
            if (entity.IsSystem == true)
                throw new InvalidOperationException("Cannot update system LookupTypes.");

            entity.Name = updateDto.Name;
            entity.DisplayColor = updateDto.DisplayColor;
            entity.Description = updateDto.Description;
            entity.Abbreviation = updateDto.Abbreviation;

            await Db.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Expires a LookupType by setting ExpiryDate to now.
        /// </summary>
        public async Task<LookupType> ExpireAsync(int id)
        {
            var entity = await Db.LookupType.FindAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"LookupType with Id {id} not found.");
            if (entity.IsSystem == true)
                throw new InvalidOperationException("Cannot expire system LookupTypes.");

            entity.ExpiryDate = DateTimeOffset.UtcNow;
            await Db.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Un-expires a LookupType and moves it to the end of the sort order in its category.
        /// </summary>
        public async Task<LookupType> UnexpireAsync(int id)
        {
            var entity = await Db.LookupType.FindAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"LookupType with Id {id} not found.");


            // Find max SortOrder in the same category among active records
            var now = DateTimeOffset.UtcNow;
            int maxSortOrder = await Db.LookupType
                .Where(x => x.Category == entity.Category && (!x.ExpiryDate.HasValue || x.ExpiryDate > now))
                .Select(x => x.SortOrder)
                .MaxAsync() ?? 0;

            entity.ExpiryDate = null;
            entity.SortOrder = maxSortOrder + 1;
            await Db.SaveChangesAsync();
            return entity;
        }
    }
}