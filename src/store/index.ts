import { defineStore, createPinia } from 'pinia'
import piniaPluginPersist from 'pinia-plugin-persist';
import axios from 'axios'

axios.defaults.headers.common['Authorization'] = "Bearer " + localStorage.getItem('jwt') //向每一个请求携带JWT
axios.defaults.baseURL = 'https://localhost:5001/';//基础Url


export const UseUserInformationStore = defineStore("userInfo", {
    state: () => {
        return {
            userId: "",
            userName: "未登录",
            userPsw: "",
            jwtToken: "",
            userImg: "",
            connection: null,
            isLogin: false,
            unReadMsg: "new",
            UserDetailInfo: {
                id: '',
                Password: '',
                Username: '',
                HeaderImg: '',
                Email: '',
                City: '',
                Sex: '',
                Age: '',
                Job: '',
                Phone: '',
                NickName: '',
                Birth: '',
                Desc: '',
                status: ''
            }
        }
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: 'userinfo',
                storage: localStorage,
            }
        ]
    },
    actions: {
    },


})

export const UseFriendsStore = defineStore("friendstore", {
    state: () => {
        return {
            Friends: [

            ]
        }
    },
    actions: {
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: 'friends',
                storage: localStorage,
            }
        ]
    }
})

//貌似将公共消息与私人消息都存储到了这里...
export const UsePublicMsgStore = defineStore('Pmsg', {
    state: () => {
        return {
            //公共消息name一律为world,私人消息name则为目标用户名称
            Cmsg: [
                {
                    targetUserName: 'world',
                    messages: [],
                    messageNames: [],
                    messageHeaders: []
                }
            ]
        }
    },
    persist: {
        enabled: true,
        strategies: [
            {
                key: '消息存储库',
                storage: localStorage,
            }
        ]
    }
})

//聊天区Tab栏
export const UseMsgStore = defineStore('Smsg', {
    state: () => {
        return {
            editableTabs: []
        }
    },
})


export const appsetting = defineStore('appset', {
    state: () => {
        return {
            input: "",
            ServerAddress: "https://localhost:5001",
            ServerHubAddress: "https://localhost:5001/Hub"
        }
    },
})

//消息栏仓库
export const UseMsgbox = defineStore('MsgBox', {
    state: () => {
        return {
            MsgCount: 0,
            MsgItems: [
            ]

        }
    },
})

interface IChatStore {
    targetUserTab: TargetUserTab[]
    selectedTab: number
}
//存储与用户正在聊天的Tab信息
interface TargetUserTab {
    tabTitle: string;
    tabName: string;
    targetUserMessage: TargetUserMessage;
}
//存储目标聊天用户的相关信息。名称，聊天内容...
interface TargetUserMessage {
    targetUserName: string,
    messages: string[],
    messageNames: string[],
    messageHeaders: string[],
}
export const UseChatStore = defineStore('Chat', {
    state: (): IChatStore => {
        return {
            targetUserTab: [{
                tabTitle: "世界频道",
                tabName: "world",
                targetUserMessage:{
                    targetUserName: '',
                    messages: [],
                    messageNames: [],
                    messageHeaders: []
                }
            }],
            selectedTab: 0
        }
    },
    actions:()=>{

    }
})

const store = createPinia()
store.use(piniaPluginPersist)
export default store