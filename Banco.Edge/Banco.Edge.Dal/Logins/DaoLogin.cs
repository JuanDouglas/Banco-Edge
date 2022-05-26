using Banco.Edge.Dml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Dal.Logins;
public sealed class DaoLogin : DaoBase
{
    public async Task<Login> BuscarLoginAsync(string token, string chaveConta)
    {
        throw new NotImplementedException();
    }
    public void InserirLogin(Login login)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter(nameof(Login.IP), login.IP),
            new SqlParameter(nameof(Login.Token), login.Token),
            new SqlParameter(nameof(Login.ClienteId), login.ClienteId)
        };

       ExecuteNonQuery("InserirLogin", parametros, true);
    }
}