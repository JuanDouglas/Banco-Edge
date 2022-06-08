using Banco.Edge.Dml;
using System.Collections.Generic;
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

    public async Task<Cliente[]?> ListarClientes(int idInicial, int maximo = 200)
    {
        SqlParameter[] parametros = {
            new SqlParameter("IdInicial", idInicial),
            new SqlParameter("Maximo", maximo)
        };

        DataSet dbSet = await ExecuteQueryAsync("ListarClientes", parametros, true);
        DataRowCollection rows = dbSet.Tables[0].Rows;

        DataRow[] dataRows = new DataRow[rows.Count];
        rows.CopyTo(dataRows, 0);

        return Converter(dataRows, true);
    }

    public async Task ExcluirAsync(int id)
    {
        SqlParameter[] parametros = {
            new SqlParameter("IdCliente", id)
        };

        await ExecuteQueryAsync("ExcluirCliente", parametros, true);
    }

    public async Task<int> InserirCliente(Cliente cliente)
    {
        SqlParameter[] parametros = {
            new(nameof(Cliente.Nome),SqlDbType.VarChar,100, cliente.Nome){ Value = cliente.Nome },
            new(nameof(Cliente.Email),SqlDbType.VarChar,500, cliente.Email){ Value = cliente.Email },
            new(nameof(Cliente.Senha),SqlDbType.VarChar,90, cliente.Senha){ Value = cliente.Senha },
            new(nameof(Cliente.Chave),SqlDbType.VarChar,96, cliente.Chave) { Value = cliente.Chave },
            new(nameof(Cliente.Telefone),SqlDbType.VarChar,15, cliente.Telefone) { Value = cliente.Telefone },
            new(nameof(Cliente.CpfOuCnpj),SqlDbType.VarChar,20, cliente.CpfOuCnpj){ Value = cliente.CpfOuCnpj },
            new(nameof(Cliente.Foto),SqlDbType.VarBinary,cliente.Foto?.Length ?? 0) {Value = cliente.Foto}
        };

        DataSet dbSet = await ExecuteQueryAsync("InserirCliente", parametros, true);
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
        List<SqlParameter> parameters = new(){
            new("Skip", skip),
            new("Take", take),
        };

        if (id != null)
            parameters.Add(new(nameof(Cliente.Id), id ?? 0));

        if (email != null)
            parameters.Add(new(nameof(Cliente.Email), email));

        if (cpfOuCnpj != null)
            parameters.Add(new(nameof(Cliente.CpfOuCnpj), cpfOuCnpj));

        DataSet ds = await ExecuteQueryAsync("BuscaCliente", parameters.ToArray());

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
            byte[]? foto = row.Field<byte[]>(nameof(Cliente.Foto));

            clientes[i] = new(id, nome, telefone, email, cpfOuCnpj, senha, chave, ocultarSensiveis)
            {
                Foto = foto
            };
        }

        return clientes;
    }
}