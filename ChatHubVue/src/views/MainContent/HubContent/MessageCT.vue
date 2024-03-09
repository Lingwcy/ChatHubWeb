<script setup lang="ts">
import { ref } from 'vue'
import { UseChatStore } from '../../../store/index'
import { ElScrollbar } from 'element-plus'
//import {ChatHubService} from '../../../services/ServicesCollector'
import { appsetting, UseServiceStore } from '../../../store/index'
import Emoji from '../HubContent/Tools/Emoji.vue'
import PicUpLoad from '../HubContent/Tools/PicUpload.vue'
import { createGroupService } from '../../../services/ServicesCollector'
const appset = appsetting() //系统设置库
const service = UseServiceStore();
const chatStore = UseChatStore() //主Chat库
//createChatHubService(userInfo.jwtToken, appset.ServerHubAddress);
//控制发送消息后聊天区域一直为底部
const myScrollbar = ref()
const scrollDown = () => {
    myScrollbar.value.forEach((element: { setScrollTop: (arg0: number) => void }) => {
        element.setScrollTop(99999)
    });
}
/*
    触发Tab-Click
    作用：将目标用户的聊天信息从状态库中渲染到Tab聊天区域
*/
const OnClickChatTab = (target: any) => {
    service.ChatHub?.PrintMessageToTab(target.paneName)
    scrollDown()
}


//手动发送
const sendMsg = async function () {
    if (appset.input == "") return
    service.ChatHub?.SendMessageToServer(appset.input);
    appset.input = ""
    scrollDown()
}
//按键发送
const txtMsgOnkeyPress = async function (e: { keyCode: number }) {
    if (appset.input.trim() == "") return
    if (e.keyCode != 13) return;
    service.ChatHub?.SendMessageToServer(appset.input);
    appset.input = ""
    scrollDown()
}


//???
const editableTabsValue = ref('1')

//选中删除Tab
const removeTab = (targetName: any) => {
    chatStore.targetUserTab.forEach((value, index, array) => {

        if (value.tabName == targetName) {
            array.splice(index, 1)
        }
    });
}

</script>

<template>
    <div id="MCT-Container">
        <el-tabs v-model="editableTabsValue" type="card" class="msgTabs" closable @tab-remove="removeTab"
            @tab-click="OnClickChatTab">
            <el-tab-pane v-for="item in chatStore.targetUserTab" :key="item.tabName" :label="item.tabTitle"
                :name="item.tabName">
                <div id="MCT-Container-RESVContent">
                    <el-scrollbar ref="myScrollbar">
                        <div id="messageRES" v-for="(_msg, index) in item.targetUserMessage.messageHeaders"
                            :key="index">
                            <div id="messageRES-Header">
                                <span style="margin-left: 3px;">{{ item.targetUserMessage.messageNames[index] }}</span>
                                <img v-bind:src="'src/images/systemHeader/' + item.targetUserMessage.messageHeaders[index]"
                                    id="UserMsgHeader" />
                            </div>
                            <div id="messageRES-Content">
                                <div id="messageRES-Content-center">
                                    <span id="messageRES-Content-center-msg">
                                        {{ item.targetUserMessage.messages[index] }}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </el-scrollbar>

                </div>
                <div id="MCT-Container-SENDContent">
                    <div id="MCT-Container-SENDContent-ToolsContainer">
                        <div id="MCT-Container-SENDContent-ToolsContainer-left">
                            <div class="tools-item">
                                <el-popover placement="top" :width="500" trigger="hover">
                                    <emoji></emoji>
                                    <template #reference>
                                        <el-button
                                            style="height: 35px; width: 35px; margin-bottom: 10px; background-color: whitesmoke; ">
                                            <svg t="1671526051283" class="icon" viewBox="0 0 1024 1024" version="1.1"
                                                xmlns="http://www.w3.org/2000/svg" p-id="8575" width="25" height="25">
                                                <path
                                                    d="M563.2 463.3L677 540c1.7 1.2 3.7 1.8 5.8 1.8 0.7 0 1.4-0.1 2-0.2 2.7-0.5 5.1-2.1 6.6-4.4l25.3-37.8c1.5-2.3 2.1-5.1 1.6-7.8s-2.1-5.1-4.4-6.6l-73.6-49.1 73.6-49.1c2.3-1.5 3.9-3.9 4.4-6.6 0.5-2.7 0-5.5-1.6-7.8l-25.3-37.8a10.1 10.1 0 0 0-6.6-4.4c-0.7-0.1-1.3-0.2-2-0.2-2.1 0-4.1 0.6-5.8 1.8l-113.8 76.6c-9.2 6.2-14.7 16.4-14.7 27.5 0.1 11 5.5 21.3 14.7 27.4zM387 348.8h-45.5c-5.7 0-10.4 4.7-10.4 10.4v153.3c0 5.7 4.7 10.4 10.4 10.4H387c5.7 0 10.4-4.7 10.4-10.4V359.2c0-5.7-4.7-10.4-10.4-10.4z m333.8 241.3l-41-20a10.3 10.3 0 0 0-8.1-0.5c-2.6 0.9-4.8 2.9-5.9 5.4-30.1 64.9-93.1 109.1-164.4 115.2-5.7 0.5-9.9 5.5-9.5 11.2l3.9 45.5c0.5 5.3 5 9.5 10.3 9.5h0.9c94.8-8 178.5-66.5 218.6-152.7 2.4-5 0.3-11.2-4.8-13.6z m186-186.1c-11.9-42-30.5-81.4-55.2-117.1-24.1-34.9-53.5-65.6-87.5-91.2-33.9-25.6-71.5-45.5-111.6-59.2-41.2-14-84.1-21.1-127.8-21.1h-1.2c-75.4 0-148.8 21.4-212.5 61.7-63.7 40.3-114.3 97.6-146.5 165.8-32.2 68.1-44.3 143.6-35.1 218.4 9.3 74.8 39.4 145 87.3 203.3 0.1 0.2 0.3 0.3 0.4 0.5l36.2 38.4c1.1 1.2 2.5 2.1 3.9 2.6 73.3 66.7 168.2 103.5 267.5 103.5 73.3 0 145.2-20.3 207.7-58.7 37.3-22.9 70.3-51.5 98.1-85 27.1-32.7 48.7-69.5 64.2-109.1 15.5-39.7 24.4-81.3 26.6-123.8 2.4-43.6-2.5-87-14.5-129z m-60.5 181.1c-8.3 37-22.8 72-43 104-19.7 31.1-44.3 58.6-73.1 81.7-28.8 23.1-61 41-95.7 53.4-35.6 12.7-72.9 19.1-110.9 19.1-82.6 0-161.7-30.6-222.8-86.2l-34.1-35.8c-23.9-29.3-42.4-62.2-55.1-97.7-12.4-34.7-18.8-71-19.2-107.9-0.4-36.9 5.4-73.3 17.1-108.2 12-35.8 30-69.2 53.4-99.1 31.7-40.4 71.1-72 117.2-94.1 44.5-21.3 94-32.6 143.4-32.6 49.3 0 97 10.8 141.8 32 34.3 16.3 65.3 38.1 92 64.8 26.1 26 47.5 56 63.6 89.2 16.2 33.2 26.6 68.5 31 105.1 4.6 37.5 2.7 75.3-5.6 112.3z"
                                                    p-id="8576" fill="#8a8a8a"></path>
                                            </svg>
                                        </el-button>
                                    </template>
                                </el-popover>
                            </div>
                            <div class="tools-item">
                                <el-popover placement="top" :width="500" trigger="click">
                                    <pic-up-load></pic-up-load>

                                    <template #reference>
                                        <el-button
                                            style="height: 35px; width: 35px; margin-bottom: 10px; background-color: whitesmoke; ">
                                            <svg t="1671525958216" class="icon" viewBox="0 0 1024 1024" version="1.1"
                                                xmlns="http://www.w3.org/2000/svg" p-id="7208" width="20" height="20">
                                                <path d="M0 0h1024v1024H0z" fill="#707070" fill-opacity="0" p-id="7209">
                                                </path>
                                                <path
                                                    d="M576 928H192c-52.992 0-96-43.093333-96-96V192c0-52.992 43.093333-96 96-96h640c52.992 0 96 43.093333 96 96v384.064c0 17.706667-14.293333 32-32 32s-32-14.293333-32-32V192.021333c0-17.706667-14.4-32-32-32H192c-17.706667 0-32 14.378667-32 32v639.957334c0 17.706667 14.4 32 32 32h384c17.706667 0 32 14.314667 32 32 0 17.706667-14.293333 32.021333-32 32.021333zM128 693.312a32.064 32.064 0 0 1-22.613333-54.698667l159.402666-159.338666a95.786667 95.786667 0 0 1 110.72-17.984l173.589334 86.805333c12.309333 6.186667 27.093333 3.797333 36.8-5.994667l287.402666-287.445333a32.042667 32.042667 0 0 1 45.290667 45.312L631.210667 587.392a95.786667 95.786667 0 0 1-110.72 18.005333l-173.589334-86.826666a31.616 31.616 0 0 0-36.8 6.016l-159.509333 159.317333c-6.186667 6.314667-14.4 9.386667-22.592 9.386667z m320-277.376c-52.906667 0-96-43.093333-96-96s43.093333-96.021333 96-96.021333 96 43.093333 96 96-43.093333 96.021333-96 96.021333z m0-128c-17.6 0-32 14.378667-32 32 0 17.6 14.4 32 32 32s32-14.4 32-32c0-17.621333-14.4-32-32-32zM768 928c-17.706667 0-32-14.293333-32-32v-192.021333c0-17.706667 14.293333-32 32-32s32 14.293333 32 32v192c0 17.706667-14.293333 32.021333-32 32.021333z m128-128c-7.893333 0-15.701333-2.922667-21.909333-8.725333L768 691.669333l-106.090667 99.712a31.936 31.936 0 0 1-45.226666-1.408 31.957333 31.957333 0 0 1 1.408-45.205333l112.213333-105.386667A48.554667 48.554667 0 0 1 768 621.44c14.72 0 28.501333 6.613333 37.696 17.92l112.213333 105.386667A31.957333 31.957333 0 0 1 896 800z"
                                                    fill="#707070" p-id="7210"></path>
                                            </svg>
                                        </el-button>
                                    </template>
                                </el-popover>
                            </div>
                        </div>
                        <div id="MCT-Container-SENDContent-ToolsContainer-right" v-on:click="sendMsg">
                            <span>Send</span>
                        </div>
                    </div>

                    <div id="MCT-Container-SENDContent-MSG">
                        <el-input v-on:keydown="txtMsgOnkeyPress" v-model="appset.input"
                            :autosize="{ minRows: 2, maxRows: 11 }" type="textarea" placeholder="请输入..." />
                    </div>
                </div>
            </el-tab-pane>
        </el-tabs>





    </div>
</template>

<style>
.tools-item {
    width: 10%;
    height: 95%;
    margin-top: 10px;
}

#MCT-Container {
    width: 100%;
    background-color: #f4f4f5;
    min-width: 627px;
    overflow-x: hidden;
    overflow-y: hidden;
    height: 100vh;
}

#UserMsgHeader {
    width: 40px;
    height: 40px;
    border-radius: 50px;
}

.msgTabs {
    height: 40px;
}

#MCT-Container-RESVContent {
    width: 100%;
    height: 50vh;
}

#MCT-Container-SENDContent {
    width: 100%;
    height: 300px;


    border-top: 1px #e9e9eb solid;
}

#MCT-Container-RESVContent-target {
    height: 40px;
    width: 100%;
    border-bottom: 1px #e9e9eb solid;
    background-color: #ffffff;
    align-items: center;
    display: flex;
    justify-content: center;
}

#MCT-Container-RESVContent-target-text {
    font-size: 15px;
    color: #73767a;
}

#MCT-Container-SENDContent-ToolsContainer {
    width: 100%;
    height: 14%;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

#MCT-Container-SENDContent-ToolsContainer-left {
    width: 400px;
    height: 100%;
    display: flex;
    align-items: center;
}

#MCT-Container-SENDContent-ToolsContainer-right {
    width: 10%;
    height: 100%;
    background-color: darkcyan;
    display: flex;
    justify-content: center;
    align-items: center;
    transition: background-color 0.3s ease-in-out;
}

#MCT-Container-SENDContent-ToolsContainer-right:hover {
    background-color: darkred;
    cursor: pointer;
}

#MCT-Container-SENDContent-ToolsContainer-right span {
    display: block;
}

#MCT-Container-SENDContent-MSG {
    width: 100%;
    height: 85%;
}

.el-textarea {
    --el-input-focus-border-color: null;
    --el-input-hover-border-color: null;

}

.el-textarea__inner {
    box-shadow: none;
    font-size: 15px;
    background-color: #f4f4f5;
    border: none;
}

#messageRES {
    display: flex;
    margin-left: 15px;
    cursor: default;
}

#messageRES-Header {
    display: flex;
    flex-direction: column;
    justify-content: center;
}

#messageRES-Header span {
    font-size: 15px;
    color: #606266;
}

#messageRES-Content {
    width: 70%;
    display: flex;
    align-items: center;
}

#messageRES-Content-center {

    background-color: darkgray;
    border-radius: 15px;
    margin-left: 10px;
    margin-top: 25px;
    align-items: center;
    display: flex;
    justify-content: center;
    padding: 5px;
}

#messageRES-Content-center-msg {
    word-wrap: break-word;
    font-size: 14px;
}
</style>
