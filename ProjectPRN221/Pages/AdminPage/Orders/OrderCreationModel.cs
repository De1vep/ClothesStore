using ProjectPRN221.Models;

namespace ProjectPRN221.Pages.AdminPage.Orders
{
    public class OrderCreationModel
    {
        public Order Order { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }

    public class OrderDetailViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
