<script lang="ts" setup>
import MessageCT from './HubContent/MessageCT.vue'
import LeftNav from './HubContent/LeftNav.vue'
import { reactive, ref, onBeforeMount } from 'vue';
import Hub from '../../views/PhonePage/Hub.vue'
import { appsetting } from '../../store';
const appset =appsetting();
onBeforeMount(() => {
})
let ChangeLeftNavDetail = reactive({
    width: ref("300px"),
    //minwidth: ref("188px"),
    flag: ref(0)
})
const ChangeLowerLeftNavDetail = function () {
    if (ChangeLeftNavDetail.flag == 0) {
        ChangeLeftNavDetail.width = "0%"
        //ChangeLeftNavDetail.minwidth = "0px"
        ChangeLeftNavDetail.flag = 1
    } else {
        ChangeLeftNavDetail.width = "300px"
       // ChangeLeftNavDetail.minwidth = "188px"
        ChangeLeftNavDetail.flag = 0
    }
}
const ChangeTopNavBar = function(){
    if(appset.NavBarShow){appset.NavBarShow = false}
    else appset.NavBarShow = true

}
document.body.style.overflow = "hidden"
</script>
<template>
    <el-col :xs="24" :sm="24" :md="0" :lg="0" :xl="0" class="main-header-phoneSize hidden-md-and-up">
        <Hub></Hub>
    </el-col>
    <el-col :xs="0" :sm="0" :md="24" :lg="24" :xl="24" class="main-header-large hidden-sm-and-down">
        <div id="MainHub">
            <div id="LeftNav">
                <div id="LeftNav-sticker">
                    <LeftNav></LeftNav>
                </div>
            </div>
            <div id="LeftNavDetail"
                v-bind:style="{ width: ChangeLeftNavDetail.width, minWidth: ChangeLeftNavDetail.minwidth }">
                <router-view></router-view>
            </div>
            <div id="MainHub-ChatContent">
                <MessageCT></MessageCT>
            </div>
        </div>
    </el-col>
</template>
<style>
#MainHub {
    width: 100%;
    display: flex;
    height: 100%;
    justify-content: space-between;
    background-color: white;
}

#LeftNav-sticker {
    display: flex;
    align-items: center;
    height: 800px;
    width: 60px;
    flex-direction: column;
}

#MainHub-ChatContent {
    min-width: 675px;
    width: 100%;
}

#LeftNavDetail {
    transition: all 0.2s ease-in-out;
    border-right: 1px #e9e9eb solid;
    background-color: #fafafa;
}

#LeftNav {
    height: 100%;
    border-right: 1px #e9e9eb solid;
}

#LeftNavDetail-lower {
    width: 80%;
    height: 7%;
    margin-top: 5px;
    align-items: center;
    display: flex;
    justify-content: center;
    box-sizing: border-box;
}

#LeftNavDetail-lower:hover {
    transition: all 0.3s ease;
    cursor: pointer;
    background-color: #f2f3f5;
}
</style>
