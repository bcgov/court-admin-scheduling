using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CAS.API.models.dto.generated;
using CAS.API.services;
using CAS.DB.models.lookupcodes;
using Mapster;
using CAS.API.infrastructure.authorization;
using CAS.DB.models;
using CAS.DB.models.auth;

namespace CAS.API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupTypeController : ControllerBase
    {
        private readonly LookupTypeService _service;

        public LookupTypeController(LookupTypeService service)
        {
            _service = service;
        }

        [HttpGet("actives")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LookupTypeDto>>> GetActive([FromQuery] LookupTypeCategory category)
        {
            var result = await _service.GetActiveAsync(category);
            return Ok(result.Adapt<List<LookupTypeDto>>());
        }

        [HttpGet("all")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<List<LookupTypeDto>>> GetAllWithExpired([FromQuery] LookupTypeCategory category)
        {
            var result = await _service.GetAllWithExpiredAsync(category);
            return Ok(result.Adapt<List<LookupTypeDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupTypeDto>> Create(AddLookupTypeDto dto)
        {
            if (dto == null) return BadRequest();
            var entity = await _service.AddAsync(dto);
            return Ok(entity.Adapt<LookupTypeDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult<LookupTypeDto>> Update(UpdateLookupTypeDto dto)
        {
            if (dto == null) return BadRequest();
            var entity = await _service.UpdateAsync(dto);
            return Ok(entity.Adapt<LookupTypeDto>());
        }

        [HttpPut("sort")]
        [PermissionClaimAuthorize(perm: Permission.EditTypes)]
        public async Task<ActionResult> UpdateSort([FromBody] List<SortOrderDto> sortOrders)
        {
            if (sortOrders == null) return BadRequest();
            var updates = sortOrders.ConvertAll(x => (x.Id, x.SortOrder));
            await _service.UpdateSortOrderAsync(updates);
            return NoContent();
        }

        [HttpPut("{id}/expire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupTypeDto>> Expire(int id)
        {
            var entity = await _service.ExpireAsync(id);
            return Ok(entity.Adapt<LookupTypeDto>());
        }

        [HttpPut("{id}/unexpire")]
        [PermissionClaimAuthorize(perm: Permission.ExpireTypes)]
        public async Task<ActionResult<LookupTypeDto>> Unexpire(int id)
        {
            var entity = await _service.UnexpireAsync(id);
            return Ok(entity.Adapt<LookupTypeDto>());
        }
    }

    public class SortOrderDto
    {
        public int Id { get; set; }
        public int SortOrder { get; set; }
    }
}