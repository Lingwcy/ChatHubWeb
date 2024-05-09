<script setup lang="ts">
import { UseUserInformationStore } from '../../../store/index'
import { reactive, ref, onMounted } from 'vue'
import { ElButton, ElDialog } from 'element-plus'
import { ElMessage } from 'element-plus'
import { UseServiceStore } from '../../../store/index'
import { createFileService } from '../../../services/ServicesCollector'
createFileService()
const service = UseServiceStore()
const UserInfo = UseUserInformationStore()
const Headvisible = ref(false)
const GetHeaderImg = () => {
    let HeaderArr = []
    let HeaderLength = 12
    for (let i = 1; i <= HeaderLength; i++) {
        HeaderArr.push("head" + i + ".svg")
    }
    return HeaderArr
}
let SelectedItem = ref('')
const DetailVisible = ref(false)
const UpdateHeaderImg = (img: any) => {
    SelectedItem.value = img
    DetailVisible.value = true
}
const SubmitUpdateHeaderRequest = () => {
    DetailVisible.value = false
    Headvisible.value = false
    const payload = {
        img: SelectedItem.value,
        userId: UserInfo.userId
    }
    service.File?.UploadAvatar(payload).then(res => {
        if (res) {
            ElMessage({
                message: '头像更换成功!.',
                type: 'success',
            })
            UserInfo.userImg = SelectedItem.value
        }
    })

}
const HeaderImgArr = GetHeaderImg()

const form = reactive({
    Username: UserInfo.UserDetailInfo.Username,
    Email: UserInfo.UserDetailInfo.Email,
    City: UserInfo.UserDetailInfo.City,
    Sex: UserInfo.UserDetailInfo.Sex,
    Job: UserInfo.UserDetailInfo.Job,
    Birth: UserInfo.UserDetailInfo.Birth,
    Desc: UserInfo.UserDetailInfo.Desc,
    Phone: UserInfo.UserDetailInfo.Phone,
    id:UserInfo.userId,
    UserName : UserInfo.userName
})
//清空表单
const onClear = () => {
    form.Username = ''
    form.Email = ''
    form.City = ''
    form.Sex = ''
    form.Job = ''
    form.Birth = ''
    form.Desc = ''
    form.Phone = ''
}
//提交表单
const onSubmit = () => {
    UserInfo.UserDetailInfo.Email = form.Email
    UserInfo.UserDetailInfo.City = form.City
    UserInfo.UserDetailInfo.Sex = form.Sex
    UserInfo.UserDetailInfo.Job = form.Job
    UserInfo.UserDetailInfo.Birth = form.Birth
    UserInfo.UserDetailInfo.Desc = form.Desc
    UserInfo.UserDetailInfo.Phone = form.Phone
    UserInfo.UserDetailInfo.id = UserInfo.userId
    UserInfo.userName = UserInfo.userName
    //Birth格式调成yyyy-MM-dd
    const Birth = new Date(form.Birth)
    const year = Birth.getFullYear()
    const month = (Birth.getMonth() + 1).toString().padStart(2, '0')
    const day = Birth.getDate().toString().padStart(2, '0')
    form.Birth = `${year}-${month}-${day}`
    service.File?.UpdateUserInfo(form).then(res => {
        if (res) {
            ElMessage({
                message: '个人信息更新成功!.',
                type: 'success',
            })
        }   
    })
}

onMounted(function () {

})
</script>
<template>
    <h2>#个人信息</h2>
    <el-divider />
    <div id="UserSetting-Content">
        <div class="UserSetting-Content-col">
            <div class="setting-item">
                <el-form :model="form" label-width="80px" style="margin-top: 20px;">
                    <el-form-item label="邮箱">
                        <el-input v-model="form.Email" />
                    </el-form-item>
                    <el-form-item label="手机号">
                        <el-input v-model="form.Phone" />
                    </el-form-item>
                    <el-form-item label="居住地">
                        <el-select v-model="form.City" placeholder="选择您的居住地">
                            <el-option label="湖北" value="湖北" />
                            <el-option label="四川" value="四川" />
                        </el-select>
                    </el-form-item>
                    <el-form-item label="生日">
                        <el-col :span="11">
                            <el-date-picker v-model="form.Birth" type="date" placeholder="选择时间" style="width: 100%" />
                        </el-col>
                    </el-form-item>
                    <el-form-item label="职业">
                        <el-radio-group v-model="form.Job">
                            <el-radio label="计算机/互联网/通信" name="type" />
                            <el-radio label="生产/工艺/制造" name="type" />
                            <el-radio label="护理/医疗/制药" name="type" />
                            <el-radio label="广告/文化/传媒" name="type" />
                        </el-radio-group>
                    </el-form-item>
                    <el-form-item label="性别">
                        <el-radio-group v-model="form.Sex">
                            <el-radio label="男" />
                            <el-radio label="女" />
                        </el-radio-group>
                    </el-form-item>
                    <el-form-item label="简述">
                        <el-input v-model="form.Desc" type="textarea" />
                    </el-form-item>
                    <el-form-item>
                        <el-button type="primary" @click="onSubmit">保存</el-button>
                        <el-button @click="onClear">清空</el-button>
                    </el-form-item>
                </el-form>

            </div>
        </div>
        <div class="UserSetting-Content-col2">
            <div class="setting-item">
                <div id="changgeHeaderContent">
                    <img :src="'../../../src/images/systemHeader/' + UserInfo.userImg" class="avater"
                        style="width: 100px; height: 100px; margin-right: 0px;" alt=""
                        v-on:click="Headvisible = !Headvisible">
                    <span style="font-weight: 200; font-size: 15px;">更换头像</span>
                </div>
            </div>
        </div>
    </div>



    <el-dialog v-model="Headvisible" title="更换头像" width="800px" id="addUser" center draggable>
        <template #header>
            <div id="addUser-Header">
                <div id="addUser-Header-sign"></div>
                <strong style="margin-left: 5px;">更换头像</strong>
            </div>
        </template>
        <div id="header-menu">
            <div class="header-menu-item" v-for="(item) in HeaderImgArr" v-on:click="UpdateHeaderImg(item)">
                <img :src="'../../../src/images/systemHeader/' + item" alt="">
            </div>
        </div>

        <template #footer>
            <div id="addUser-footer">
                <el-button @click="Headvisible = false">取消</el-button>
                <el-button type="primary" @click="Headvisible = false">
                    确认
                </el-button>
            </div>
        </template>
    </el-dialog>

    <el-dialog v-model="DetailVisible" style="width: 300px;">
        <img w-full :src="'../../../src/images/systemHeader/' + SelectedItem" alt="Preview Image" />
        <template #footer>
            <div id="addUser-footer">
                <el-button type="primary" @click="SubmitUpdateHeaderRequest()">
                    确认
                </el-button>
            </div>
        </template>
    </el-dialog>
</template>
<style>
#header-menu {
    height: 300px;
    width: 100%;
    display: flex;
    flex-wrap: wrap;
}

.header-menu-item {
    width: 70px;
    height: 70px;
    margin: 10px;
    border-radius: 10px;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: rgb(236, 241, 241);
}

.header-menu-item:hover {
    transition: all 0.3s ease-in-out;
    background-color: rgb(190, 192, 192);
    cursor: pointer;
}

#addUser {
    border-radius: 10px;
    width: 500px;
}

#addUser-Header {
    height: 30px;
    font-size: 20px;
    align-items: center;
    display: flex;
}

#addUser-Header-sign {
    width: 10px;
    height: 30px;
    background-color: #337ec9;
    border-radius: 10px;
    margin-left: 10px;
}

.el-dialog__footer {
    border-top: 1px solid #c8c9cc;
    height: 50px;
}

.el-dialog__header {
    border-bottom: 1px solid #c8c9cc;
}

#addUser-Content {
    display: flex;
    justify-content: center;

}


#changgeHeaderContent {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 50%;
    flex-direction: column;
}

#UserSetting-Content {
    height: 500px;
    display: flex;
}

.UserSetting-Content-col {
    min-width: 250px;
    width: 70%;
    height: 100%;
    margin-right: 15px;
    display: flex;
    justify-content: center;
}

.UserSetting-Content-col2 {
    min-width: 250px;
    width: 30%;
    height: 100%;
    margin-right: 15px;
    display: flex;
    justify-content: center;
}

.setting-item {
    min-width: 250px;
    width: 90%;
    height: 530px;
    border: 1px solid rgb(239, 236, 236);
    border-radius: 10px;
    padding: 10px;
    overflow: hidden;
    display: flex;
    justify-content: center;

}
</style>
