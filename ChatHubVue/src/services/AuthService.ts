import * as signalR from "@microsoft/signalr"
import { postLogin, getVerify, postAESKey, postRegister } from "../common/api";
import { ElMessage } from "element-plus";
import { loginForm, registerForm } from "../models/data/Form";
import { crypto } from "../Crypto/crypto";
import http from "../common/axiosSetting";

interface LoginData {
    userId: number;
    userName: string;
    userPsw: string;
    jwtToken: string;
    userImg: string;
}
interface loginParams {
    userName: string,
    passworld: string
}
interface registerParams {
    userName: string,
    passworld: string
}
export class Auth {
    public async Login(params: loginParams, userStore: any): Promise<boolean> {
        return await postLogin(params).then(
            result => {
                if (result.data.code == 1) {
                    let data: LoginData = JSON.parse(result.data.data)
                    localStorage.setItem('token', data.jwtToken,)
                    loginForm.loginFormModel.jwtToken = data.jwtToken
                    userStore.connection = new signalR.HubConnectionBuilder();
                    userStore.userName = data.userName
                    userStore.userPsw = data.userPsw
                    userStore.jwtToken = data.jwtToken
                    userStore.userImg = data.userImg
                    userStore.userId = data.userId
                    return true;
                } else if (result.data.code == 2) {
                    ElMessage({
                        message: result.data.message,
                        type: 'warning',
                    })
                    return false
                }
                return false
            },
            error => {
                ElMessage.error(error);
                console.log(error)
                return false;
            }
        );
    }


    public async Register(params: registerParams, userStore: any): Promise<boolean> {
        return await postRegister(params).then(
            result => {
                if (result.data.code == 1) {
                    let data: LoginData = JSON.parse(result.data.data)
                    localStorage.setItem('token', data.jwtToken,)
                    registerForm.registerFormModel.jwtToken = data.jwtToken
                    userStore.connection = new signalR.HubConnectionBuilder();
                    userStore.userName = registerForm.registerFormModel.account
                    userStore.userPsw = registerForm.registerFormModel.password
                    userStore.jwtToken = data.jwtToken
                    userStore.userImg = data.userImg
                    userStore.userId = data.userId
                    return true;
                } else if (result.data.code == 2) {
                    ElMessage({
                        message: result.data.message,
                        type: 'warning',
                    })
                    return false
                }
                return false
            },
            error => {
                ElMessage.error(error);
                console.log(error)
                return false;
            }
        );
    }


    //判断是否鉴权
    public async Verify(): Promise<boolean> {
        //http.defaults.headers.common['Authorization'] = 'Bearer ' + localStorage.getItem('token') || ''
        return await getVerify().then(res => {
            return true;
        })
    }

    //生成并发送key
    public async SendAESKey(): Promise<boolean> {
        const config = {
            headers: {
                'Key': crypto.gkey,
            }
        }
        return await postAESKey("switchKey", config).then(res => {
            return true;
        })
    }

}

