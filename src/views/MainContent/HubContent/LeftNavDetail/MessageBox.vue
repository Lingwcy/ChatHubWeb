<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import axios from '../../../../common/axiosSetting';
import { ElMessage } from 'element-plus'
import { UseUserInformationStore, UseMsgbox, UseMsgStore, UseChatStore } from '../../../../store/index'
const Pmsg = UseMsgStore()
const userInfo = UseUserInformationStore()
const MsgBox = UseMsgbox()
const chatStore = UseChatStore()
const DetailTargetModel = reactive({
    value: ''
})
const DetailVisible = ref(false)
onMounted(function () {
    //http载入msgbox
    getFriendsReq()
    getMsgBox()
})
axios.defaults.headers.common['Authorization'] = "Bearer " + userInfo.jwtToken //向每一个请求携带JWT

let friendsReq = []
let friendsReqJson = reactive({}) as any;

//加载好友请求
const getFriendsReq = () => {
    axios.get('api/friends/frined-request-list/' + userInfo.userName + '')
        .then(res => {

            friendsReq = res.data.data.split('$')
            for (let i = 0; i < friendsReq.length - 1; i++) {
                friendsReqJson[i] = JSON.parse(friendsReq[i])
            }
        })
}
//加载消息盒子
const getMsgBox = () => {
    MsgBox.$reset()
    axios.get('api/friends/msg-box-list/' + userInfo.userName + '')
        .then(res => {

            let msgitems = res.data.data.split('$')
            for (let i = 0; i < msgitems.length - 1; i++) {
                MsgBox.MsgItems.push(JSON.parse(msgitems[i]) as never)
            }
        })
        .catch(errors => {
            alert(errors)
        })
}

const TurnFriendsToMessageBox = (friends: any) => {
    if (friends.isNew == 1) {
        friends.isNew = 0
    }
    axios.put('api/friends/msg-items-redbob/' + friends.targetfont + '/' + userInfo.userName)
    let flag1 = false//判断消息存储库
    Pmsg.messageItems.forEach(element => {
        if (element.targetUserName == friends.targetfont) {
            flag1 = true


            for (let i = 0; i < chatStore.targetUserTab.length; i++) {
                if (chatStore.targetUserTab[i].tabName == friends.targetfont) {
                    return
                }
            }
            chatStore.targetUserTab.push({
                tabTitle: friends.targetfont,
                tabName: friends.targetfont,
                targetUserMessage: reactive(element)
            })
        }

    });
    if (flag1) return
    Pmsg.messageItems.push(
        {
            targetUserName: friends.targetfont,
            messages: [],
            messageNames: [],
            messageHeaders: [],
        }
    )
    chatStore.targetUserTab.push({
        tabTitle: friends.targetfont,
        tabName: friends.targetfont,
        targetUserMessage: reactive(Pmsg.messageItems[Pmsg.messageItems.length - 1])
    })
}


//获取一个好友的详细信息
const GetDetailInformation = (name: any) => {
    axios.get("api/user/user/" + (name))
        .then(res => {
            if (res.data.errors != null) {
                ElMessage({
                    message: '用户数据获取失败!.',
                    type: 'error',
                })
                return;
            }
            if (res.data.data) {
                DetailTargetModel.value = JSON.parse(res.data.data)


            }
        })
        .catch(error => [
            ElMessage({
                message: '用户数据获取失败!.' + error,
                type: 'error',
            })

        ])
}


const handleCommand = (command: any) => {
    let commands = command.split(',')
    if (commands[0] == "show") {
        GetDetailInformation(commands[1])
        DetailVisible.value = true
    }
    else if (commands[0] == "delete") {
        axios.delete('https://localhost:5001/api/message/msg-box-item/' + commands[1])
            .then(res => {
                if (res.data.errors != null) {
                    ElMessage({
                        message: '删除失败!.' + res.data.errors,
                        type: 'error',
                    })
                    return
                }
                if (res.data.data == '1') {
                    ElMessage({
                        message: '删除成功!.',
                        type: 'success',
                    })

                    MsgBox.MsgItems.forEach((value, index, array) => {
                        if (value.id == commands[1]) {
                            array.splice(index, 1)
                        }

                    });
                }
            })
    }
}
</script>
<template>
    <div v-for="(req, _index) in MsgBox.MsgItems">

        <div class="FriendMSGContent" v-on:click="TurnFriendsToMessageBox(req)">
            <div id="friendReq-Header">
                <el-badge is-dot :hidden="!req.isNew">
                    <img v-bind:src="'../../../src/images/systemHeader/' + req.targetImage" id="friendReq-Header-img" />
                </el-badge>
            </div>
            <div class="friendMSG-Body">
                <div class="friendMSG-Body-coentent">
                    <span class="friendMSG-name">{{ req.targetfont }}</span>
                    <el-dropdown @command="handleCommand">
                        <span class="el-dropdown-link">
                            <svg t="1671181070434" class="icon" viewBox="0 0 1024 1024" version="1.1"
                                xmlns="http://www.w3.org/2000/svg" p-id="2957" width="20" height="20">
                                <path
                                    d="M512 704c35.2 0 64 28.8 64 64s-28.8 64-64 64-64-28.8-64-64 28.8-64 64-64z m-64-192c0 35.2 28.8 64 64 64s64-28.8 64-64-28.8-64-64-64-64 28.8-64 64z m0-256c0 35.2 28.8 64 64 64s64-28.8 64-64-28.8-64-64-64-64 28.8-64 64z"
                                    p-id="2958"></path>
                            </svg>
                        </span>
                        <template #dropdown>
                            <el-dropdown-menu>
                                <el-dropdown-item :command="'show,' + req.targetfont">查看资料</el-dropdown-item>
                                <el-dropdown-item :command="'delete,' + req.id">删除会话</el-dropdown-item>
                            </el-dropdown-menu>
                        </template>
                    </el-dropdown>
                </div>
            </div>

        </div>

    </div>


    <el-dialog v-model="DetailVisible" width="380px" id="addUser" center draggable>
        <template #header>
            <div class="DetailInfo-header">
                <div class="DetailInfo-header-img">
                    <img :src="'../../../../src/images/systemHeader/' + DetailTargetModel.value.HeaderImg" alt="">
                </div>
                <div class="DetailInfo-header-content">
                    <span v-if="DetailTargetModel.value.NickName" class="DetailInfo-header-content-font1">{{
                        DetailTargetModel.value.NickName }}</span>
                    <span v-else class="DetailInfo-header-content-font1">{{ DetailTargetModel.value.Username }}</span>
                    <span class="DetailInfo-header-content-font2">{{ DetailTargetModel.value.Job }}</span>
                    <span class="DetailInfo-header-content-font3">暂无个性签名</span>
                </div>
            </div>
        </template>
        <div class="DetailInfo-content">
            <el-descriptions title="详细信息" direction="vertical" :column="2" border>
                <el-descriptions-item label="账户">{{ DetailTargetModel.value.Username }}</el-descriptions-item>
                <el-descriptions-item label="电话号">{{ DetailTargetModel.value.Phone }}</el-descriptions-item>
                <el-descriptions-item label="居住地">{{ DetailTargetModel.value.City }}</el-descriptions-item>
                <el-descriptions-item label="邮箱">
                    <el-tag size="small">{{ DetailTargetModel.value.Email }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="个人简介">{{ DetailTargetModel.value.Desc }}</el-descriptions-item>
            </el-descriptions>
        </div>
        <template #footer>
            <div id="addUser-footer">
                <el-button @click="Headvisible = false" style="width: 150px;">分享</el-button>
                <el-button type="primary" @click="Headvisible = false" style="width: 150px;">
                    发送消息
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>
<style>
.DetailInfo-header {
    width: 90%;
    height: 90px;
    border-bottom: 1px rgb(211, 207, 206) solid;
    display: flex;
}

.DetailInfo-header-img {
    width: 75px;
    height: 75px;
    border-radius: 5px;
}

.DetailInfo-header-img img {
    width: 75px;
    height: 75px;
    overflow: hidden;
    border-radius: 5px;
}

.DetailInfo-header-content {
    width: 130px;
    height: 75px;
    margin-left: 15px;
    display: flex;
    flex-direction: column;
    align-items: flex-start;
}

.DetailInfo-header-content-font1 {
    font-size: 16px;
    font-weight: 800;
}

.DetailInfo-header-content-font2 {
    font-size: 13px;
    font-weight: 400;
}

.DetailInfo-header-content-font3 {
    font-size: 12px;
    font-weight: 400;
    color: #939497;
}

.DetailInfo-content {
    height: 280px;
}

.FriendsRequestContent {
    display: flex;
    height: 100%;
    border-bottom: 1px solid #e9e9eb;
}

#friendReq-Header {
    width: 28%;
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
    border-radius: 5px;
}

#friendReq-Body {
    width: 70%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    margin-left: 10px;

}

#friendReq-Header-img {
    width: 40px;
    height: 40px;
    overflow: hidden;
    border-radius: 15px;
}

.friendReq-Body-info {
    width: 100%;
    height: 22px;
    overflow: hidden;
    color: #606266;
    font-size: 12px;
    font-weight: 500;
    margin-top: 5px;
}

.friendReq-Body-info2 {
    width: 100%;
    height: 27px;
    overflow: hidden;
    color: #606266;
    font-size: 10px;
    font-weight: 500;
}

.friendReq-Body-info-bt {
    width: 50px;
    height: 20px;
    font-size: 11px;
}

#friendReq-Body-inner {
    width: 90%;
    height: 80%;
    display: flex;
    flex-direction: column;
    justify-content: end;

}

.FriendMSGContent {
    height: 60px;
    border-bottom: 1px solid #e9e9eb;
    display: flex;
}

.FriendMSGContent:hover {
    background-color: #b4b5b8;
    transition: 0.3s ease-in-out all;
    cursor: pointer;
}

.friendMSG-Body {
    width: 70%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
}

.friendMSG-Body-coentent {
    width: 90%;
    height: 80%;
    font-size: 15px;
    display: flex;
    justify-content: space-between;
    align-items: center;

}

.friendMSG-name {
    width: 120px;
    height: 20px;
    overflow: hidden;
    font-size: 12px;
    margin-bottom: 5px;
    color: #606266;
}
</style>
