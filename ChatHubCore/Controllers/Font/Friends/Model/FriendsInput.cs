using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace construct.Application.System.FontServices.Friends.Model
{
    public class FriendsInput
    {
    }

    public record sendRequestModel (string targetName, string userName, string ReqMsg, string mark,int TargetGroupId);
    public record sendRedBobModel (string username, string targetname);
}
