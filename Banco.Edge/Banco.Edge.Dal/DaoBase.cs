using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal;
public abstract class DaoBase : IDisposable
{
    private protected readonly SqlConnection conn;
    private protected readonly SqlCommand cmd;
    public DaoBase()
    {
        cmd = new();
        conn = new()
        {
            ConnectionString = Resources.ConnectionString
        };
        conn.Open();
    }
    private protected async Task<DataSet> ExecuteQueryAsync(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
    {
        SqlCommand cmd = new();
        using SqlConnection conn = new(Resources.ConnectionString);
        
        foreach (var item in parametros)
            cmd.Parameters.Add(item);

        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync();

        cmd.CommandText = nomeProcedure;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Connection = conn;

        if (transaction)
            cmd.Transaction = conn.BeginTransaction();

        SqlDataAdapter adapter = new(cmd);
        DataSet dbSet = new();

        try
        {
            adapter.Fill(dbSet);

            if (transaction)
                await cmd.Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (transaction)
                await cmd.Transaction.RollbackAsync();

            throw;
        }

        return dbSet;
    }
    private protected async Task ExecuteNonQueryAsync(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
    {
        cmd.Parameters.Clear();
        foreach (var item in parametros)
            cmd.Parameters.Add(item);

        if (conn.State == ConnectionState.Closed)
            await conn.OpenAsync();

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = nomeProcedure;
        cmd.Connection = conn;

        if (transaction)
            cmd.Transaction = conn.BeginTransaction();

        try
        {
            await cmd.ExecuteNonQueryAsync();

            if (transaction)
                await cmd.Transaction.CommitAsync();
        }
        catch (Exception)
        {
            if (transaction)
                await cmd.Transaction.RollbackAsync();

            throw;
        }
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
        cmd.Dispose();
        conn.Dispose();

        if (conn.State != ConnectionState.Closed)
            conn.Close();

        SqlConnection.ClearPool(conn);
        GC.Collect();

        if (string.IsNullOrEmpty(conn.ConnectionString))
            conn.ConnectionString = Resources.ConnectionString;

        GC.SuppressFinalize(this);
    }
}