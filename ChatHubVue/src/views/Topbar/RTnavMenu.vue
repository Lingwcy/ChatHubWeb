<script setup lang="ts">
import { ChangeTheme } from '../../theme/styleHandler'
import { HTMLElement } from '../../global_env'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { UseTheme, UseUserInformationStore } from '../../store/index'
import {Sunny,Moon} from '@element-plus/icons-vue'
let themeStore = UseTheme();
const router = useRouter()
const userinfo = UseUserInformationStore()


const pushHub = () => {
    router.push({ path: '/Hub/Friends' })
}

const bgValue = ref(true)
let changeBg = function () {
    var a: HTMLElement = document.getElementById("themeSwitch") as any
    if (bgValue.value) {
        a.href = "src/theme/default.css"
        document.body.style.backgroundImage = "url(./src//images/bg.jpg)";
    } else {
        a.href = "src/theme/dark.css"
        document.body.style.backgroundImage = "url(./src//images/bg2.jpg)";

    }
}

</script>

<template>
    <nav id="Container">
        <div class="eleBlock">
            <el-switch v-model="themeStore.Icon.SwitchTheme.isDark" class="ml-2"
                style="--el-switch-on-color:#303133; --el-switch-off-color:#c8c9cc"
                v-on:click="ChangeTheme(themeStore)" id="themeSwitcher" inline-prompt>
                <template #active-action>
                    <el-icon><Moon color="black" /></el-icon>
                </template>

                <template #inactive-action>
                    <el-icon><Sunny color="black" /></el-icon>
                </template>
            </el-switch>
        </div>
        <div class="eleBlock">
            <img :src="themeStore.Icon.Maxmize" width="25" height="25" />
        </div>

    </nav>
</template>

<style>
#Container {
    width: 100%;
    height: 100%;
    display: flex;
}

.example-showcase .el-dropdown-link {
    color: var(--el-color-primary);
    display: flex;
    align-items: center;
}

.eleBlock {
    margin: 10px;
    align-items: center;
    display: flex;
    justify-content: center;
    cursor: pointer;
}

.el-font {

    font-size: 14px;
}

#logOut {
    font-size: 13px;
    width: 60px;
    height: 30px;
    background-color: #909399;
    border: 1px solid #606266;

}

#logOut-font {
    width: 90%;
    height: 80%;
    margin-bottom: 4px;
}
</style>
