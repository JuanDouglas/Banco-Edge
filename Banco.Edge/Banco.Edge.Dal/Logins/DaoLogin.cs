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
    public async Task<Login> BuscarLoginAsync(string token, int contaId)
    {
        throw new NotImplementedException();
    }
    public async Task InsertLoginAsync(Login login)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter(nameof(Login.IP), login.IP),
            new SqlParameter(nameof(Login.Token), login.Token),
            new SqlParameter(nameof(Login.ClienteId), login.ClienteId)
        };

        await ExecuteNonQueryAsync("InserirLogin", parametros, true);
    }
}