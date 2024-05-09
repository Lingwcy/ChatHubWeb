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
  return http.request<AllUserResult>("get", baseUrlApi("/admin/User/GetAll"), { params });
};

/** 新增用户 */
export const addUser = (data: object) => {
  return http.request<any>("post", baseUrlApi("/admin/User/Add"), { data })
};

/** 删除用户 */
export const deleteUser = (params: object) => {
  return http.request<any>("post", baseUrlApi("/admin/User/Delete"), { params })
};

/** 批量删除用户 */
export const deletesUser = (params: object) => {
  return http.request<any>("post", baseUrlApi("/admin/User/Deletes"), { params })
};

/** 获取所有在线用户 */
export const getOlineUser = (params?: object) => {
  return http.request<any>("get", baseUrlApi("/admin/OnlineUser/GetAll"), { params });
};

/** 获取所有好友关系 */
export const getFriends = (params?: object) => {
  return http.request<any>("get", baseUrlApi("/admin/FriendAdmin/GetAll"), { params });
};

/** 获取所有好友请求 */
export const getFriendRequests = (params?: object) => {
  return http.request<any>("get", baseUrlApi("/admin/FriendAdmin/GetAllRequests"), { params });
};

/** 获取所有群组 */
export const getGroups = (params?: object) => {
  return http.request<any>("get", baseUrlApi("/admin/GroupAdmin/GetAll"), { params });
};