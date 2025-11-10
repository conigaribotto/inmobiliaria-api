namespace _Net.Models
{
	public interface IRepositorio<T>
	{
		int Alta(T p);
		int Baja(int id);
		int Modificar(T p);

		IList<T> ObtenerTodos();
		T? ObtenerPorId(int id);
	}
}