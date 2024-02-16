<script setup lang="ts">
import { ElLoading } from 'element-plus'
import { onBeforeMount} from 'vue'
onBeforeMount(function(){
  loadingInstance.close()
})
  //配置loading
  const mainLoading = {
    text: "加载中...请稍后"
  }
const loadingInstance = ElLoading.service(mainLoading )

</script>
<template>
    <div id="blank"></div>
    <div id="mainApp">
        <router-view></router-view>
    </div>
</template>
<style>
    #mainApp{
        width: 100%;
        display: flex;
        justify-content:center;
        height: 98.4vh;

    }

</style>
