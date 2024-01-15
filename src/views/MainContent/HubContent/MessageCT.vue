<script setup lang="ts">
import { reactive, onMounted, onUnmounted, ref } from 'vue'
import { UseUserInformationStore, UsePublicMsgStore, UseMsgStore, UseMsgbox } from '../../../store/index'
import { ElScrollbar, ElMessageBox, ElNotification } from 'element-plus'
import { ChatHubService, createChatHubService } from '../../../services/ServicesCollector'
import { appsetting } from '../../../store/index'
import axios from '../../../common/axiosSetting'
import Emoji from '../HubContent/Tools/Emoji.vue'
import PicUpLoad from '../HubContent/Tools/PicUpload.vue'
const appset = appsetting()
const msgbox = UseMsgbox()
const userInfo = UseUserInformationStore()
const msgStore = UseMsgStore()
const Pmsg = UsePublicMsgStore()

const state = reactive({
    userMsg: "",
    messages: [],
    messagesNames: [],
    messagesHeader: [],
    username: userInfo.userName,
    jwtToken: userInfo.jwtToken,
    toUserName: "",
    PrivateMsg: ""
});



let MSGmodel =
{
    name: "",
    messages: [],
    messagesNames: [],
    messagesHeader: [],
}

const myScrollbar = ref()
const scrollDown = () => {
    myScrollbar.value.forEach((element: { setScrollTop: (arg0: number) => void }) => {
        element.setScrollTop(99999)
    });
}
const test = (target: any) => {
    Pmsg.Cmsg.forEach(element => {
        if (element.name == target.paneName) {
            MSGmodel = element
        }
        msgStore.editableTabs.forEach(element => {
            if ((element as any).name == MSGmodel.name) {
                (element as any).xmsg = reactive(MSGmodel)
            }
        });
    });
    scrollDown()

}


//发起请求
const verify = async function () {
    //构造json querystring
    const config = {
        headers: {
            Authorization: "Bearer " + state.jwtToken,//附带Jwt认证
        }
    }
    await axios.get('api/font-login/verify', config)//认证
        .then(async _res => {
            ConnectHub()

        })
        .catch(err => {
            ElMessageBox.alert(err, '连接错误',
                {
                    confirmButtonText: '确认',
                    type: 'error',
                })
        })
}



const ConnectHub = async function () {
    createChatHubService(userInfo.jwtToken, appset.ServerHubAddress);
    ChatHubService.startHub();
    //公共消息接收器
    ChatHubService.HubConnection.on('publicMsgReceived', (HeaderImg, fromUserName, msg) => {
        Pmsg.Cmsg[0].messages.push(msg as never)
        Pmsg.Cmsg[0].messagesNames.push(fromUserName as never)
        Pmsg.Cmsg[0].messagesHeader.push(HeaderImg as never)
        scrollDown()
    });

    //好友添加请求
    ChatHubService.HubConnection.on('FriendsRequestReceived', (fromUserName) => {
        ElNotification({
            title: '新的好友请求!',
            message: ` 来自 :${fromUserName} `,
        })
    });
    //离线消息接收器(漏洞-请求权限问题)
    /*请求离线消息 并添加到Pinia[UseMsgbox] */
    ChatHubService.HubConnection.on('MsgBoxFlasherReceived', (_fromUserName) => {
        axios.get('api/friends/msg-box-list/' + userInfo.userName + '')
            .then(res => {
                msgbox.$reset()
                let msgitems = res.data.data.split('$')
                for (let i = 0; i < msgitems.length - 1; i++) {
                    msgbox.MsgItems.push(JSON.parse(msgitems[i]) as never)
                }
            })
            .catch(errors => {
                alert(errors)
            })
    });
    //私人消息接收器
    /*这里需要做到的：将消息渲染到聊天区域 */
    ChatHubService.HubConnection.on('PrivateMsgReceived', (HeaderImg, fromUserName, msg) => {

        ElNotification({
            title: '新的消息',
            message: `${fromUserName} : ${msg}`,
        })
        /*
            私聊消息接收区  
            此方法需要做到的操作: 
               1. 接受fromUserName(发送者) 和 msg(讯息体) 并存储到相应的 消息状态库(key=>fromUserName) 如果没有则创建
               2. 判断接受者（本客户端）是否处于和发送者Tab打开状态(换言之 判断状态库中 editableTabs 是否存在 key=>fromUserName)
                    2.1 如果不存在此Tab => 把消息记录到中央服务器的临时表中 并在 消息盒子栏浮出红点(实现方法=>登陆时查询消息临时表 有记录则冒红点)
                    2.2 临时消息记录将渲染在 消息盒子中
                    2.3 当用户点击消息盒子信息 => 将临时消息表内容分发到 消息状态库中 且 渲染消息状态库到dom上 同时删除临时消息表中的对应元组 
                
            
        */
        //现在考虑的前提是 接收方 也就是此客户端在线下的操作（if message-receiver is online）

        let existMsgStore = false
        //如果本地有存储库 直接放
        for (let i = 0; i < Pmsg.Cmsg.length; i++) {
            if (Pmsg.Cmsg[i].name == fromUserName) {
                existMsgStore = true
                Pmsg.Cmsg[i].messages.push(msg as never)
                Pmsg.Cmsg[i].messagesNames.push(fromUserName as never)
                Pmsg.Cmsg[i].messagesHeader.push(HeaderImg as never)
            }
        }
        if (!existMsgStore) {//如果没有这个库则创建
            Pmsg.Cmsg.push({

                name: fromUserName,
                messages: [msg as never],
                messagesNames: [fromUserName as never],
                messagesHeader: [HeaderImg as never],
            })
        }
        //消息发入接受端之后 在 messageBox 上渲染 且冒红点 =》 此实现位于MessageBox.vue

    });

}


//手动发送
const sendMsg = async function () {

    if (appset.input == "") return

    if (MSGmodel.name == "world") {
        await ChatHubService.HubConnection.invoke("SendPublicMsg", userInfo.userName, appset.input);
        appset.input = ""
        scrollDown()
    } else {
        await ChatHubService.HubConnection.invoke("SendPrivateMsg", MSGmodel.name, appset.input);
        for (let i = 0; i < Pmsg.Cmsg.length; i++) {
            if (Pmsg.Cmsg[i].name == MSGmodel.name) {
                Pmsg.Cmsg[i].messages.push(appset.input as never)
                Pmsg.Cmsg[i].messagesNames.push(userInfo.userName as never)
                Pmsg.Cmsg[i].messagesHeader.push(userInfo.userImg as never)
            }
        }

        await ChatHubService.HubConnection.invoke("MsgBoxFlasher", MSGmodel.name);
        appset.input = ""
        scrollDown()

    }

}
//按键发送
const txtMsgOnkeyPress = async function (e: { keyCode: number }) {
    if (appset.input.trim() == "") return
    if (e.keyCode != 13) return;
    if (MSGmodel.name == "world") {
        await ChatHubService.HubConnection.invoke("SendPublicMsg", userInfo.userName, appset.input);
        appset.input = ""
        scrollDown()
    } else {
        await ChatHubService.HubConnection.invoke("SendPrivateMsg", MSGmodel.name, appset.input);
        for (let i = 0; i < Pmsg.Cmsg.length; i++) {
            if (Pmsg.Cmsg[i].name == MSGmodel.name) {
                Pmsg.Cmsg[i].messages.push(appset.input as never)
                Pmsg.Cmsg[i].messagesNames.push(userInfo.userName as never)
                Pmsg.Cmsg[i].messagesHeader.push(userInfo.userImg as never)
            }
        }

        await ChatHubService.HubConnection.invoke("MsgBoxFlasher", MSGmodel.name);
        appset.input = ""
        scrollDown()

    }
}


//???
const editableTabsValue = ref('1')

//选中删除Tab
const removeTab = (targetName: any) => {
    msgStore.editableTabs.forEach((value, index, array) => {
        
        if ((value as any).name == targetName) {
            array.splice(index, 1)
        }
    });
}


//从后端获取离线消息并存储到Pinia
/*只获取不渲染*/
const getofflinemsg = () => {
    axios.get('https://localhost:5001/api/message/offline-message/' + userInfo.userName)
        .then(res => {
            let result = res.data.data
            //userInfo.unReadMsg = res.data.data.length > 0 ? false : true
            for (let x = 0; x < result.length; x++) {
                let existMsgStore = false
                //如果本地有存储库 直接放
                for (let i = 0; i < Pmsg.Cmsg.length; i++) {
                    if (Pmsg.Cmsg[i].name == result[x].sender) {
                        existMsgStore = true
                        Pmsg.Cmsg[i].messages.push(result[x].sendMessage as never)
                        Pmsg.Cmsg[i].messagesNames.push(result[x].sender as never)
                        Pmsg.Cmsg[i].messagesHeader.push(result[x].senderImg as never)
                    }
                }
                if (!existMsgStore) {//如果没有这个库则创建
                    Pmsg.Cmsg.push({

                        name: result[x].sender,
                        messages: [result[x].sendMessage as never],
                        messagesNames: [result[x].sender as never],
                        messagesHeader: [result[x].senderImg as never],
                    })
                }
            }
        })
        .catch(err => {
            alert(err)
        })
}

onMounted(function () {
    verify()
    if (msgStore.editableTabs.length < 1) {
        msgStore.editableTabs.push({
            title: "世界频道",
            name: "world",
            content: '世界频道',
            xmsg: ""
        } as never)
    }

    getofflinemsg()


})
onUnmounted(() => {
    ChatHubService.HubConnection.stop();
    ChatHubService.IsLogin = false;
})
</script>
<template>
    <div id="MCT-Container">
        <el-tabs v-model="editableTabsValue" type="card" class="msgTabs" closable @tab-remove="removeTab" @tab-click="test">
            <el-tab-pane v-for="item in msgStore.editableTabs" :key="(item as any).name " :label="(item as any).title" :name="(item as any).name">
                <div id="MCT-Container-RESVContent">
                    <el-scrollbar ref="myScrollbar">
                        <div id="messageRES" v-for="(_msg, index) in (item as any).xmsg.messagesHeader" :key="index">
                            <div id="messageRES-Header">
                                <span style="margin-left: 3px;">{{ (item as any).xmsg.messagesNames[index] }}</span>
                                <img v-bind:src="'src/images/systemHeader/' + (item as any).xmsg.messagesHeader[index]"
                                    id="UserMsg-img" />
                            </div>
                            <div id="messageRES-Content">
                                <div id="messageRES-Content-center">
                                    <span id="messageRES-Content-center-msg">
                                        {{ (item as any).xmsg.messages[index] }}

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
    height: 95vh;
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
