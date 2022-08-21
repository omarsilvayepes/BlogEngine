using BlogEngine.Models;
using BlogEngine.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace BlogEngine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        protected Response response;


        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            response = new Response();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            string result = await _userRepository.Register(new User { UserName = userDTO.UserName,Role=userDTO.Role }, userDTO.PassWord);
            if (result.Equals("User already register"))
            {
                response.IsSuccess = false;
                response.DisplayMessage = "User already register";
                return BadRequest(response);
            }
            if (result.Equals("500"))
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Error for create the user";
                return BadRequest(response);
            }

            response.DisplayMessage = "Successfully registered user";
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDTO userDTO)
        {
            string token = await _userRepository.Login(userDTO.UserName, userDTO.PassWord,userDTO.Role);
            if (token.Equals("NoUser"))
            {
                response.IsSuccess = false;
                response.DisplayMessage = "User is not registered";
                return BadRequest(response);
            }
            if (token.Equals("PassWordWrong"))
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Wrong Password";
                return BadRequest(response);
            }
            if (token.Equals("RoleNotAuthorize"))
            {
                response.IsSuccess = false;
                response.DisplayMessage = "Role Not Authorize";
                return BadRequest(response);
            }
            response.DisplayMessage = "Sucessfully logged user";
            JWTPackage jWTPackage = new JWTPackage();
            jWTPackage.UserName = userDTO.UserName;
            jWTPackage.Token = token;
            response.result = jWTPackage;
            return Ok(response);
        }
    }
}
