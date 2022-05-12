using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banco.Edge.Bll.Exceptions
{
    [Serializable]
    public class InUseException : Exception
    {
        public string Field { get; set; }
        public InUseException(string fieldName) : base($"O campo '{fieldName}' já está em uso!")
        {
            Field = fieldName;
        }
        protected InUseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
