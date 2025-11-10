namespace _Net.Models
{
    public interface IRepositoryContratos : IRepositorio<Contrato>
    {
        IList<Contrato> ObtenerTodosOPorFiltros(int? dias = null, bool? vigente = null, int? idInquilino = null);

        // NUEVO: lo usamos desde el controller
        IList<Contrato> ObtenerPorInmueble(int idInmueble);

        bool ExisteSuperposicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContratoExcluir = null);
    }
}
