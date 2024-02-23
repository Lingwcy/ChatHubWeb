<script  setup lang="ts">
import { ref, reactive } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {createFriendsService } from '../../../../services/ServicesCollector';
import { UseUserInformationStore,UseServiceStore, UseFriendsStore, UseMsgStore, UseChatStore } from '../../../../store/index'
const userInfo = UseUserInformationStore()
const service = UseServiceStore();
const fristore = UseFriendsStore()
const Pmsg = UseMsgStore()
const chatStore = UseChatStore()
const dialogVisible = ref(false)
createFriendsService();
const getFriendsReq = () => {
  dialogVisible.value = true
  service.Friend?.GetUserFriendsRquest(userInfo.userName, fristore);
}
let reqmsg = {
  mark: "",
  msg: "",
}
const AddFriends = () => {
  ElMessageBox.prompt('请输入需要添加的好友昵称:', '添加为好友', {
    confirmButtonText: '查找',
    cancelButtonText: '取消',
    inputErrorMessage: '内容不能为空',
    inputPattern:
      /\S/,
  })
    .then(async ({ value }) => {
      let payload = {
        targetName: value,
        userName: userInfo.userName,
      }
      let flag = service.Friend?.FindFriend(payload)
      if(flag == undefined){ return;}
      flag.then(res => {
        if (res) {
          sendFridendsRequst(value, userInfo.userName)
        }
      })
    })
    .catch((_error) => {
      ElMessage({
        type: 'info',
        message: '用户查找取消',
      })

    })
}

const sendFridendsRequst = async (TagetName: any, FromName: any) => {
  await ElMessageBox.prompt(
    '找到此用户,请输入验证信息',
    '发送添加请求',
    {
      confirmButtonText: '确认',
      cancelButtonText: '返回',
      inputErrorMessage: '内容不能为空',
      inputPlaceholder: "我是...",
      inputPattern:
        /\S/,
      type: 'success',
    }
  )
    .then((value) => {
      reqmsg.msg = value.value
    })

  await ElMessageBox.prompt(
    '给定一个备注',
    '发送添加请求',
    {
      confirmButtonText: '确认',
      cancelButtonText: '返回',
      inputPlaceholder: TagetName,
      inputErrorMessage: '内容不能为空',
      inputPattern:
        /\S/,
      type: 'success',
    }
  ).then((value) => {
    reqmsg.mark = value.value
    let payload = {
      targetName: TagetName,
      userName: FromName,
      ReqMsg: reqmsg.msg,
      mark: reqmsg.mark
    }
    service.Friend?.SendFriendRequest(payload);
  })
}

const getFriendsList = async () => {
  service.Friend?.GetUserFriends(userInfo.userId, fristore);
}

const TurnFriendsToMessageBox = (friends: any) => {
  let flag1 = false//判断消息存储库
  Pmsg.messageItems.forEach(element => {
    if (element.targetUserName == friends.FriendName) {
      flag1 = true
      for (let i = 0; i < chatStore.targetUserTab.length; i++) {
        if (chatStore.targetUserTab[i].tabName == friends.FriendName) {
          return
        }
      }

      chatStore.targetUserTab.push({
        tabTitle: friends.FriendName,
        tabName: friends.FriendName,
        targetUserMessage: reactive(element)
      })
    }

  });
  if (flag1) return
  Pmsg.messageItems.push(
    {
      targetUserName: friends.FriendName,
      messages: [],
      messageNames: [],
      messageHeaders: [],
    }
  )
  chatStore.targetUserTab.push({
    tabTitle: friends.FriendName,
    tabName: friends.FriendName,
    targetUserMessage: reactive(Pmsg.messageItems[Pmsg.messageItems.length - 1])
  })
}
const acceptFriendsReq = (TargetModel: any) => {
  let flag = service.Friend?.AcceptFriendRequest(TargetModel)
  if(flag == undefined) return ;
  flag.then(res => {
    if (res) {
      getFriendsList()
      for (let key in fristore.RequestList) {
        if (fristore.RequestList[key]["UserId"] == TargetModel["UserId"]) {
          delete fristore.RequestList[key]
        }
      }
    }
  })
}
const rejectFriendsReq = (TargetModel: any) => {
  let flag = service.Friend?.RejectFriendRequest(TargetModel)
  if(flag == undefined) return ;
  flag.then(res => {
    if (res) {
      for (let key in fristore.RequestList) {
        if (fristore.RequestList[key]["UserId"] == TargetModel["UserId"]) {
          delete fristore.RequestList[key]
        }
      }
      getFriendsList()
    }
  })
}
getFriendsList()
</script>
<template>
  <div id="friends-header">
    <el-button plain @click="getFriendsReq">好友请求</el-button>
    <el-button plain @click="AddFriends">添加</el-button>

  </div>
  <div id="friends-list">
    <div class="friends-list-item" v-for="(req, index) in fristore.Friends">
      <div class="friends-list-item-img">
        <img v-bind:src="'src/images/systemHeader/' + req.FriendImg" alt="">
      </div>
      <div class="friends-list-item-font">
        <span style="margin-left: 5px;" v-if="fristore.Friends[index].remark">{{ fristore.Friends[index].remark
        }}</span>
        <span style="margin-left: 5px;" v-else>{{ fristore.Friends[index].FriendName }}</span>
        <el-button style="margin-right: 10px; width: 50px; height: 30px; font-size: 13px;"
          v-on:click="TurnFriendsToMessageBox(fristore.Friends[index])">私聊</el-button>
      </div>
    </div>
  </div>



  <el-dialog v-model="dialogVisible" title="好友请求" width="300px" draggable>
    <div v-for="(req, index) in fristore.RequestList">
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
                v-on:click="acceptFriendsReq(fristore.RequestList[index])">同意</el-button>
              <el-button class="friendReq-Body-info-bt" round
                v-on:click="rejectFriendsReq(fristore.RequestList[index])">拒绝</el-button>
            </div>
          </div>
        </div>

      </div>

    </div>
    <template #footer>
      <span class="dialog-footer">
        <el-button @click="dialogVisible = false">关闭</el-button>
      </span>
    </template>
  </el-dialog>
</template>
<style>
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
  width: 100%;
  height: 45px;
  border-bottom: 1px solid rgb(221, 217, 217);
  display: flex;
  margin-top: 3px;
  padding-bottom: 2px;
}

.friends-list-item:hover {
  background-color: #cfd1d6;
  transition: all 0.5s ease;
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
}

.friends-list-item-img img {
  width: 45px;
}

.friends-list-item-font {
  width: 75%;
  height: 100%;
  display: flex;
  align-items: center;
  color: #606266;
  font-size: 14px;
  font-family: 幼圆;
  justify-content: space-between;

}
</style>
