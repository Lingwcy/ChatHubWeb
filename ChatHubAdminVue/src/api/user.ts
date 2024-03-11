import { http } from "@/utils/http";
import { baseUrlApi } from "./untils";

export type UserResult = {
  success: boolean;
  data: {
    /** 用户名 */
    username: string;
    /** 当前登陆用户的角色 */
    roles: Array<string>;
    /** `token` */
    accessToken: string;
    /** 用于调用刷新`accessToken`的接口时所需的`token` */
    refreshToken: string;
    /** `accessToken`的过期时间（格式'xxxx/xx/xx xx:xx:xx'） */
    expires: Date;
  };
};

export type RefreshTokenResult = {
  success: boolean;
  data: {
    /** `token` */
    accessToken: string;
    /** 用于调用刷新`accessToken`的接口时所需的`token` */
    refreshToken: string;
    /** `accessToken`的过期时间（格式'xxxx/xx/xx xx:xx:xx'） */
    expires: Date;
  };
};
export type AllUserResult = {
  code: number,
  message: string,
  time: string,
  data: string
};
export type AllUserResultData = [
  {
    id: number,
    Passworld: number,
    Username: string,
    HeaderImg: string,
    Email: string,
    City: string,
    Sex: string,
    Age: number,
    Job: string,
    Phone: string,
    NickName: string,
    Birth: string,
    Desc: string,
    status: any
  }
]
/** 登录 */
export const getLogin = (data?: object) => {
  return http.request<UserResult>("post", "/login", { data });
};

/** 刷新token */
export const refreshTokenApi = (data?: object) => {
  return http.request<RefreshTokenResult>("post", "/refresh-token", { data });
};

/** 获取所有用户 */
export const getUser = (params?: object) => {
  return http.request<AllUserResult>("get", baseUrlApi("/User/GetAll"), { params });
};

/** 新增用户 */

export const addUser = (data: object) => {
  return http.request<any>("post", baseUrlApi("/User/Add"), { data })
};