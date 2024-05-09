
export default {
  path: "/hubData",
  meta: {
    title: "系统管理"
  },
  children: [
    {
      path: "/hubData/User",
      name: "HubData",
      component: () => import("@/views/hubData/user/user.vue"),
      meta: {
        title: "用户管理"
      }
    },
    {
      path: "/hubData/onlineUser",
      name: "OnlineUser",
      component: () => import("@/views/hubData/onlineUser/onlineUser.vue"),
      meta: {
        title: "在线用户管理"
      }
    },
    {
      path: "/hubData/friends",
      name: "Friends",
      component: () => import("@/views/hubData/friends/friends.vue"),
      meta: {
        title: "好友关系管理"
      }
    },
    {
      path: "/hubData/friendRequest",
      name: "FriendsRequest",
      component: () => import("@/views/hubData/friendRequest/friendRequest.vue"),
      meta: {
        title: "好友请求管理"
      }
    },
    {
      path: "/hubData/group",
      name: "Group",
      component: () => import("@/views/hubData/group/group.vue"),
      meta: {
        title: "群组管理"
      }
    }
  ]
};
