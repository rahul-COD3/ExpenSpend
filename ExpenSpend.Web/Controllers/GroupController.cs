using ExpenSpend.Domain.DTOs.Groups;
using ExpenSpend.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExpenSpend.Web.Controllers
{
    [ApiController]
    [Route("api/groups")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupAppService _groupAppService;

        public GroupController(IGroupAppService groupAppService)
        {
            _groupAppService = groupAppService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            var groups = await _groupAppService.GetAllGroupsAsync();
            return Ok(groups.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            var group = await _groupAppService.GetGroupByIdAsync(id);
            if (group.IsSuccess)
            {
                return Ok(group.Data);
            }
            return NotFound(group);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGroupsByUserId(Guid userId)
        {
            var groups = await _groupAppService.GetGroupsByUserId(userId);
            if(groups.IsSuccess)
            {
                return Ok(groups.Data);
            }
            return NotFound(groups.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupDto input)
        {
            var result = await _groupAppService.CreateGroupAsync(input);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpPost("with-members")]
        public async Task<IActionResult> CreateGroupWithMembers(CreateGroupWithMembersDto input)
        {
            var result = await _groupAppService.CreateGroupWithMembers(input);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(Guid id, UpdateGroupDto input)
        {
            var result = await _groupAppService.UpdateGroupAsync(id, input);
            if(result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            var result = await _groupAppService.SoftDeleteAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Data);
        }
    }
}
