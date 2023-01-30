using Cwiczenia5.Models;
using Cwiczenia5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cwiczenia5.Controllers
{
    [Route("api/warehouses2")]
    [ApiController]
    public class Warehouses2Controller : ControllerBase
    {
        private IDbService _service;
        public Warehouses2Controller(IDbService dbService)
        {
            _service = dbService;
        }


        [HttpPost]
        public async Task<IActionResult> RegisterProductInWarehouseWithProcedureAsync(Warehouse warehouse)
        {
            try
            {
                return Ok(await _service.RegisterProductInWarehouseWithProcedureAsync(warehouse));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
