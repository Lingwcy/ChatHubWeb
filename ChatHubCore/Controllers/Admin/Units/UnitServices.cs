
using System.Text;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using ChatHubApi.Controllers.AdminServices.Units.Model;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using ChatHubApi.System.Entity;

namespace ChatHubApi.Controllers.AdminServices.Units
{/// <summary>
 /// 单位服务
 /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class UnitServices : ControllerBase
    {
        private readonly ISqlSugarClient db;
        public UnitServices(ISqlSugarClient db)
        {
            this.db = db;
        }



        /// <summary>
        /// 构建机构树       
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult<object> GetInstitutionTree()
        {
            StringBuilder sb = new StringBuilder();
            //获取该机构下的所有单位
            var units = db.Queryable<sysUnits>().Where(a => a.institution == "武汉城市公共基础设施运营发展有限公司")
                .GroupBy(a => a.unit).Select(a => a.unit).ToList();

            List<Tree> rootchildrens = new List<Tree>();
            foreach (var unit in units)
            {
                var departments = db.Queryable<sysUnits>().Where(a => a.unit == unit).Select(a => a.department).ToList();
                if (departments.Count == 0)
                {
                    rootchildrens.Add(new Tree()
                    {
                        label = unit,
                    });
                    continue;
                }
                List<Tree> childrens = new List<Tree>();
                foreach (var dep in departments)
                {
                    Tree tree = new Tree() { label = dep };
                    childrens.Add(tree);
                }
                Tree tree1 = new Tree()
                {
                    label = unit,
                    children = childrens.ToArray(),
                };
                childrens = new List<Tree>();
                rootchildrens.Add(tree1);
            }

            Tree root = new Tree()
            {
                label = "武汉城市公共基础设施运营发展有限公司",
                children = rootchildrens.ToArray(),
            };

            var res = JsonSerializer.Serialize(root, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            return res;
        }



        /// <summary>
        /// 返回机构查询列表       
        /// </summary>
        /// <param name="labelName">机构名称</param>
        /// <returns></returns>
        ///  
        [HttpPost]
        public async Task<ActionResult<object>> GetUnitsTable(string labelName)
        {
            StringBuilder json = new StringBuilder();
            var ins = await db.Queryable<sysUnits>().AnyAsync(a => a.institution == labelName);
            //如果没有此机构
            if (!ins)
            {
                var units = await db.Queryable<sysUnits>().AnyAsync(a => a.unit == labelName);
                if (!units)
                {
                    var develop = await db.Queryable<sysUnits>().SingleAsync(a => a.department == labelName);
                    if (develop == null)
                    {
                        return "nodata";
                    }
                    var res = JsonSerializer.Serialize(new
                    {
                        name = develop.unit,
                        develop.createtime,
                        develop.id
                    }, new JsonSerializerOptions()
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    });
                    return res;
                }
                else
                {
                    //返回该单位的所有部门
                    var develop = db.Queryable<sysUnits>().Where(a => a.unit == labelName).Select(a => new
                    {
                        name = a.department,
                        a.createtime,
                        a.id
                    }).ToList();
                    if (develop.Count == 1)
                    {
                        sysUnits res = await db.Queryable<sysUnits>().SingleAsync(a => a.unit == labelName);

                        return JsonSerializer.Serialize(new
                        {
                            name = res.unit,
                            res.createtime,
                            res.id
                        }, new JsonSerializerOptions()
                        {
                            // 整齐打印
                            //WriteIndented = true,
                            //重新编码，解决中文乱码问题
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                        });
                    }
                    foreach (var items in develop)
                    {
                        json.Append(JsonSerializer.Serialize(items, new JsonSerializerOptions()
                        {
                            // 整齐打印
                            //WriteIndented = true,
                            //重新编码，解决中文乱码问题
                            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                        }) + "$");
                    }

                    return json.ToString();
                }
            }
            else
            {
                //返回该机构下的所有单位
                var units = db.Queryable<sysUnits>().Where(a => a.institution == labelName).GroupBy(a => a.unit).Select(a => new
                {
                    name = a.unit,
                    a.createtime,
                    a.id
                }).ToList();
                foreach (var items in units)
                {
                    json.Append(JsonSerializer.Serialize(items, new JsonSerializerOptions()
                    {
                        // 整齐打印
                        //WriteIndented = true,
                        //重新编码，解决中文乱码问题
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    }) + "$");

                }

                return json.ToString();
            }
        }
    }
}
