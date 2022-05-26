using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal;
public abstract class DaoBase
{
    public DaoBase()
    {

    }
    private protected DataSet ExecuteQuery(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
    {
        SqlConnection conn = new(Resources.ConnectionString);
        SqlCommand cmd = conn.CreateCommand();

        cmd.Parameters.Clear();

        foreach (var item in parametros)
            cmd.Parameters.Add(item);

        if (conn.State == ConnectionState.Closed)
            conn.Open();

        cmd.CommandText = nomeProcedure;
        cmd.CommandType = CommandType.StoredProcedure;

        if (transaction)
            cmd.Transaction = conn.BeginTransaction();

        SqlDataAdapter adapter = new(cmd);
        DataSet dbSet = new();

        try
        {
            adapter.Fill(dbSet);

            if (transaction)
                cmd.Transaction.Commit();
        }
        catch (Exception ex)
        {
            if (transaction)
                cmd.Transaction.Rollback();

            throw;
        }

        return dbSet;
    }
    private protected void ExecuteNonQuery(string nomeProcedure, List<SqlParameter> parametros, bool transaction = false)
    {
        SqlConnection conn = new(Resources.ConnectionString);
        SqlCommand cmd = conn.CreateCommand();

        cmd.Parameters.Clear();
        foreach (var item in parametros)
            cmd.Parameters.Add(item);

        if (conn.State == ConnectionState.Closed)
            conn.Open();

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = nomeProcedure;
        cmd.Connection = conn;

        if (transaction)
            cmd.Transaction = conn.BeginTransaction();

        try
        {
            cmd.ExecuteNonQuery();

            if (transaction)
                cmd.Transaction.Commit();
        }
        catch (Exception)
        {
            if (transaction)
                cmd.Transaction.Rollback();

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
}