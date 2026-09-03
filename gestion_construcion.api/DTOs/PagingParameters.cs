namespace Firmeza.Api.DTOs
{
    public class PagingParameters
    {
        private const int MaxPageSize = 50;
        private int _pageNumber = 1;
        public int PageNumber { get => _pageNumber; set => _pageNumber = (value < 1) ? 1 : value; }
        private int _pageSize = 6; // Un buen número para grids de 3 columnas
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
