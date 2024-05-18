<script lang="ts" setup>
import { Search, Plus, Tools, ArrowRight } from '@element-plus/icons-vue'
import { reactive, ref, watch } from 'vue'
import { appsetting, UseChatStore, UseGroupStore, UseFriendsStore, UseMsgStore, UseServiceStore, UseUserInformationStore } from '../../../../store';
import Add from '../../../Compoents/Add.vue';
import FriendManager from '../../../Compoents/FriendManager.vue';
import GroupRequestMessage from '../../../Compoents/GroupRequestMessage.vue';
import FriendRequestMessage from '../../../Compoents/FriendRequestMessage.vue';
import CreateGroup from '../../../Compoents/CreateGroup.vue';
import { createMessageService, createUserService, createFriendsService, createGroupService } from '../../../../services/ServicesCollector';
import { UserGroup } from '../../../../store/Istore';
let appset = appsetting();
createMessageService();
createUserService();
createGroupService();
createFriendsService();
const Pmsg = UseMsgStore()
const service = UseServiceStore();
const userInfo = UseUserInformationStore()
const friendStore = UseFriendsStore()
const chatStore = UseChatStore()
const groupStore = UseGroupStore()
const DetailVisible = ref(false)
const mainSearch = ref("")
const friendPayload = {
    userId: userInfo.userId,
    xusername: userInfo.userName
}
service.Friend?.FindFriendTree(friendPayload, friendStore)
service.Group?.GetGroupList(Number(userInfo.userId), userInfo.userName, groupStore)
const treeProps = {
    children: 'Children',
    label: 'UnitName'
}
const friendBtStyle = reactive({
    backgroundColor: ref("rgb(248, 250, 250)"),
})
const groupBtStyle = reactive({
    backgroundColor: "",
})
function friendSelected() {
    friendBtStyle.backgroundColor = "rgb(248, 250, 250)"
    groupBtStyle.backgroundColor = ""
    appset.CompoentsEvent.isContractSwitch.isFriendOpen = true
    appset.CompoentsEvent.isContractSwitch.isGroupOpen = false
}

function groupSelected() {
    friendBtStyle.backgroundColor = ""
    groupBtStyle.backgroundColor = "rgb(248, 250, 250)"
    appset.CompoentsEvent.isContractSwitch.isFriendOpen = false
    appset.CompoentsEvent.isContractSwitch.isGroupOpen = true
}


const testdataSource = ref([
    {
        id: 1,
        unitName: '游戏',
        children: [ // 注意这里将 userList 重命名为 children  
            {
                id: 1,
                username: "wcy", // 注意属性名应该是小写的，以符合 JavaScript 命名规范  
                headerImg: "head2.svg",
                email: "wcy",
                city: "wcy",
                sex: "wcy",
                age: "wcy",
                job: "wcy",
                phone: "wcy",
                nickName: "wcy",
                birth: "wcy",
                desc: "now you see me",
                status: 1,
            },
            {
                id: 2,
                username: "junjie",
                headerImg: "head3.svg",
                email: "wcy",
                city: "wcy",
                sex: "wcy",
                age: "wcy",
                job: "wcy",
                phone: "wcy",
                nickName: "wcy",
                birth: "wcy",
                desc: "aaaaa wtf!!",
                status: 1,
            },
            // ... 其他用户  
        ],
    },
    {
        id: 2,
        unitName: '同学',
        children: [
            {
                id: 1,
                username: "戚风", // 注意属性名应该是小写的，以符合 JavaScript 命名规范  
                headerImg: "head5.svg",
                email: "wcy",
                city: "wcy",
                sex: "wcy",
                age: "wcy",
                job: "wcy",
                phone: "wcy",
                nickName: "wcy",
                birth: "wcy",
                desc: "很困",
                status: 1,
            },
        ]
    }
    // ... 其他单元  
]);

//点击好友更多时触发的选择事件
const handleCommand = (command: any) => {
    let commands = command.split(',')
    if (commands[0] == "show") {
        GetDetailInformation(commands[1])
        DetailVisible.value = true
    }
    else if (commands[0] == "communicate") {
    }
}
//获取一个好友的详细信息
const GetDetailInformation = (name: any) => {
    service.User?.GetUserInfo(name);
}

const TurnFriendsToMessageBox = (data: any) => {
    service.Friend?.GetUserFriends(userInfo.userId, userInfo.userName, friendStore)
    let flag1 = false//判断消息存储库
    Pmsg.messageItems.forEach(element => {
        if (element.targetUserName == data.Username) {
            flag1 = true
            for (let i = 0; i < chatStore.targetUserTab.length; i++) {
                if (chatStore.targetUserTab[i].tabName == data.Username) {
                    return
                }
            }

            chatStore.targetUserTab.push({
                tabTitle: data.Username,
                tabName: data.Username,
                targetUserMessage: reactive(element),
                tabType:0,
                tabId:data.id
            })
        }

    });
    if (flag1) return
    Pmsg.messageItems.push(
        {
            targetUserName: data.Username,
            messageContent: [],
            messageNames: [],
            messageHeaders: [],
        }
    )
    chatStore.targetUserTab.push({
        tabTitle: data.Username,
        tabName: data.Username,
        tabType:0,
        tabId:data.id,
        targetUserMessage: reactive(Pmsg.messageItems[Pmsg.messageItems.length - 1])
    })
}
const TurnGroupToMessageBox = (data: UserGroup) => {
    let flag1 = false//判断消息存储库
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
                tabType:1,
                tabId:data.Group.GroupId
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
        }
    )
    chatStore.targetUserTab.push({
        tabTitle: data.Group.GroupName,
        tabName: data.Group.GroupName,
        tabType:1,
        tabId:data.Group.GroupId,
        targetUserMessage: reactive(Pmsg.messageItems[Pmsg.messageItems.length - 1])
    })
}

watch(
    () => appset.CompoentsEvent.isFriendReqestAccepted,
    (newVal, oldVal) => {
        if (newVal > oldVal) {
            service.Friend?.FindFriendTree(friendPayload, friendStore)
        }
    }
);
const visible = ref(false)
</script>
<template>
    <div id="main">
        <Add />
        <FriendManager />
        <FriendRequestMessage/>
        <GroupRequestMessage />
        <CreateGroup/>
        <div id="main-content">
            <div class="main-search">
                <el-input v-model="mainSearch" style="width: 210px; height: 30px;" placeholder="搜索"
                    :prefix-icon="Search" />
                <el-popover :visible="visible" placement="top" :width="160">
                    <div id="addContent">
                        <el-button size="small" text @click="visible = false; appset.CompoentsEvent.isGroupCreateOpen = true" >发起群聊</el-button>
                        <el-button size="small" text @click="visible = false; appset.CompoentsEvent.isAddOpen = true"
                            style="margin: 0;">添加好友/群</el-button>
                    </div>
                    <template #reference>
                        <el-button @click="visible = true" :icon="Plus" style="height: 29px; width: 30px;"></el-button>
                    </template>
                </el-popover>
            </div>
            <div class="main-modify">
                <el-button size="large" :icon="Tools" style="width: 100%; height: 30px;"
                    onclick="openFriendManager()">好友管理</el-button>
            </div>
            <div class="main-request">
                <div class="main-request-content" @click="appset.CompoentsEvent.isFriendRequestMessageOpen = true">
                    <div class="left">好友通知</div>
                    <div class="right"><el-icon>
                            <ArrowRight />
                        </el-icon></div>
                </div>
                <div class="main-request-content" @click="appset.CompoentsEvent.isGroupRequestMessageOpen = true">
                    <div class="left">群通知</div>
                    <div class="right"><el-icon>
                            <ArrowRight />
                        </el-icon></div>
                </div>
            </div>
            <div class="main-contract-list">
                <div class="switch">
                    <div class="switch-item-friend" v-bind:style="friendBtStyle" v-on:click="friendSelected()">
                        <div>好友</div>
                    </div>
                    <div class="switch-item-group" v-bind:style="groupBtStyle" v-on:click="groupSelected()">
                        <div>群聊</div>
                    </div>
                </div>
                <div style="height: 20px;"></div>
                <el-scrollbar v-if="appset.CompoentsEvent.isContractSwitch.isFriendOpen" height="400px"
                    id="treeContent">
                    <el-tree style="width: 220px;background-color: #fafafa;" :data="friendStore.FriendTree?.Units"
                        :props="treeProps">
                        <template #default="{ node, data }">
                            <span class="mytree">
                                <span>{{ data.UnitName }}</span> <!-- 假设 unitName 是节点标签 -->
                                <span v-if="node.isLeaf && node.level != 1"> <!-- 这里判断是否是叶子节点，不显示任何内容 -->
                                    <div class="friends-list-item">
                                        <div class="friends-list-item-img">
                                            <img v-bind:src="'src/images/systemHeader/' + data.HeaderImg" alt="">
                                        </div>
                                        <div class="friends-list-item-font">
                                            <span style="margin-left: 0px; font-size: 15px;" v-if="data.Remark">{{
                    data.remark }}</span>
                                            <span style="margin-left: 0px; font-size: 15px;" v-else>{{ data.Username
                                                }}</span>
                                            <span style="margin-left: 0px; font-size: 12px;"> [<svg t="1712725667344"
                                                    class="icon" viewBox="0 0 1024 1024" version="1.1"
                                                    xmlns="http://www.w3.org/2000/svg" p-id="6052" width="10"
                                                    height="10">
                                                    <path d="M512 512m-512 0a512 512 0 1 0 1024 0 512 512 0 1 0-1024 0Z"
                                                        fill="#1296db" opacity=".3" p-id="6053"></path>
                                                    <path
                                                        d="M512 512m-307.2 0a307.2 307.2 0 1 0 614.4 0 307.2 307.2 0 1 0-614.4 0Z"
                                                        fill="#1296db" p-id="6054"></path>
                                                </svg>在线] {{ data.Desc }}</span>
                                        </div>
                                        <div class="friends-list-item-option">
                                            <el-dropdown @command="handleCommand">
                                                <span class="el-dropdown-link">
                                                    <svg t="1671181070434" class="icon" viewBox="0 0 1024 1024"
                                                        version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="2957"
                                                        width="20" height="20">
                                                        <path
                                                            d="M512 704c35.2 0 64 28.8 64 64s-28.8 64-64 64-64-28.8-64-64 28.8-64 64-64z m-64-192c0 35.2 28.8 64 64 64s64-28.8 64-64-28.8-64-64-64-64 28.8-64 64z m0-256c0 35.2 28.8 64 64 64s64-28.8 64-64-28.8-64-64-64-64 28.8-64 64z"
                                                            p-id="2958"></path>
                                                    </svg>
                                                </span>
                                                <template #dropdown>
                                                    <el-dropdown-menu>
                                                        <el-dropdown-item
                                                            :command="'show,' + data.Username">查看资料</el-dropdown-item>
                                                        <el-dropdown-item :command="'communicate,' + data.Username"
                                                            @click="TurnFriendsToMessageBox(data)">发起会话</el-dropdown-item>
                                                    </el-dropdown-menu>
                                                </template>
                                            </el-dropdown>
                                        </div>
                                    </div>
                                </span>
                            </span>
                        </template>
                    </el-tree>
                </el-scrollbar>
                <el-scrollbar v-if="appset.CompoentsEvent.isContractSwitch.isGroupOpen" height="400px">
                    <div  class="group-item" v-for="(item, index) in groupStore.MyGroups" :key="index" @click="TurnGroupToMessageBox(item)">
                        <div class="group-item-content">
                            <div class="group-item-content-img">
                                <img src="../../../../images/assets/群默认头像.svg" alt="">
                            </div>
                            <div class="group-item-content-font">
                                <span>{{ item.Group.GroupName}}</span>
                            </div>
                        </div>
                    </div>
                </el-scrollbar>
            </div>
        </div>
    </div>


    <el-dialog v-model="DetailVisible" width="380px" id="addUser" center draggable>
        <template #header>
            <div class="DetailInfo-header">
                <div class="DetailInfo-header-img">
                    <img style=""
                        :src="'../../../../src/images/systemHeader/' + friendStore.TargetUserProfile?.HeaderImg" alt="">
                </div>
                <div class="DetailInfo-header-content">
                    <span v-if="friendStore.TargetUserProfile?.NickName" class="DetailInfo-header-content-font1">{{
                    friendStore.TargetUserProfile?.NickName }}</span>
                    <span v-else class="DetailInfo-header-content-font1">{{ friendStore.TargetUserProfile?.Username
                        }}</span>
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
<style scoped>
.DetailInfo-header-img{
    width: 100px;
    height: 100px;
    border-radius: 50%;
    margin-left: 110px;
}
.group-item{
    display: flex;
    justify-content: center;
    align-items: center;
    height:40px;
    width: 200px;
    border-bottom: 1px solid #e3e3e7;
    padding-bottom: 5px;
    cursor: pointer;
}
.group-item:hover{
    background-color: #f5f5f5;
    transition: all 0.3s ease;
}
.group-item-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 100%;
    width: 100%;
}

.group-item-content-img {
    border-radius: 50%;
    height: 100%;
    width: 40px;
    display: flex;
    align-items: center;
    justify-content: center;
    overflow: hidden;
    flex-direction: column;
}

.group-item-content-img img {
    width: 35px;
    height: 35px;
}

.group-item-content-font {
    width: 55%;
    height: 100%;
    text-overflow: ellipsis;
    display: flex;
    flex-direction: column;
    line-height: 21px;
    overflow: hidden;
    color: #606266;
    font-size: 14px;
    font-family: 幼圆;
    margin-top: 1px;
    margin-right: 38px;
    font-size: 12px;
}
:deep(.el-tree-node__content:last-child) {
    height: auto;
    padding-left: 0;
    padding: 0 !important;
}

:deep(.el-tree-node__content>.el-tree-node__expand-icon) {
    padding: 0 !important;
}

#friends-header {
    width: 100%;
    height: 35px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    flex-wrap: wrap;
    border-bottom: 1px solid #e3e3e7;



}

#friends-list {
    width: 100%;
    height: 95vh;
    display: flex;
    flex-direction: column;
    overflow: hidden;
    transition: all 0.3s ease-in-out;
}

.friends-list-item {
    width: 200px;
    height: 45px;
    border-bottom: 1px solid rgb(221, 217, 217);
    display: flex;
    margin-top: 3px;
    padding-bottom: 2px;
}

.friends-list-item:hover {
    background-color: #cfd1d6;
    transition: all 0.3s ease;
    cursor: pointer;
}

.friends-list-item:active {
    background-color: #515253;
    transition: all 0.5s ease;
}

.friends-list-item-img {
    width: 25%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
}

.friends-list-item-img img {
    width: 45px;
}

.friends-list-item-font {
    width: 55%;
    height: 100%;
    text-overflow: ellipsis;
    display: flex;
    flex-direction: column;
    margin-left: 5px;
    line-height: 21px;
    overflow: hidden;
    color: #606266;
    font-size: 14px;
    font-family: 幼圆;
    margin-top: 1px;

}

.friends-list-item-option {
    width: 15%;
    display: flex;
    justify-content: center;
    align-items: center;
}

/* 去掉最顶层的虚线，放最下面样式才不会被上面的覆盖了 */
:deep(&>.el-tree-node::after) {
    border-top: none;
}

:deep(&>.el-tree-node::before) {
    border-left: none;
}


#addContent {
    display: flex;
    flex-direction: column;
}

.switch {
    height: 35px;
    width: 100%;
    background-color: rgb(235, 233, 233);
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-radius: 3px;
    cursor: pointer;
    transition: 0.3s all ease-in-out;
}

.switch-item-friend {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-left: 5px;
    margin-right: 5px;
    width: 48%;
    height: 80%;
    border-radius: 5px;
    transition: 0.3s all ease-in-out;
}

.switch-item-group {
    display: flex;
    justify-content: center;
    align-items: center;
    margin-left: 5px;
    margin-right: 5px;
    width: 48%;
    height: 80%;
    border-radius: 5px;
    transition: 0.3s all ease-in-out;
}

#main {
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;

}

#main-content {
    width: 90%;
    height: 98%;
    display: flex;
    flex-direction: column;
}

.main-item {
    height: 20%;
    width: 100%;

}

.main-search {
    height: 30px;
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.main-modify {
    height: 40px;
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.main-request {
    height: 80px;
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.main-request-content {
    display: flex;
    width: 100%;
    height: 35px;
    align-items: center;
    justify-content: space-between;
    font-size: 14px;
}

.main-request-content:hover {
    background-color: rgb(245, 245, 245);
    border-radius: 5px;
    transition: 0.3s all ease-in-out;
    cursor: pointer;
}

.main-contract-list {
    margin-top: 0px;
    display: flex;
    width: 100%;
    height: 500px;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    font-size: 14px;
}
</style>