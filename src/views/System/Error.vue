<script lang="ts" setup>
import { useRoute } from 'vue-router'
import {ref,Ref} from 'vue'
const route = useRoute();

let errorMessage: string;
let errorDescription: string;
let errorimg: Ref<string> = ref('/src/images/systemPage/');
if (route.query.type == 'NotFound') {
    errorMessage = '资源未找到'
    errorDescription = '您访问的资源不存在，请检查您输入的数据是否正确。'
    errorimg.value = errorimg.value.concat('NotFound.svg')
}
</script>

<template>
    <div class="error-container">
        <img class="error-img" :src="errorimg"/>
        <h1 class="error-title">{{ errorMessage }}</h1>
        <p class="error-description">{{ errorDescription }}</p>
        <button class="error-button">返回</button>
    </div>
</template>

<style>
.error-img {
    width: 50%;
    max-width: 300px;
}

.error-container {
    background-color: white;
    display: flex;
    width: 100%;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
}

.error-title {
    font-size: 2rem;
    font-weight: bold;
    color: #eb5045;
    /* Red color for error */
}

.error-description {
    font-size: 1.2rem;
    color: #757575;
    margin-top: 3px;
    margin-left: 20px;
    margin-right: 20px;
    /* Grey color for description */
}
.error-button{
    width: 200px;
    color: #dad5d5;
    font-size: 17px;
    font-weight: 400;
    margin-top: 50px;
}


</style>