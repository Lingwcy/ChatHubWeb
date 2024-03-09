<template>

    <el-dialog v-model="appset.CompoentsEvent.isAddOpen" width="700" draggable overflow id="AddMain">
        <template #header>
            <el-tabs v-model="activeTab" @tab-click="handleTabClick">
                <el-tab-pane label="添加好友" name="friend"></el-tab-pane>
                <el-tab-pane label="添加群组" name="group"></el-tab-pane>
                <el-tab-pane label="请求" name="request"></el-tab-pane>
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
                <div v-for="group in groups" :key="group.id" class="group-list-item">
                    <div class="group-info">
                        <div class="group-info-content">
                            <div class="block">
                                <el-avatar class="group-avatar" shape="square" :size="50" :src="group.avatar" />
                            </div>
                            <div class="group-details">
                                <h3 class="group-name">{{ group.name }}</h3>
                                <p class="group-desc">{{ group.description }}</p>
                                <p class="group-member-count">成员数: {{ group.memberCount }}</p>
                            </div>
                        </div>
                        <div class="group-info-footer">
                            <el-button style="background-color: #337ecc; height: 20px; font-size: 12px;" type="primary" @click="joinGroup(group)">加入</el-button>
                        </div>
                    </div>
                    
                </div>
            </div>

            <div v-if="activeTab === 'friend'">
                <!-- 好友搜索和列表 -->
                <el-list v-for="friend in friends" :key="friend.id" class="friend-list-item">
                    <el-list-item>
                        <div class="friend-info">
                            <span class="friend-name">{{ friend.name }}</span>
                        </div>
                        <el-button type="primary" @click="addFriend(friend)">添加</el-button>
                    </el-list-item>
                </el-list>
            </div>
        </el-scrollbar>
        </div>

        <template #footer>
            <div class="dialog-footer">
                <el-button  @click="appset.CompoentsEvent.isAddOpen = false" >取消</el-button>
            </div>
        </template>
    </el-dialog>
</template>

<script lang="ts" setup>
import { appsetting } from '../../store';
import { ref } from 'vue';
import { UseServiceStore } from '../../store';
let service = UseServiceStore();
let appset = appsetting();
// 响应式数据  
const activeTab = ref('group');
const groupSearch = ref('');
// 假设 groups 数组中的每个对象都包含 avatar 和 memberCount 属性  
const groups = ref<Array<{
    id: number;
    name: string;
    description: string;
    avatar: string; // 群组头像的 URL  
    memberCount: number; // 群组成员数量  
}>>([]);

function joinGroup(group: {
    id: number;
    name: string;
    description: string;
    avatar: string;
    memberCount: number;
}) {
    // 处理加入群组的逻辑  
    console.log('加入群组:', group);
}

// 你可以在此处添加方法来填充 groups 数组，比如从 API 获取数据  
// 例如: fetchGroupsData().then(data => groups.value = data);  
const friendSearch = ref('');
const friends = ref<Array<{ id: number; name: string }>>([]);

// 方法  
function searchGroups() {
    service.Group?.SearchtGroup(groupSearch.value)
    // 模拟搜索群组，实际应用中应调用API  
    groups.value = [
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },
    { id: 1, name: '武汉城市学院', description: '官方通讯群组', avatar: "/src/images/assets/WCCU.jpg", memberCount: 8423 },

        { id: 1, name: '西南民族大学', description: '官方通讯群组', avatar: "/src/images/assets/SouthMinzuU.jpg", memberCount: 5214 },
        // ...其他群组数据  
    ];
}


function searchFriends() {
    // 模拟搜索好友，实际应用中应调用API  
    friends.value = [
        { id: 1, name: '张三' },
        // ...其他好友数据  
    ];
}

function addFriend(friend: { id: number; name: string }) {
    // 处理添加好友的逻辑  
    console.log('添加好友:', friend);
}

function handleTabClick(tab: { name: string }) {
    activeTab.value = tab.name;
}

</script>

<style scoped>
.add-group-or-friend{
    margin-top:  -30px;
}
.group{
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
    background-color:#f1f1f3;
}
.group-info-content{
    display: flex;
    align-items: center;
}
.group-info-footer{
    width: 100%;
    background-color: rgb(227, 228, 230);
    display: flex;
    height: 25px;
    align-items: center;
    justify-content: end;
}

.group-avatar {
    width: 60px;
    height: 60px;
    margin-right: 18px;
}

.group-details {
    font-size: 13px;
    height: 90px;
    line-height: 10px;
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
</style>