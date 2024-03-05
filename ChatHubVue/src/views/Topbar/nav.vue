<script setup lang="ts">
import { ref } from 'vue'
import RTnavMenu from './RTnavMenu.vue'
import { useRouter } from 'vue-router'
import logo from '../Compoents/logo.vue'
import '../../../node_modules/element-plus/theme-chalk/display.css'
import { UseTheme } from '../../store'
let themeStore = UseTheme();
const bgValue = ref(true)
let changeBg= function(){
    let a = document.getElementById("themeSwitch") as any
    if(bgValue.value){
        a.href ="src/theme/default.css"
        document.body.style.backgroundImage = "url(./src//images/bg.jpg)";
    }else{
        a.href ="src/theme/dark.css"
        document.body.style.backgroundImage = "url(./src//images/bg2.jpg)";

    }
}
const router = useRouter()
const pushabout = () => {
    router.push({ path: "/About" })
}

const confirmEvent = () => {
    localStorage.clear()
    router.push({ path: "/" })
}
const cancelEvent = () => {
}
</script>
<template>
    <div id="container">
        <el-col :xs="24" :sm="24" :md="0" :lg="0" :xl="0" class="main-header-phoneSize hidden-md-and-up">
            <div class="phoneNavItems">
                <svg t="1672456842548" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg"
                    p-id="2846" width="28" height="28">
                    <path
                        d="M133.310936 296.552327l757.206115 0c19.781623 0 35.950949-16.169326 35.950949-35.950949 0-19.781623-15.997312-35.950949-35.950949-35.950949L133.310936 224.650428c-19.781623 0-35.950949 16.169326-35.950949 35.950949C97.359987 280.383 113.529313 296.552327 133.310936 296.552327z"
                        fill="#575B66" p-id="2847"></path>
                    <path
                        d="M890.51705 476.135058 133.310936 476.135058c-19.781623 0-35.950949 16.169326-35.950949 35.950949 0 19.781623 16.169326 35.950949 35.950949 35.950949l757.206115 0c19.781623 0 35.950949-16.169326 35.950949-35.950949C926.467999 492.304384 910.298673 476.135058 890.51705 476.135058z"
                        fill="#575B66" p-id="2848"></path>
                    <path
                        d="M890.51705 727.447673 133.310936 727.447673c-19.781623 0-35.950949 15.997312-35.950949 35.950949s16.169326 35.950949 35.950949 35.950949l757.206115 0c19.781623 0 35.950949-15.997312 35.950949-35.950949S910.298673 727.447673 890.51705 727.447673z"
                        fill="#575B66" p-id="2849"></path>
                </svg>
            </div>
            <div class="Icon-phone">
                <logo logo-type="NavWindow"></logo>
            </div>
            
            <div class="eleBlock-phone">
            <el-switch
                v-model="bgValue"
                class="ml-2"
                style="--el-switch-on-color: #c8c9cc; --el-switch-off-color: #18191a"
                v-on:click="changeBg" 
                id="themeSwitcher"              
            />
            <svg t="1666957084178" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="4627" width="40" height="50"><path d="M344.189719 297.542353l-57.889397-57.889397-48.231443 48.232466 57.889397 57.889397L344.189719 297.542353zM254.129654 480.812217l-96.462886 0L157.666768 545.103411l96.462886 0L254.129654 480.812217zM543.518311 162.503932l-64.291194 0 0 93.214915 64.291194 0L543.518311 162.503932zM784.677572 287.885422l-48.231443-48.232466-57.89042 57.889397 45.031568 45.027474L784.677572 287.885422zM678.555709 728.42137l57.89042 57.841302 45.07557-44.982449-57.934423-57.885304L678.555709 728.42137zM768.614751 545.103411l96.464932 0 0-64.291194-96.464932 0L768.614751 545.103411zM511.397785 320.009018c-106.116747 0-192.926795 86.855073-192.926795 192.927818 0 106.113677 86.810048 192.923725 192.926795 192.923725 106.11777 0 192.923725-86.810048 192.923725-192.923725C704.32151 406.864091 617.515555 320.009018 511.397785 320.009018M479.227117 863.459791l64.291194 0 0-93.259941-64.291194 0L479.227117 863.459791zM238.068879 738.030205l48.231443 48.231443 57.889397-57.841302-44.982449-45.027474L238.068879 738.030205z" p-id="4628" fill="#8a8a8a"></path></svg>
        </div>
        
        </el-col>
        <el-col :xs="0" :sm="0" :md="24" :lg="24" :xl="24" class="main-header-large hidden-sm-and-down">
            <div class="Icon">
                <logo logo-type="NavWindow"></logo>
            </div>
            <div id="rtNav">
                <RTnavMenu></RTnavMenu>
            </div>
        </el-col>
    </div>
</template>
<style>
#container{
    width: 100%;
}
.phoneNavItems{
    margin-left: 10px;
}
.eleBlock-phone{
    display: flex;
    justify-content: center;
    align-items: center;
}
.main-header-phoneSize {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.main-header-large {
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: space-between;
}


.Icon {
    height: 50px;
    width: 120px;
    margin-left: 15px;
    display: flex;
}
.Icon-phone {
    margin-left: 30px;
    height: 50px;
    width: 120px;
    display: flex;
}



#rtNav {
    height: 100%;
}

#rtNavMenu {
    width: 100%;
    height: 100%;
}

#loginMenu {
    width: 20%;
    height: 100%;
}
</style>
