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
            <svg t="1667904026732" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="5611" width="40" height="40"><path d="M870.1 775.2c-2.4-7.1-0.8-14.8 3.8-20.7 53.6-67 85.2-149.5 85.2-238.7 0-224.8-200.5-407.1-447.8-407.1S63.7 290.9 63.7 515.7s200.5 407.1 447.8 407.1c90.8 0 175.3-24.6 245.9-66.9 5.9-3.5 12.7-4.8 19.4-3.4l110.7 22.6c7.7 1.6 14.2-5.8 11.7-13.2l-29.1-86.7z m-419.7-76.3H328.2c-33.7 0-61.1-27.3-61.1-61.1 0-33.7 27.3-61.1 61.1-61.1h122.1c33.7 0 61.1 27.3 61.1 61.1s-27.3 61.1-61 61.1m244.2-203.5H328.2c-33.7 0-61.1-27.3-61.1-61.1 0-33.7 27.3-61.1 61.1-61.1h366.3c33.7 0 61.1 27.3 61.1 61.1s-27.3 61.1-61 61.1" fill="#707070" p-id="5612"></path></svg>
        </el-badge>
        </div>

        <div class="LeftNav-Container-items" v-on:click="DetailSelector(1)">
            <svg t="1667304357130" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="4836" width="40" height="40"><path d="M631.119772 619.422812l0.344028-16.685369c79.298505-55.388544 117.829666-153.092558 117.829666-297.756425 0-139.503444-87.211154-222.758273-233.423148-222.758273l-7.912649 0c-146.03998 0-233.423148 83.254829-233.423148 222.758273 0 143.975811 38.875189 241.679825 118.86175 297.412397l0 16.857383c-133.138922 17.889467-266.621871 70.869814-266.621871 151.88846 0 113.357299 129.526625 170.810012 385.139593 170.810012s385.139593-57.452713 385.139593-170.810012C897.053586 690.292626 763.914665 637.31228 631.119772 619.422812z" p-id="4837" fill="#707070"></path></svg>
        </div>

        <div class="LeftNav-Container-items" v-on:click="DetailSelector(2)">
            <svg t="1667293655447" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="12436" width="40" height="40"><path d="M512 612.6c-68.5 0-133-26.7-181.4-75.2S255.5 424.5 255.5 356s26.7-133 75.2-181.4S443.5 99.4 512 99.4c68.5 0 133 26.7 181.4 75.2s75.2 112.9 75.2 181.4-26.7 133-75.2 181.4-112.8 75.2-181.4 75.2z" fill="#707070" p-id="12437"></path><path d="M826.1 926.5c-21 0-38-17-38-38 0-152.1-123.8-275.9-275.9-275.9S236.3 736.4 236.3 888.5c0 21-17 38-38 38s-38-17-38-38c0-47.5 9.3-93.6 27.7-137 17.7-41.9 43.1-79.5 75.4-111.8 32.3-32.3 69.9-57.7 111.8-75.4 43.4-18.4 89.5-27.7 137-27.7s93.6 9.3 137 27.7c41.9 17.7 79.5 43.1 111.8 75.4s57.7 69.9 75.4 111.8c18.4 43.4 27.7 89.5 27.7 137 0 21-17.1 38-38 38zM39.7 788.4c-21 0-38-17-38-38C1.7 689 25.6 631.3 69 587.8c14.2-14.2 30-26.3 46.9-36.2-34.4-31.4-56-76.6-56-126.8 0-29.7 7.7-59.1 22.4-84.8 14.2-24.9 34.5-46.1 58.9-61.2 17.8-11.1 41.3-5.6 52.3 12.3 11.1 17.8 5.6 41.3-12.3 52.3-28.4 17.6-45.3 48-45.3 81.4 0 52.7 42.9 95.7 95.7 95.7h0.1c21 0 38 17 38 38s-17 38-37.9 38h-0.1c-84.9 0-153.9 69-153.9 153.9-0.1 21-17.1 38-38.1 38z" fill="#707070" p-id="12438"></path><path d="M985 788c-21 0-38-17-38-38 0-84.9-69-153.9-153.9-153.9-21 0-38-17-38-38s17-38 38-38c52.8 0 95.7-42.9 95.7-95.7 0-33.3-17-63.8-45.3-81.4-17.8-11.1-23.3-34.5-12.3-52.3 11.1-17.8 34.5-23.3 52.3-12.3 24.4 15.1 44.7 36.3 58.9 61.2 14.6 25.7 22.4 55.1 22.4 84.8 0 50.2-21.6 95.4-56 126.8 16.9 9.9 32.7 22 46.9 36.2 43.4 43.4 67.3 101.2 67.3 162.6 0 21-17 38-38 38z" fill="#707070" p-id="12439"></path></svg>
        </div>

        <div class="LeftNav-Container-items" style="margin-top: 15px;" v-on:click="DetailSelector(4)" >
            <svg t="1667299992243" class="icon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2577" width="40" height="40"><path d="M951.7 428.8c-0.2-1.8-0.4-3.3-0.7-4.2v-0.8l-0.4-2.2c-7.1-34.6-30-56.9-58.5-56.9h-4.7c-48.6 0-88-39.6-88-88 0-11.2 5.2-27.1 7.4-32.7 13.8-32.2-0.9-68.9-35-87.6L664.5 95.7l-2-0.7c-8-2.6-17.3-5.7-27.7-5.7-19.4 0-41.2 9-54.7 22.5-16.9 16.7-51.2 41.6-71.6 41.6-20.3 0-54.7-24.8-71.6-41.6-14.4-14.2-34.3-22.5-54.7-22.5-10.7 0-19.7 3-27.7 5.7l-1.8 0.7-112.5 60.9-0.7 0.4c-27.3 17.1-38.4 56.3-24.5 87.4l0.2 0.4 0.2 0.4c2.2 4.9 9 21.5 9 36 0 48.6-39.6 88-88 88h-4.7c-29.8 0-52.1 22-58.5 57.3l-0.4 2v0.7c0 1-0.4 2.4-0.7 4.2-2.5 15.1-8.5 50.7-8.5 79.8 0 29.1 5.9 64.7 8.5 79.8 0.2 1.8 0.4 3.3 0.7 4.2v0.8l0.4 2.2c7.1 34.6 30 56.9 58.5 56.9h2.4c48.6 0 88 39.6 88 88 0 11.2-5.2 27.1-7.4 32.7-13.3 30.3-0.7 69.2 28.7 88.8l0.8 0.4 106 59 2 0.7c8 2.6 17.1 5.7 27.5 5.7 22.2 0 42.2-8.5 54.7-22.5 1.2-0.9 2.4-2.1 3.8-3.3 12.8-11.2 47.2-40.8 69.9-40.8 16.9 0 45.2 17.7 73.7 46.2 14.4 14.2 34.3 22.5 54.7 22.5 13.8 0 24-3.8 35.6-9.5l0.4-0.2 108.7-60.1 0.4-0.4c27.3-17.1 38.4-56.3 24.5-87.4l-0.2-0.4-0.2-0.4c-0.2-0.1-8.7-17.8-7.1-33.7l0.2-1v-1c0-48.6 39.6-88 88-88h5c29.8 0 52.1-22 58.5-57.3l0.4-2v-0.7c0.2-0.8 0.4-2 0.7-3.6 2.6-14.7 8.6-49 8.6-80.4 0.2-29-5.7-64.5-8.3-79.6z m-440 222.4c-76.9 0-139.2-62.3-139.2-139.2s62.3-139.2 139.2-139.2S650.9 435.1 650.9 512s-62.3 139.2-139.2 139.2z" fill="#707070" p-id="2578"></path></svg>
        </div>
        
        
    </div>
</template>
<style>

    #LeftNav-Container{
       width: 80%;
       height: 800px;
       display: flex;
       flex-direction: column;


    }
    .LeftNav-Container-items{
        width: 100%;
        height: 7%;
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
