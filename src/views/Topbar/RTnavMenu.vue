<script setup lang="ts">

import { ArrowDown } from '@element-plus/icons-vue'
import {HTMLElement} from '../../global_env'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {UseUserInformationStore} from '../../store/index'
const router =useRouter()
const userinfo = UseUserInformationStore()


const pushHub =()=>{
    router.push({path:'/Hub/Friends'})
}

const bgValue = ref(true)
let changeBg= function(){
    var a:HTMLElement = document.getElementById("themeSwitch") as any
    if(bgValue.value){
        a.href ="src/theme/default.css"
        document.body.style.backgroundImage = "url(./src//images/bg.jpg)";
    }else{
        a.href ="src/theme/dark.css"
        document.body.style.backgroundImage = "url(./src//images/bg2.jpg)";

    }
}

</script>
<template>
    <nav id="Container">
        <div class="eleBlock">
            <el-dropdown>
                <span class="el-dropdown-link">
                    <span class="el-font">关于</span>
                    <el-icon class="el-icon--right">
                        <arrow-down />
                    </el-icon>
                </span>
                <template #dropdown>
                    <el-dropdown-menu>
                        <el-dropdown-item>创建频道</el-dropdown-item>
                        <el-dropdown-item>好友添加</el-dropdown-item>
                        <el-dropdown-item>行为规范</el-dropdown-item>
                        <el-dropdown-item disabled>区块链</el-dropdown-item>
                        <el-dropdown-item divided>关于我们</el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>

        <div class="eleBlock">
            <el-dropdown>
                <span class="el-dropdown-link">
                    <span class="el-font">更新日志</span>
                    <el-icon class="el-icon--right">
                        <arrow-down />
                    </el-icon>
                </span>
                <template #dropdown>
                    <el-dropdown-menu>
                        <el-dropdown-item>最新日志</el-dropdown-item>
                        <el-dropdown-item divided>全部功能</el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>

        <div class="eleBlock">
            <el-dropdown>
                <span class="el-dropdown-link">
                    <span class="el-font">语言选择</span>
                    <el-icon class="el-icon--right">
                        <arrow-down />
                    </el-icon>
                </span>
                <template #dropdown>
                    <el-dropdown-menu>
                        <el-dropdown-item>简体中文</el-dropdown-item>
                        <el-dropdown-item disabled>English</el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>


        <div class="eleBlock">
            <el-button  plain id="logOut" v-on:click="pushHub">
                
                <strong id="logOut-font" v-if="userinfo.UserDetailInfo.NickName">{{ userinfo.UserDetailInfo.NickName }}</strong>
                <strong  id="logOut-font" v-else>{{userinfo.userName}}</strong>
          
            </el-button>
            
        </div>


        <div class="eleBlock">
            <el-switch
                v-model="bgValue"
                class="ml-2"
                style="--el-switch-on-color: #c8c9cc; --el-switch-off-color:#303133"
                v-on:click="changeBg" 
                id="themeSwitcher"              
            />
            <svg t="1666957084178" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="4627" width="40" height="50"><path d="M344.189719 297.542353l-57.889397-57.889397-48.231443 48.232466 57.889397 57.889397L344.189719 297.542353zM254.129654 480.812217l-96.462886 0L157.666768 545.103411l96.462886 0L254.129654 480.812217zM543.518311 162.503932l-64.291194 0 0 93.214915 64.291194 0L543.518311 162.503932zM784.677572 287.885422l-48.231443-48.232466-57.89042 57.889397 45.031568 45.027474L784.677572 287.885422zM678.555709 728.42137l57.89042 57.841302 45.07557-44.982449-57.934423-57.885304L678.555709 728.42137zM768.614751 545.103411l96.464932 0 0-64.291194-96.464932 0L768.614751 545.103411zM511.397785 320.009018c-106.116747 0-192.926795 86.855073-192.926795 192.927818 0 106.113677 86.810048 192.923725 192.926795 192.923725 106.11777 0 192.923725-86.810048 192.923725-192.923725C704.32151 406.864091 617.515555 320.009018 511.397785 320.009018M479.227117 863.459791l64.291194 0 0-93.259941-64.291194 0L479.227117 863.459791zM238.068879 738.030205l48.231443 48.231443 57.889397-57.841302-44.982449-45.027474L238.068879 738.030205z" p-id="4628" fill="#8a8a8a"></path></svg>
        </div>
        

    </nav>
</template>
<style>

    #Container{
        width: 100%;
        height: 100%;
        display: flex;
        justify-content: flex-start;
    }
    
    .example-showcase .el-dropdown-link {
        color: var(--el-color-primary);
        display: flex;
        align-items: center;
    }

    .eleBlock{
        width: 25%;
        align-items: center;
        display: flex;
        justify-content: center;
        cursor: pointer;
    }
    .el-font{
       
        font-size: 14px;
    }
    #logOut{
        font-size: 13px;
        width: 60px;
        height: 30px;
        background-color: #909399;
        border: 1px solid #606266;
        
    }
    #logOut-font{
        width: 90%;
        height: 80%;
        margin-bottom: 4px;
    }
</style>
