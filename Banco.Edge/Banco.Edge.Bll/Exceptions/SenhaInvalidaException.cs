using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Bll.Exceptions;

[Serializable]
public class SenhaInvalidaException : Exception
{
    public SenhaInvalidaException() : base("A senha informada não confere a senha registrada!") { }
    protected SenhaInvalidaException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}