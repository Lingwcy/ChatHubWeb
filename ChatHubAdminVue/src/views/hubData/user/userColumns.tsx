import { message } from "@/utils/message";
import { usertableData } from "../data";

export function useColumns() {
  const columns: TableColumnList = [
    {
      label: "ID",
      prop: "id"
    },
    {
      label: "用户名",
      prop: "Username"
    },
    {
      label: "头像",
      prop: "HeaderImg"
    },
    {
      label: "邮箱",
      prop: "Email"
    },
    {
      label: "城市",
      prop: "City"
    },
    {
      label: "年龄",
      prop: "Age"
    },
    {
      label: "工作",
      prop: "Job"
    },
    {
      label: "电话",
      prop: "Phone"
    },
    {
      label: "操作",
      cellRenderer: ({ index, row }) => (
        <>
          <el-button size="small" onClick={() => handleEdit(index + 1, row)}>
            编辑
          </el-button>
          <el-button
            size="small"
            type="danger"
            onClick={() => handleDelete(index + 1, row)}
          >
            删除
          </el-button>
        </>
      )
    }
  ];

  const handleEdit = (index: number, row) => {
    message(`您修改了第 ${index} 行，数据为：${JSON.stringify(row)}`, {
      type: "success"
    });
  };

  const handleDelete = (index: number, row) => {
    message(`您删除了第 ${index} 行，数据为：${JSON.stringify(row)}`);
  };

  return {
    columns,
    usertableData
  };
}
