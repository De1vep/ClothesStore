using asm2prn221.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Orders
{
    public class IndexModel : PageModel
    {
        private readonly ClothesStoreContext _context;
        private readonly IConfiguration configuration;

        public IndexModel(ClothesStoreContext context, IConfiguration configuration)
        {
            _context = context;
            this.configuration = configuration;
        }
        public string IdSort { get; set; }
        public string DateSort { get; set; }
        public string CustomerNameSort { get; set; }
        public string TotalAmountSort { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public string SelectedStatus { get; set; }
        public PaginatedList<Order> Orders { get; set; }
        

        private async Task LoadOrdersAsync(string sortOrder, int pageIndex, DateTime? searchDate, string statusFilter)
        {
            ViewData["page"] = "Admin";
            IQueryable<Order> orderIQ = from o in _context.Orders.Include(o => o.User)
                                        select o;

            if (searchDate.HasValue)
            {
                orderIQ = orderIQ.Where(o => o.OrderDate.Date == searchDate.Value.Date);
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                orderIQ = orderIQ.Where(o => o.OrderStatus == statusFilter);
            }

            switch (sortOrder)
            {
                case "id_desc":
                    orderIQ = orderIQ.OrderByDescending(o => o.OrderId);
                    break;
                case "id":
                    orderIQ = orderIQ.OrderBy(o => o.OrderId);
                    break;
                case "date_asc":
                    orderIQ = orderIQ.OrderBy(o => o.OrderDate);
                    break;
                case "date":
                    orderIQ = orderIQ.OrderByDescending(o => o.OrderDate);
                    break;
                case "customerName_desc":
                    orderIQ = orderIQ.OrderByDescending(o => o.User.Fullname);
                    break;
                case "customerName":
                    orderIQ = orderIQ.OrderBy(o => o.User.Fullname);
                    break;
                case "totalAmount_desc":
                    orderIQ = orderIQ.OrderByDescending(o => o.TotalAmount);
                    break;
                case "totalAmount":
                    orderIQ = orderIQ.OrderBy(o => o.TotalAmount);
                    break;
                default:
                    orderIQ = orderIQ.OrderByDescending(o => o.OrderDate);
                    break;
            }

            int pageSize = configuration.GetValue("PageSize", 10);
            Orders = await PaginatedList<Order>.CreateAsync(orderIQ.AsNoTracking(), pageIndex, pageSize);
        }

        public async Task<IActionResult> OnGetAsync(string sortOrder, int? pageIndex, DateTime? searchDate, string statusFilter)
        {
            IdSort = sortOrder == "id" ? "id_desc" : "id";
            DateSort = sortOrder == "date_asc" ? "date" : "date_asc";
            CustomerNameSort = sortOrder == "customerName" ? "customerName_desc" : "customerName";
            TotalAmountSort = sortOrder == "totalAmount" ? "totalAmount_desc" : "totalAmount";

            CurrentSort = sortOrder;
            SelectedStatus = statusFilter;
            await LoadOrdersAsync(sortOrder, pageIndex ?? 1, searchDate, statusFilter);

            return Page();
        }

    }
}
