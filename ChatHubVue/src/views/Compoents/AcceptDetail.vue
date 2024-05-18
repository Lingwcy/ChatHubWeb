<script lang="ts" setup>
import { ref } from 'vue';
import { appsetting } from '../../store';
import { UseServiceStore, UseUserInformationStore, UseFriendsStore } from '../../store';
import { ElMessage } from 'element-plus';
let service = UseServiceStore();
let friendStore = UseFriendsStore();
let appset = appsetting();
let mark = ref("")//备注
let userStore = UseUserInformationStore();
const groupvalue = ref('')

let options: any[] = []
const payload = {
    userId: userStore.userId,
    xusername: userStore.userName
}
service.Friend?.FindFriendTree(payload, friendStore).then(res => {
    if (res) {
        friendStore.FriendTree?.Units.forEach((element: { id: unknown; UnitName: string; }) => {
            let payload = {
                value: element.id as unknown as string,
                label: element.UnitName as string
            }
            options.push(payload)
        });
    }
});

const acceptFriendsReq = () => {
    if (groupvalue.value == "") {
        ElMessage({
            message: '你必须选择一个分组',
            type: 'warning',
        })
        return
    }
    appset.CompoentsEvent.isAcceptDetail.selectedUser.remark = mark.value;
    appset.CompoentsEvent.isAcceptDetail.selectedUser.AccepterGroupId = Number(groupvalue.value);
    appset.CompoentsEvent.isAcceptDetail.selectedUser.xusername = appset.CompoentsEvent.isAcceptDetail.selectedUser.TargetName
    let flag = service.Friend?.AcceptFriendRequest(appset.CompoentsEvent.isAcceptDetail.selectedUser)
    if (flag == undefined) return;
    flag.then(res => {
        if (res) {
            const paload = {
                userId: userStore.userId,
                xusername: userStore.userName
            }
            service.Friend?.FindFriendTree(paload, friendStore)
            friendStore.RequestList = friendStore.RequestList.filter(request => request.UserId !== appset.CompoentsEvent.isAcceptDetail.selectedUser.UserId);
        }
    })
}

</script>
<template>
    <el-dialog v-model="appset.CompoentsEvent.isAcceptDetail.isOpen" width="330" draggable overflow id="AddDetailMain">
        <template #header>
            <p>同意添加</p>
        </template>
        <div id="addDetailContent">
            <div id="addDetailUserInfo">
                <div class="friend-info">
                    <div class="friend-info-content">
                        <div class="block">
                            <el-avatar class="friend-avatar" shape="square" :size="50"
                                v-bind:src="'src/images/systemHeader/' + appset.CompoentsEvent.isAcceptDetail.selectedUser.UserImg" alt="" />
                        </div>
                        <div class="friend-details">
                            <h3 class="friend-name">{{ appset.CompoentsEvent.isAcceptDetail.selectedUser.UserName}}</h3>
                            <p class="friend-id">ID:{{ appset.CompoentsEvent.isAcceptDetail.selectedUser.UserId }}</p>
                        </div>
                    </div>
                </div>
            </div>
            <div id="addDetailMessage" style="margin-top: -20px;">
                <p style="color: gray;font-size: 11px;">备注</p>
                <el-input v-model="mark" maxlength="20" style="width: 100%;margin-top: -5px;" type="textarea" />
                <p style="color: gray;font-size: 11px;">分组</p>
                <el-select v-model="groupvalue" placeholder="请选择" style="width: 100%;">
                    <el-option v-for="item in options" :key="item.value" :label="item.label"
                        :value="item.value" /></el-select>
            </div>
        </div>
        <template #footer>
            <div class="dialog-footer">
                <el-button type="primary"
                    @click="appset.CompoentsEvent.isAcceptDetail.isOpen = false; acceptFriendsReq()">同意</el-button>
                <el-button @click="appset.CompoentsEvent.isAcceptDetail.isOpen = false">取消</el-button>
            </div>
        </template>
    </el-dialog>
</template>
<style scoped>
:deep(.el-select) {
    --el-select-border-color-hover: none
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