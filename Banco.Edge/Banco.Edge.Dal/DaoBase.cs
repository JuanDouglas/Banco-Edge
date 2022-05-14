using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal;
public abstract class DaoBase : IDisposable
{
    private protected readonly SqlConnection conn;

    public DaoBase()
    {
        conn = new SqlConnection(Resources.ConnectionString);
        conn.Open();
    }
    private protected async Task<DataSet> ExecuteQueryAsync(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
    {
        SqlCommand comando = new();

        foreach (var item in parametros)
            comando.Parameters.Add(item);

        comando.CommandType = CommandType.StoredProcedure;
        comando.CommandText = nomeProcedure;
        comando.Connection = conn;
        if (transaction)
            comando.Transaction = conn.BeginTransaction();

        SqlDataAdapter adapter = new(comando);
        DataSet dbSet = new();

        try
        {
            await Task.Run(() => adapter.Fill(dbSet));

            if (transaction)
                await comando.Transaction.CommitAsync();
        }
        catch (Exception)
        {
            if (transaction)
                await comando.Transaction.RollbackAsync();

            throw;
        }

        return dbSet;
    }
    private protected async Task ExecuteNonQueryAsync(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
    {
        SqlCommand comando = new();

        foreach (var item in parametros)
            comando.Parameters.Add(item);

        await conn.OpenAsync();

        comando.CommandType = CommandType.StoredProcedure;
        comando.CommandText = nomeProcedure;
        comando.Connection = conn;

        if (transaction)
            comando.Transaction = conn.BeginTransaction();

        await comando.ExecuteNonQueryAsync();
    }
    private protected DataRow[] DataTableToRows(DataSet ds)
    {
        List<DataRow> rows = new();

        foreach (DataTable dt in ds.Tables)
        {
            DataRow[] dataRows = new DataRow[dt.Rows.Count];

            dt.Rows.CopyTo(dataRows, 0);

            rows.AddRange(dataRows);
        }

        return rows.ToArray();
    }

    public void Dispose()
    {
        if (conn.State != ConnectionState.Closed)
            conn.Close();

        SqlConnection
            .ClearPool(conn);
    }
}
