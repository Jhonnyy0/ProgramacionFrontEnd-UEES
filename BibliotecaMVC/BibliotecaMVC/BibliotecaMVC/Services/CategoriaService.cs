using System.Data;
using BibliotecaMVC.Models;
using Microsoft.Data.SqlClient;

namespace BibliotecaMVC.Services
{
    /// <summary>
    /// Implementación de ICategoriaService usando ADO.NET contra SQL Server.
    /// Todas las consultas son parametrizadas (SqlParameter) para evitar
    /// inyección SQL y respetar los tipos de datos de la tabla.
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly string _cadenaConexion;

        public CategoriaService(IConfiguration configuration)
        {
            _cadenaConexion = configuration.GetConnectionString("BibliotecaDB")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'BibliotecaDB' en appsettings.json.");
        }

        // ===================== MOSTRAR =====================
        public IEnumerable<Categoria> ObtenerTodas()
        {
            var categorias = new List<Categoria>();

            const string sql = @"SELECT Id, Nombre, Descripcion
                                 FROM dbo.Categorias
                                 ORDER BY Nombre";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        categorias.Add(MapearCategoria(lector));
                    }
                }
            }

            return categorias;
        }

        // ============ OBTENER UNA (para el formulario de edición) ============
        public Categoria? ObtenerPorId(int id)
        {
            Categoria? categoria = null;

            const string sql = @"SELECT Id, Nombre, Descripcion
                                 FROM dbo.Categorias
                                 WHERE Id = @Id";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conexion.Open();

                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        categoria = MapearCategoria(lector);
                    }
                }
            }

            return categoria;
        }

        // ===================== AGREGAR (INSERT) =====================
        public int Crear(Categoria categoria)
        {
            const string sql = @"INSERT INTO dbo.Categorias (Nombre, Descripcion)
                                 VALUES (@Nombre, @Descripcion);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = categoria.Nombre;
                comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 250).Value =
                    string.IsNullOrWhiteSpace(categoria.Descripcion)
                        ? (object)DBNull.Value
                        : categoria.Descripcion;

                conexion.Open();

                object? resultado = comando.ExecuteScalar();
                return resultado == null ? 0 : Convert.ToInt32(resultado);
            }
        }

        // ===================== EDITAR (UPDATE) =====================
        public bool Actualizar(Categoria categoria)
        {
            const string sql = @"UPDATE dbo.Categorias
                                 SET Nombre = @Nombre,
                                     Descripcion = @Descripcion
                                 WHERE Id = @Id";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = categoria.Nombre;
                comando.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 250).Value =
                    string.IsNullOrWhiteSpace(categoria.Descripcion)
                        ? (object)DBNull.Value
                        : categoria.Descripcion;
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = categoria.Id;

                conexion.Open();

                int filasAfectadas = comando.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

        // ===================== ELIMINAR (DELETE) =====================
        public bool Eliminar(int id)
        {
            const string sql = @"DELETE FROM dbo.Categorias
                                 WHERE Id = @Id";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            using (SqlCommand comando = new SqlCommand(sql, conexion))
            {
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                conexion.Open();

                int filasAfectadas = comando.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }

        // ===================== Utilitario privado =====================
        private static Categoria MapearCategoria(SqlDataReader lector)
        {
            int posicionDescripcion = lector.GetOrdinal("Descripcion");

            return new Categoria
            {
                Id = lector.GetInt32(lector.GetOrdinal("Id")),
                Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
                Descripcion = lector.IsDBNull(posicionDescripcion)
                    ? string.Empty
                    : lector.GetString(posicionDescripcion)
            };
        }
    }
}
