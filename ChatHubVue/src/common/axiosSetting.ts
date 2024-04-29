import axios from "axios";
import { ElMessage } from 'element-plus'
import router from "../router/index";
import { crypto } from "../Crypto/crypto";

function generateUUIDv4() {  
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {  
        var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3) | 0x8;  
        return v.toString(16);  
    });  
}  

const http = axios.create({
    baseURL: 'https://localhost:5001/',
    timeout: 2 * 60 * 1000,
    headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('token') || '',
        'Identifier' : generateUUIDv4(),
        'Key': "Empty"
    }
})

//请求拦截器
http.interceptors.request.use(
    config => {
        // 发起请求时，重新获取最新的token，这一步有时很重要
        // 因为创建 axios 实例的时候，获取到的 token 未必是有效的，或说未必能获取到
        const token = localStorage.getItem('token');

        // 特殊配置：登录接口，将 请求的头的 token 设置为空字符串
        if (config.url === '/Auth/login') {
            config.headers!!['token'] = ''
        } else {
            config.headers!!['token'] = token || ''
        }
        //如果url是https://localhost:5001/File/Upload则不加密
        if (config.url!.indexOf('File') > -1) {
            return config;
        }

        config.data = crypto.encrypt(JSON.stringify(config.data))
        return config;
    },
    error => {
        console.warn(error);
        return Promise.reject(error);
    }
)

//响应拦截器
http.interceptors.response.use(
    response => {
        const res = response;
        if (res.data.code == 3) {//数据库中无此资源
            return Promise.reject(res.data.message)
        }
        if (res.data.code == 4) {//冗余的数据
            return Promise.reject(res.data.message)
        }
        if (res.status == 404) {
            ElMessage({
                message: res.data.message,
                type: 'warning',
            })
            router.push({
                name: 'ErrorPage',
                query: { type: 'NoPermisson' }
            }).then()
            return res;
        }
        if (res.data.code == -3) {
            //登录过期
            ElMessage({
                message: res.data.message,
                type: 'warning',
            })
            router.push('/Login/Account').then();
            return Promise.reject(res.data.message)
        }
        return res;
    },
    error => {
        if (error.message.indexOf('timeout') > -1) {
            error.message = '请求超时'
        }
        if (error.message.indexOf('Network') > -1) {
            error.message = '网络错误'
        }
        const res = error.response;
        if (res.status == 500) {
            ElMessage({
                message: '系统内部错误，请联系管理员',
                type: 'error',
                duration: 3 * 1000
            })
        }
        if (res.status == 404) {
            ElMessage({
                message: '未找到此资源',
                type: 'error',
                duration: 3 * 1000
            })
        }
        if (res.status == 403) {
            ElMessage({
                message: '权限不足',
                type: 'error',
                duration: 3 * 1000
            })
        }
        if (res.status == 401) {
            ElMessage({
                message: '用户未登录或Token已过期',
                type: 'error',
                duration: 3 * 1000
            })
            router.push('/')
        }
        return Promise.reject(error);
    }
)


export default http;