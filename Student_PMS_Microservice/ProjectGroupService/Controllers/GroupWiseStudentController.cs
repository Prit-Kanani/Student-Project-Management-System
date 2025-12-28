using Comman.DTOs.CommanDTOs;
using Comman.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectGroupService.DTOs;
using ProjectGroupServices.Data;

namespace ProjectGroupService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupWiseStudentController : ControllerBase
    {
        #region CONFIGURATION
        private readonly AppDbContext _context;
        public GroupWiseStudentController(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region GET STUDENT WISE GROUP PAGE
        [HttpGet]
        [Route("Page")]
        [Produces<ListResult<GroupWiseStudentListDTO>>]
        public async Task<IActionResult> GetGroupWiseStudentPage(int page = 0,int total = 10)
        {
            var gws = await _context.GroupWiseStudent.Skip(page * total).Take(total).ToListAsync();

            var response = ReflectionMapper.Map<ListResult<GroupWiseStudentListDTO>>(gws);

            return Ok(response);
        }

        #endregion
    }
}
