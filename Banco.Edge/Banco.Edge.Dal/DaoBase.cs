using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal;
public abstract class DaoBase
{
    private protected readonly SqlConnection conn;

    public DaoBase()
    {
        conn = new SqlConnection(Resources.ConnectionString);
    }
    private protected async Task<DataSet> ExecutarAsync(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
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

        await conn.CloseAsync();
        return dbSet;
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
}
