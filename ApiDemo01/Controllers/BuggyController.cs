using ApiDemo01.ResponseModule;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : ControllerBase
    {
        private readonly StoreDbContext _context;

        public BuggyController(StoreDbContext context)
        {
            _context = context;
        }
        [HttpGet("TestText")]
        [Authorize]
        public ActionResult<string> GetText()
        {
            return "Some Text";
        }
        [HttpGet("notfound")]
        //[ProducesResponseType(typeof(ApiResponce),StatusCode.Status404NotFound)]
        public ActionResult GetNotFound()
        {
            var anyting = _context.products.Find(1000);
            if (anyting == null)
                return NotFound(new ApiResponce(404));
            return Ok();
        }
        [HttpGet("badRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponce(400));
        }
    }
}
