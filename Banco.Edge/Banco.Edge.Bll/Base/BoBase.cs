using System.Data.SqlClient;

namespace Banco.Edge.Bll.Base;

public abstract class BoBase : IDisposable
{

    /// <summary>
    /// Gera um token com o tamanho especificado.
    /// </summary>
    /// <param name="tamanho">Tamanho do token</param>
    /// <param name="usarMaisculos">Usa caracters minusculos.</param>
    /// <param name="usarMinusculos">Usa caracters maisculos.</param>
    /// <returns>String contendo token gerado.</returns>
    public static string GerarToken(int tamanho, bool usarMinusculos = true, bool usarMaisculos = true)
    {
        // ASCII characters rangers
        byte[] minusculos = new byte[] { 97, 123 };
        // Letras minusculas
        byte[] maisculos = new byte[] { 65, 91 };
        // ASCII numeros
        byte[] numeros = new byte[] { 48, 58 };

        Random random = new();
        string resultado = string.Empty;

        for (int i = 0; i < tamanho; i++)
        {
            int tipo = random.Next(0, usarMaisculos ? 3 : 2);

            byte[] possiveis = tipo switch
            {
                1 => usarMinusculos ? maisculos : numeros,
                2 => minusculos,
                _ => numeros
            };

            int gerado = random.Next(possiveis[0], possiveis[1]);
            char caracter = (char)gerado;

            resultado += caracter;
        }

        return resultado;
    }

    public abstract void Dispose();
}