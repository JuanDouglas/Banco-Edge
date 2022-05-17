using Banco.Edge.Bll;
using Banco.Edge.Bll.Base;
using Banco.Edge.Dml;
using Banco.Edge.Dml.Enums;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banco.Edge.Test
{
    public class TesteCliente
    {
        BoCliente boCliente;
        Cliente cliente;
        static string senha = "S3nh@D3Acess0";
        [SetUp]
        public async Task Setup()
        {
            cliente = new(0,
                "Juan Douglas",
                "(61) 99260-6441",
                "juandouglas2004@gmail.com",
                "43.647.914/0001-51",
                senha,
                BoBase.GerarToken(96));

            cliente = await BoCliente.BuscarAsync(cliente.Email, false) ?? cliente;

            boCliente = new(cliente);

            if (cliente.Id > 0)
            {
                await boCliente.ExcluirAsync(senha);
            }
        }

        [Test]
        public async Task CriarContaValidaAsync()
        {
            cliente.Id = await BoCliente.CadastroAsync(cliente);

            cliente = await BoCliente.BuscarAsync(cliente.Email);

            Assert.IsNotNull(cliente);

            Assert.AreNotEqual(cliente.Id, 0);
        }

        [Test]
        public async Task CriarContaExistenteAsync()
        {

        }

        [Test]
        public async Task CriarContaAsync()
        {
            await boCliente.ObterContasAsync();

            int nContas = (cliente.Contas ?? new List<Conta>()).Count;

            await boCliente.CriarContaAsync(TipoConta.Poupanca);
            await boCliente.ObterContasAsync();

            Assert.IsTrue((cliente.Contas ?? new List<Conta>()).Count > nContas);
        }
    }
}