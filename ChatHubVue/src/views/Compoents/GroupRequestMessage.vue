<template>
    <el-drawer v-model="appset.CompoentsEvent.isGroupRequestMessageOpen" title="群通知" direction="rtl" size="20%">
        <div v-for="(req, index) in groupStore.GroupRequestList" :key="req.Id">
            <div class="FriendsRequestContent">
                <div id="friendReq-Body">
                    <div id="friendReq-Body-inner">
                        <div class="friendReq-Body-info">{{ req.UserId }}</div>
                        <div class="friendReq-Body-info2">申请加入 {{ req.GroupName }}</div>

                        <div class="friendReq-Body-info2">
                            <el-button class="friendReq-Body-info-bt" round
                                v-on:click="acceptGroupReq(req)">同意</el-button>
                            <el-button class="friendReq-Body-info-bt" round
                                v-on:click="rejectGroupReq(req)">拒绝</el-button>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </el-drawer>
    <AcceptDetail />


</template>

<script lang="ts" setup>
import { ElDrawer } from 'element-plus'
import { watch } from 'vue'
import { UseServiceStore } from '../../store';
import { appsetting, UseGroupStore, UseUserInformationStore } from '../../store';
import { createGroupService } from '../../services/ServicesCollector';
import AcceptDetail from './AcceptDetail.vue';
import { GroupRequestList } from '../../store/Istore';
createGroupService()
const appset = appsetting();
const service = UseServiceStore();
const groupStore = UseGroupStore()
const userInfo = UseUserInformationStore();
const acceptGroupReq = (TargetModel: GroupRequestList) => {
    TargetModel.xusername = userInfo.userName  
    let payload ={
        Id: TargetModel.Id,
        xusername: userInfo.userName,
    }
    let flag = service.Group?.AcceptGroupRequest(payload)
    if (flag == undefined) return;
    flag.then(res => {
        if (res) {
            groupStore.GroupRequestList = groupStore.GroupRequestList.filter(request => request.Id !== TargetModel.Id);
        }
    }) 
    //这里需要发送HUB消息告知对方已经同意了
}
const rejectGroupReq = (TargetModel: GroupRequestList) => {
    TargetModel.xusername = userInfo.userName  
    let payload ={
        Id: TargetModel.Id,
        xusername: userInfo.userName,
    }
    let flag = service.Group?.RejectGroupRequest(payload)
    if (flag == undefined) return;
    flag.then(res => {
        if (res) {
            groupStore.GroupRequestList = groupStore.GroupRequestList.filter(request => request.Id !== TargetModel.Id);
        }
    })
}
watch(
    () => appset.CompoentsEvent.isGroupRequestMessageOpen,
    (count, prevCount) => {
        service.Group?.GetGroupRequestList(Number(userInfo.userId),userInfo.userName,groupStore)
    }
)
</script>
<style scoped>
.FriendsRequestContent {
    display: flex;
    height: 100%;
    border-bottom: 1px solid #e9e9eb;
}

#friendReq-Header {
    width: 20%;
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
    width: 70px;
    height: 70px;
    overflow: hidden;
    border-radius: 15px;
}

.friendReq-Body-info {
    width: 100%;
    height: 22px;
    overflow: hidden;
    color: #606266;
    font-size: 17px;
    font-weight: 500;
    margin-top: 0px;
}

.friendReq-Body-info2 {
    width: 100%;
    height: 27px;
    overflow: hidden;
    color: #606266;
    font-size: 13px;
    font-weight: 500;
    margin-top: -5px;
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
</style>