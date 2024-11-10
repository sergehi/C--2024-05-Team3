using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksService.DataAccess.Entities
{
    public enum ETaskModifyFlags : long
    {
        TMF_NONE = 0,
        TMF_MODIFY_NAME = 1,
        TMF_MODIFY_DESCRIPTION = 2,
        TMF_MODIFY_DEADLINE = 4,
        TMF_MODIFY_TYPE = 8,
        TMF_MODIFY_URGENCY = 16,
        TMF_MODIFY_STATE = 32,
        TMF_REC_CREATE = 64,
        TMF_REC_DELETE = 128,
        TMF_REC_DOERS = 256
    }
}
