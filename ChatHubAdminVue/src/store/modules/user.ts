import { defineStore } from "pinia";
import { store } from "@/store";
import type { userType } from "./types";
import { routerArrays } from "@/layout/types";
import { router, resetRouter } from "@/router";
import { storageLocal } from "@pureadmin/utils";
import { getLogin, refreshTokenApi, getUser, addUser, deletesUser } from "@/api/user";
import { UserResult, RefreshTokenResult, AllUserResult, getOlineUser, getFriends, getFriendRequests, getGroups, deleteUser } from "@/api/user";
import { useMultiTagsStoreHook } from "@/store/modules/multiTags";
import { type DataInfo, setToken, removeToken, userKey } from "@/utils/auth";

export const useUserStore = defineStore({
  id: "pure-user",
  state: (): userType => ({
    // 用户名
    username: storageLocal().getItem<DataInfo<number>>(userKey)?.username ?? "",
    // 页面级别权限
    roles: storageLocal().getItem<DataInfo<number>>(userKey)?.roles ?? [],
    // 是否勾选了登录页的免登录
    isRemembered: false,
    // 登录页的免登录存储几天，默认7天
    loginDay: 7
  }),
  actions: {
    /** 存储用户名 */
    SET_USERNAME(username: string) {
      this.username = username;
    },
    /** 存储角色 */
    SET_ROLES(roles: Array<string>) {
      this.roles = roles;
    },
    /** 存储是否勾选了登录页的免登录 */
    SET_ISREMEMBERED(bool: boolean) {
      this.isRemembered = bool;
    },
    /** 设置登录页的免登录存储几天 */
    SET_LOGINDAY(value: number) {
      this.loginDay = Number(value);
    },
    /** 登入 */
    async loginByUsername(data) {
      return new Promise<UserResult>((resolve, reject) => {
        getLogin(data)
          .then(data => {
            if (data) {
              setToken(data.data);
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 前端登出（不调用接口） */
    logOut() {
      this.username = "";
      this.roles = [];
      removeToken();
      useMultiTagsStoreHook().handleTags("equal", [...routerArrays]);
      resetRouter();
      router.push("/login");
    },
    /** 刷新`token` */
    async handRefreshToken(data) {
      return new Promise<RefreshTokenResult>((resolve, reject) => {
        refreshTokenApi(data)
          .then(data => {
            if (data) {
              setToken(data.data);
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 获取所有用户 */
    async getAllUsers(params?) {
      return new Promise<AllUserResult>((resolve, reject) => {
        getUser(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 删除用户 */
    async postDeleteUser(params?) {
      return new Promise<AllUserResult>((resolve, reject) => {
        deleteUser(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 批量删除用户 */
    async postDeletesUser(params?) {
      return new Promise<AllUserResult>((resolve, reject) => {
        deletesUser(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 获取群组 */
    async getGroup(params?) {
      return new Promise<AllUserResult>((resolve, reject) => {
        getGroups(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 获取所有好友关系 */
    async getAllFriends(params?) {
      return new Promise<AllUserResult>((resolve, reject) => {
        getFriends(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 获取所有在线用户 */
    async getAllOnlineUsers(params?) {
      return new Promise<any>((resolve, reject) => {
        getOlineUser(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 获取所有用户请求 */
    async getAllFriendRequests(params?) {
      return new Promise<any>((resolve, reject) => {
        getFriendRequests(params)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 添加用户 */
    async addUserByAdmin(data) {
      return new Promise<AllUserResult>((resolve, reject) => {
        addUser(data)
          .then(data => {
            if (data) {
              resolve(data);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
  }
});

export function useUserStoreHook() {
  return useUserStore(store);
}
