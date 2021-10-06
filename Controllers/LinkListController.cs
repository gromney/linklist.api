using System.Security.Claims;
using System.Threading.Tasks;
using LinkList.api.Contracts;
using LinkList.api.Domain;
using LinkList.api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkList.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinkListController : ControllerBase
    {
        private readonly ILinkListRepository repository;
        public LinkListController(ILinkListRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await repository.GetlAll(userId);

            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Data);
        }

        [HttpGet("{title}")]
        public async Task<IActionResult> GetCollection([FromRoute] string title)
        {
            var response = await repository.GetCollection(title);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }
            return Ok(response.Data);
        }

        [HttpGet("available/{title}")]
        public async Task<IActionResult> Available([FromRoute] string title)
        {
            var response = await repository.Available(title);

            return Ok(response.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateLinkListRequest request)
        {
            request.UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await repository.Publish(request);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

            return Created("GetCollection", response.Data.Title);
        }

        [HttpPut("{title}")]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute] string title, UpdateLinkListRequest request)
        {
            var response = await repository.Update(title, request);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }
            
            return NoContent();
        }

    }
}