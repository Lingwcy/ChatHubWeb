<template>
    <el-dialog v-model="appset.CompoentsEvent.isGroupCreateOpen" title="创建群组" width="500" destroy-on-close>
        <template #header>
            <div class="my-header">
                <span>发起群聊</span>
                <span class="seleceted-count" v-if="selectedUsers.length > 0">已经选择{{ selectedUsers.length }}人</span>
            </div>
        </template>

        <div>
            <el-container class="group-create-container">
                <el-aside class="group-create-container-aside">
                    <el-scrollbar height="300px" id="treeContent">
                        <el-tree ref="tree" style="width: 220px;background-color: #fafafa;"
                            :data="friendStore.FriendTree?.Units" :props="treeProps" show-checkbox :node-key="'id'"
                            @check="addCheckedNodes">
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
                                            </div>
                                        </div>
                                    </span>
                                </span>
                            </template>
                        </el-tree>
                    </el-scrollbar>
                </el-aside>
                <el-main class="group-create-container-main">
                    <el-scrollbar height="300px" id="treeContent" class="group-create-container-main-selectedItems">
                        <div class="group-create-container-main-selectedItem" v-for="user in selectedUsers" @click="addCheckedNodes(user, null)">
                            <div class="friends-selected-list-item">
                                <div class="friends-list-item-img">
                                    <img v-bind:src="'src/images/systemHeader/' + user.HeaderImg" alt="">
                                </div>
                                <div class="friends-list-item-font">
                                    <span style="margin-left: 0px; font-size: 15px;">{{ user.Username
                                        }}</span>
                                    <el-icon>
                                        <CircleCloseFilled />
                                    </el-icon>
                                </div>
                            </div>
                        </div>
                    </el-scrollbar>
                </el-main>
            </el-container>
        </div>
        <template #footer>
            <div class="dialog-footer">
                <el-button @click="appset.CompoentsEvent.isGroupCreateOpen = false">取消</el-button>
                <el-button type="danger" @click="appset.CompoentsEvent.isGroupCreateOpen = false;createGroupConfirm()">
                    创建
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script lang="ts" setup>
import { ref,watch } from 'vue'
import { appsetting } from '../../store';
import { UseFriendsStore ,UseUserInformationStore,UseServiceStore,UseGroupStore} from '../../store';
import { CircleCloseFilled } from '@element-plus/icons-vue'
import { CreateGroupModel } from '../../services/GroupService';
import { createGroupService } from '../../services/ServicesCollector';
import { ElMessage } from 'element-plus';
createGroupService();
const selectedUsers = ref<IUserProfile[]>([]);
const friendStore = UseFriendsStore();
const groupStore = UseGroupStore()
const userInformationStore = UseUserInformationStore()
const service = UseServiceStore()
const tree = ref('') as any
const treeProps = {
    children: 'Children',
    label: 'UnitName'
}
const appset = appsetting()
const addCheckedNodes = (a: any, b: any) => {
    if (a.Children == undefined) {
        //寻找selectedUsers是否已经存在这个节点
        const index = selectedUsers.value.findIndex((item) => item.id == a.id)
        //如果存在就删除
        if (index > -1) {
            selectedUsers.value.splice(index, 1)
            tree.value.setChecked(a, false,false)
            return;
        }
        selectedUsers.value.push(a)
    } else {
        a.Children.forEach((element: any) => {
            //寻找selectedUsers是否已经存在这个节点
            const index = selectedUsers.value.findIndex((item) => item.id == element.id)
            //如果存在就删除
            if (index > -1) {
                selectedUsers.value.splice(index, 1)
                return;
            }
            selectedUsers.value.push(element)
        });
    }

}
const getAllCheckedNodes = () => {
    console.log(tree.value.getCheckedNodes(true, false))
}

const createGroupConfirm =() =>{
    //查找selectedUsers中的所有id然后添加到number数组中
    const ids = selectedUsers.value.map((item) => item.id)
    //选择selectedUsers的前三个用户名合并成一个字符串用逗号分开
    const usernames = selectedUsers.value.slice(0, 2).map((item) => item.Username).join(',')
    const payload:CreateGroupModel = {
        Name: userInformationStore.userName+ usernames + '...',
        UserId: ids,
        CreateUserId:Number(userInformationStore.userId),
        xusername: userInformationStore.userName,
    }
    service.Group?.CreateGroup(payload).then(([flag,res]) => {
        if(flag){
            //执行建群操作成功，此时需要发送SignalR消息:1.通知群组的成员刷新群组列表
            //                                       2.将群组内在线的成员添加进signalR的群组列表中
            //                                       3.刷新自己的群组列表
            appset.CompoentsEvent.isGroupCreateOpen = false;
            service.ChatHub?.CreateGroupTask(res,ids)
            service.Group?.GetGroupList(Number(userInformationStore.userId),userInformationStore.userName,groupStore)
        }
    }).catch((err) => {
        ElMessage.error(err.message)
    })
}
watch(
    () => appset.CompoentsEvent.isGroupCreateOpen,
    (newVal, oldVal) => {
        // 在这里添加你希望在 IsNewMessageCome 变化时执行的代码  
        if (newVal) {
            selectedUsers.value = [];
        }
    }
); 
</script>

<style scoped>
.my-header {
    display: flex;
    height: 0px;
    width: 100%;
    justify-content: space-between;
}

.seleceted-count {
    color: #606266;
    font-size: 13px;
    font-family: 幼圆;
}

.group-create-container-main-selectedItems {
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;

}

.group-create-container {
    width: 100%;
    height: 100%;
    display: flex;
}

.group-create-container-aside {
    width: 50%;
    height: 100%;
    border-right: 1px solid #e3e3e7;
}

.group-create-container-main {
    width: 50%;
    height: 100%;
    padding: 0;
}

#friends-header {
    width: 100%;
    height: 35px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    flex-wrap: wrap;

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
    height: 35px;
    display: flex;
    margin-top: 3px;
    padding-bottom: 2px;
}

.friends-selected-list-item:hover {
    background-color: #eeeeee;
    transition: all 0.3s ease;
    cursor: pointer;
    border-radius: 10px;
}

.friends-selected-list-item {
    width: 200px;
    height: 35px;
    display: flex;
    margin-top: 3px;
    padding-bottom: 2px;
}

.friends-list-item:active {
    background-color: #cfd0d1;
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
    width: 35px;
}

.friends-list-item-font {
    width: 70%;
    height: 100%;
    text-overflow: ellipsis;
    display: flex;
    margin-left: 5px;
    line-height: 21px;
    overflow: hidden;
    color: #606266;
    font-size: 14px;
    font-family: 幼圆;
    align-items: center;
    justify-content: space-between;

}


/* 去掉最顶层的虚线，放最下面样式才不会被上面的覆盖了 */
:deep(&>.el-tree-node::after) {
    border-top: none;
}

:deep(&>.el-tree-node::before) {
    border-left: none;
}
</style>