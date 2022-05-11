using Banco.Edge.Dal.Resources;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Bll;
public abstract class DaoBase
{
    private protected readonly SqlConnection conn;

    public DaoBase()
    {
        conn = new SqlConnection(Resources.ConnectionString);
    }
    private protected async Task<DataSet> ExecutarCAsync(string nomeProcedure, List<SqlParameter> parametros)
    {
        using (conn)
        {
            SqlCommand comando = new();

            foreach (var item in parametros)
                comando.Parameters.Add(item);

            comando.CommandType = CommandType.StoredProcedure;
            comando.CommandText = nomeProcedure;
            comando.Connection = conn;

            SqlDataAdapter adapter = new(comando);
            DataSet dbSet = new();

            try
            {
                await Task.Run(()=> adapter.Fill(dbSet));
                await comando.Transaction.CommitAsync();
            }
            catch (Exception)
            {
                await comando.Transaction.RollbackAsync();
                throw;
            }

            return dbSet;
        }
    }
}
