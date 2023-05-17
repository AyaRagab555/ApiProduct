using ApiDemo01.Dto;
using ApiDemo01.ResponseModule;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiDemo01.Controllers
{
    
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenServices _tokenServices;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManger,
            SignInManager<AppUser> signInManager,
            ITokenServices tokenServices,
            IMapper mapper)
        {
            _userManger = userManger;
            _signInManager = signInManager;
            _tokenServices = tokenServices;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("GetUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
         {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManger.FindByEmailAsync(email);
            if (user is null)
               return NotFound(new ApiResponce(404));

            return new UserDto
            {
                Email = email,
                DisplayName = user.DisplayName,
                Token = _tokenServices.CreateToken(user)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManger.FindByEmailAsync(loginDto.Email);
            if (user is null)
                return Unauthorized(new ApiResponce(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponce(401));

            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = _tokenServices.CreateToken(user)
            };
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(CheckEmailExistAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[]
                    {
                        "Email Adress is in Use"
                    }
                });

            }
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
            };
            var result = await _userManger.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponce(400));

            return new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = _tokenServices.CreateToken(user)
            };
        }
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync([FromQuery] string email)
              => await _userManger.FindByEmailAsync(email) != null;

         
        [Authorize]
        [HttpGet("GetAddress")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManger.Users.Include(x => x.Address)
                           .SingleOrDefaultAsync(x=> x.Email == email);

            var mappedAddress = _mapper.Map<AddressDto>(user.Address);
            return Ok(mappedAddress);
        }

        [Authorize]
        [HttpPost("UpdateAddress")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManger.Users.Include(x => x.Address)
                .SingleOrDefaultAsync(x => x.Email == email);

            user.Address = _mapper.Map<Address>(addressDto);
            var result = await _userManger.UpdateAsync(user);
            if(result.Succeeded)
                return Ok(_mapper.Map<AddressDto>(user.Address));
            return BadRequest(new ApiResponce(400, "Problem updating the user adress"));
        }

    }
}
 