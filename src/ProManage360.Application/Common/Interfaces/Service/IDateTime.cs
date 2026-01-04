using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManage360.Application.Common.Interfaces.Service
{
    public interface IDateTime
    {
        public DateTime UtcNow { get; }
    }
}
