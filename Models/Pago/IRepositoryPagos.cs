namespace _Net.Models

{
	public interface IRepositoryPagos: IRepositorio<Pago>
	{
		IList<Pago> ObtenerPorContrato(int idContrato);
	}
}