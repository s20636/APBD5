using Cwiczenia5.Models;
using Cwiczenia5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cwiczenia5.Controllers
{
    [Route("api/warehouses")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private IDbService _service;

        public WarehousesController(IDbService dbService)
        {
            _service = dbService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouseAsync(Warehouse warehouse)
        {
            try
            {
                int response = await _service.RegisterProductInWarehouseAsync(warehouse);
                if (response == 0)
                {
                    return StatusCode(500, "Błąd serwera SQL");
                }
                return Ok(response);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
                
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }



        }
    }
}
