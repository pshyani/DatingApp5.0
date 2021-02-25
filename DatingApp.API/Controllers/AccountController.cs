using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using DatingApp.API.DTO;
using DatingApp.API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                 ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName))
                return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDTO);

            // using 
            var hmac = new HMACSHA512();

            user.UserName = registerDTO.UserName.ToLower();
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            // user.PasswordSalt = hmac.Key;           

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            
            if(!result.Succeeded)
                return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);
                
            return new UserDTO()
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users
                            .Include(P => P.Photos)
                            .SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName.ToLower());

            if (user == null)
                return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if(!result.Succeeded)
                return Unauthorized();
            // using var hmac = new HMACSHA512(user.PasswordSalt);

            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            // for (int i = 0; i < computedHash.Length; i++)
            // {
            //     if (computedHash[i] != user.PasswordHash[i])
            //         return Unauthorized("Invalid password");
            // }

            return new UserDTO()
            {
                UserName = user.UserName,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url ?? "",
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}