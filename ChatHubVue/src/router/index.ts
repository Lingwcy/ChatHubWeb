import { createRouter, createWebHashHistory } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import axios from 'axios'
// 1. 定义路由组件.
// 也可以从其他文件导入

//路由懒加载
const Setting = () => import('../views/MainContent/Setting.vue')
const About = () => import('../views/MainContent/About.vue')
const Hub = () => import('../views/MainContent/Hub.vue')
const LoinContent = () => import('../views/LoginView/LoginContent.vue')
const Account = () => import('../views/LoginView/Login.vue')
const Register = () => import('../views/LoginView/Register.vue')
const Phone = () => import('../views/LoginView/Phone.vue')


const Msgbox = () => import('../views/MainContent/HubContent/LeftNavDetail/MessageBox.vue')
const UserSetting = () => import('../views/MainContent/Setting/UserSetting.vue')
const LocalSetting = () => import('../views/MainContent/Setting/LocalSetting.vue')
const Contract = () => import('../views/MainContent/HubContent/LeftNavDetail/Contract.vue')
//system
const Error = () => import('../views/System/Error.vue');


const routes = [
    {
        path: "/",
        redirect: '/Login/Account'
    },
    {
        path: '/error/:type?', 
        name: 'ErrorPage',
        component: Error,
    },
    {
        path: '/Hub',
        name: 'Hub',
        component: Hub,
        children: [
            {
                path: 'Message',
                component: Msgbox
            },
            {
                path: 'Contract',
                component: Contract
            }
        ]
    },
    {
        path: '/Login',
        component: LoinContent,
        children: [
            {
                path: 'Account',
                component: Account
            },
            {
                path: 'Register',
                component: Register
            },
            {
                path: 'Phone',
                component: Phone
            }
        ],
    },
    {
        path: '/About',
        component: About
    },
    {
        path: '/Setting',
        component: Setting,
        children: [
            {
                path: 'User',
                component: UserSetting
            },
            {
                path: 'Local',
                component: LocalSetting
            },

        ],
        beforeEnter: async (to: any, from: any, next: any) => {
            const jwt = localStorage.getItem("jwt")
            const config = {
                headers: {
                    Authorization: "Bearer " + jwt,//附带Jwt认证
                }
            }
            await axios.get('https://localhost:5001/api/font-login/verify', config)
                .then(so => {
                    next()
                })
                .catch(err => {
                    ElMessageBox.alert('请登陆', '登陆',
                        {
                            confirmButtonText: '确认',
                        })
                    router.push('/')
                })

        },
    }

]

// 3. 创建路由实例并传递 `routes` 配置
// 你可以在这里输入更多的配置，但我们在这里
// 暂时保持简单
const router = createRouter({
    // 4. 内部提供了 history 模式的实现。为了简单起见，我们在这里使用 hash 模式。
    history: createWebHashHistory(),
    routes, // `routes: routes` 的缩写
})

//全球守卫 在每一次跳转都会触发
/* router.beforeEach(
    (to,from,next)=>{
        console.log(to)
        console.log(from)
        next()//路由守卫 
    }
) */
export default router