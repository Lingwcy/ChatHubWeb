<template>
    <el-drawer v-model="appsetStore.CompoentsEvent.isGroupInfoDetailOpen" :size="300" :with-header="false"
        :before-close="handleClose">
        <el-scrollbar height="800px" class="group-info-detail">
            <div class="group-info-detail-header">
                <div class="group-info-detail-title">
                    <div class="group-info-detail-title-imgcontent">
                        <img class="group-info-detail-title-img" src="../../images/assets/群默认头像.svg" />
                    </div>
                    <div class="group-info-detail-title-name">
                        <span class="group-info-detail-title-name-text1">{{
        groupStore.OnConnectedGroup.GroupInfo.GroupName }}</span>
                        <div class="group-info-detail-title-name-text2">
                            <span style="font-size: 11.4px;">{{ groupStore.OnConnectedGroup.GroupInfo.GroupId }}</span>
                        </div>
                    </div>
                </div>
                <div class="group-info-detail-share">
                    <el-button size="small" color="#626aef" plain :icon="Grid" style="height: 30px;">分享</el-button>
                </div>
            </div>
            <div class="group-info-detail-memeber">
                <div class="group-info-detail-memeber-title">
                    <span>群聊成员</span>
                    <span class="group-info-detail-memeber-title-num">查看{{
        groupStore.OnConnectedGroup.GroupInfo.MemberNumber }}名群成员<el-icon>
                            <ArrowRight />
                        </el-icon></span>
                </div>
                <el-scrollbar style="max-height: 185px;">
                    <div class="group-info-detail-memeber-content">
                        <div class="group-info-detail-memeber-content-item"
                            v-for="(item, index) in groupStore.OnConnectedGroup.GroupMemebers">
                            <div class="group-info-detail-memeber-content-item-img">
                                <img v-bind:src="'src/images/systemHeader/' + item.HeaderImg" alt="">
                            </div>
                            <div class="group-info-detail-memeber-content-item-name">
                                <span>{{ item.name }}</span>
                            </div>
                        </div>
                        <div class="group-info-detail-memeber-content-item">
                            <div class="group-info-detail-memeber-content-item-img">
                                <div>
                                    <el-icon>
                                        <Plus />
                                    </el-icon>
                                </div>
                            </div>
                            <div class="group-info-detail-memeber-content-item-name">
                                <span style="line-height: 0; font-size: 11.7px;">邀请</span>
                            </div>
                        </div>
                        <div class="group-info-detail-memeber-content-item"
                            v-if="group?.Role == '群主' || group?.Role == '管理员'">
                            <div class="group-info-detail-memeber-content-item-img">
                                <div>
                                    <el-icon>
                                        <Close />
                                    </el-icon>
                                </div>
                            </div>
                            <div class="group-info-detail-memeber-content-item-name">
                                <span style="line-height: 0; font-size: 11.7px;">移出</span>
                            </div>
                        </div>
                    </div>
                </el-scrollbar>
            </div>
            <div v-if="group?.Role == '群主'" class="group-info-detail-option">
                <div class="group-info-detail-option-name">
                    <span>群聊名称</span>
                </div>
                <div class="group-info-detail-option-btn">
                    <el-input v-model="groupName" />
                </div>
            </div>
            <div class="group-info-detail-option">
                <div class="group-info-detail-option-name">
                    <span>我的本群昵称</span>
                </div>
                <div class="group-info-detail-option-btn">
                    <el-input v-model="myGroupNickName" />
                </div>
            </div>
            <div class="group-info-detail-option">
                <div class="group-info-detail-option-name">
                    <span>群聊备注</span>
                </div>
                <div class="group-info-detail-option-btn">
                    <el-input v-model="groupRemark" placeholder="填写备注" />
                </div>
            </div>
            <button class="group-info-detail-option-btns">
                <span>清空聊天记录</span>
            </button>
            <button v-if="group?.Role == '成员' || group?.Role == '管理员'" class="group-info-detail-option-btns"
                style="justify-content: center; color: red;" @click="exitGroup">
                退出群聊
            </button>
            <button v-else-if="group?.Role == '群主'" class="group-info-detail-option-btns"
                style="justify-content: center; color: red;" @click = "dismissGroup">
                解散群聊
            </button>
        </el-scrollbar>
    </el-drawer>
</template>

<script lang="ts" setup>

import { appsetting, UseGroupStore, UseServiceStore, UseUserInformationStore,UseChatStore } from '../../store';
import { ref, watch } from 'vue';
import { Grid, ArrowRight, Plus, Close } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox, ElNotification } from 'element-plus';
import router from '../../router';
const appsetStore = appsetting();
const userInformationStore = UseUserInformationStore();
const groupStore = UseGroupStore();
const serviceStore = UseServiceStore();
const chatStore = UseChatStore();
const groupName = ref('')
const myGroupNickName = ref('')
const groupRemark = ref('')
//根据groupStore.OnConnectedGroup.GroupInfo.GroupId拿到MyGroups里面的对于的group
let group = ref<any>(null);
const handleClose = () => {

    ElMessageBox.confirm('确定要提交已更改的数据吗?')
        .then(async () => {
            if (groupName.value !== groupStore.OnConnectedGroup.GroupInfo.GroupName && groupName.value !== '') {
                let payload = {
                    ChangedName: groupName.value,
                    GroupId: groupStore.OnConnectedGroup.GroupInfo.GroupId,
                    UserId: Number(userInformationStore.userId),
                    xusername: userInformationStore.userName,
                }
                const f1 = await serviceStore.Group?.ChangeGroupName(payload);
                if (f1) {
                    ElNotification.info({
                        title: '提示',
                        message: '修改群信息操作成功',
                        duration: 2000
                    });
                    //刷新前台信息
                    groupStore.OnConnectedGroup.GroupInfo.GroupName = groupName.value;
                    //发送siganlR通知群内的其他人...
                }
            }
            appsetStore.CompoentsEvent.isGroupInfoDetailOpen = false;
        })
        .catch(() => {
            // catch error
            appsetStore.CompoentsEvent.isGroupInfoDetailOpen = false;
        })
}

const exitGroup = () => {
    ElMessageBox.confirm(
        '退出群组后，您将失去该群组的所有内容和联系，确定要退出吗?',
        '警告',
        {
            confirmButtonText: '确认',
            cancelButtonText: '取消',
            type: 'warning',
        }
    )
        .then(() => {
            const payload = {
                GroupId: groupStore.OnConnectedGroup.GroupInfo.GroupId,
                UserId: Number(userInformationStore.userId),
                xusername: userInformationStore.userName,
            }
            serviceStore.Group?.ExitGroup(payload).then((res) => {
                if (res) {
                    ElMessage({
                        message: '退出群组成功.',
                        type: 'success',
                    })
                    //退出群组之后的一系列操作...
                    appsetStore.CompoentsEvent.isGroupInfoDetailOpen = false;
                    //删除MyGroups里面的group
                    groupStore.MyGroups = groupStore.MyGroups.filter(item => item.GroupId !== groupStore.OnConnectedGroup.GroupInfo.GroupId);
                    chatStore.targetUserTab = chatStore.targetUserTab.filter(item => item.tabId !== groupStore.OnConnectedGroup.GroupInfo.GroupId);
                    
                }
            })
        })
        .catch(() => {
        })
}

const dismissGroup = () => {
    ElMessageBox.confirm(
        '解散群组后，所有群组成员将失去该群组的所有内容和联系，确定要解散吗?',
        '警告',
        {
            confirmButtonText: '确认',
            cancelButtonText: '取消',
            type: 'warning',
        }
    )
        .then(() => {
            const payload = {
                GroupId: groupStore.OnConnectedGroup.GroupInfo.GroupId,
                UserId: Number(userInformationStore.userId),
                xusername: userInformationStore.userName,
            }
            serviceStore.Group?.DismissGroup(payload).then((res) => {
                if (res) {
                    ElMessage({
                        message: '退出群组成功.',
                        type: 'success',
                    })
                    //退出解散之后的一系列操作...
                    appsetStore.CompoentsEvent.isGroupInfoDetailOpen = false;
                    //删除MyGroups里面的group
                    groupStore.MyGroups = groupStore.MyGroups.filter(item => item.GroupId !== groupStore.OnConnectedGroup.GroupInfo.GroupId);
                    chatStore.targetUserTab = chatStore.targetUserTab.filter(item => item.tabId !== groupStore.OnConnectedGroup.GroupInfo.GroupId);
                    
                }
            })
        })
        .catch(() => {
        })
}
watch(
    () => appsetStore.CompoentsEvent.isGroupInfoDetailOpen,
    (newVal, oldVal) => {
        // 在这里添加你希望在 IsNewMessageCome 变化时执行的代码  
        if (newVal) {
            if (appsetStore.CompoentsEvent.isGroupInfoDetailOpen == true) {
                setTimeout(() => {
                    group.value = groupStore.MyGroups.find(item => item.GroupId === groupStore.OnConnectedGroup.GroupInfo.GroupId)
                    groupName.value = groupStore.OnConnectedGroup.GroupInfo.GroupName;
                }, 300)
                return;
            }
            if (appsetStore.CompoentsEvent.isGroupInfoDetailOpen == false) {
                //注销属性
                groupName.value = '';
                myGroupNickName.value = '';
                groupRemark.value = '';
                group.value = null;
                console.log("sss")
            }
        }
    }
);

</script>

<style scoped>
.group-info-detail {
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: justify-center;
}

.group-info-detail-memeber-title-num {
    color: #606266;
    font-size: 12.4px;
    font-family: 幼圆;
    cursor: pointer;
}

:deep(.el-input:hover) {
    --el-input-hover-border-color: null;
}

:deep(.el-input__wrapper) {
    box-shadow: none;
    border-radius: 10px;
    transition: border-bottom 0.3s ease-in-out;
    background-color: unset;
    border-bottom: none;
}

:deep(.el-input__wrapper.is-focus) {
    box-shadow: none;
    border-bottom: 1px solid #626aef;
    transition: border-bottom 0.3s ease-in-out;

}

.group-info-detail-header {
    width: 100%;
    height: 60px;
    background-color: #f7f7f7;
    border-radius: 10px;
    display: flex;
    justify-content: space-between;
}

.group-info-detail-memeber-content {
    height: auto;
    padding-left: 15px;
    padding-right: 15px;
    padding-bottom: 15px;
    display: flex;
    flex-wrap: wrap;
}

.group-info-detail-memeber-content-item {
    width: 55px;
    height: 60px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;

}

.group-info-detail-memeber-content-item:hover {
    background-color: #dfdede;
    cursor: pointer;
    transition: all 0.2s ease-in-out;
    border-radius: 10px;
}

.group-info-detail-memeber-content-item-img {}

.group-info-detail-memeber-content-item-img img {
    width: 35px;
    height: 35px;
    margin-top: -5px;
}

.group-info-detail-memeber-content-item-img div {
    width: 35px;
    height: 35px;
    margin-top: -5px;
    display: flex;
    background-color: #d4d3d3;
    align-items: center;
    border-radius: 50%;
    justify-content: center;
    margin-bottom: 8px;
}

.group-info-detail-memeber-content-item-name {
    font-size: 13px;
    line-height: 0;
}

.group-info-detail-memeber {
    width: 100%;
    margin-top: 20px;
    height: auto;
    background-color: #f7f7f7;
    border-radius: 10px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    font-size: 14px;
}

.group-info-detail-memeber-title {
    margin: 10px;
    width: 90%;
    display: flex;
    justify-content: space-between;
    color: black;
}

.group-info-detail-option {
    width: 100%;
    margin-top: 20px;
    height: 60px;
    border-radius: 10px;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    font-size: 14px;
}

.group-info-detail-option span {
    color: #686868;
    margin-left: 10px;
    margin-top: 5px;
}

.group-info-detail-option-btn {
    width: 100%;
    height: 40px;
    background-color: #f7f7f7;
    border-radius: 10px;
}

.group-info-detail-option-btns {
    margin-top: 20px;
    width: 100%;
    height: 40px;
    background-color: #f7f7f7;
    border-radius: 10px;
    display: flex;
    font-size: 14px;
}

.group-info-detail-option-btns span {
    margin-left: -5px;
}

.group-info-detail-title {
    width: 60%;
    display: flex;
}

.group-info-detail-share {
    width: 30%;
    display: flex;
    justify-content: center;
    align-items: center;
}

.group-info-detail-title-imgcontent {
    width: 60px;
    display: flex;
    justify-content: center;
}

.group-info-detail-title-img {
    width: 35px;
    border-radius: 50%;
}

.group-info-detail-title-name {
    display: flex;
    flex-direction: column;
    justify-content: center;
    width: 100px;
}

.group-info-detail-title-name-text1 {
    font-size: 14px;
    line-height: 0.8;
    margin-left: 5px;
    margin-top: 5px;

}

.group-info-detail-title-name-text2 {
    background-color: rgb(233, 232, 232);
    border-radius: 5px;
    height: 18px;
    width: 70px;
    display: flex;
    justify-content: center;
    margin-top: 5px;

}

.group-info-detail-title-name-text2 span {
    line-height: 1.8
}

.el-drawer {
    background-color: #f7f7f7;
}
</style>