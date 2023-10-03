using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItensPresentes.Model
{
    public class ApresentationItensCasaModel : PageModel
    {
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public List<ItemCasaModel> Data { get; set; }
    }

}
