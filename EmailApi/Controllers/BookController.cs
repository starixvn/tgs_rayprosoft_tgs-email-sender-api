using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.EmailApi.Metrics;

namespace TGS.EmailApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly OtelMetrics _meters;

        public BookController(OtelMetrics meters)
        {
            _meters = meters;
        }

        [HttpPost]
        public int AddBook()
        {
            _meters.AddBook();
            _meters.IncreaseTotalBooks();
            return 0;
        }

        [HttpPut]
        public int UpdateBook()
        {
            _meters.UpdateBook();
            return 0;
        }

        [HttpDelete]
        public int DeleteBook()
        {
            _meters.DeleteBook();
            _meters.DecreaseTotalBooks();
            return 0;
        }


        [HttpPost("category")]
        public int AddCategory()
        {
            _meters.AddCategory();
            _meters.IncreaseTotalCategories();
            return 0;
        }

        [HttpPut("category")]
        public int UpdateCategory()
        {
            _meters.UpdateCategory();
            return 0;
        }

        [HttpDelete("category")]
        public int DeleteCategory()
        {
            _meters.DeleteCategory();
            _meters.DecreaseTotalCategories();
            return 0;
        }

        [HttpPost("record_order_price/{price:double}")]
        public int RecordOrderTotalPrice(double price)
        {
            _meters.RecordOrderTotalPrice(price);
            return 0;
        }

        [HttpPost("record_book_number/{amount:int}")]
        public int RecordNumberOfBooks(int amount)
        {
            _meters.RecordNumberOfBooks(amount);
            return 0;
        }

        [HttpPost("order_cancel")]
        public int IncreaseOrdersCanceled()
        {
            _meters.IncreaseOrdersCanceled();
            return 0;
        }

        [HttpPost("city/{city}")]
        public int IncreaseTotalOrders([FromRoute] string city)
        {
            _meters.IncreaseTotalOrders(city);
            return 0;
        }
    }
}
