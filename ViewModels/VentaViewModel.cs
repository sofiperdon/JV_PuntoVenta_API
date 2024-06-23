namespace JV_PuntoVenta_API.ViewModels
{
    public class VentaViewModel
    {
        public DateTime TransactionDateTime { get; set; }
        public List<VentaProductoViewModel> VentaProductos { get; set; }
    }
}
