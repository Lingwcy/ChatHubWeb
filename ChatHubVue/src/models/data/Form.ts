import { ILoginFormModel,IRegisterFormModel} from '../interface/IForm'
import type {FormRules } from 'element-plus'
import { reactive} from 'vue';

export class loginForm{
    static loginFormModel = reactive<ILoginFormModel>({
        account: '',
        password: '',
        jwtToken: '',
    })
    static loginRules = reactive<FormRules<ILoginFormModel>>({
        account: [
            { required: true, message: '账号或邮箱不能为空', trigger: 'blur' },
        ],
        password: [
            {
                required: true,
                message: '密码不能为空',
                trigger: 'blur',
            },
            { min: 3, max: 20, message: '密码长度应在3-20个字符之间', trigger: 'blur' },
        ],
    })
    static randomNum = (min: any, max: any) => {
        return Math.floor(Math.random() * (max - min) + min)
    }
}


export class registerForm{
    static registerFormModel = reactive<IRegisterFormModel>({
        account: '',
        password: '',
        repassword: '',
        jwtToken: '',
    })
    static validatePassCheck = (rule: any, value: any, callback: any) => {
        if (value === '') {
          callback(new Error('请再次输入密码'))
        } else if (value !== registerForm.registerFormModel.password) {
          callback(new Error("两次密码输入不匹配!"))
        } else {
          callback()
        }
      }
    static registerRules = reactive<FormRules<IRegisterFormModel>>({
        account: [
            { required: true, message: '账号或邮箱不能为空', trigger: 'blur' },
        ],
        password: [
            {
                required: true,
                message: '密码不能为空',
                trigger: 'blur',
            },
            { min: 3, max: 20, message: '密码长度应在3-20个字符之间', trigger: 'blur' },
        ],
        repassword: [{ validator: registerForm.validatePassCheck, trigger: 'blur' }],
    })
    static randomNum = (min: any, max: any) => {
        return Math.floor(Math.random() * (max - min) + min)
    }

}