using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DatingApp.API.Interfaces;
using DatingApp.API.DTO;
using AutoMapper;

namespace DatingApp.API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var users = await _userRepository.GetMembersAsync();
            return Ok(users);
            
            // var users = await _userRepository.GetUsersAsync();
            // var usersToReturn = _mapper.Map<IEnumerable<MemberDTO>>(users);
            // return Ok(usersToReturn);
        }

        [Authorize]
        [HttpGet("{userName}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string userName)
        {
            return await _userRepository.GetMemberAsync(userName);
            // var user = await _userRepository.GetUsersByUserNameAsync(userName);
            // var userToReturn = _mapper.Map<MemberDTO>(user);
            // return Ok(userToReturn);
        }
    }
}