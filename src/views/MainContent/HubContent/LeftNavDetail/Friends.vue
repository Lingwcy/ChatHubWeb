<script  setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from '../../../../common/axiosSetting';
import { UseUserInformationStore, UseFriendsStore, UseMsgStore, UseChatStore } from '../../../../store/index'
const userInfo = UseUserInformationStore()
const fristore = UseFriendsStore()
const Pmsg = UseMsgStore()
const chatStore = UseChatStore()
const dialogVisible = ref(false)
let friendsReq = []
let friendsReqJson = reactive({}) as any
onMounted(function () {
})



//加载好友请求
const getFriendsReq = () => {
  dialogVisible.value = true
  axios.get('api/friends/frined-request-list/' + userInfo.userName + '')
    .then(res => {

      friendsReq = res.data.data.split('$')
      for (let i = 0; i < friendsReq.length - 1; i++) {
        friendsReqJson[i] = JSON.parse(friendsReq[i]);
      }
    })
}



let reqmsg = {
  mark: "",
  msg: "",
}
axios.defaults.headers.common['Authorization'] = "Bearer " + userInfo.jwtToken //向每一个请求携带JWT
const AddFriends = () => {
  ElMessageBox.prompt('请输入需要添加的好友昵称:', '添加为好友', {
    confirmButtonText: '查找',
    cancelButtonText: '取消',
    inputErrorMessage: '内容不能为空',
    inputPattern:
      /\S/,
  })
    .then(async ({ value }) => {
      await axios.get('api/friends/friends-find/' + value + '/' + userInfo.userName + '')
        .then(res => {
          if (res.data.errors != null) {
            ElMessage({
              type: 'error',
              message: `用户不存在或已经是好友!${res.data.errors}`,
            })
            return
          }
          if (res.data.data == "3") {
            ElMessage({
              type: 'success',
              message: `找到用户为:${value}`,
            })
            sendFridendsRequst(value, userInfo.userName)
          }
        })
        .catch(_res => {
          ElMessage({
            type: 'error',
            message: `用户不存在或已经是好友!`,
          })
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
      reqmsg.msg = value
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
    reqmsg.mark = value
    axios.post('api/friends/frined-req/' + TagetName + '/' + FromName + '/' + reqmsg.msg + '/' + reqmsg.mark + '')
      .then(async res => {

        if (res.data.errors != null) {
          ElMessage({
            type: 'error',
            message: `已经发送过验证了!${res.data.errors}`,
          })
          return
        }

        if (res.data.data == 1) {
          ElMessage({
            type: 'success',
            message: '添加请求已经成功发送!',
          })
          await userInfo.connection.invoke("SendFriendsRequest", TagetName);
        }
        else {
          ElMessage({
            type: 'info',
            message: '添加请求失败',
          })
        }
      })

  })
}

const getFriendsList = async () => {
  await axios.get('api/friends/friends-all/' + userInfo.userId)
    .then(res => {
      fristore.$reset()
      let fri = res.data.data.split("$")
      for (let i = 0; i < fri.length - 1; i++) {
        fristore.Friends.push(JSON.parse(fri[i]) as never)
      }
    })
    .catch(_err => {

    })
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
const acceptFriendsReq = (TargetModel:any) => {
  axios.post('api/friends/friend-accept', TargetModel)
    .then(res => {
      if (res.data.errors != null) {
        ElMessage({
          type: 'error',
          message: `已经是好友了 ${res.data.errors}`,
        })
        return

      }
      if (res.data.data == "1") {
        ElMessage({
          type: 'success',
          message: `已成功添加好友!`,
        })
        getFriendsList()

        for (let key in friendsReqJson) {
          if (friendsReqJson[key]["UserId"] == TargetModel["UserId"]) {
            delete friendsReqJson[key]
          }
        }
      }
    })
    .catch(err => {
      ElMessage({
        type: 'info',
        message: '系统错误' + err,
      })
    })
}
const rejectFriendsReq = (TargetModel:any) => {
  axios.delete('api/friends/frined-req-remove', {
    data: TargetModel
  })
    .then(res => {
      if (res.data.data == "1") {
        ElMessage({
          type: 'success',
          message: `已拒绝请求!`,
        })
        for (let key in friendsReqJson) {
          if (friendsReqJson[key]["UserId"] == TargetModel["UserId"]) {
            delete friendsReqJson[key]
          }
        }

      }
    })
    .catch(err => {
      ElMessage({
        type: 'info',
        message: '系统错误' + err,
      })
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
    <div v-for="(req, index) in friendsReqJson">
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
                v-on:click="acceptFriendsReq(friendsReqJson[index])">同意</el-button>
              <el-button class="friendReq-Body-info-bt" round
                v-on:click="rejectFriendsReq(friendsReqJson[index])">拒绝</el-button>
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
