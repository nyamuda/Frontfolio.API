using Frontfolio.API.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Frontfolio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlogsController : ControllerBase
    {
        private readonly BlogService _blogService;
        private readonly JwtService _jwtService;

        public BlogsController(BlogService blogService,JwtService jwtService) { 
            _blogService = blogService;
            _jwtService = jwtService;
        
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                //First, extract the user's access token from the Authorization header
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                //Manually validate the token and then grab the User ID from the token claims
                ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
                string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());


                //Get the blog       
                if (int.TryParse(tokenUserId, out int userId))
                {
                    BlogDto blog = await _blogService.GetAsync(blogId: id, tokenUserId: userId);
                    return Ok(blog);
                }
                //throw an exception if tokenUserId cannot be parsed
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ErrorMessageHelper.UnexpectedErrorMessage(),
                    details = ex.Message
                };

                return StatusCode(500, response);
            }

        }


        [HttpGet]
        public async Task<IActionResult> GetAll(
            BlogStatusFilter status = BlogStatusFilter.All,
            BlogSortOption sortBy = BlogSortOption.PublishedAt,
            int page = 1, int pageSize = 5)
        {
            try
            {
   
                //First, extract the user's access token from the Authorization header
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                //Manually validate the token and then grab the User ID from the token claims
                ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
                string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

                //Get the paginated blogs for a user with the given ID
                if (int.TryParse(tokenUserId, out int userId))
                {
                    PageInfo<BlogDto> paginatedBlogs = await _blogService
                    .GetAllAsync(page: page, pageSize: pageSize, userId: userId, sortOption: sortBy, filterOption: status);

                    return Ok(paginatedBlogs);
                }
                //throw an exception if tokenUserId cannot be parsed
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ErrorMessageHelper.UnexpectedErrorMessage(),
                    details = ex.Message
                };

                return StatusCode(500, response);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Post(AddBlogDto addBlogDto)
        {
            try
            {
                //First, extract the user's access token from the Authorization header
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                //Manually validate the token and then grab the User ID from the token claims
                ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
                string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

                if (int.TryParse(tokenUserId, out int userId))
                {
                    //Add the new blog
                    var blog = await _blogService.CreateAsync(userId, addBlogDto);

                    return CreatedAtAction(actionName: nameof(Get), new { id = blog.Id }, blog);
                }

                //throw an exception if tokenUserId cannot be parsed
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ErrorMessageHelper.UnexpectedErrorMessage(),
                    details = ex.Message
                };

                return StatusCode(500, response);
            }

        }

        //Update a blog
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBlogDto updateBlogDto)
        {
            try
            {
                //First, extract the user's access token from the Authorization header
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                //Manually validate the token and then grab the User ID from the token claims
                ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
                string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

                if (int.TryParse(tokenUserId, out int userId))
                {
                    //update blog
                    await _blogService.UpdateAsync(blogId: id, tokenUserId: userId, updateBlogDto);

                    return NoContent();
                }

                //throw an exception if tokenUserId cannot be parsed
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ErrorMessageHelper.UnexpectedErrorMessage(),
                    details = ex.ToString()
                };

                return StatusCode(500, response);
            }
        }

        //Delete a blog
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //First, extract the user's access token from the Authorization header
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                //Manually validate the token and then grab the User ID from the token claims
                ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
                string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier) ??
                    throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());


                if (int.TryParse(tokenUserId, out int userId))
                {
                    //delete blog
                    await _blogService.DeleteAsync(blogId: id, tokenUserId: userId);

                    return NoContent();
                }

                //throw an exception if tokenUserId cannot be parsed
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ErrorMessageHelper.UnexpectedErrorMessage(),
                    details = ex.Message
                };

                return StatusCode(500, response);
            }
        }

        //Delete background paragraph for a specific blog
        [HttpDelete("{blogId}/backgrounds/{paragraphId}")]
        public async Task<IActionResult> DeleteBlogBackgroundParagraph(int blogId, int paragraphId)
        {
            try
            {
                //Retrieve the access token from the request
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                //Manually validate the token
                ClaimsPrincipal claims = _jwtService.ValidateJwtToken(token);
                //Get the user ID claim from the token
                string tokenUserId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

                if (int.TryParse(tokenUserId, out int userId))
                {
                    await _paragraphService.DeleteByIdAsync(blogId: blogId, paragraphId: paragraphId, tokenUserId: userId);
                    return NoContent();
                }
                //throw an exception if tokenUserId cannot be parsed
                throw new UnauthorizedAccessException(ErrorMessageHelper.InvalidNameIdentifierMessage());

            }

            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    message = ErrorMessageHelper.UnexpectedErrorMessage(),
                    details = ex.Message
                };

                return StatusCode(500, response);
            }

        }
    }
}
