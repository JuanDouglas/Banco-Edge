using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Bll.Exceptions
{
    [Serializable]
    public class EmUsoException : Exception
    {
        public string Field { get; set; }
        public EmUsoException(string fieldName) : base($"O campo '{fieldName}' já está em uso!")
        {
            Field = fieldName;
        }
        protected EmUsoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
