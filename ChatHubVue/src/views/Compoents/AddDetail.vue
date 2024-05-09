<script lang="ts" setup>
import { ref } from 'vue';
import { appsetting } from '../../store';
import { UseServiceStore, UseGroupStore, UseUserInformationStore,UseFriendsStore } from '../../store';
import { ElMessage, ElNotification } from 'element-plus';
let service = UseServiceStore();
let groupStore = UseGroupStore();
let friendStore = UseFriendsStore();
let appset = appsetting();
let textarea = ref("") //验证信息
let mark = ref("")//备注
let userStore = UseUserInformationStore();
const groupvalue = ref('')

let options: any[] = []
friendStore.FriendTree?.Units.forEach((element: { id: unknown; UnitName: string; }) => {
    let payload = {
        value: element.id as unknown as string,
        label:element.UnitName as string
    }
    options.push(payload)
});

const test =()=>{
    console.log(groupvalue.value)
}
const sendFridendsRequst = async () => {
    if (groupvalue.value == "") {
        ElMessage({
            message: '你必须选择一个分组',
            type: 'warning',
        })
        return
    }
    let payload = {
        mark: mark.value,
        ReqMsg: textarea.value,
        userName: userStore.userName,
        targetName: groupStore.SelectedUser.Username,
        xusername: userStore.userName,
        TargetGroupId:Number(groupvalue.value)
    }
    let flag = service.Friend?.SendFriendRequest(payload);
    if (flag == undefined) return;
    flag.then(res => {
        if (res) {
            ElNotification({
                title: '成功',
                message: '发送好友申请成功',
                type: 'success',
            })
        } else {
            ElNotification({
                title: '失败',
                message: '发生未知错误,请联系管理员',
                type: 'error',
            })
        }
    })

}
const sendGroupRequst =()=>{
    let payload = {
        ReqMsg: textarea.value,
        UserId: Number(userStore.userId),
        GroupId:groupStore.SelectedGroup.GroupId,
        xusername:userStore.userName
    }
    service.Group?.SendGroupRequest(payload)
}
</script>
<template>
    <el-dialog v-if="appset.CompoentsEvent.isAddDetail.mode == 'group'"
        v-model="appset.CompoentsEvent.isAddDetail.isOpen" width="330" draggable overflow id="AddDetailMain">
        <template #header>
            <p>申请加群</p>
        </template>
        <div id="addDetailContent">
            <div id="addDetailInfo">
                <div class="group-info">
                    <div class="group-info-content">
                        <div class="block">
                            <el-avatar class="group-avatar" shape="square" :size="50"
                                src="/src/images/assets/WCCU.jpg" />
                        </div>
                        <div class="group-details">
                            <h3 class="group-name">{{ groupStore.SelectedGroup.GroupName }}({{
        groupStore.SelectedGroup.GroupId
    }})</h3>
                            <p class="group-desc">{{ groupStore.SelectedGroup.GroupDescription }}</p>
                            <p class="group-member-count">成员数: {{ groupStore.SelectedGroup.MemberNumber }}</p>
                        </div>
                    </div>
                    <div class="group-info-footer">

                    </div>
                </div>
            </div>
            <div id="addDetailMessage">
                <p style="color: gray;">请填写验证信息</p>
                <el-input v-model="textarea" maxlength="30" style="width: 100%;margin-top: -5px;" show-word-limit
                    type="textarea" />
            </div>
        </div>
        <template #footer>
            <div class="dialog-footer">
                <el-button type="primary" @click="appset.CompoentsEvent.isAddDetail.isOpen = false;sendGroupRequst()">提交</el-button>
                <el-button @click="appset.CompoentsEvent.isAddDetail.isOpen = false">取消</el-button>
            </div>
        </template>
    </el-dialog>


    <el-dialog v-if="appset.CompoentsEvent.isAddDetail.mode == 'user'"
        v-model="appset.CompoentsEvent.isAddDetail.isOpen" width="330" draggable overflow id="AddDetailMain">
        <template #header>
            <p>添加好友</p>
        </template>
        <div id="addDetailContent">
            <div id="addDetailUserInfo">
                <div class="friend-info">
                    <div class="friend-info-content">
                        <div class="block">
                            <el-avatar class="friend-avatar" shape="square" :size="50"
                                v-bind:src="'src/images/systemHeader/' + groupStore.SelectedUser.HeaderImg" alt="" />
                        </div>
                        <div class="friend-details">
                            <h3 class="friend-name">{{ groupStore.SelectedUser.Username }}</h3>
                            <p class="friend-id">ID:{{ groupStore.SelectedUser.id }}</p>
                            <p class="friend-desc"><i>{{ groupStore.SelectedUser.Desc }}</i></p>
                            <p v-if="groupStore.SelectedUser.Age != null" class="friend-member-count">年龄: {{
        groupStore.SelectedUser.Age }}</p>
                        </div>
                    </div>
                </div>
            </div>
            <div id="addDetailMessage" style="margin-top: -20px;">
                <p style="color: gray; font-size: 11px;">请填写验证信息</p>
                <el-input v-model="textarea" maxlength="30" style="width: 100%;margin-top: -5px;" show-word-limit
                    type="textarea" />
                <p style="color: gray;font-size: 11px;">备注</p>
                <el-input v-model="mark" maxlength="20" style="width: 100%;margin-top: -5px;" type="textarea" />
                <p style="color: gray;font-size: 11px;">分组</p>
                <el-select v-model="groupvalue" placeholder="请选择"  style="width: 100%;">
                    <el-option v-for="item in options" :key="item.value" :label="item.label"
                        :value="item.value" /></el-select>
            </div>
        </div>
        <template #footer>
            <div class="dialog-footer">
                <el-button type="primary"
                    @click="appset.CompoentsEvent.isAddDetail.isOpen = true;test()">测试</el-button>
                <el-button type="primary"
                    @click="appset.CompoentsEvent.isAddDetail.isOpen = false;sendFridendsRequst()">提交</el-button>
                <el-button @click="appset.CompoentsEvent.isAddDetail.isOpen = false">取消</el-button>
            </div>
        </template>
    </el-dialog>
</template>
<style scoped>
:deep(.el-select){
    --el-select-border-color-hover:none
}
.group-name,
.group-desc,
.group-member-count {
    /* 设置最大宽度，或者你可以使用 min-width 和 max-width 来定义更灵活的范围 */
    max-width: 200px;
    /* 举例的宽度值 */
    /* 防止文本换行 */
    white-space: nowrap;
    /* 隐藏超出容器的内容 */
    overflow: hidden;
    /* 当文本溢出时显示省略号 */
    text-overflow: ellipsis;
}

#addDetailUserInfo {
    width: 100%;
    display: flex;
    justify-content: center;
    margin-top: -50px;
}

.block {
    margin-right: 3px;
}

.friend-info {
    width: 100%;
    background-color: white;
}

.group-info {
    width: 100%;
    background-color: white;
}

#addDetailContent {
    margin-top: -30px;
}

.group-info-footer {
    background-color: white;
}
</style>