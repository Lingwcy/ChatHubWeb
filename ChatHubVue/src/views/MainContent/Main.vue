<script setup lang="ts">
import { ElLoading, ElNotification } from 'element-plus'
import { onBeforeMount, watch } from 'vue'
import { UseServiceStore, UseUserInformationStore, UseMsgStore, appsetting, UseChatStore, UseMsgbox, UseGroupStore,UseFriendsStore } from '../../store';
import { ChatHub } from '../../services/HubService';
import { Auth } from '../../services/AuthService';
import { Message } from '../../services/MessageService';
import navVue from '../Topbar/nav.vue';

import "../../styles/index.scss"
import router from '../../router';
const userInfoStore = UseUserInformationStore();
const msgStore = UseMsgStore();
const chatStore = UseChatStore();
const msgboxStore = UseMsgbox();
const service = UseServiceStore();
const groupStore = UseGroupStore();
const appset = appsetting();
const friendsStore = UseFriendsStore();


onBeforeMount(function () {
  loadingInstance.close()
  //判断出data则为老用户，在登录之前加载服务
  //const data = localStorage.getItem('服务数据');
  service.Auth = new Auth();
  service.Auth?.SendAESKey();
  if (service.ChatHub?.IsLogin) {
    service.ChatHub = new ChatHub(localStorage.getItem('token') as string
      , chatStore, msgStore, userInfoStore, msgboxStore, groupStore, appset,friendsStore )
    service.Message = new Message(msgboxStore);
    if (service.ChatHub.HubConnection.state != 'Connected') {
      service.ChatHub.startHub();
    }
  }else{
    router.push('/')
    ElNotification({
        title: '连接断开',
        message: '服务器连接断开，即将跳转到登录界面',
        type: 'error'
      });
  }
})
//配置loading
const mainLoading = {
  text: "加载中...请稍后"
}
const loadingInstance = ElLoading.service(mainLoading)


watch(
  () => service.ChatHub?.HubConnection.state,
  (newVal, oldVal) => {
    console.log('连接状态改变', newVal, oldVal);
    if (newVal == 'Disconnected') {
      //重定向到登录页面并刷新整个页面
      service.ChatHub!.IsLogin = false;
      ElNotification({
        title: '连接断开',
        message: '服务器连接断开，即将跳转到登录界面',
        type: 'error'
      });
      setTimeout(() => {
        window.location.href = '/';
      }, 1000);
    }
  }
); 
</script>
<template>
  <div id="mainApp">
    <el-container direction="vertical">
      <el-collapse-transition>
        <nav-vue v-show="appset.NavBarShow" id="nav"></nav-vue>
      </el-collapse-transition>
      <router-view></router-view>
    </el-container>
  </div>
</template>
<style scoped>
#mainApp {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

#NavBar {
  width: 100%;
  background-color: black;
  display: flex;
  justify-content: center;
}

#nav {
  width: 100%;
  height: 60px;
}
</style>
