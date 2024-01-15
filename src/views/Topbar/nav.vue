<script setup lang="ts">
import RTnavMenu from './RTnavMenu.vue'
import { useRouter } from 'vue-router'
import { InfoFilled } from '@element-plus/icons-vue'






const router = useRouter()
const pushabout = () => {
    router.push({ path: "/About" })
}

const confirmEvent = () => {
    localStorage.clear()
    router.push({ path: "/" })
}
const cancelEvent = () => {
}
</script>
<template>
    <div id="container">
        <div id="Icon" style="cursor:default ;">
            <a class="linkBlock" v-on:click="pushabout">
                <span>Chat Hub</span>
            </a>
        </div>
        <div id="Space"></div>
        <div id="rtNav">
            <RTnavMenu></RTnavMenu>
        </div>



    </div>
</template>
<style>
.doSeachElement2:hover {
    transition: all 0.3s ease-in-out;
    background-color: rgb(170, 166, 166);
    border-radius: 15px;
}

#container {
    width: 1200px;
    height: 100%;
    display: flex;

}

#Icon {
    height: 100%;
    width: 10%;
    min-width: 110px;
}

.linkBlock {
    height: 100%;
    width: 100%;
    align-items: center;
    display: flex;
    text-decoration: inherit;
    font-size: 25px;
}

#Search {
    width: 20%;
    height: 100%;
    display: flex;
    min-width: 135px;
}

.doSeachElement {
    display: flex;
    align-items: center;
}

.doSeachElement2 {
    margin-left: 10px;
    margin-top: 5px;
}

.Searchicon {
    margin-right: 10px;
}

#SearchPlaceHolder {
    display: inline-block;
    font-size: 13px;
}

.DoSearchBlcok {
    justify-content: flex-start;
    display: flex;
    align-items: center;
    border: none;
}

#Space {
    width: 30%;
    height: 100%;
}

#rtNav {
    width: 40%;
    height: 100%;
    min-width: 315px;
}

#rtNavMenu {
    width: 100%;
    height: 100%;
}

#loginMenu {
    width: 20%;
    height: 100%;
}
</style>
