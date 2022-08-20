using BlogEngine.Models;
using BlogEngine.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BlogEngine.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class PostController : ControllerBase
    {
        private readonly IPostRepository postRepository;
        protected Response response;

        public PostController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
            response = new Response();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            try
            {
                Post model = await postRepository.createOrUpdate(post);
                if (model == null)
                {
                    response.IsSuccess = false;
                    response.DisplayMessage = "Post is already createed";
                    return BadRequest(response);
                }
                response.result = model;
                response.DisplayMessage = "Post created sucessfully";
                return Ok(response);
            }
            catch (Exception ex)
            {

                response.IsSuccess = false;
                response.DisplayMessage = "Error for create the register";
                response.ErrorMessages = new List<string> { ex.ToString() };
                return BadRequest(response);
            }

        }

        [HttpGet]
        public async Task<IActionResult> getPublishedPosts()
        {
            try
            {
                var lista = await postRepository.getPublishedPosts();
                response.result = lista;
                response.DisplayMessage = "Post's List";
            }

            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessages = new List<string> { ex.ToString() };

            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentById([FromBody] Comment comment)
        {
            string  status = await postRepository.AddCommentById(comment);
            if (status.Equals("OK"))
            {
                response.IsSuccess = true;
                response.DisplayMessage = "Added comment";
                return Ok(response);
            }
            response.IsSuccess = false;
            response.DisplayMessage = "Cannot add comment";
            return NotFound(response);
        }
    }
}
