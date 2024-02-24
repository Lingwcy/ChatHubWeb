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
    minwidth: ref("188px"),
    flag: ref(0)
})
const ChangeLowerLeftNavDetail = function () {
    if (ChangeLeftNavDetail.flag == 0) {
        ChangeLeftNavDetail.width = "0%"
        ChangeLeftNavDetail.minwidth = "0px"
        ChangeLeftNavDetail.flag = 1
    } else {
        ChangeLeftNavDetail.width = "300px"
        ChangeLeftNavDetail.minwidth = "188px"
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
                    <el-button v-on:click="ChangeTopNavBar"></el-button>
                    <div id="LeftNavDetail-lower" v-on:click="ChangeLowerLeftNavDetail">
                        <svg v-if="ChangeLeftNavDetail.flag == 0" t="1667291870133" class="icon" viewBox="0 0 1024 1024"
                            version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="5038" width="40" height="40">
                            <path
                                d="M896 810.666667a42.666667 42.666667 0 0 1-42.666667 42.666666H170.666667a42.666667 42.666667 0 0 1 0-85.333333h682.666666a42.666667 42.666667 0 0 1 42.666667 42.666667zM296.533333 202.965333v319.402667a21.333333 21.333333 0 0 1-36.437333 15.061333L115.498667 392.832a42.666667 42.666667 0 0 1 0-60.330667l144.64-144.64a21.333333 21.333333 0 0 1 36.394666 15.104zM896 512a42.666667 42.666667 0 0 1-42.666667 42.666667h-298.666666a42.666667 42.666667 0 0 1 0-85.333334h298.666666a42.666667 42.666667 0 0 1 42.666667 42.666667z m0-298.666667a42.666667 42.666667 0 0 1-42.666667 42.666667h-298.666666a42.666667 42.666667 0 0 1 0-85.333333h298.666666a42.666667 42.666667 0 0 1 42.666667 42.666666z"
                                fill="#5D6E7F" p-id="5039"></path>
                        </svg>
                        <svg v-else t="1667292511889" class="icon" viewBox="0 0 1024 1024" version="1.1"
                            xmlns="http://www.w3.org/2000/svg" p-id="9713" width="40" height="40">
                            <path
                                d="M937.386667 488.106667L772.266667 372.48c-12.8-9.386667-30.293333-6.826667-40.106667 5.546667-3.84 4.693333-5.546667 10.666667-5.546667 17.066666v233.813334c0 22.613333 25.6 36.693333 45.653334 22.613333l165.546666-115.626667c14.08-14.08 14.08-36.693333-0.426666-47.786666zM914.346667 213.333333h-785.066667c-18.773333 0-34.133333-15.36-34.133333-34.133333s15.36-34.133333 34.133333-34.133333h785.066667c18.773333 0 34.133333 15.36 34.133333 34.133333s-14.933333 34.133333-34.133333 34.133333zM914.346667 878.933333h-785.066667c-18.773333 0-34.133333-15.36-34.133333-34.133333s15.36-34.133333 34.133333-34.133333h785.066667c18.773333 0 34.133333 15.36 34.133333 34.133333s-14.933333 34.133333-34.133333 34.133333zM624.213333 435.2h-494.933333c-18.773333 0-34.133333-15.36-34.133333-34.133333s15.36-34.133333 34.133333-34.133334h494.933333c18.773333 0 34.133333 15.36 34.133334 34.133334s-14.933333 34.133333-34.133334 34.133333zM624.64 657.066667H129.28c-18.773333 0-34.133333-15.36-34.133333-34.133334s15.36-34.133333 34.133333-34.133333h495.36c18.773333 0 34.133333 15.36 34.133333 34.133333v0.426667c-0.426667 18.346667-15.36 33.706667-34.133333 33.706667z"
                                fill="#8a8a8a" p-id="9714"></path>
                        </svg>
                    </div>
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
    min-width: 70px;
    display: flex;
    align-items: center;
    height: 800px;
    flex-direction: column;
}

#MainHub-ChatContent {
    min-width: 675px;
    width: 100%;
}

#LeftNavDetail {
    transition: all 0.2s ease-in-out;
    border-right: 1px #e9e9eb solid;
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
