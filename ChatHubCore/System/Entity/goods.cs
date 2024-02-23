using SqlSugar;

namespace ChatHubApi.System.Entity
{
    [SugarTable("goods")]
    public class goods
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public string gno { get; set; }

        [SugarColumn(ColumnName = "gname")]
        public string gname { get; set; }
        public float price { get; set; }
        public string producer { get; set; }

    }
}
