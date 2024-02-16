<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import type { FormInstance } from 'element-plus'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from 'axios';
import { useRouter } from 'vue-router'
import { ElLoading } from 'element-plus'
import { UseUserInformationStore } from '../../store/index'
import { loginForm } from '../../models/data/Form'
import { createAuthService, AuthService } from "../../services/ServicesCollector";
const loginFormRef = ref<FormInstance>();
onMounted(function () {
  createAuthService();
})
const router = useRouter()
//创建用户状态库变量
const userInfo = UseUserInformationStore()

//将详细信息写入状态库
const GetUserInformation = () => {
  axios.get("api/user/user/" + userInfo.userName)
    .then(res => {
      if (res.data.errors != null) {
        ElMessage({
          message: '用户数据获取失败!.',
          type: 'error',
        })
        return;
      }
      if (res.data.data) {
        userInfo.UserDetailInfo = JSON.parse(res.data.data)

      }
    })
    .catch(error => [
      ElMessage({
        message: '用户数据获取失败!.' + error,
        type: 'error',
      })

    ])
}
//提交表单
const submitForm = (formType: FormInstance | undefined) => {
  if (!formType) return
  formType.validate((isValid, _invalidFields) => {
    if (isValid) {
      login()
    } else {
      ElMessageBox.alert('用户名或者密码格式不满足条件', '错误',
        {
          confirmButtonText: '确认',
          type: 'error',
          callback: () => {
            resetForm(formType)
          }
        })
      return false
    }
  })
}
//清空表单
const resetForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.resetFields()
}
//写入数据到状态库
const writeUserInfo = (res: any) => {
  userInfo.userName = loginForm.loginFormModel.account
  userInfo.userPsw = loginForm.loginFormModel.password
  userInfo.jwtToken = res.jwtToken
  userInfo.userImg = res.userImg
  userInfo.userId = res.userId
  GetUserInformation()
}
//登陆请求方法
const login = async function () {
  if (loginForm.loginFormModel.account != userInfo.userName) {
    localStorage.clear()
    userInfo.$reset()
  }
  const payload = {
    userName: loginForm.loginFormModel.account,
    passworld: loginForm.loginFormModel.password
  };
  //配置loading
  const loadingOptions = {
    text: "登陆中...请稍后"
  }
  const loadingInstance = ElLoading.service(loadingOptions)
  let flag: Promise<boolean> = AuthService.Login(payload, userInfo);
  flag.then(res => {
    if (res) {
      ElMessageBox.alert(`欢迎 ${loginForm.loginFormModel.account} 加入\n Chat Hub`, '登录成功',
        {
          confirmButtonText: '确认',
          type: 'success',
        })
      router.push({
        path: '/Hub'
      })
    }else{
      resetForm(loginFormRef.value)
    }
  })
  loadingInstance.close()

}


</script>
<template>
  <el-col :xs="24" :sm="24" :md="0" :lg="0" :xl="0" class="LogincontainerContent-Phone hidden-md-and-up">
    <el-form class="Loinform-phone" ref="loginFormRef" :model="loginForm.loginFormModel" :rules="loginForm.loginRules"
      status-icon label-width="0px">
      <div class="rightLogin-content-main-center-items-phone">
        <el-form-item prop="account" style="width:380px; height: 50px;">
          <el-input v-model="loginForm.loginFormModel.account" placeholder="账号或者邮箱" class="account-input" />
        </el-form-item>
      </div>
      <div class="rightLogin-content-main-center-items-phone">
        <el-form-item prop="password" style="width:380px; height: 50px;">
          <el-input v-model="loginForm.loginFormModel.password" type="password" placeholder="密码" />
        </el-form-item>
      </div>
      <div class="rightLogin-content-main-center-items-phone">
        <el-button class="loginBt-phone" type="primary" @click="submitForm(loginFormRef)">登录</el-button>
      </div>
      <p style=" color: #989899; font-size: 13px;">需要新的账号？<router-link to="/Login/Register">注册</router-link></p>
    </el-form>
  </el-col>
  <el-col :xs="0" :sm="0" :md="24" :lg="24" :xl="24" class="LogincontainerContent-Window hidden-sm-and-down">
    <el-form class="Loinform" ref="loginFormRef" :model="loginForm.loginFormModel" :rules="loginForm.loginRules"
      status-icon label-width="0px">
      <div class="rightLogin-content-main-center-items">
        <el-form-item prop="account" style="width:300px;height: 70px;">
          <el-input v-model="loginForm.loginFormModel.account" placeholder="账号或者邮箱" class="account-input" />
        </el-form-item>
      </div>
      <div class="rightLogin-content-main-center-items">
        <el-form-item prop="password" style="width:300px; height: 70px;">
          <el-input v-model="loginForm.loginFormModel.password" type="password" placeholder="密码" />
        </el-form-item>
      </div>
      <div class="rightLogin-content-main-center-items" style="height: 15%;">
        <el-button class="loginBt" type="primary" @click="submitForm(loginFormRef)">登录</el-button>
      </div>
      <p style=" color: #989899; font-size: 13px;">需要新的账号？<router-link to="/Login/Register">注册</router-link></p>
    </el-form>
  </el-col>
</template>
<style>
.demo-ruleForm {
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;

}

.loginBt-phone {
  width: 380px;
  height: 50px;
}

.checkCodeBox-phone {
  overflow: hidden;
  margin-bottom: 10px;
  margin-left: 20px;
}

#rightLogin-content-footers {
  width: 50%;
  height: 10px;
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 10px;
}

.rightLogin-content-main-center-items-phone {
  align-items: center;
  display: flex;
  justify-content: start;
  flex-wrap: nowrap;
}

.el-form-item__content {
  height: 40px;
}

.rightLogin-content-main-center-items {
  width: 100%;
  height: 24%;
  align-items: center;
  display: flex;
  justify-content: center;
  flex-wrap: nowrap;
}

.Loinform {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}

.Loinform-phone {
  width: 100%;
  height: 100%;
}

.el-input__inner {
  color: rgb(226, 226, 226);
  font-size: 16px;
}

.checkCodeBox {
  overflow: hidden;
  margin-bottom: 10px;
  margin-left: 20px;
}

.el-button--primary {
  height: 40px;
  background-color: #5b66ed;
  border: none;
}
</style>
