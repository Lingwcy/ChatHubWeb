<script setup lang="ts">
defineOptions({
  name: "HubData"
});
import { useUserStoreHook } from "@/store/modules/user";
import { useColumns } from "./userColumns";
import { AllUserResultData } from "@/api/user";
const { columns } = useColumns();
let result = await useUserStoreHook().getAllUsers();
let tableData = JSON.parse(result.data) as AllUserResultData;
</script>

<template>
  <pure-table :data="tableData" :columns="columns" />
</template>
