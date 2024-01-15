import { ILoginFormModel} from '../interface/IForm'
import type {FormRules } from 'element-plus'
import { reactive, ref } from 'vue';

export class loginForm{
    static loginFormModel = reactive<ILoginFormModel>({
        account: '',
        password: '',
        jwtToken: '',
        checkCode: ''
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
        checkCode: [
            {
                required: true,
                validator(_rule, value, callback, _source, _options) {
                    if (value != loginForm.loginFormCode.Code.value) {
                        callback(new Error('验证码错误'))
                    } else {
                        callback()
                    }
                },
            },
        ],
    })
    static loginFormCode = {
        Key: "1234567890",
        Code: ref('3212')
    }
    static randomNum = (min: any, max: any) => {
        return Math.floor(Math.random() * (max - min) + min)
    }
    static makeCode = (o: any, l: any) => {
        for (let i = 0; i < l; i++) {
            this.loginFormCode.Code.value += o[
                this.randomNum(0, o.length)
            ];
        }
    }
    static refreshCode = () => {
        this.loginFormCode.Code.value = "";
        this.makeCode(this.loginFormCode.Key, 4);
    }
}