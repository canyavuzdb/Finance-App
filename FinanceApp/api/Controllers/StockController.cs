using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepository;
        public StockController(ApplicationDBContext context, IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject queryObject)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stocks = await _stockRepository.GetAllAsync(queryObject);
            var stocksDto = stocks.Select(stock => stock.ToStockDto());

            return Ok(stocks);
        }

        [HttpGet("{Id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stock = await _stockRepository.GetByIdAsync(Id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto StockDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stock = StockDto.ToStockFromCreateDTO();
            await _stockRepository.CreateAsync(stock);
            return CreatedAtAction(nameof(GetById), new { Id = stock.Id }, stock.ToStockDto());
        }

        [HttpPut]
        [Route("{Id:int}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stock = await _stockRepository.UpdateAsync(Id, updateDto);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());

        }

        [HttpDelete]
        [Route("{Id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var stock = await _stockRepository.DeleteAsync(Id);
            if (stock == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}