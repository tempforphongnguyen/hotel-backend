using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enum
{
    public enum ResponseEnum
    {
        Success = 0,
        EmailAlreadyExisted = 1,
        IncorrectResetPwdCode = 2,
        SomewhereWrong = 3,
        InvalidRequest = 4,
        InvaildUserOrRole = 5,
        UserVaildRole = 6,
        IncorrectCurrentPassword = 7,
        NameIsExisted = 8,
        EmailNotFound =9,
        RoomNotAvailable = 10,

    }
}
