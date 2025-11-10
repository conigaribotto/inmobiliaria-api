namespace _Net.Models

{
	public interface IRepositoryInmuebles: IRepositorio<Inmueble>
	{
		IList<Inmueble> ObtenerDisponiblesEntreFechas(DateTime fechaInicio, DateTime fechaFin);
        IList<Inmueble> ObtenerTodosOPorFiltro(int? idPropietario = null, bool? disponible = null);
	}
}