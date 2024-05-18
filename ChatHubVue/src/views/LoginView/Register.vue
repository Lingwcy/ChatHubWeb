<script lang="ts" setup>
import { ref } from 'vue'
import type { FormInstance } from 'element-plus'
import { ElMessageBox } from 'element-plus'
import type { Action } from 'element-plus'
import { UseUserInformationStore, UseServiceStore } from '../../store/index'
import { ElLoading } from 'element-plus'
import { useRouter } from 'vue-router'
import { registerForm } from '../../models/data/Form';
import { createChatHubService } from '../../services/ServicesCollector';

const router = useRouter()
const service = UseServiceStore()
const registerFormRef = ref<FormInstance>();
const userInfo = UseUserInformationStore()


const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate((valid) => {
    if (valid) {
      register()
    } else {
      ElMessageBox.alert('输入错误', '错误',
        {
          confirmButtonText: '确认',
          type: 'error',
          callback: (_action: Action) => {
            resetForm(formEl)
          }
        })

      return false
    }
  })
}

const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.resetFields()
}

//请求方法
const register = async function () {
  if (registerForm.registerFormModel.account != userInfo.userName) {
    localStorage.clear()
    userInfo.$reset()
  }
  const payload = {
    userName: registerForm.registerFormModel.account,
    passworld: registerForm.registerFormModel.password
  };
  //配置loading
  const loadingOptions = {
    text: "登陆中...请稍后"
  }
  const loadingInstance = ElLoading.service(loadingOptions)
  let flag: Promise<boolean> | undefined = service.Auth?.Register(payload, userInfo);
  if (flag == undefined) return;
  flag.then(res => {
    if (res) {
      ElMessageBox.alert(`欢迎 ${registerForm.registerFormModel.account} 加入\n Chat Hub`, '注册成功',
        {
          confirmButtonText: '确认',
          type: 'success',
        })
      //未连接状态。重新创建服务
      if (service.ChatHub?.HubConnection.state != 'Connected') {
        createChatHubService();
        service.ChatHub?.startHub();
      }
      router.push({
        path: '/Hub'
      })

      setTimeout(() => {
        location.reload()
      }, 1000)
    } else {
      resetForm(registerFormRef.value)
    }
  })
  loadingInstance.close()
}
</script>
<template>
  <el-form id="Loinform" ref="registerFormRef" :model="registerForm.registerFormModel"
    :rules="registerForm.registerRules" status-icon label-width="0px" class="demo-ruleForm">
    <div class="rightRegister-content-main-center-items">
      <el-form-item prop="account" style="width:300px;">
        <el-input v-model="registerForm.registerFormModel.account" placeholder="账号或者邮箱" />
      </el-form-item>
    </div>
    <div class="rightRegister-content-main-center-items">
      <el-form-item prop="password" style="width:300px;">
        <el-input v-model="registerForm.registerFormModel.password" type="password" placeholder="密码" />
      </el-form-item>
    </div>
    <div class="rightRegister-content-main-center-items">
      <el-form-item prop="repassword" style="width:300px;">
        <el-input v-model="registerForm.registerFormModel.repassword" type="password" placeholder="再次输入密码" />
      </el-form-item>
    </div>
    <div class="rightRegister-content-main-center-items" style="height: 35%;">
      <el-button id="loginBt" type="primary" style="width: 350px; margin-top: -30px;"
        @click="submitForm(registerFormRef)">注册</el-button>
    </div>
    <div id="rightLogin-content-footers">
      <p style=" color: #989899; font-size: 13px; margin-top: -30px;">已经拥有账号？<router-link
          to="/Login/Account">登录</router-link></p>
    </div>
  </el-form>
</template>
<style>
.rightRegister-content-main-center-items {
  width: 100%;
  height: 24%;
  align-items: center;
  display: flex;
  justify-content: center;
  flex-wrap: nowrap;
}

#rightLogin-content-footers {
  width: 50%;
  height: 10px;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 10px;
}

#Loinform {
  width: 100%;
  height: 100%;
}
</style>