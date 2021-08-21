using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleAuthServer.API.Models.Exceptions;
using SimpleAuthServer.API.Models.Requests;
using SimpleAuthServer.API.Models.Responses;
using SimpleAuthServer.API.Services;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserRepository userRepository;
        private readonly ICryptoService cryptoService;
        
        public AuthController(IUserRepository repository, ICryptoService cryptoService, TokenService tokenService) : base(tokenService)
        {
            this.userRepository = repository;
            this.cryptoService = cryptoService;
        }
        
        [HttpPost, Route("register")]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(InvalidModelResponse(ModelState));
            }

            try
            {
                var newUser = await userRepository.CreateUser(request.Username, request.Password);
                var response = new RegisterResponse()
                {
                    Success = true,
                    CreatedDate = newUser.CreatedDate,
                    Username = newUser.Username
                };

                return Ok(response);
            }
            catch (UserAlreadyExistsException)
            {
                return BadRequest(BadRequestWithMessage("Username already exists."));
            }
        }

        [HttpPost, Route("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(InvalidModelResponse(ModelState));
            }

            if(string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(BadRequestWithMessage("Invalid username or password."));
            }

            var matchedUser = await userRepository.FindUserByUserName(request.Username);
            if(matchedUser == null)
            {
                return BadRequest(BadRequestWithMessage("Username not found."));
            }

            var loginResult = await cryptoService.ValidateLogin(request.Password, matchedUser.PasswordHash);
            if(loginResult)
            {
                var responseObject = new LoginResponse()
                {
                    Success = true,
                    AccessToken = tokenService.GenerateAccessTokenForUser(matchedUser),
                    RefreshToken = tokenService.GetRefreshTokenForUser(matchedUser)
                };

                Response.Headers.Add("Authorization", $"Bearer {responseObject.AccessToken}");
                return Ok(responseObject);
            }
            else
            {
                return Unauthorized("Login failed.");
            }
        }

        [Authorize]
        [HttpPost, Route("logout")]
        public async Task<IActionResult> Logout()
        {
            tokenService.DeactivateToken(Request.Headers["Authorization"]);
            return Ok();
        }

        [HttpPost, Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var matchedUser = tokenService.GetUserFromRefreshToken(refreshToken);
            
            if (matchedUser != null && tokenService.ValidateRefreshToken(refreshToken))
            {
                var responseObject = new LoginResponse()
                {
                    Success = true,
                    AccessToken = tokenService.GenerateAccessTokenForUser(matchedUser),
                    RefreshToken = tokenService.GetRefreshTokenForUser(matchedUser)
                };

                Response.Headers.Add("Authorization", $"Bearer {responseObject.AccessToken}");
                return Ok(responseObject);
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpGet, Route("deactivated")]
        public async Task<IActionResult> GetDeactivatedTokens()
        {
            return Ok(tokenService.GetDeactivatedTokens());
        }
    }
}
