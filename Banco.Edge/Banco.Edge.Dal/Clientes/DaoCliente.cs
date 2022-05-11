using Banco.Edge.Dml;
using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal.Clientes;

public sealed class DaoCliente : DaoBase
{
    public async Task<Cliente?> ExisteAsync(string email, string cpfOuCnpj)
    {
        if (string.IsNullOrEmpty(cpfOuCnpj))
            throw new ArgumentNullException(nameof(cpfOuCnpj));

        if (string.IsNullOrEmpty(email))
            throw new ArgumentNullException(email);

        Cliente[] clientes = await ExecutarBuscaAsync(email: email, cpfOuCnpj: cpfOuCnpj);

        return clientes.Length < 1 ? null : clientes[0];
    }

    public async Task<int> InserirCliente(string nome, string email, string cpfOrCnpj)
    {
        List<SqlParameter> parameters = new()
        {
            new SqlParameter("Email", email),
            new SqlParameter("Nome", nome),
            new SqlParameter("CpfOrCnpj", cpfOrCnpj)
        };

        DataSet dbSet = await ExecutarAsync("InsereCliente", parameters, true);
        DataRowCollection rows = dbSet.Tables[0].Rows;

        int id = -1;

        if (rows.Count > 0)
            _ = int.TryParse(rows[0][0].ToString(), out id);

        return id;
    }

    private async Task<Cliente[]> ExecutarBuscaAsync(
        int? id = null,
        string? email = null,
        string? cpfOuCnpj = null,
        int take = 1,
        int skip = 0)
    {
        List<SqlParameter> parameters = new()
        {
            new("Skip", skip),
            new("Take", take),
            new("Email", email),
            new("CpfOuCnpj", cpfOuCnpj),
            new("Id", id)
        };

        DataSet ds = await ExecutarAsync("BuscaCliente", parameters);

        DataRow[] rows = DataTableToRows(ds);

        return Converter(rows);
    }
    private Cliente[] Converter(DataRow[] rows)
    {
        Cliente[] clientes = new Cliente[rows.Length];

        for (int i = 0; i < rows.Length; i++)
        {
            DataRow row = rows[i];

            int id = row.Field<int>(nameof(Cliente.Id));
            string nome = row.Field<string>(nameof(Cliente.Nome)) ?? string.Empty;
            string email = row.Field<string>(nameof(Cliente.Email)) ?? string.Empty;
            string cpfOuCnpj = row.Field<string>(nameof(Cliente.CpfOuCnpj)) ?? "00000000000";

            clientes[i] = new(id, nome, email, cpfOuCnpj);
        }

        return clientes;
    }
}