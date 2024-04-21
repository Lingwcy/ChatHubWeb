<script setup>
import {reactive,ref,h} from 'vue'
import { ElNotification } from 'element-plus'
import { useRouter } from 'vue-router'
import { UseServiceStore } from '../../../store/index'
import {UseUserInformationStore} from '../../../store/index'
import { createUserService } from '../../../services/ServicesCollector'
import { CaretBottom } from '@element-plus/icons-vue'
createUserService()
const service=UseServiceStore()
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

function exitLogin(){
    userInfo.logout()
    service.ChatHub.IsLogin=false
    ElNotification({
    title: '消息',
    message: h('i', { style: 'color: teal' }, '退出登录成功'),
  })
  router.push({
        path:'/Login/Account'
    })
}

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
    }else if(index == 0){
        router.push({
            path:'/Hub/Contract'
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
        <div class="LeftNav-Container-items" >
            <div id="UserMsg">
                <el-popconfirm 
                   confirm-button-text="修改资料"
                   cancel-button-text="退出登录"
                   cancel-button-type="success"
                   title="配置您的个人信息来获得更高的辨识度"
                   icon-color="#626AEF"
                   @confirm="confirm()"
                   @cancel="exitLogin()"
                   >
                    <template #reference>
                        <el-button style="border-radius: 50px; width: 100%; height: 100%; overflow: hidden; box-sizing: border-box;">
                            <img v-bind:src="'src/images/systemHeader/'+userInfo.userImg"  id="UserMsg-img"/>
                        </el-button>
                    </template>
                </el-popconfirm>
            </div>
        </div>

        <div class="LeftNav-Container-items" v-on:click="DetailSelector(3)" style="margin-top: 10px;" >
            <el-badge value="new" class="redpop" :max="99" :hidden="userInfo.unReadMsg">
            <img src="../../../images/icon/dark/消息-黑.svg" class="LeftNavItem"/>
        </el-badge>
        </div>

        <div class="LeftNav-Container-items" v-on:click="DetailSelector(0) "   >
            <img src="../../../images/icon/dark/好友-黑.svg" class="LeftNavItem"/>
        </div>

        <div class="LeftNav-Container-items" style="margin-top: 15px;" v-on:click="DetailSelector(4)" >
            <img src="../../../images/icon/dark/设置.svg" class="LeftNavItem"/>
        </div>
        
        
    </div>
</template>
<style>
.selected {  
  background-color: #d3d3d4;  
}  
.LeftNavItem{
    width:25px;
    height:25px;
}
    #LeftNav-Container{
       width: 80%;
       height: 700px;
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
        background-color: #d3d3d4;   
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
