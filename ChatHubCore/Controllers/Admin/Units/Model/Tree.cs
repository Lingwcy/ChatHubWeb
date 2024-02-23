using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.Units.Model
{
    public class Tree
    {
        public string label { get; set; }
        public Tree[] children { get; set; }

    }
}
