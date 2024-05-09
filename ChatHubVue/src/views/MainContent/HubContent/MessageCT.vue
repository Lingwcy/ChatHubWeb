<script setup lang="ts">
import { nextTick, ref, watch } from 'vue'
import { ArrowRight, Search } from '@element-plus/icons-vue'
import { UseChatStore } from '../../../store/index'
import groupMemeber from '../../Compoents/groupMemeber.vue'
import { ElNotification, ElScrollbar } from 'element-plus'
//import {ChatHubService} from '../../../services/ServicesCollector'
import { appsetting, UseServiceStore, UseUserInformationStore, UseGroupStore } from '../../../store/index'
import { Group } from '../../../store/Istore'
import GroupInfoDetail from '../../Compoents/GroupInfoDetail.vue'
import { submitUpload } from './Tools/PicUploadHook'
import SendEditor from '../../Compoents/SendEditor.vue'
import TextElemItem from './componets/TextElemItem.vue'
import ImageElemItem from './componets/ImageElemItem.vue'
const appset = appsetting() //系统设置库
const groupStore = UseGroupStore() //群组信息库
const service = UseServiceStore();
const userInfo = UseUserInformationStore() //用户信息库
const chatStore = UseChatStore() //主Chat库
//createChatHubService(userInfo.jwtToken, appset.ServerHubAddress);
//控制发送消息后聊天区域一直为底部
const scrollbarRef = ref()
const groupNoticeOpen = ref(false)
const groupNoticeText = ref('')
const groupNoticeAble = ref(true)
const groupNoticeConfirmText = ref('确认')
const scrollDown = () => {
    setTimeout(() => {
        nextTick(() => {
            scrollbarRef.value.forEach((element: { setScrollTop: (arg0: number) => void }) => {
                element.setScrollTop(999999)
            });
        })
    }, 100)
}
/*
    触发Tab-Click
    作用：将目标用户的聊天信息从状态库中渲染到Tab聊天区域
*/
const OnClickChatTab = (target: any) => {
    service.ChatHub?.PrintMessageToTab(target.props.label, target.props.name)
    //如果traget.Type 是1 那么就是群组，此时就根据targetId去寻找groupStore.Mygroups中对应的群组
    //然后将找到的群组内容赋值给groupStore.OnconectedGroup
    groupStore.OnConnectedGroup.GroupInfo = groupStore.MyGroups.find(item => item.GroupId == target.props.name)?.Group as unknown as Group
    //判断groupStore.OnConnectedGroup.GroupInfo是否是undefined
    if (!(typeof (groupStore.OnConnectedGroup.GroupInfo) == 'undefined')) {
        service.Group?.GetGroupMemberList(target.props.name, userInfo.userName, groupStore)
    } else {

    }

    //延迟1秒执行这个方法，以便渲染完成后再滚动到底部
    setTimeout(() => {
        scrollDown()
    }, 100)
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
    submitUpload()
    scrollDown()
}

const openGroupNotice = () => {
    groupNoticeOpen.value = true;
    groupNoticeText.value = groupStore.OnConnectedGroup.GroupInfo.GroupDescription;
    const myRole = groupStore.OnConnectedGroup.GroupMemebers.find(item => item.id == Number(userInfo.userId))?.Role
    console.log(myRole)
    if (myRole == '群主' || myRole == '管理员') {
        groupNoticeAble.value = false;
        groupNoticeConfirmText.value = '确认修改'
    }
}
const GroupNoticeConfirm = () => {
    if (groupNoticeConfirmText.value == '确认') return;
    groupNoticeOpen.value = false;
    groupStore.OnConnectedGroup.GroupInfo.GroupDescription = groupNoticeText.value
    let payload = {
        Notice: groupNoticeText.value,
        GroupId: groupStore.OnConnectedGroup.GroupInfo.GroupId,
        UserId: Number(userInfo.userId),
        xusername: userInfo.userName,
    }
    service.Group?.ChangeGroupNotice(payload)
        .then((res) => {
            if (res) {
                groupStore.OnConnectedGroup.GroupInfo.GroupDescription = groupNoticeText.value;
                const group = groupStore.MyGroups.find(item => item.GroupId == groupStore.OnConnectedGroup.GroupInfo.GroupId)
                group!.Group.GroupDescription = groupNoticeText.value;
                ElNotification.info({
                    title: '修改成功',
                    message: '修改群公告成功',
                    duration: 2000,
                });
                //发送siganlR通知群内的其他人...
            }
        })
        .catch((err) => {
            console.log(err)
        })

}


//???
const editableTabsValue = ref('1')

//选中删除Tab
const removeTab = (targetName: any) => {
    chatStore.targetUserTab.forEach((value, index, array) => {

        if (value.tabId == targetName) {
            array.splice(index, 1)
        }
    });
}
//用watch监控appset.MessageContract.IsNewMessageCome是否变化
//如果变化，则说明有最新消息 然后比较正在对话的name是否与到来的消息的name相同
//如果相同，则下拉到聊天栏到底部，否则不做任何操作
watch(
    () => appset.MessageContract.IsNewMessageCome,
    (newVal, oldVal) => {
        // 在这里添加你希望在 IsNewMessageCome 变化时执行的代码  
        if (newVal) {
            if (appset.MessageContract.OnConnectedName = 'public') {
                scrollDown()
                return;
            }

            const targetUser = chatStore.targetUserTab.find(item => item.tabName == appset.MessageContract.OnConnectedName)
            if (targetUser) {
                // 如果找到了目标用户，则下拉到聊天栏到底部
                scrollDown()
            }
        }
    }
);
watch(
    () => appset.CompoentsEvent.isSendNewMessage,
    (newVal, oldVal) => {
        // 在这里添加你希望在 IsNewMessageCome 变化时执行的代码  
        if (newVal> oldVal) {
            if (appset.MessageContract.OnConnectedName = 'public') {
                scrollDown()
                return;
            }

            const targetUser = chatStore.targetUserTab.find(item => item.tabName == appset.MessageContract.OnConnectedName)
            if (targetUser) {
                // 如果找到了目标用户，则下拉到聊天栏到底部
                scrollDown()
            }
        }
    }
);
const msgType = (elem_type: any) => {
    let resp = "";
    switch (elem_type) {
        case "TIMTextElem":
            resp = "message-view__text"; // 文本
            break;
        case "TIMGroupTipElem":
            resp = "message-view__tips-elem"; // 群消息提示
            break;
        case "TIMImageElem":
            resp = "message-view__img"; // 图片消息
            break;
        case "TIMFileElem":
            resp = "message-view__file"; // 文件消息
            break;
        case "TIMGroupSystemNoticeElem":
            resp = "message-view__system"; // 系统通知
            break;
        case "TIMCustomElem":
            resp = "message-view__text message-view__custom"; // 自定义消息
            break;
        default:
            resp = "";
            break;
    }
    return resp;
};
const loadMsgModule = (item: any) => {;
    switch (item) {
        case "TIMTextElem":
            return TextElemItem; // 文本
        case "TIMImageElem":
           return ImageElemItem ; // 图片
    }
};
</script>

<template>
    <GroupInfoDetail />
    <div id="MCT-Container">
        <el-tabs v-model="editableTabsValue" type="card" class="msgTabs" closable @tab-remove="removeTab"
            @tab-click="OnClickChatTab">
            <el-tab-pane v-for="item in chatStore.targetUserTab" :key="item.tabId" :label="item.tabTitle"
                :name="item.tabId">
                <div class="msgTabContent">
                    <div class="msgTabContent-Normal">
                        <div id="MCT-Container-RESVContent">
                            <el-scrollbar class="h-full" ref="scrollbarRef">
                                <div class="message-view" ref="messageViewRef">
                                    <div v-for="(_msg, index) in item.targetUserMessage.messageContent"
                                        :class="{ 'reset-select': true }">
                                        <div class="message-view__item--blank"></div>
                                        <!-- 时间 -->
                                        <div v-if="true" class="message-view__item--time-divider">
                                        </div>
                                        <!-- 消息体 -->
                                        <div class="message-view__item" :class="[
                                            (userInfo.userName == item.targetUserMessage.messageNames[index]) ? 'is-self' : 'is-other',
                                            false ? 'style-choice' : '',
                                        ]">
                                            <div class="picture select-none" v-if="true">
                                                <el-avatar :size="36" shape="square"
                                                    :src="'src/images/systemHeader/' + item.targetUserMessage.messageHeaders[index]"
                                                    @error="() => true">
                                                    <img
                                                        :src="'src/images/systemHeader/' + item.targetUserMessage.messageHeaders[index]" />
                                                </el-avatar>
                                            </div>
                                            <div class="message-view__item--index">
                                                <div class="message_name">
                                                    <span>{{ item.targetUserMessage.messageNames[index] }}</span>
                                                </div>
                                                <div
                                                    :class="msgType(item.targetUserMessage.messageContent[index].messageType)">
                                                    <component
                                                        :message="item.targetUserMessage.messageContent[index].message"
                                                        :is="loadMsgModule(item.targetUserMessage.messageContent[index].messageType)" :self=true :key="index" status="success">
                                                    </component>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </el-scrollbar>
                        </div>
                        <div id="MCT-Container-SENDContent">
                            <SendEditor />
                        </div>
                    </div>
                    <div v-if="item.tabType == 1" class="msgTabContent-GroupOption">
                        <div class="msgTabContent-GroupOption-advertising">
                            <div class="msgTabContent-GroupOption-advertising-option"
                                style="border-bottom: 1px #e9e9eb solid;">

                                <img class="group-option" src="../../../images/assets/分类.svg" />
                                <img class="group-option" src="../../../images/assets/邀请.svg" />
                                <img class="group-option" src="../../../images/assets/设置.svg"
                                    @click="appset.CompoentsEvent.isGroupInfoDetailOpen = true" />

                            </div>
                            <div class="msgTabContent-GroupOption-advertising-title" @click="openGroupNotice()">
                                <p style="margin-left: 10px;">群公告</p>
                                <el-icon>
                                    <ArrowRight />
                                </el-icon>
                            </div>
                            <div class="msgTabContent-GroupOption-advertising-content">
                                <el-scrollbar height="120px" :always="false"
                                    class="msgTabContent-GroupOption-advertising-content-text">
                                    <p
                                        v-if="groupStore.OnConnectedGroup.GroupInfo && typeof groupStore.OnConnectedGroup.GroupInfo.GroupDescription !== 'undefined'">
                                        {{ groupStore.OnConnectedGroup.GroupInfo.GroupDescription }}</p>
                                </el-scrollbar>
                            </div>
                        </div>
                        <div class="msgTabContent-GroupOption-members">
                            <div class="msgTabContent-GroupOption-members-title">
                                <p style="margin-left: 10px; margin-right: 5px;">群聊成员 </p>
                                <p v-if="groupStore.OnConnectedGroup.GroupInfo && typeof groupStore.OnConnectedGroup.GroupInfo.MemberNumber !== 'undefined'"
                                    style=" margin-right: 25px;">{{ groupStore.OnConnectedGroup.GroupInfo.MemberNumber
                                    }}</p>
                                <el-icon :size="15" style="cursor: pointer;">
                                    <Search />
                                </el-icon>
                            </div>
                            <div class="msgTabContent-GroupOption-members-content">
                                <el-scrollbar height="59%" :always="false"
                                    class="msgTabContent-GroupOption-members-content-list">
                                    <groupMemeber />
                                </el-scrollbar>
                            </div>
                        </div>
                    </div>
                </div>
            </el-tab-pane>
        </el-tabs>
    </div>

    <el-dialog v-model="groupNoticeOpen" title="群公告" width="500" center>
        <div style="display: flex; justify-content: center; width: 100%; height: 100%;">
            <el-input :disabled="groupNoticeAble" v-model="groupNoticeText" style="width: 100%;height: 100%;"
                :autosize="{ minRows: 2, maxRows: 4 }" type="textarea" placeholder="" />
        </div>
        <template #footer>
            <div class="dialog-footer">
                <el-button @click="groupNoticeOpen = false">取消</el-button>
                <el-button type="primary" @click="groupNoticeOpen = false; GroupNoticeConfirm()">
                    {{ groupNoticeConfirmText }}
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<style lang="scss">
.message-view__tips-elem {
    margin: auto;
}

.message-view__tips-elem .message_name {
    display: none;
}

.message-view__item--index {
    max-width: 85%;
}

.message-info-view-content {
    height: calc(100% - 70px - 206px);
    border-bottom: 1px solid var(--color-border-default);

}

.style-MsgBox {
    height: calc(100% - 60px) !important;
}

.stlyle-Reply {
    height: calc(100% - 70px - 206px - 60px) !important;
}

.message-view__item--time-divider {
    user-select: none;
    position: relative;
    top: 8px;
    margin: 20px 0;
    max-height: 20px;
    text-align: center;
    font-weight: 400;
    font-size: 12px;
    color: var(--color-time-divider);
}

.message-view {
    display: flex;
    flex-direction: column;
    height: 100%;
    overflow-y: overlay;
    overflow-x: hidden;
    padding: 0 16px 16px 16px;
    box-sizing: border-box;

    .picture {
        --el-border-radius-base: 6px;
        cursor: pointer;
    }
}

.style-select {
    border-radius: 3px;
    background: var(--color-multiple-choice);
}

.reset-select {
    border-radius: 3px;
}

.style-choice {
    padding-left: 35px;
}

.message-view__item {
    display: flex;
    flex-direction: row;

    margin: 8px 0;
    position: relative;
}

.is-other {
    .picture {
        margin-left: 0;
        margin-right: 8px;
    }

    .message-view__img {
        margin-bottom: 5px;
        width: fit-content;
    }

    .message-view__file {
        margin-bottom: 5px;
    }

    .message-view__text {
        width: fit-content;
        margin-bottom: 5px;
    }
}

.is-self {
    flex-direction: row-reverse;
    display: flex;

    .picture {
        margin-right: 0;
        margin-left: 8px;
        width: 36px;
        height: 36px;
    }

    .message_name {
        display: none;
    }

    .message-view__img {
        display: flex;
        justify-content: flex-end;
        align-items: center;
    }

    .message-view__file {
        display: flex;
        justify-content: flex-end;
        align-items: center;
    }

    .message-view__text {
        display: flex;
        justify-content: flex-end;
        align-items: center;
    }
}

.v-contextmenu {
    width: 200px;

    .item {
        margin: 0;
    }
}

.message_name {
    margin-bottom: 5px;
    color: var(--color-time-divider);
    font-size: 12px;
}

.el-tabs__header {
    margin: 0;
}

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
    height: 94%;
}

.msgTabContent {
    width: 100%;
    display: flex;
}

.msgTabContent-Normal {
    width: 100%;
    height: 100%;

}

.msgTabContent-GroupOption {
    width: 200px;
    border-left: 1px #e9e9eb solid;
    display: flex;
    flex-direction: column;
}

.msgTabContent-GroupOption-advertising {
    height: 180px;
    width: 100%;
    display: flex;
    flex-direction: column;
    border-bottom: 1px #e9e9eb solid;
}

.msgTabContent-GroupOption-advertising-title {
    font-size: 14px;
    display: flex;
    align-items: center;
    height: 40px;
    width: 100%;
    cursor: pointer;
}

.msgTabContent-GroupOption-advertising-option {
    font-size: 14px;
    display: flex;
    align-items: center;
    justify-content: space-around;
    height: 40px;
    width: 100%;
    cursor: pointer;
}

.group-option {
    width: 20px;
    height: 20px;
    margin: 5px;
}

.msgTabContent-GroupOption-advertising-content-text {
    width: 90%;
    height: 90%;
}

.msgTabContent-GroupOption-advertising-content-text p {
    line-height: 1.7;
    font-size: 13px;
    color: #8f9092;
    margin-top: -5px;
}

.msgTabContent-GroupOption-advertising-content {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 120px;
}

.msgTabContent-GroupOption-members {
    height: 100%;
    width: 100%;
    display: flex;
    flex-direction: column;
}

.msgTabContent-GroupOption-members-title {
    font-size: 14px;
    display: flex;
    align-items: center;
    height: 40px;
    width: 100%;

}

.msgTabContent-GroupOption-members-content {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100%;
}

.msgTabContent-GroupOption-members-content-list {
    display: flex;
    height: 100%;
    width: 90%;
    justify-content: center;

}

.el-table tr {
    background-color: #f4f4f5;
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
    height: 55vh;
}

#MCT-Container-SENDContent {}

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

    background-color: rgb(255, 255, 255);
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
