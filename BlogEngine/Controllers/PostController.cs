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

        [Authorize(Policy = "WriterOnly")]
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

        [Authorize(Policy = "WriterOnly")]
        [HttpPost]
        public async Task<IActionResult> UpdatePost([FromBody] Post post)
        {
            try
            {
                Post model = await postRepository.createOrUpdate(post);
                response.result = model;
                return Ok(response);
            }
            catch (Exception ex)
            {

                response.IsSuccess = false;
                response.DisplayMessage = "Error for update post";
                response.ErrorMessages = new List<string> { ex.ToString() };
                return BadRequest(response);
            }
        }

        [Authorize(Policy = "WriterOnly")]
        [HttpGet]
        public async Task<IActionResult> getCreateAndPendingPosts()
        {
            try
            {
                var lista = await postRepository.getCreateAndPendingPosts();
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

        [Authorize(Policy = "WriterOnly")]
        [HttpPost]
        public async Task<IActionResult> SubmitPost([FromBody] Post post)
        {
            string status = await postRepository.SubmitPost(post);
            if (status.Equals("OK"))
            {
                response.IsSuccess = true;
                response.DisplayMessage = "Submitted post";
                return Ok(response);
            }
            response.IsSuccess = false;
            response.DisplayMessage = "Cannot submit post";
            return NotFound(response);
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize(Policy = "EditorOnly")]
        [HttpGet]
        public async Task<IActionResult> getPendingPosts()
        {
            try
            {
                var lista = await postRepository.getPendingPosts();
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

        [Authorize(Policy = "EditorOnly")]
        [HttpPost]
        public async Task<IActionResult> ApprovePendingPost([FromBody] Post post)
        {
            string status = await postRepository.ApprovePendingPost(post);
            if (status.Equals("OK"))
            {
                response.IsSuccess = true;
                response.DisplayMessage = "Published post";
                return Ok(response);
            }
            response.IsSuccess = false;
            response.DisplayMessage = "Cannot Approve post";
            return NotFound(response);
        }

        [Authorize(Policy = "EditorOnly")]
        [HttpPost]
        public async Task<IActionResult> RejectPendingPost([FromBody] Post post)
        {
            string status = await postRepository.RejectPendingPost(post);
            if (status.Equals("OK"))
            {
                response.IsSuccess = true;
                response.DisplayMessage = "Rejected post";
                return Ok(response);
            }
            response.IsSuccess = false;
            response.DisplayMessage = "Cannot Reject post";
            return NotFound(response);
        }
    }
}
