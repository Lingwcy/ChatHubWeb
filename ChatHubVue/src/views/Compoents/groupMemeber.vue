<template>
    <el-table ref="tableRef" :data="groupStore.OnConnectedGroup.GroupMemebers" style="width: 100%" :show-header="false">
      <el-table-column prop="name" label="Name" width="60px" show-overflow-tooltip/>
      <el-table-column
        prop="tag"
        label="Role"
        width="80px"
      >
        <template #default="scope">
          <el-tag
            :type="scope.row.Role === '群主' ? '' : 'success'"
            disable-transitions
            >{{ scope.row.Role}}</el-tag
          >
        </template>
      </el-table-column>
    </el-table>
  </template>
  
  <script lang="ts" setup>
  import { ref } from 'vue'
  import type { TableInstance } from 'element-plus'
  import { createGroupService } from '../../services/ServicesCollector';
  import { UseGroupStore,UseServiceStore,UseUserInformationStore} from '../../store';
  createGroupService();
  const groupStore = UseGroupStore();
  const serviceStore = UseServiceStore();
  const userInformationStore = UseUserInformationStore();
  serviceStore.Group?.GetGroupMemberList(groupStore.OnConnectedGroup.GroupInfo.GroupId,userInformationStore.userName,groupStore)
  interface User {
    name: string
    tag: string
  }
  
  const tableRef = ref<TableInstance>()
  
  
  const testtableData: User[] = [
    {
      name: 'WCY',
      tag: '群主',
    },
    {
      name: 'hash',
      tag: '成员',
    },
    {
      name: 'Tom',
      tag: '成员',
    },
    {
      name: 'testHSSSSSSSSSSSSSSA',
      tag: '成员',
    },
    {
      name: 'WCY',
      tag: '群主',
    },
    {
      name: 'hash',
      tag: '成员',
    },
    {
      name: 'Tom',
      tag: '成员',
    },
    {
      name: 'testHSSSSSSSSSSSSSSA',
      tag: '成员',
    },
    {
      name: 'WCY',
      tag: '群主',
    },
    {
      name: 'hash',
      tag: '成员',
    },
    {
      name: 'Tom',
      tag: '成员',
    },
    {
      name: 'testHSSSSSSSSSSSSSSA',
      tag: '成员',
    },
    {
      name: 'WCY',
      tag: '群主',
    },
    {
      name: 'hash',
      tag: '成员',
    },
    {
      name: 'Tom',
      tag: '成员',
    },
    {
      name: 'testHSSSSSSSSSSSSSSA',
      tag: '成员',
    },
    {
      name: 'WCY',
      tag: '群主',
    },
    {
      name: 'hash',
      tag: '成员',
    },
    {
      name: 'Tom',
      tag: '成员',
    },
    {
      name: 'testHSSSSSSSSSSSSSSA',
      tag: '成员',
    },
  ]
  </script>
  