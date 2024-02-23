<script setup lang="ts">
import { ElLoading } from 'element-plus'
import { onBeforeMount } from 'vue'
import { UseServiceStore, UseUserInformationStore, UseMsgStore, UseChatStore, UseMsgbox } from '../../store';
import { ChatHub } from '../../services/HubService';
import { Message } from '../../services/MessageService';
import navVue from '../Topbar/nav.vue';
onBeforeMount(function () {
  const userInfoStore = UseUserInformationStore();
  const msgStore = UseMsgStore();
  const chatStore = UseChatStore();
  const msgboxStore = UseMsgbox();
  const service = UseServiceStore();
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
    <div id="NavBar">
      <nav-vue id="nav"></nav-vue>
    </div>
    <router-view></router-view>
  </div>
</template>
<style scoped>
#mainApp {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}
#NavBar{
  width: 100%;
  background-color: black;
  display: flex;
  justify-content: center;
}
#nav{
  width: 96%;
  height: 70px;
}
</style>
