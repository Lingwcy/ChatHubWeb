<template>

    <el-dialog v-model="appset.CompoentsEvent.isAddOpen" width="700" draggable overflow id="AddMain">
        <template #header>
            <el-tabs v-model="activeTab" @tab-click="handleTabClick">
                <el-tab-pane label="添加好友" name="friend"></el-tab-pane>
                <el-tab-pane label="添加群组" name="group"></el-tab-pane>
            </el-tabs>
            <div v-if="activeTab === 'group'" class="searchTab">
                <el-input v-model="groupSearch" placeholder="搜索群组" style="width: 570px;"></el-input>
                <el-button @click="searchGroups" style="margin-left: 10px; height: 40px;">搜索</el-button>
            </div>
            <div v-if="activeTab === 'friend'" class="searchTab">
                <el-input v-model="friendSearch" placeholder="搜索好友" style="width: 570px;"></el-input>
                <el-button @click="searchFriends" style="margin-left: 10px; height: 40px;">搜索</el-button>
            </div>
        </template>
        <div class="add-group-or-friend">
            <el-scrollbar height="240px">
                <div v-if="activeTab === 'group'" class="group">
                    <div v-for="group in groupStore.SearchGroup" :key="group.GroupId" class="group-list-item">
                        <div class="group-info">
                            <div class="group-info-content">
                                <div class="block">
                                    <el-avatar class="group-avatar" shape="square" :size="50"
                                        src="/src/images/assets/WCCU.jpg" />
                                </div>
                                <div class="group-details">
                                    <h3 class="group-name">{{ group.GroupName }}</h3>
                                    <p class="group-desc">{{ group.GroupDescription }}</p>
                                    <p class="group-member-count">成员数: {{ group.MemberNumber }}</p>
                                </div>
                            </div>
                            <div class="group-info-footer">
                                <el-button style="background-color: #337ecc; height: 20px; font-size: 12px;"
                                    type="primary" @click="joinGroup(group)">加入</el-button>
                            </div>
                        </div>
                    </div>
                </div>

                <div v-if="activeTab === 'friend'" class="friend">
                    <div v-for="user in groupStore.SearchUser" :key="user.id" class="friend-list-item">
                        <div class="friend-info">
                            <div class="friend-info-content">
                                <div class="block">
                                    <el-avatar class="friend-avatar" shape="square" :size="50"
                                    v-bind:src="'src/images/systemHeader/' + user.HeaderImg" alt="" />
                                </div>
                                <div class="friend-details">
                                    <h3 class="friend-name">{{ user.Username }}</h3>
                                    <p class="friend-id">ID:{{user.id }}</p>
                                    <p class="friend-desc"><i>{{ user.Desc }}</i></p>
                                    <p v-if="user.Age!=null" class="friend-member-count" >年龄: {{ user.Age }}</p>
                                </div>
                            </div>
                            <div class="friend-info-footer">
                                <el-button style="background-color: #337ecc; height: 20px; font-size: 12px;"
                                    type="primary" @click="addFriend(user)">添加</el-button>
                            </div>
                        </div>
                    </div>
                </div>
            </el-scrollbar>
        </div>

        <template #footer>
            <div class="dialog-footer">
                <el-button @click="appset.CompoentsEvent.isAddOpen = false">取消</el-button>
            </div>
        </template>
    </el-dialog>

    <AddDetail></AddDetail>
</template>

<script lang="ts" setup>
import { appsetting } from '../../store';
import { ref } from 'vue';
import { createFriendsService,createGroupService } from '../../services/ServicesCollector';
import { UseServiceStore, UseGroupStore,UseUserInformationStore } from '../../store';
import { Group } from '../../store/Istore';
import AddDetail from './AddDetail.vue';
createFriendsService();
createGroupService();
let service = UseServiceStore();
let userStore = UseUserInformationStore();
let groupStore = UseGroupStore();
let appset = appsetting();
// 响应式数据  
const activeTab = ref('group');
const groupSearch = ref('');


function joinGroup(group: Group) {
    // 处理加入群组的逻辑  
    appset.CompoentsEvent.isAddDetail.isOpen = true;
    appset.CompoentsEvent.isAddDetail.mode = 'group'
    groupStore.SelectedGroup = group;
}


const friendSearch = ref('');

// 方法  
function searchGroups() {
    service.Group?.SearchtGroup(groupSearch.value, groupStore)
}


function searchFriends() {
    let payload = {
        targetName: friendSearch.value,
        userName: userStore.userName,
        xusername: userStore.userName
      }
    service.Friend?.FindFriends(payload,groupStore)
}

function addFriend(user:any) {
    // 处理添加好友的逻辑  
    appset.CompoentsEvent.isAddDetail.isOpen = true;
    groupStore.SelectedUser = user;
    appset.CompoentsEvent.isAddDetail.mode = 'user'
}

function handleTabClick(tab: { name: string }) {
    activeTab.value = tab.name;
}

</script>

<style scoped>
.group-name,
.group-desc,
.group-member-count {
    /* 设置最大宽度，或者你可以使用 min-width 和 max-width 来定义更灵活的范围 */
    max-width: 100px;
    /* 举例的宽度值 */
    /* 防止文本换行 */
    white-space: nowrap;
    /* 隐藏超出容器的内容 */
    overflow: hidden;
    /* 当文本溢出时显示省略号 */
    text-overflow: ellipsis;
}

.add-group-or-friend {
    margin-top: -30px;
}

.group {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.searchTab {
    display: flex;
}

.group-list-item {
    display: flex;
    width: 200px;
    height: 100px;
    margin-left: 10px;
    margin-bottom: 20px;
}

.group-info {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    width: 200px;
    height: 100px;
    background-color: #f1f1f3;
}

.group-info-content {
    display: flex;
    align-items: center;
}

.group-info-footer {
    width: 100%;
    background-color: rgb(227, 228, 230);
    display: flex;
    height: 25px;
    align-items: center;
    justify-content: end;
}

.group-avatar {
    width: 65px;
    height: 65px;
    margin-right: 18px;
}

.group-details {
    font-size: 13px;
    height: 90px;
    line-height: 13px;
}

.group-name {
    font-weight: bold;
    margin-bottom: 5px;
}

.group-desc {
    font-size: 12px;
}

.group-member-count {
    font-size: 14px;
    color: #666;
    /* 或你选择的颜色 */
}


.friend-name,
.friend-desc,
.friend-member-count {
    /* 设置最大宽度，或者你可以使用 min-width 和 max-width 来定义更灵活的范围 */
    max-width: 95px;
    /* 举例的宽度值 */
    /* 防止文本换行 */
    white-space: nowrap;
}


.friend {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}
.friend-list-item {
    display: flex;
    width: 205px;
    height: 100px;
    margin-left: 10px;
    margin-bottom: 20px;
    cursor: default;
}

.friend-info {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    width: 200px;
    height: 100px;
    background-color: #f1f1f3;
}

.friend-info-content {
    display: flex;
    align-items: center;
    width: 100%;
}

.friend-info-footer {
    width: 100%;
    background-color: rgb(227, 228, 230);
    display: flex;
    height: 25px;
    align-items: center;
    justify-content: end;
}

.friend-avatar {
    width: 65px;
    height: 65px;
    margin-top: 10px;
    margin-right: 13px;
    margin-left: 13px;
}

.friend-details {
    font-size: 15px;
    height: 90px;
    line-height: 6px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}
.friend-id{
    font-size: 12px;
}
.friend-name {
    font-weight: bold;
    margin-bottom: 5px;
}

.friend-desc {
    font-size: 11px;
}

.friend-member-count {
    font-size: 11px;
    color: #666;
    /* 或你选择的颜色 */
}
</style>