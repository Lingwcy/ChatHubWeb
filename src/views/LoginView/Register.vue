<script lang="ts" setup>
import { ref, reactive } from 'vue'
import type { FormInstance } from 'element-plus'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { Action } from 'element-plus'
import axios from 'axios';
import { UseUserInformationStore } from '../../store/index'
import { ElLoading } from 'element-plus'
import { useRouter } from 'vue-router'
const router = useRouter()
const ruleFormRef = ref<FormInstance>()
const userMsg = reactive({
  account: '',
  password: '',
  repassword: '',
  jwtToken: ''
})
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




//写入数据到状态库
const writeUserInfo = (res: any) => {
  userInfo.userName = userMsg.account
  userInfo.userPsw = userMsg.password
  userInfo.jwtToken = res.jwtToken
  userInfo.userImg = res.userImg
  userInfo.userId = res.userId
  GetUserInformation()

}




//表单检验
const validateAccount = (value: any, callback: any) => {
  if (value === '') {
    callback(new Error('账号或邮箱不能为空'))
  } else {
    callback()
  }
}
const validatePass = (value: any, callback: any) => {
  if (value === '') {
    callback(new Error('密码不能为空'))
  }
  else {
    callback()
  }
}

const validatePassCheck = (value: any, callback: any) => {
  if (value === '') {
    callback(new Error('密码不能为空'))
  } else if (userMsg.password !== value) {
    callback(new Error('两次输入密码不一致'))
  }
  else {
    callback()
  }
}

const rules = reactive({
  account: [{ validator: validateAccount, trigger: 'blur' }],
  password: [{ validator: validatePass, trigger: 'blur' }],
  repassword: [{ validator: validatePassCheck, trigger: 'blur' }],
})


const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate((valid) => {
    if (valid) {
      console.log('submit!')
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

//配置loading
const loadingOptions = {
  text: "注册中...请稍后"
}
//发起请求
const register = async function () {
  const loadingInstance = ElLoading.service(loadingOptions)
  //构造json querystring
  const payload = {
    userName: userMsg.account,
    passworld: userMsg.password
  };
  localStorage.clear()
  userInfo.$reset()
  await axios.post('https://localhost:5001/api/font-login/register', payload)//拿取jwt
    .then(async res => {
      if (res.data.errors != null) {
        ElMessageBox.alert("已经存在此账户", '注册失败',
          {
            confirmButtonText: '确认',
            type: 'error',
          })
      }
      if (res.data.data) {
        ElMessageBox.alert("注册成功", '注册',
          {
            confirmButtonText: '确认',
            type: 'success',
          })

        userInfo.connection = ''
        let data = JSON.parse(res.data.data)
        userMsg.jwtToken = data.jwtToken//客户端存储JWT方便之后的认证
        localStorage.setItem("jwt", data.jwtToken)
        writeUserInfo(data)//写入状态库
        loadingInstance.close()
        ElMessageBox.alert(`欢迎 ${userMsg.account} 加入\n Chat Hub`, '注册成功',
          {
            confirmButtonText: '确认',
            type: 'success',
          })
        router.push({
          path: '/Hub'
        })

      }


    })
    .catch(err => {
      ElMessageBox.alert(err, '未知错误',
        {
          confirmButtonText: '确认',
          type: 'error',
        })
    })
}

</script>
<template>
  <el-form id="Loinform" ref="ruleFormRef" :model="userMsg" :rules="rules" status-icon label-width="0px"
    class="demo-ruleForm">
    <div class="rightRegister-content-main-center-items">
      <el-form-item prop="account" style="width:300px;">
        <el-input v-model="userMsg.account" placeholder="账号或者邮箱" />
      </el-form-item>
    </div>
    <div class="rightRegister-content-main-center-items">
      <el-form-item prop="password" style="width:300px;">
        <el-input v-model="userMsg.password" type="password" placeholder="密码" />
      </el-form-item>
    </div>
    <div class="rightRegister-content-main-center-items">
      <el-form-item prop="repassword" style="width:300px;">
        <el-input v-model="userMsg.repassword" type="password" placeholder="再次输入密码" />
      </el-form-item>
    </div>
    <div class="rightRegister-content-main-center-items" style="height: 35%;">
      <el-button id="loginBt" type="primary" @click="submitForm(ruleFormRef)">注册</el-button>
    </div>
    <div id="rightLogin-content-footers">
      <p style=" color: #989899; font-size: 13px;">需要新的账号？<router-link to="/Login/Register">注册</router-link></p>
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