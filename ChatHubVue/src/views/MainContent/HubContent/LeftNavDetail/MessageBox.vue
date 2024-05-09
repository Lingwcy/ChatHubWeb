<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';
import { createMessageService, createUserService, createGroupService } from '../../../../services/ServicesCollector';
import { UseUserInformationStore, UseMsgbox, UseGroupStore, UseMsgStore, UseChatStore, UseServiceStore, UseFriendsStore } from '../../../../store/index'
import { UserGroup } from '../../../../store/Istore';
createMessageService();
createUserService();
createGroupService();
const service = UseServiceStore();
const Pmsg = UseMsgStore()
const userInfo = UseUserInformationStore()
const MsgBox = UseMsgbox()
const chatStore = UseChatStore()
const friendStore = UseFriendsStore()
const DetailVisible = ref(false)
const groupStore = UseGroupStore()

onMounted(function () {
    //http载入msgbox
    getMsgBox()
    service.Group?.GetGroupList(Number(userInfo.userId), userInfo.userName, groupStore);
})

const getMsgBox = () => {
    MsgBox.$reset()
    service.Message?.GetMessageBox(userInfo.userName);
}
//点击私聊弹出聊天框并取消最新红点
const TurnFriendsToMessageBox = (friends: any) => {
    if (friends.isNew == 1) {
        let servicePayLoad = {
            username: userInfo.userName,
            targetname: friends.targetfont,
            xusername: userInfo.userName
        }
        service.Message?.PostRedbob(servicePayLoad)
        friends.isNew = 0
        Pmsg.messageItems.find(item => item.targetUserName == friends.targetfont)!.unReadCount = 0
    }
    if (friends.Type == 'person') {
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
                    targetUserMessage: reactive(element),
                    tabType: 0,
                    tabId: friends.targetId
                })
            }
        });
        if (flag1) return
        Pmsg.messageItems.push(
            {
                targetUserName: friends.targetfont,
                messageContent: [],
                messageNames: [],
                messageHeaders: [],
                unReadCount: 1,
            }
        )
        chatStore.targetUserTab.push({
            tabTitle: friends.targetfont,
            tabName: friends.targetfont,
            tabType: 0,
            tabId: friends.userId,
            targetUserMessage: reactive(Pmsg.messageItems[Pmsg.messageItems.length - 1])
        })
    }
    else if (friends.Type == 'group') {
        let flag1 = false//判断消息存储库
        const data: UserGroup = groupStore.MyGroups.find(item => item.GroupId == friends.targetId) as UserGroup
        groupStore.OnConnectedGroup.GroupInfo = data.Group
        Pmsg.messageItems.forEach(element => {
            if (element.targetUserName == data.Group.GroupName) {
                flag1 = true
                for (let i = 0; i < chatStore.targetUserTab.length; i++) {
                    if (chatStore.targetUserTab[i].tabName == data.Group.GroupName) {
                        return
                    }
                }

                chatStore.targetUserTab.push({
                    tabTitle: data.Group.GroupName,
                    tabName: data.Group.GroupName,
                    targetUserMessage: reactive(element),
                    tabType: 1,
                    tabId: data.Group.GroupId
                })
            }

        });
        if (flag1) return
        Pmsg.messageItems.push(
            {
                targetUserName: data.Group.GroupName,
                messageContent: [],
                messageNames: [],
                messageHeaders: [],
                unReadCount: 1,
            }
        )
        chatStore.targetUserTab.push({
            tabTitle: data.Group.GroupName,
            tabName: data.Group.GroupName,
            tabType: 1,
            tabId: data.Group.GroupId,
            targetUserMessage: reactive(Pmsg.messageItems[Pmsg.messageItems.length - 1])
        })
    }

}


//获取一个好友的详细信息
const GetDetailInformation = (name: any) => {
    service.User?.GetUserInfo(name);
}

//点击好友更多时触发的选择事件
const handleCommand = (command: any) => {
    let commands = command.split(',')
    if (commands[0] == "show") {
        GetDetailInformation(commands[1])
        DetailVisible.value = true
    }
    else if (commands[0] == "delete") {
        service.Message?.DeleteMessageBoxItem(commands[1], userInfo.userName);
    }
}
</script>
<template>
    <v-contextmenu ref="contextmenu">
        <v-contextmenu-item>删除对话</v-contextmenu-item>
        <v-contextmenu-item>查看资料</v-contextmenu-item>
    </v-contextmenu>
    <div v-for="(req, _index) in MsgBox.MsgItems">
        <div v-if="req.Type == 'person'" class="FriendMSGContent" v-on:click="TurnFriendsToMessageBox(req)"
            v-contextmenu:contextmenu>
            <div id="friendReq-Header">
                <el-badge is-dot :hidden="!req.isNew">
                    <img v-if="req.Type == 'person'" v-bind:src="'../../../src/images/systemHeader/' + req.targetImage"
                        id="friendReq-Header-img" />
                    <img v-else src="../../../../images/assets/群默认头像.svg" alt=""
                        id="friendReq-Header-img">
                </el-badge>
            </div>
            <div class="friendMSG-Body">
                <div class="friendMSG-Body-coentent">
                    <div class="friendMSG-Body-coentent-inner">
                        <span class="friendMSG-name">{{ req.targetfont }}</span>
                        <span class="friendMSG-time">{{ req.lastTime }}</span>
                    </div>
                    <div class="friendMSG-Body-coentent-inner" v-if="req.isNew">
                        <span class="friendMSG-msg">{{
                            Pmsg.messageItems.find(item => item.targetUserName ==
                                req.targetfont)?.messageContent.at(-1)?.message
                        }}</span>
                        <el-badge
                            :value="Pmsg.messageItems.find(item => item.targetUserName == req.targetfont)?.unReadCount"
                            class="item" type="primary">
                        </el-badge>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="req.Type == 'group'" class="FriendMSGContent" v-on:click="TurnFriendsToMessageBox(req)" v-contextmenu:contextmenu>
            <div id="friendReq-Header">
                <el-badge is-dot :hidden="!req.isNew">
                    <img src="../../../../images/assets/群默认头像.svg" alt="" id="friendReq-Header-img">
                </el-badge>
            </div>
            <div class="friendMSG-Body">
                <div class="friendMSG-Body-coentent">
                    <div class="friendMSG-Body-coentent-inner">
                        <span class="friendMSG-name">{{ req.targetfont }}</span>
                        <span class="friendMSG-time">{{ req.lastTime }}</span>
                    </div>
                    <div class="friendMSG-Body-coentent-inner" v-if="req.isNew">
                        <span class="friendMSG-msg">{{
                            Pmsg.messageItems.find(item => item.targetUserName == req.targetfont)?.messageNames.at(-1)
                            }}:
                            {{
                                (Pmsg.messageItems.find(item => item.targetUserName ==
                                    req.targetfont)?.messageContent.at(-1))?.message
                            }}</span>
                        <el-badge
                            :value="Pmsg.messageItems.find(item => item.targetUserName == req.targetfont)?.unReadCount"
                            class="item" type="primary">
                        </el-badge>
                    </div>

                </div>
            </div>
        </div>
    </div>
    <el-dialog v-model="DetailVisible" width="380px" id="addUser" center draggable>
        <template #header>
            <div class="DetailInfo-header">
                <div class="DetailInfo-header-img">
                    <img :src="'../../../../src/images/systemHeader/' + friendStore.TargetUserProfile?.HeaderImg"
                        alt="">
                </div>
                <div class="DetailInfo-header-content">
                    <span v-if="friendStore.TargetUserProfile?.NickName" class="DetailInfo-header-content-font1">{{
                        friendStore.TargetUserProfile?.NickName }}</span>
                    <span v-else class="DetailInfo-header-content-font1">{{ friendStore.TargetUserProfile?.Username
                        }}</span>
                    <span class="DetailInfo-header-content-font2">{{ friendStore.TargetUserProfile?.Job }}</span>
                    <span class="DetailInfo-header-content-font3">暂无个性签名</span>
                </div>
            </div>
        </template>
        <div class="DetailInfo-content">
            <el-descriptions title="详细信息" direction="vertical" :column="2" border>
                <el-descriptions-item label="账户">{{ friendStore.TargetUserProfile?.Username }}</el-descriptions-item>
                <el-descriptions-item label="电话号">{{ friendStore.TargetUserProfile?.Phone }}</el-descriptions-item>
                <el-descriptions-item label="居住地">{{ friendStore.TargetUserProfile?.City }}</el-descriptions-item>
                <el-descriptions-item label="邮箱">
                    <el-tag size="small">{{ friendStore.TargetUserProfile?.Email }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="个人简介">{{ friendStore.TargetUserProfile?.Desc }}</el-descriptions-item>
            </el-descriptions>
        </div>
        <template #footer>
            <div id="addUser-footer">
                <el-button  style="width: 150px;">分享</el-button>
                <el-button type="primary" style="width: 150px;">
                    发送消息
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>
<style>
.friendMSG-Body-coentent-inner {
    display: flex;
    width: 100%;
    justify-content: space-between;
}

.friendMSG-msg {
    width: 120px;
    height: 20px;
    overflow: hidden;
    font-size: 12px;
    margin-bottom: 5px;
    color: #606266;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.friendMSG-time {
    color: #606266;
    font-size: 11px;
}

.friendMSG-name {
    height: 20px;
    overflow: hidden;
    font-size: 16px;
    margin-bottom: 5px;
    text-overflow: ellipsis;
    white-space: nowrap;
    color: #000000;
}

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
    width: 45px;
    height: 45px;
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
    border-bottom: 1px solid #eeecec;
    display: flex;
}

.FriendMSGContent:hover {
    background-color: #f5f5f5;
    transition: 0.3s ease-in-out all;
    cursor: pointer;
}

.friendMSG-Body {
    width: 80%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    margin-left: -8px;
}

.friendMSG-Body-coentent {
    width: 85%;
    height: 80%;
    font-size: 15px;
    margin-left: -10px;
    display: flex;
    flex-direction: column;

}
</style>
