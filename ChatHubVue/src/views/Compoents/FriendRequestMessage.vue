<template>
    <el-drawer v-model="appset.CompoentsEvent.isFriendRequestMessageOpen" title="好友请求列表" direction="rtl" size="20%">
        <div v-for="(req, index) in fristore.RequestList" :key="req.TargetId">
            <div class="FriendsRequestContent">
                <div id="friendReq-Header">
                    <img v-bind:src="'../../../src/images/systemHeader/' + req.UserImg" id="friendReq-Header-img" />
                </div>
                <div id="friendReq-Body">
                    <div id="friendReq-Body-inner">
                        <div class="friendReq-Body-info">{{ req.UserName }}</div>
                        <div class="friendReq-Body-info2">{{ req.ReqMsg }}</div>

                        <div class="friendReq-Body-info2">
                            <el-button class="friendReq-Body-info-bt" round
                                v-on:click="acceptFriendsReq(req as any)">同意</el-button>
                            <el-button class="friendReq-Body-info-bt" round
                                v-on:click="rejectFriendsReq(req)">拒绝</el-button>
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
import { appsetting, UseFriendsStore, UseUserInformationStore } from '../../store';
import { AcceptRequest, UserRequest } from '../../services/FriendsService';
import { createFriendsService } from '../../services/ServicesCollector';
import AcceptDetail from './AcceptDetail.vue';
createFriendsService()
const appset = appsetting();
const service = UseServiceStore();
const fristore = UseFriendsStore()
const userInfo = UseUserInformationStore();
const acceptFriendsReq = (TargetModel: AcceptRequest) => {
    appset.CompoentsEvent.isAcceptDetail.selectedUser = TargetModel as any;
    appset.CompoentsEvent.isAcceptDetail.isOpen = true;
}
const rejectFriendsReq = (TargetModel: UserRequest) => {
    TargetModel.xusername = TargetModel.TargetName
    let flag = service.Friend?.RejectFriendRequest(TargetModel)
    if (flag == undefined) return;
    flag.then(res => {
        if (res) {
            fristore.RequestList = fristore.RequestList.filter(request => request.UserId !== TargetModel.UserId);
            const paload = {
                userId: userInfo.userId,
                xusername: userInfo.userName
            }
            service.Friend?.FindFriendTree(paload, fristore)
        }
    })
}
watch(
    () => appset.CompoentsEvent.isFriendRequestMessageOpen,
    (count, prevCount) => {
        service.Friend?.GetUserFriendsRquest(userInfo.userName, fristore)
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