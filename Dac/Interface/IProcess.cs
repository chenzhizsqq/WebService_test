using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Entity;
using DacBase;

namespace Dac.Interface
{
    public interface IProcess : INpgsqlDacBase
    {
        int InsertProcess(userEntity entity);
    }
}
