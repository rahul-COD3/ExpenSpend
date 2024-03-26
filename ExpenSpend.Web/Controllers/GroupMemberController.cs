using AutoMapper;
using ExpenSpend.Domain.DTOs.GroupMembers;
using ExpenSpend.Domain.Models.GroupMembers;
using ExpenSpend.Service.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ExpenSpend.Web.Controllers
{
    [Route("api/group-members")]
    [ApiController]
    [Authorize]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupMemberAppService _groupMemberService;
        private readonly IMapper _mapper;

        public GroupMemberController(IGroupMemberAppService groupMemberService, IMapper mapper)
        {
            _groupMemberService = groupMemberService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroupMembers()
        {
            var groupMembers = await _groupMemberService.GetAllGroupMembersAsync();
            if(groupMembers.IsSuccess)
            {
                return Ok(groupMembers.Data);
            }
            return NotFound(groupMembers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupMemberById(Guid id)
        {
            var groupMember = await _groupMemberService.GetGroupMemberByIdAsync(id);
            if(groupMember.IsSuccess)
            {
                return Ok(groupMember.Data);
            }
            return NotFound(groupMember);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupMember(CreateGroupMemberDto input)
        {
            var result = await _groupMemberService.CreateGroupMemberAsync(input);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpPut("make-admin/{id}")]
        public async Task<IActionResult> MakeGroupAdmin(Guid id)
        {
            var result = await _groupMemberService.MakeGroupAdminAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpPut("remove-admin/{id}")]
        public async Task<IActionResult> RemoveGroupAdmin(Guid id)
        {
            var result = await _groupMemberService.RemoveGroupAdminAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupMember(Guid id)
        {
            var result = await _groupMemberService.DeleteGroupMemberAsync(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result);
        }
    }
}
