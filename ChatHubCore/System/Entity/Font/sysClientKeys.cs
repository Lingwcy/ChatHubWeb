using SqlSugar;
using System.ComponentModel;

namespace ChatHubApi.System.Entity.Font
{
    /// <summary>
    /// 密钥对[前台]
    /// </summary>
    [SugarTable("t_clientKeys")]
    [Description("密钥表")]
    public class sysClientKeys
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Identifier { get; set; }
        public string Key { get; set; }
    }
}
