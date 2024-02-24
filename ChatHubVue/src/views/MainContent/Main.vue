<script setup lang="ts">
import { ElLoading } from 'element-plus'
import { onBeforeMount } from 'vue'
import { UseServiceStore, UseUserInformationStore, UseMsgStore,appsetting, UseChatStore, UseMsgbox } from '../../store';
import { ChatHub } from '../../services/HubService';
import { Message } from '../../services/MessageService';
import navVue from '../Topbar/nav.vue';


const userInfoStore = UseUserInformationStore();
const msgStore = UseMsgStore();
const chatStore = UseChatStore();
const msgboxStore = UseMsgbox();
const service = UseServiceStore();
const appset = appsetting();
onBeforeMount(function () {
  loadingInstance.close()
  //判断出data则为老用户，在登录之前加载服务
  const data = localStorage.getItem('服务数据');
  if (data != null) {
    service.ChatHub = new ChatHub(localStorage.getItem('token') as string
      , chatStore, msgStore, userInfoStore, msgboxStore)
    service.Message = new Message(msgboxStore);
    if (service.ChatHub.HubConnection.state != 'Connected') {
      service.ChatHub.startHub();
    }
  }
})
//配置loading
const mainLoading = {
  text: "加载中...请稍后"
}
const loadingInstance = ElLoading.service(mainLoading)

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
  height: 70px;
  justify-content: center;
}

#nav {
  width: 100%;
  height: 70px;
}
</style>
