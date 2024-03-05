<script setup>
import {reactive} from 'vue'
import { useRouter } from 'vue-router'
import {UseUserInformationStore} from '../../../store/index'

import { CaretBottom } from '@element-plus/icons-vue'


const userInfo=UseUserInformationStore()
const router =useRouter()
const state = reactive({
    userMsg:"",
    messages:[],
    username:userInfo.userName,
    jwtToken:userInfo.jwtToken,
    toUserName:"",
    PrivateMsg:""
});




const DetailSelector=(index)=>{
    if(index==1){
            router.push({
            path:'/Hub/Friends'
        })
    }
    else if(index ==2){
        router.push({
            path:'/Hub/Groups'
        })
    }else if(index ==3){
        router.push({
            path:'/Hub/Message'
        })
    }else if(index ==4){
        router.push({
            path:'/Setting/User'
        })
    }

}
const confirm =()=>{
    router.push({
            path:'/Setting/User'
        })
}

</script>
<template>
    <div id="LeftNav-Container">
        <div class="LeftNav-Container-items">
            <div id="UserMsg">
                <el-popconfirm 
                   confirm-button-text="."
                   cancel-button-text="修改资料"
                   cancel-button-type="success"
                   title="配置您的个人信息来获得更高的辨识度"
                   icon-color="#626AEF"
                   @confirm="confirm()"
                   @cancel="confirm()"
                   >
                    <template #reference>
                        <el-button style="border-radius: 50px; width: 100%; height: 100%; overflow: hidden; box-sizing: border-box;">
                            <img v-bind:src="'src/images/systemHeader/'+userInfo.userImg"  id="UserMsg-img"/>
                        </el-button>
                    </template>
                </el-popconfirm>
            </div>
        </div>

        <div class="LeftNav-Container-items" v-on:click="DetailSelector(3)">
            <el-badge value="new" class="redpop" :max="99" :hidden="userInfo.unReadMsg">
            <img src="../../../images/icon/dark/消息-黑.svg" class="LeftNavItem"/>
        </el-badge>
        </div>

        <div class="LeftNav-Container-items" v-on:click="DetailSelector(1)">
            <img src="../../../images/icon/dark/好友-黑.svg" class="LeftNavItem"/>
        </div>
        <div class="LeftNav-Container-items" v-on:click="DetailSelector(2)">
            <img src="../../../images/icon/dark/群组-黑.svg" class="LeftNavItem"/>
        </div>

        <div class="LeftNav-Container-items" style="margin-top: 15px;" v-on:click="DetailSelector(4)" >
            <img src="../../../images/icon/dark/设置.svg" class="LeftNavItem"/>
        </div>
        
        
    </div>
</template>
<style>
.LeftNavItem{
    width:30px;
    height:30px;
}
    #LeftNav-Container{
       width: 80%;
       height: 800px;
       display: flex;
       flex-direction: column;


    }
    .LeftNav-Container-items{
        width: 100%;
        height: 6.5%;
        margin-top: 5px;
        box-sizing: border-box;
        display: flex;
        justify-content: center;
        align-items: center;
        
    }
    .LeftNav-Container-items:hover{
        transition: all 0.3s ease;
        cursor: pointer;
        background-color: #f2f3f5;   
        border-radius: 10px;    
    }
    #UserMsg{
        width: 85%;
        height: 85%;
        background-color: darkblue;
        border-radius: 50px;
        display: flex;
        justify-content: center;
        align-items: center;
    }
    #UserMsg-img{
        width: 40px;
        height: 40px;
        border-radius: 50px;
    }
</style>
