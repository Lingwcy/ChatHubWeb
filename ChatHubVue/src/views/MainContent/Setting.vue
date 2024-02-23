<script lang="ts" setup>
import { UseUserInformationStore } from '../../store/index'

import {
    Document,
    Menu as IconMenu,
    Location,
    Setting,
  } from '@element-plus/icons-vue'
  
document.body.style.overflow="scroll"
const UserInfo = UseUserInformationStore()

  const handleOpen = (key: string, keyPath: string[]) => {
    console.log(key, keyPath)
  }
  const handleClose = (key: string, keyPath: string[]) => {
    console.log(key, keyPath)
  }

</script>
<template>
    <div id="setting-bg">
        <div id="Setting-Container">
        <div id="Setting-Container-header">
            <div id="Setting-Container-header-center">
                <img :src="'../../../src/images/systemHeader/'+UserInfo.userImg" class="avater" alt="">
                <div id="Setting-Container-header-center-font">
                    <strong v-if="UserInfo.UserDetailInfo.NickName">{{ UserInfo.UserDetailInfo.NickName }}</strong>
                    <strong v-else>{{UserInfo.userName}}</strong>
                    <i>你的个人账户</i>
                </div>
            </div>
            <div id="Setting-Container-header-footer">
                <button style="color: darkgrey;">返回聊天室</button>
            </div>
        </div>

        <div id="Setting-Container-center">
            <div id="setting-main">
                <div id="setting-main-nav">

                    <el-menu active-text-color="gold" class="el-menu-vertical-demo"
                        default-active="2" text-color="black" router @open="handleOpen" @close="handleClose" style="height: 700px;">
                        <el-menu-item index="/Setting/User">
                            <template #title>
                                <el-icon>
                                    <location />
                                </el-icon>
                                <span>个人信息</span>
                            </template>
                        </el-menu-item>
                        <el-menu-item index="/Setting/Local">
                            <el-icon><icon-menu /></el-icon>
                            <span>本地存储</span>
                        </el-menu-item>
                        <el-menu-item index="3" disabled>
                            <el-icon>
                                <document />
                            </el-icon>
                            <span>隐私</span>
                        </el-menu-item>
                        <el-menu-item index="4">
                            <el-icon>
                                <setting />
                            </el-icon>
                            <span>系统设置</span>
                        </el-menu-item>
                    </el-menu>
                </div>
                <div id="setting-main-main">
                    <router-view></router-view>
                </div>
            </div>
        </div>
    </div>
    </div>

</template>
<style>
#setting-bg{
    width: 100%;
    justify-content: center;
    display: flex;
    background-color: white;
}
#Setting-Container {
    width: 80%;
    margin-top: 20px;
}

#Setting-Container-header {
    width: 100%;
    height: 60px;
    display: flex;
    justify-content: space-between;
    margin-bottom: 10px;
}

#Setting-Container-header-center {
    width: 180px;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
}

.avater {
    width: 50px;
    height: 50px;
    border-radius: 50px;
    margin-right: 15px;
}

#Setting-Container-header-center strong {
    font-size: 20px;
    line-height: 1.3;
}

#Setting-Container-header-center i {
    font-size: 12px;
}

#Setting-Container-header-center-font {
    display: flex;
    flex-direction: column;
}

#Setting-Container-header-footer {
    width: 200px;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
}

#Setting-Container-center {
    width: 100%;
}

#setting-main{
    width: 100%;
    display: flex;
}
#setting-main-nav{
    width: 200px;
    height: 100vh;
    margin-right: 30px;
}
#setting-main-main{
    width: 100%;
}


</style>
