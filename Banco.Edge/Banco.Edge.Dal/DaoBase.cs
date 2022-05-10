using Banco.Edge.Dal.Resources;
using System.Data.SqlClient;

namespace Banco.Edge.Bll;
public abstract class DaoBase
{
    private protected readonly SqlConnection conn;

    public DaoBase()
    {
        conn = new SqlConnection(Resources.ConnectionString);
    }
}
