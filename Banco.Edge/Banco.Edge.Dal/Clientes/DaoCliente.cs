using Banco.Edge.Dal.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Bll.Clientes;

public class DaoCliente : DaoBase
{
    public async Task<long> InserirCliente(string nome, string email, string cpfOrCnpj)
    {
        List<SqlParameter> parameters = new()
        {
            new SqlParameter("Email", email),
            new SqlParameter("Nome", nome),
            new SqlParameter("CpfOrCnpj", cpfOrCnpj)
        };

        DataSet dbSet = await ExecutarAsync("InsertCliente",parameters);
        DataRowCollection rows = dbSet.Tables[0].Rows;

        long id = -1;

        if (rows.Count > 0)
            _ = long.TryParse(rows[0][0].ToString(), out id);

        return id;
    }
}