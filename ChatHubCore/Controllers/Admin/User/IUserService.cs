using ChatHubApi.Controllers.AdminServices.User.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.User
{
    interface IUserService
    {
        Task<ActionResult<int>> AddUser(AddUserInput input);
    }
}
