using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {

        private readonly ApplicationDBContext _context;
        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ID == id);

            if (stock == null)
            {
                return null;
            }
            _context.Remove(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task<List<Stock>> GetAllAsync(QueryObject queryObject)
        {
            var stocks = _context.Stocks.Include(x => x.Comments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
            {
                stocks = stocks.Where(x => x.Symbol.Contains(queryObject.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(queryObject.CompanyName))
            {
                stocks = stocks.Where(x => x.CompanyName.Contains(queryObject.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if (queryObject.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = queryObject.IsDescending ? stocks.OrderByDescending(x => x.Symbol) : stocks.OrderBy(x => x.Symbol);
                }
            }

            var skipNumber = (queryObject.PageNumber-1) * queryObject.PageSize;
            return await stocks.Skip(skipNumber).Take(queryObject.PageSize ).ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.Include(x => x.Comments).FirstOrDefaultAsync(x => x.ID == id);
        }

        public Task<bool> StockExists(int id)
        {
            return _context.Stocks.AnyAsync(x => x.ID == id);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto stockDto)
        {
            var existingStock = _context.Stocks.FirstOrDefault(x => x.ID == id);
            if (existingStock == null)
            {
                return null;
            }
            existingStock.Symbol = stockDto.Symbol;
            existingStock.CompanyName = stockDto.CompanyName;
            existingStock.Purchase = stockDto.Purchase;
            existingStock.LastDiv = stockDto.LastDiv;
            existingStock.Industry = stockDto.Industry;
            existingStock.MarketCap = stockDto.MarketCap;
            await _context.SaveChangesAsync();
            return existingStock;
        }
    }
}