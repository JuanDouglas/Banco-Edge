using Banco.Edge.Dml;
using System.Data;
using System.Data.SqlClient;

namespace Banco.Edge.Dal.Clientes;

public sealed class DaoCliente : DaoBase
{
    public async Task<Cliente?> ExisteAsync(string? email, string? cpfOuCnpj, bool ocultarSensiveis = true)
    {
        if (string.IsNullOrEmpty(cpfOuCnpj) &&
            string.IsNullOrEmpty(email))
            throw new ArgumentNullException(nameof(cpfOuCnpj), "Ao menos um parametro deve conter valor!");

        Cliente[] clientes = await ExecutarBuscaAsync(email: email, cpfOuCnpj: cpfOuCnpj, ocultarSensiveis: ocultarSensiveis);

        return clientes.Length < 1 ? null : clientes[0];
    }

    public async Task ExcluirAsync(int id)
    {
        List<SqlParameter> parametros = new()
        {
            new SqlParameter("IdCliente", id)
        };

        await ExecuteQueryAsync("ExcluirCliente", parametros, true);
    }

    public async Task<int> InserirCliente(Cliente cliente)
    {
        List<SqlParameter> parameters = new()
        {
            new SqlParameter(nameof(Cliente.Nome), cliente.Nome),
            new SqlParameter(nameof(Cliente.Email), cliente.Email),
            new SqlParameter(nameof(Cliente.Senha), cliente.Senha),
            new SqlParameter(nameof(Cliente.Chave), cliente.Chave),
            new SqlParameter(nameof(Cliente.Telefone), cliente.Telefone),
            new SqlParameter(nameof(Cliente.CpfOuCnpj), cliente.CpfOuCnpj)
        };

        DataSet dbSet = await ExecuteQueryAsync("InserirCliente", parameters, true);
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
        int skip = 0,
        bool ocultarSensiveis = true)
    {
        List<SqlParameter> parameters = new()
        {
            new("Skip", skip),
            new("Take", take),
            new(nameof(Cliente.Id), id),
            new(nameof(Cliente.Email), email),
            new(nameof(Cliente.CpfOuCnpj), cpfOuCnpj)
        };

        DataSet ds = await ExecuteQueryAsync("BuscaCliente", parameters);

        DataRow[] rows = DataTableToRows(ds);

        return Converter(rows, ocultarSensiveis);
    }
    private Cliente[] Converter(DataRow[] rows, bool ocultarSensiveis)
    {
        Cliente[] clientes = new Cliente[rows.Length];

        for (int i = 0; i < rows.Length; i++)
        {
            DataRow row = rows[i];

            int id = row.Field<int>(nameof(Cliente.Id));
            string nome = row.Field<string>(nameof(Cliente.Nome)) ?? string.Empty;
            string email = row.Field<string>(nameof(Cliente.Email)) ?? string.Empty;
            string cpfOuCnpj = row.Field<string>(nameof(Cliente.CpfOuCnpj)) ?? "00000000000";
            string telefone = row.Field<string>(nameof(Cliente.Telefone)) ?? "+00 (00) 00000-0000";
            string chave = row.Field<string>(nameof(Cliente.Chave)) ?? string.Empty;
            string senha = row.Field<string>(nameof(Cliente.Senha)) ?? string.Empty;

            clientes[i] = new(id, nome, telefone, email, cpfOuCnpj, senha, chave, ocultarSensiveis);
        }

        return clientes;
    }
}