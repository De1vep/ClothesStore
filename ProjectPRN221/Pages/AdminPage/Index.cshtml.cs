using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectPRN221.Pages.AdminPage
{
    public class IndexModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        public int MonthlyOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyProductsSold { get; set; }
        public string PercentageProductChange { get; set; }
        public string PercentageOrderChange { get; set; }
        public string PercentageRevenueChange { get; set; }
        public int DailyOrders { get; set; }
        public decimal DailyRevenue { get; set; }
        public string OrdersChartData { get; set; } = default!;
        public string RevenueChartData { get; set; } = default!;
        public string TopProductsChartData { get; set; } = default!;
        public IList<Product> LowStockProducts { get; set; } = default!;
        public int SelectedMonth { get; set; }
        public int SelectedYear { get; set; }
        public IndexModel(ClothesStoreContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            ViewData["page"] = "Admin";
            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            MonthlyOrders = await GetMonthlyOrdersCountAsync(currentMonth, currentYear);
            MonthlyRevenue = await GetMonthlyRevenueAsync(currentMonth, currentYear);
            MonthlyProductsSold = await GetMonthlyProductsSoldAsync(currentMonth, currentYear);

            var previousMonth = (currentMonth == 1) ? 12 : currentMonth - 1;
            var previousYear = (currentMonth == 1) ? currentYear - 1 : currentYear;

            var previousMonthProductsSold = await GetMonthlyProductsSoldAsync(previousMonth, previousYear);
            var previousMonthOrders = await GetMonthlyOrdersCountAsync(previousMonth, previousYear);
            var previousMonthRevenue = await GetMonthlyRevenueAsync(previousMonth, previousYear);
            PercentageProductChange = CalculatePercentageChange(MonthlyProductsSold, previousMonthProductsSold);
            PercentageOrderChange = CalculatePercentageChange(MonthlyOrders, previousMonthOrders);
            PercentageRevenueChange = CalculatePercentageChange(MonthlyRevenue, previousMonthRevenue);

            var today = DateTime.Now.Date;

            DailyOrders = await GetDailyOrdersCountAsync(today);
            DailyRevenue = await GetDailyRevenueAsync(today);

            OrdersChartData = await GetOrdersChartDataAsync(currentMonth, currentYear);
            RevenueChartData = await GetRevenueChartDataAsync(currentMonth, currentYear);
            TopProductsChartData = await GetTopProductsChartDataAsync(currentMonth, currentYear);
            LowStockProducts = await GetLowStockProductsAsync();
        }

        private async Task<int> GetMonthlyOrdersCountAsync(int month, int year)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.OrderStatus.ToLower() != "cancel" && o.OrderStatus.ToLower() != "shipped")
                .CountAsync();
        }

        private async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.Month == month && od.Order.OrderDate.Year == year && od.Order.OrderStatus.ToLower() != "cancel" && od.Order.OrderStatus.ToLower() != "shipped")
                .ToListAsync();

            return orderDetails.Sum(od => od.Quantity * od.UnitPrice);
        }

        private async Task<int> GetMonthlyProductsSoldAsync(int month, int year)
        {
            return await _context.OrderDetails
                .Where(od => od.Order.OrderDate.Month == month && od.Order.OrderDate.Year == year && od.Order.OrderStatus.ToLower() != "cancel" && od.Order.OrderStatus.ToLower() != "shipped")
                .SumAsync(od => od.Quantity);
        }

        private async Task<int> GetDailyOrdersCountAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.OrderDate.Date == date && o.OrderStatus.ToLower() != "cancel" && o.OrderStatus.ToLower() != "shipped")
                .CountAsync();
        }

        private async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            var orderDetails = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.Date == date && od.Order.OrderStatus.ToLower() != "cancel" && od.Order.OrderStatus.ToLower() != "shipped")
                .ToListAsync();

            return orderDetails.Sum(od => od.Quantity * od.UnitPrice);
        }

        private async Task<string> GetOrdersChartDataAsync(int month, int year)
        {
            var dailyOrders = await _context.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.OrderStatus.ToLower() != "cancel" && o.OrderStatus.ToLower() != "shipped")
                .GroupBy(o => o.OrderDate.Day)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .OrderBy(x => x.Day)
                .ToListAsync();

            var days = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => day.ToString("D2")).ToList();
            var counts = days.Select(day => dailyOrders.FirstOrDefault(d => d.Day == int.Parse(day))?.Count ?? 0).ToList();

            var json = new
            {
                Labels = days,
                Data = counts
            };

            return System.Text.Json.JsonSerializer.Serialize(json);
        }

        private async Task<string> GetRevenueChartDataAsync(int month, int year)
        {
            var dailyRevenue = await _context.Orders
                .Where(o => o.OrderDate.Month == month && o.OrderDate.Year == year && o.OrderStatus.ToLower() != "cancel" && o.OrderStatus.ToLower() != "shipped")
                .SelectMany(o => o.OrderDetails)
                .GroupBy(od => od.Order.OrderDate.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    Total = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderBy(x => x.Day)
                .ToListAsync();

            var days = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .Select(day => day.ToString("D2")).ToList();
            var totals = days.Select(day =>
                dailyRevenue.FirstOrDefault(d => d.Day == int.Parse(day))?.Total ?? 0m
            ).ToList();

            var json = new
            {
                Labels = days,
                Data = totals
            };

            return System.Text.Json.JsonSerializer.Serialize(json);
        }

        [NonAction]
        public async Task<string> GetTopProductsChartDataAsync(int month, int year)
        {
            var topProducts = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.Month == month && od.Order.OrderDate.Year == year)
                .GroupBy(od => new { od.ProductId, od.Product.ProductName })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(10)
                .ToListAsync();

            var productNames = topProducts.Select(p => p.ProductName).ToList();
            var quantities = topProducts.Select(p => p.TotalQuantitySold).ToList();

            var json = new
            {
                Labels = productNames,
                Data = quantities
            };

            return System.Text.Json.JsonSerializer.Serialize(json);
        }

        [HttpGet]
        public async Task<IActionResult> OnGetTopProductsChartData(int month, int year)
        {
            var json = await GetTopProductsChartDataAsync(month, year);
            return Content(json, "application/json");
        }
        private async Task<IList<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Where(p => p.Quantity < 10)
                .ToListAsync();
        }

        private string CalculatePercentageChange(decimal currentMonthValue, decimal previousMonthValue)
        {
            if (previousMonthValue == 0)
            {
                return currentMonthValue == 0 ? "0%" : "+100%";
            }

            var percentageChange = ((currentMonthValue - previousMonthValue) / previousMonthValue) * 100;
            var formattedChange = percentageChange.ToString("0.00");

            return percentageChange >= 0
                ? $"+{formattedChange}%"
                : $"{formattedChange}%";
        }

        [HttpGet]
        public async Task<IActionResult> OnGetRevenueChartData(int month, int year)
        {
            var json = await GetRevenueChartDataAsync(month, year);
            return Content(json, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> OnGetOrdersChartData(int month, int year)
        {
            var json = await GetOrdersChartDataAsync(month, year);
            return Content(json, "application/json");
        }
    }

}
