import { ILoginFormModel} from '../interface/IForm'
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