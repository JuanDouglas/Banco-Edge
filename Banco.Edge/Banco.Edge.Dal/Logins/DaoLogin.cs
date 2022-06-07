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
    public async Task InsertLoginAsync(Login login)
    {
        SqlParameter[] parametros =
        {
            new(nameof(Login.IP), login.IP),
            new(nameof(Login.Token), login.Token),
            new(nameof(Login.ClienteId), login.ClienteId)
        };

        await ExecuteNonQueryAsync("InserirLogin", parametros, true);
    }
}