using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Banco.Edge.Dal;
public abstract class DaoBase : IDisposable
{
    private protected readonly SqlConnection conn;
    private protected readonly SqlCommand cmd;
    public virtual event EventHandler<QueryEndEventArgs> QueryExecuted;
    public DaoBase()
    {
        conn = new()
        {
            ConnectionString = Resources.ConnectionString
        };
        cmd = new();
        QueryExecuted += QueryEndLog;
        conn.Open();
    }
    private protected async Task<DataSet> ExecuteQueryAsync(string nomeProcedure, SqlParameter[] parametros, bool transaction = false)
    {
        int rows = 0;
        Stopwatch stopwatch = new();

        cmd.Parameters.Clear();
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
            stopwatch.Start();
            rows = adapter.Fill(dbSet);

            if (transaction)
                await cmd.Transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            if (transaction)
                await cmd.Transaction.RollbackAsync();

            throw;
        }
        finally
        {
            stopwatch.Stop();
            await conn.CloseAsync();
        }

        QueryExecuted.Invoke(this, new(stopwatch, rows));
        return dbSet;
    }
    private protected async Task ExecuteNonQueryAsync(string nomeProcedure, SqlParameter[] parametros, bool transaction = false)
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
        finally
        {
            await conn.CloseAsync();
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

    private protected virtual void QueryEndLog(object? sender, EventArgs args)
    {
    }
   
    public void Dispose()
    {
        cmd.Dispose();
        conn.Dispose();

        SqlConnection.ClearPool(conn);
        GC.Collect();

        if (string.IsNullOrEmpty(conn.ConnectionString))
            conn.ConnectionString = Resources.ConnectionString;

        GC.SuppressFinalize(this);
    }
    public class QueryEndEventArgs : EventArgs
    {  
        public TimeSpan Elapsed { get; }
        public int Rows { get; set; }
        public QueryEndEventArgs(Stopwatch stopwatch, int rows) : base()
        {
            Elapsed = stopwatch.Elapsed;
            Rows = rows;
        }
    }
}
