interface FormItemProps2 {
  id?: number;
  /** 用于判断是`新增`还是`修改` */
  title: string;
  higherDeptOptions: Record<string, unknown>[];
  parentId: number;
  nickname: string;
  username: string;
  password: string;
  phone: string | number;
  email: string;
  sex: string | number;
  status: number;
  dept?: {
    id?: number;
    name?: string;
  };
  remark: string;
}
interface FormItemProps {
  title: string;
  id: string; // 假设ID是字符串类型  
  username: string; // 用户名是字符串类型  
  headerImg: string; // 头像可能是URL字符串  
  email: string; // 邮箱是字符串类型  
  city: string; // 城市是字符串类型  
  age: number; // 年龄是数字类型  
  job: string; // 工作是字符串类型  
  phone: string | number; // 电话是字符串类型  
  status: number;
  nickname: string;
  password: string;
}

export interface DeleteProps {
  id: number
}

export interface DeletesProps {
  ids: number[]
}
interface FormProps {
  formInline: FormItemProps;
}

interface RoleFormItemProps {
  username: string;
  /** 角色列表 */
  roleOptions: any[];
  /** 选中的角色列表 */
  ids: Record<number, unknown>[];
}
interface RoleFormProps {
  formInline: RoleFormItemProps;
}

export type { FormItemProps, FormProps, RoleFormItemProps, RoleFormProps };
